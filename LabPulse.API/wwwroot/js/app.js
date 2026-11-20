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
          this.startAutoRefresh();
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
          // Merge updates without full UI flicker
          const map = new Map(data.map(s => [s.id, s]));
          this.services = this.services.map(existing => {
            const fresh = map.get(existing.id);
            return fresh ? { ...existing, ...fresh } : existing;
          });
          // Add any new services
          const existingIds = new Set(this.services.map(s => s.id));
          for (const s of data) {
            if (!existingIds.has(s.id)) {
              this.services.push(s);
            }
          }
        }
      } catch {
        // Silent failure on background polling
      }
    },

    startAutoRefresh() {
      this.stopAutoRefresh();
      this.refreshIntervalId = setInterval(() => {
        this.refreshServicesSilently();
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

      // Create a map of categories
      const catMap = new Map();
      this.categories.forEach(c => {
        catMap.set(c.id, {
          category: c,
          services: []
        });
      });

      // Default ungrouped category for orphan services if any
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

      // Include categories that have matching services, or all categories if no search is active
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
          // Map backend field errors to form fields
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
