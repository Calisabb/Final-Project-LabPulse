/**
 * LabPulse Main Application
 * Alpine.js components and reactive stores for homelab service monitoring.
 */

document.addEventListener('alpine:init', () => {
  // Global Toast Notification Store
  Alpine.store('toasts', {
    items: [],
    show(message, type = 'info', duration = 3500) {
      const id = Date.now() + Math.random();
      this.items.push({ id, message, type });
      setTimeout(() => {
        this.remove(id);
      }, duration);
    },
    remove(id) {
      this.items = this.items.filter(item => item.id !== id);
    }
  });

  // Root Application Component
  Alpine.data('app', () => ({
    // Auth & Session
    isAuthenticated: window.Auth ? window.Auth.isAuthenticated() : false,
    userName: window.Auth ? window.Auth.getUserName() : 'Operator',
    loginMessage: '',
    loginLoading: false,
    loginForm: {
      userName: '',
      password: ''
    },
    loginError: '',

    // Active Navigation View ('services' | 'incidents')
    currentView: 'services',

    // Data State
    categories: [],
    services: [],
    loadingServices: false,
    loadingCategories: false,

    // Filter & Search State
    searchQuery: '',
    selectedCategory: null, // null for 'All'
    selectedStatus: 'all',  // 'all' | 'online' | 'offline'

    // Service Health Checks State
    checkingServiceIds: {},
    lastCheckErrors: {},
    isCheckingAll: false,

    // Uptime Strip Lazy Loading & Concurrency Queue (max 5 in flight)
    uptimeCache: {},
    uptimeQueue: [],
    uptimeActiveCount: 0,
    maxUptimeConcurrency: 5,

    // Service Modal State (Create / Edit)
    isServiceModalOpen: false,
    isEditMode: false,
    savingService: false,
    serviceForm: {
      id: null,
      name: '',
      host: '',
      port: 80,
      useHttps: false,
      checkType: 'Http',
      healthPath: '/health',
      timeoutMs: 5000,
      isMonitoringEnabled: true,
      description: '',
      icon: 'server',
      categoryId: ''
    },
    serviceFormErrors: {},

    // Service Delete Confirmation Modal
    isDeleteModalOpen: false,
    serviceToDelete: null,
    deletingService: false,

    // Service Detail Side Panel State
    isPanelOpen: false,
    selectedService: null,
    panelTab: 'overview', // 'overview' | 'uptime' | 'runbooks' | 'incidents'

    // Runbooks State
    serviceRunbooks: [],
    loadingRunbooks: false,
    isRunbookModalOpen: false,
    isRunbookEditMode: false,
    savingRunbook: false,
    deletingRunbookId: null,
    copiedRunbookId: null,
    runbookForm: {
      id: null,
      serviceId: null,
      title: '',
      shellType: 'Bash',
      commandText: '',
      notes: ''
    },
    runbookFormErrors: {},

    // Service-specific Incidents State
    serviceIncidents: [],
    loadingServiceIncidents: false,

    // Global Incidents Dashboard State
    incidents: [],
    loadingIncidents: false,
    incidentsOnlyOpen: false,

    // Auto-refresh timer
    refreshIntervalId: null,

    init() {
      // Listen for session changes (e.g. 401 expiry or manual logout)
      window.addEventListener('auth:change', (e) => {
        this.isAuthenticated = e.detail.isAuthenticated;
        this.userName = e.detail.userName || 'Operator';
        if (!this.isAuthenticated) {
          if (e.detail.reason) {
            this.loginMessage = e.detail.reason;
          }
          this.stopAutoRefresh();
          this.closeServicePanel();
        } else {
          this.loadInitialData();
          this.startAutoRefresh();
        }
      });

      if (this.isAuthenticated) {
        this.loadInitialData();
        this.startAutoRefresh();
      }

      // Handle visibilitychange to pause auto-refresh when tab is hidden
      document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
          this.stopAutoRefresh();
        } else if (this.isAuthenticated) {
          this.refreshServicesSilently();
          if (this.currentView === 'incidents') {
            this.loadIncidents();
          }
          this.startAutoRefresh();
        }
      });

      // Watch currentView changes
      this.$watch('currentView', (view) => {
        if (view === 'incidents') {
          this.loadIncidents();
        }
      });

      // Watch incidentsOnlyOpen toggle
      this.$watch('incidentsOnlyOpen', () => {
        if (this.currentView === 'incidents') {
          this.loadIncidents();
        }
      });
    },

    async handleLogin() {
      this.loginError = '';
      this.loginMessage = '';

      if (!this.loginForm.userName.trim()) {
        this.loginError = 'Username is required.';
        return;
      }
      if (!this.loginForm.password) {
        this.loginError = 'Password is required.';
        return;
      }

      this.loginLoading = true;
      try {
        const result = await window.Api.login(this.loginForm.userName, this.loginForm.password);
        window.Auth.setSession(result);
        this.isAuthenticated = true;
        this.userName = result.userName || this.loginForm.userName;
        this.loginForm.password = '';
        Alpine.store('toasts').show(`Signed in as ${this.userName}`, 'success');
        await this.loadInitialData();
        this.startAutoRefresh();
      } catch (err) {
        this.loginError = err.message || 'Login failed. Please check your credentials.';
      } finally {
        this.loginLoading = false;
      }
    },

    handleLogout() {
      this.stopAutoRefresh();
      this.closeServicePanel();
      window.Auth.clearSession();
      this.isAuthenticated = false;
      this.loginMessage = 'You have signed out.';
      Alpine.store('toasts').show('Signed out successfully', 'info');
    },

    /* ------------------- Initial Data Loading ------------------- */
    async loadInitialData() {
      await Promise.all([
        this.loadCategories(),
        this.loadServices()
      ]);
    },

    async loadCategories() {
      this.loadingCategories = true;
      try {
        const data = await window.Api.getCategories();
        this.categories = Array.isArray(data) ? data : [];
      } catch (err) {
        Alpine.store('toasts').show(`Failed to load categories: ${err.message}`, 'error');
      } finally {
        this.loadingCategories = false;
      }
    },

    async loadServices() {
      this.loadingServices = true;
      try {
        const data = await window.Api.getServices();
        this.services = Array.isArray(data) ? data : [];
        // Trigger lazy loading of 30-day uptime strips with concurrency control
        this.services.forEach(s => this.requestUptime(s.id));
      } catch (err) {
        Alpine.store('toasts').show(`Failed to load services: ${err.message}`, 'error');
      } finally {
        this.loadingServices = false;
      }
    },

    async refreshServicesSilently() {
      if (!this.isAuthenticated) return;
      try {
        const data = await window.Api.getServices();
        if (Array.isArray(data)) {
          const map = new Map(data.map(s => [s.id, s]));
          this.services = this.services.map(existing => {
            const fresh = map.get(existing.id);
            return fresh ? { ...existing, ...fresh } : existing;
          });
          const existingIds = new Set(this.services.map(s => s.id));
          for (const s of data) {
            if (!existingIds.has(s.id)) {
              this.services.push(s);
              this.requestUptime(s.id);
            }
          }

          if (this.selectedService) {
            const updated = map.get(this.selectedService.id);
            if (updated) {
              this.selectedService = { ...this.selectedService, ...updated };
            }
          }
        }
      } catch {
        // Silent background polling
      }
    },

    startAutoRefresh() {
      this.stopAutoRefresh();
      this.refreshIntervalId = setInterval(() => {
        this.refreshServicesSilently();
        if (this.currentView === 'incidents') {
          this.loadIncidents(true);
        }
      }, 30000); // 30 seconds requirement
    },

    stopAutoRefresh() {
      if (this.refreshIntervalId) {
        clearInterval(this.refreshIntervalId);
        this.refreshIntervalId = null;
      }
    },

    /* ------------------- Filtering & Grouping ------------------- */
    get filteredServices() {
      const q = this.searchQuery.trim().toLowerCase();
      return this.services.filter(s => {
        // Status filter
        if (this.selectedStatus !== 'all') {
          const statusKey = window.Format.statusKey(s.serviceStatus);
          if (statusKey !== this.selectedStatus) return false;
        }

        // Category filter
        if (this.selectedCategory !== null && s.categoryId !== this.selectedCategory) {
          return false;
        }

        // Search by name and host
        if (q) {
          const nameMatch = s.name && s.name.toLowerCase().includes(q);
          const hostMatch = s.host && s.host.toLowerCase().includes(q);
          if (!nameMatch && !hostMatch) return false;
        }

        return true;
      });
    },

    get groupedCategories() {
      const filtered = this.filteredServices;
      const groups = [];

      const catMap = new Map();
      this.categories.forEach(c => {
        catMap.set(c.id, {
          category: c,
          services: []
        });
      });

      const uncategorized = {
        category: { id: 0, name: 'Uncategorized', color: '#64748b' },
        services: []
      };

      filtered.forEach(s => {
        if (catMap.has(s.categoryId)) {
          catMap.get(s.categoryId).services.push(s);
        } else {
          uncategorized.services.push(s);
        }
      });

      catMap.forEach(group => {
        if (group.services.length > 0) {
          groups.push(group);
        }
      });

      if (uncategorized.services.length > 0) {
        groups.push(uncategorized);
      }

      return groups;
    },

    resetFilters() {
      this.searchQuery = '';
      this.selectedCategory = null;
      this.selectedStatus = 'all';
    },

    /* ------------------- Health Check Actions ------------------- */
    async checkSingleService(service) {
      if (!service || !service.id) return;
      const id = service.id;

      this.checkingServiceIds = { ...this.checkingServiceIds, [id]: true };

      try {
        const res = await window.Api.checkService(id);
        const isOnline = res.isOnline !== undefined ? res.isOnline : res.IsOnline;
        const latency = res.responseTimeMs !== undefined ? res.responseTimeMs : res.ResponseTimeMs;
        const errMsg = res.errorMessage !== undefined ? res.errorMessage : res.ErrorMessage;

        service.serviceStatus = isOnline ? 'Online' : 'Offline';
        service.lastResponceTimeMs = latency;
        service.lastCheckedAt = new Date().toISOString();

        if (this.selectedService && this.selectedService.id === id) {
          this.selectedService = { ...service };
        }

        if (!isOnline && errMsg) {
          this.lastCheckErrors = { ...this.lastCheckErrors, [id]: errMsg };
        } else {
          const copy = { ...this.lastCheckErrors };
          delete copy[id];
          this.lastCheckErrors = copy;
        }

        // Invalidate uptime cache for this service so it refreshes with latest probe
        this.requestUptime(id, true);

        const msg = isOnline
          ? `${service.name} is online (${window.Format.responseTime(latency)})`
          : `${service.name} is offline: ${errMsg || 'probe failed'}`;
        Alpine.store('toasts').show(msg, isOnline ? 'success' : 'error');
      } catch (err) {
        Alpine.store('toasts').show(`Check failed for ${service.name}: ${err.message}`, 'error');
      } finally {
        const copy = { ...this.checkingServiceIds };
        delete copy[id];
        this.checkingServiceIds = copy;
      }
    },

    async checkAllServices() {
      if (this.isCheckingAll) return;
      this.isCheckingAll = true;

      try {
        const res = await window.Api.checkAll();
        const total = res.total ?? res.Total ?? 0;
        const online = res.online ?? res.Online ?? 0;
        const offline = res.offline ?? res.Offline ?? 0;
        const results = res.results ?? res.Results ?? [];

        if (Array.isArray(results) && results.length > 0) {
          const resMap = new Map();
          results.forEach(r => {
            const rId = r.serviceId ?? r.ServiceId;
            resMap.set(rId, r);
          });

          this.services.forEach(s => {
            const probe = resMap.get(s.id);
            if (probe) {
              const isOnline = probe.isOnline ?? probe.IsOnline;
              const latency = probe.responseTimeMs ?? probe.ResponseTimeMs;
              const errMsg = probe.errorMessage ?? probe.ErrorMessage;

              s.serviceStatus = isOnline ? 'Online' : 'Offline';
              s.lastResponceTimeMs = latency;
              s.lastCheckedAt = new Date().toISOString();

              if (!isOnline && errMsg) {
                this.lastCheckErrors[s.id] = errMsg;
              } else {
                delete this.lastCheckErrors[s.id];
              }
              this.requestUptime(s.id, true);
            }
          });
          this.lastCheckErrors = { ...this.lastCheckErrors };
        } else {
          await this.refreshServicesSilently();
        }

        Alpine.store('toasts').show(
          `Checked ${total} services: ${online} online, ${offline} offline`,
          offline > 0 ? 'info' : 'success',
          4500
        );
      } catch (err) {
        Alpine.store('toasts').show(`Bulk check failed: ${err.message}`, 'error');
      } finally {
        this.isCheckingAll = false;
      }
    },

    /* ------------------- Uptime Strip (Concurrency Limited) ------------------- */
    requestUptime(serviceId, force = false) {
      if (!serviceId) return;
      if (!force && this.uptimeCache[serviceId]) return;

      this.uptimeCache = {
        ...this.uptimeCache,
        [serviceId]: { loading: true, data: this.uptimeCache[serviceId]?.data || null, error: false }
      };

      if (!this.uptimeQueue.includes(serviceId)) {
        this.uptimeQueue.push(serviceId);
      }
      this.processUptimeQueue();
    },

    async processUptimeQueue() {
      while (this.uptimeActiveCount < this.maxUptimeConcurrency && this.uptimeQueue.length > 0) {
        const serviceId = this.uptimeQueue.shift();
        this.uptimeActiveCount++;

        this.fetchUptime(serviceId).finally(() => {
          this.uptimeActiveCount--;
          this.processUptimeQueue();
        });
      }
    },

    async fetchUptime(serviceId) {
      try {
        const data = await window.Api.getServiceUptime(serviceId, 30);
        const list = Array.isArray(data) ? data : [];
        this.uptimeCache = {
          ...this.uptimeCache,
          [serviceId]: { loading: false, data: list, error: false }
        };
      } catch {
        // Fail silently per card requirement
        this.uptimeCache = {
          ...this.uptimeCache,
          [serviceId]: { loading: false, data: [], error: true }
        };
      }
    },

    getUptimeDays(serviceId) {
      const item = this.uptimeCache[serviceId];
      if (item && item.data && item.data.length > 0) {
        return item.data;
      }
      // Return 30 placeholders for loading skeleton or empty
      return Array.from({ length: 30 }, (_, i) => ({
        dummy: true,
        date: `Day ${i + 1}`,
        totalChecks: 0,
        onlineChecks: 0,
        uptimePercentage: 0
      }));
    },

    isUptimeLoading(serviceId) {
      const item = this.uptimeCache[serviceId];
      return !item || item.loading;
    },

    getUptimeCellClass(serviceId, day) {
      if (this.isUptimeLoading(serviceId)) {
        return 'status-skeleton';
      }
      if (!day || day.totalChecks === 0) {
        return 'status-nodata';
      }
      const pct = day.uptimePercentage !== undefined ? day.uptimePercentage : 0;
      if (pct >= 99) return 'status-perfect';
      if (pct >= 90) return 'status-degraded';
      return 'status-down';
    },

    getUptimeTooltip(serviceId, day) {
      if (this.isUptimeLoading(serviceId)) {
        return 'Loading telemetry...';
      }
      if (!day || day.totalChecks === 0) {
        return `${day && day.date ? day.date : 'Day'}: No data recorded`;
      }
      const pct = window.Format.percentage(day.uptimePercentage);
      return `${day.date}: ${pct} (${day.onlineChecks}/${day.totalChecks} checks online)`;
    },

    getAverageUptime(serviceId) {
      const item = this.uptimeCache[serviceId];
      if (!item || !item.data || item.data.length === 0) return '—';
      const activeDays = item.data.filter(d => d.totalChecks > 0);
      if (activeDays.length === 0) return 'No data';
      const sum = activeDays.reduce((acc, d) => acc + (d.uptimePercentage || 0), 0);
      return window.Format.percentage(sum / activeDays.length);
    },

    /* ------------------- Side Panel: Service Details, Runbooks, Incidents ------------------- */
    openServicePanel(service, tab = 'overview') {
      this.selectedService = service;
      this.panelTab = tab;
      this.isPanelOpen = true;

      if (tab === 'runbooks') {
        this.loadServiceRunbooks(service.id);
      } else if (tab === 'incidents') {
        this.loadServiceIncidents(service.id);
      } else if (tab === 'uptime') {
        this.requestUptime(service.id);
      }
    },

    closeServicePanel() {
      this.isPanelOpen = false;
      this.selectedService = null;
    },

    switchPanelTab(tab) {
      this.panelTab = tab;
      if (!this.selectedService) return;

      if (tab === 'runbooks') {
        this.loadServiceRunbooks(this.selectedService.id);
      } else if (tab === 'incidents') {
        this.loadServiceIncidents(this.selectedService.id);
      } else if (tab === 'uptime') {
        this.requestUptime(this.selectedService.id);
      }
    },

    /* ------------------- Runbooks Management ------------------- */
    async loadServiceRunbooks(serviceId) {
      this.loadingRunbooks = true;
      try {
        const data = await window.Api.getServiceRunbooks(serviceId);
        this.serviceRunbooks = Array.isArray(data) ? data : [];
      } catch (err) {
        Alpine.store('toasts').show(`Failed to load runbooks: ${err.message}`, 'error');
      } finally {
        this.loadingRunbooks = false;
      }
    },

    openCreateRunbookModal() {
      if (!this.selectedService) return;
      this.isRunbookEditMode = false;
      this.runbookFormErrors = {};
      this.runbookForm = {
        id: null,
        serviceId: this.selectedService.id,
        title: '',
        shellType: 'Bash',
        commandText: '',
        notes: ''
      };
      this.isRunbookModalOpen = true;
    },

    openEditRunbookModal(runbook) {
      this.isRunbookEditMode = true;
      this.runbookFormErrors = {};
      this.runbookForm = {
        id: runbook.id,
        serviceId: runbook.serviceId,
        title: runbook.title || '',
        shellType: typeof runbook.shellType === 'number'
          ? (runbook.shellType === 1 ? 'Docker' : runbook.shellType === 2 ? 'PowerShell' : 'Bash')
          : (runbook.shellType || 'Bash'),
        commandText: runbook.commandText || '',
        notes: runbook.notes || ''
      };
      this.isRunbookModalOpen = true;
    },

    closeRunbookModal() {
      this.isRunbookModalOpen = false;
      this.runbookFormErrors = {};
    },

    validateRunbookForm() {
      const errs = {};
      const f = this.runbookForm;

      if (!f.title || !f.title.trim()) {
        errs.title = 'Title is required.';
      } else if (f.title.length > 150) {
        errs.title = 'Title cannot exceed 150 characters.';
      }

      if (!f.commandText || !f.commandText.trim()) {
        errs.commandText = 'Command text is required.';
      } else if (f.commandText.length > 500) {
        errs.commandText = 'Command text cannot exceed 500 characters.';
      }

      if (f.notes && f.notes.length > 500) {
        errs.notes = 'Notes cannot exceed 500 characters.';
      }

      this.runbookFormErrors = errs;
      return Object.keys(errs).length === 0;
    },

    async submitRunbookForm() {
      if (!this.validateRunbookForm()) return;

      this.savingRunbook = true;
      this.runbookFormErrors = {};

      const dto = {
        title: this.runbookForm.title.trim(),
        shellType: this.runbookForm.shellType,
        commandText: this.runbookForm.commandText.trim(),
        notes: this.runbookForm.notes ? this.runbookForm.notes.trim() : null
      };

      try {
        if (this.isRunbookEditMode) {
          await window.Api.updateRunbook(this.runbookForm.id, dto);
          Alpine.store('toasts').show(`Runbook "${dto.title}" updated`, 'success');
        } else {
          dto.serviceId = this.selectedService.id;
          await window.Api.createRunbook(dto);
          Alpine.store('toasts').show(`Runbook "${dto.title}" added`, 'success');
        }
        this.closeRunbookModal();
        if (this.selectedService) {
          await this.loadServiceRunbooks(this.selectedService.id);
        }
      } catch (err) {
        if (err.errors && typeof err.errors === 'object') {
          const mapped = {};
          for (const key of Object.keys(err.errors)) {
            const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
            mapped[camelKey] = err.getFieldError(key);
          }
          this.runbookFormErrors = mapped;
        } else {
          this.runbookFormErrors = { general: err.message };
        }
      } finally {
        this.savingRunbook = false;
      }
    },

    async deleteRunbook(runbook) {
      if (!runbook || !confirm(`Delete runbook "${runbook.title}"?`)) return;

      this.deletingRunbookId = runbook.id;
      try {
        await window.Api.deleteRunbook(runbook.id);
        this.serviceRunbooks = this.serviceRunbooks.filter(r => r.id !== runbook.id);
        Alpine.store('toasts').show(`Runbook "${runbook.title}" deleted`, 'info');
      } catch (err) {
        Alpine.store('toasts').show(`Failed to delete runbook: ${err.message}`, 'error');
      } finally {
        this.deletingRunbookId = null;
      }
    },

    async copyCommand(runbook) {
      if (!runbook || !runbook.commandText) return;
      try {
        await navigator.clipboard.writeText(runbook.commandText);
        this.copiedRunbookId = runbook.id;
        setTimeout(() => {
          if (this.copiedRunbookId === runbook.id) {
            this.copiedRunbookId = null;
          }
        }, 2000);
        Alpine.store('toasts').show('Command copied to clipboard', 'info', 2000);
      } catch {
        Alpine.store('toasts').show('Failed to copy to clipboard', 'error');
      }
    },

    /* ------------------- Service Incidents & Global Incidents ------------------- */
    async loadServiceIncidents(serviceId) {
      this.loadingServiceIncidents = true;
      try {
        const data = await window.Api.getServiceIncidents(serviceId);
        this.serviceIncidents = Array.isArray(data) ? data : [];
      } catch (err) {
        Alpine.store('toasts').show(`Failed to load service incidents: ${err.message}`, 'error');
      } finally {
        this.loadingServiceIncidents = false;
      }
    },

    async loadIncidents(silent = false) {
      if (!silent) this.loadingIncidents = true;
      try {
        const data = await window.Api.getIncidents(this.incidentsOnlyOpen);
        this.incidents = Array.isArray(data) ? data : [];
      } catch (err) {
        if (!silent) {
          Alpine.store('toasts').show(`Failed to load incidents: ${err.message}`, 'error');
        }
      } finally {
        if (!silent) this.loadingIncidents = false;
      }
    },

    /* ------------------- Service CRUD Modal ------------------- */
    openCreateServiceModal() {
      this.isEditMode = false;
      this.serviceFormErrors = {};
      const defaultCatId = this.categories.length > 0 ? this.categories[0].id : '';

      this.serviceForm = {
        id: null,
        name: '',
        host: '',
        port: 80,
        useHttps: false,
        checkType: 'Http',
        healthPath: '/health',
        timeoutMs: 5000,
        isMonitoringEnabled: true,
        description: '',
        icon: 'server',
        categoryId: defaultCatId
      };
      this.isServiceModalOpen = true;
    },

    openEditServiceModal(service) {
      this.isEditMode = true;
      this.serviceFormErrors = {};
      this.serviceForm = {
        id: service.id,
        name: service.name || '',
        host: service.host || '',
        port: service.port || 80,
        useHttps: !!service.useHttps,
        checkType: typeof service.checkType === 'number'
          ? (service.checkType === 2 ? 'Tcp' : 'Http')
          : (service.checkType || 'Http'),
        healthPath: service.healthPath || '',
        timeoutMs: service.timeoutMs || 5000,
        isMonitoringEnabled: service.isMonitoringEnabled !== false,
        description: service.description || '',
        icon: service.icon || '',
        categoryId: service.categoryId || ''
      };
      this.isServiceModalOpen = true;
    },

    closeServiceModal() {
      this.isServiceModalOpen = false;
      this.serviceFormErrors = {};
    },

    validateServiceForm() {
      const errs = {};
      const f = this.serviceForm;

      if (!f.name || !f.name.trim()) {
        errs.name = 'Service name is required.';
      } else if (f.name.length > 100) {
        errs.name = 'Service name cannot exceed 100 characters.';
      }

      if (!f.host || !f.host.trim()) {
        errs.host = 'Host is required.';
      } else if (f.host.length > 255) {
        errs.host = 'Host cannot exceed 255 characters.';
      }

      const portNum = Number(f.port);
      if (isNaN(portNum) || portNum < 1 || portNum > 65535) {
        errs.port = 'Port must be between 1 and 65535.';
      }

      const timeoutNum = Number(f.timeoutMs);
      if (isNaN(timeoutNum) || timeoutNum < 500 || timeoutNum > 30000) {
        errs.timeoutMs = 'Timeout must be between 500 and 30000 ms.';
      }

      if (f.healthPath && f.healthPath.length > 255) {
        errs.healthPath = 'Health path cannot exceed 255 characters.';
      }

      if (f.description && f.description.length > 500) {
        errs.description = 'Description cannot exceed 500 characters.';
      }

      if (f.icon && f.icon.length > 50) {
        errs.icon = 'Icon identifier cannot exceed 50 characters.';
      }

      const catNum = Number(f.categoryId);
      if (!catNum || catNum <= 0) {
        errs.categoryId = 'Please select a valid category.';
      }

      this.serviceFormErrors = errs;
      return Object.keys(errs).length === 0;
    },

    async submitServiceForm() {
      if (!this.validateServiceForm()) return;

      this.savingService = true;
      this.serviceFormErrors = {};

      const dto = {
        name: this.serviceForm.name.trim(),
        host: this.serviceForm.host.trim(),
        port: Number(this.serviceForm.port),
        useHttps: !!this.serviceForm.useHttps,
        checkType: this.serviceForm.checkType,
        healthPath: this.serviceForm.healthPath ? this.serviceForm.healthPath.trim() : null,
        timeoutMs: Number(this.serviceForm.timeoutMs),
        isMonitoringEnabled: !!this.serviceForm.isMonitoringEnabled,
        description: this.serviceForm.description ? this.serviceForm.description.trim() : null,
        icon: this.serviceForm.icon ? this.serviceForm.icon.trim() : null,
        categoryId: Number(this.serviceForm.categoryId)
      };

      try {
        if (this.isEditMode) {
          await window.Api.updateService(this.serviceForm.id, dto);
          Alpine.store('toasts').show(`Service "${dto.name}" updated`, 'success');
        } else {
          await window.Api.createService(dto);
          Alpine.store('toasts').show(`Service "${dto.name}" created`, 'success');
        }
        this.closeServiceModal();
        await this.loadServices();
      } catch (err) {
        if (err.errors && typeof err.errors === 'object') {
          const mapped = {};
          for (const key of Object.keys(err.errors)) {
            const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
            mapped[camelKey] = err.getFieldError(key);
          }
          this.serviceFormErrors = mapped;
        } else {
          this.serviceFormErrors = { general: err.message };
        }
      } finally {
        this.savingService = false;
      }
    },

    /* ------------------- Service Delete Confirmation ------------------- */
    confirmDeleteService(service) {
      this.serviceToDelete = service;
      this.isDeleteModalOpen = true;
    },

    closeDeleteModal() {
      this.isDeleteModalOpen = false;
      this.serviceToDelete = null;
    },

    async executeDeleteService() {
      if (!this.serviceToDelete) return;

      this.deletingService = true;
      const id = this.serviceToDelete.id;
      const name = this.serviceToDelete.name;

      try {
        await window.Api.deleteService(id);
        this.services = this.services.filter(s => s.id !== id);
        if (this.selectedService && this.selectedService.id === id) {
          this.closeServicePanel();
        }
        Alpine.store('toasts').show(`Service "${name}" deleted`, 'info');
        this.closeDeleteModal();
      } catch (err) {
        Alpine.store('toasts').show(`Failed to delete service: ${err.message}`, 'error');
      } finally {
        this.deletingService = false;
      }
    }
  }));
});
