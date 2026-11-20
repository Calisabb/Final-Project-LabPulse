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

    init() {
      // Listen for session changes (e.g. 401 expiry or manual logout)
      window.addEventListener('auth:change', (e) => {
        this.isAuthenticated = e.detail.isAuthenticated;
        this.userName = e.detail.userName || 'Operator';
        if (!this.isAuthenticated && e.detail.reason) {
          this.loginMessage = e.detail.reason;
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
      } catch (err) {
        this.loginError = err.message || 'Login failed. Please check your credentials.';
      } finally {
        this.loginLoading = false;
      }
    },

    handleLogout() {
      window.Auth.clearSession();
      this.isAuthenticated = false;
      this.loginMessage = 'You have signed out.';
      Alpine.store('toasts').show('Signed out successfully', 'info');
    }
  }));
});
