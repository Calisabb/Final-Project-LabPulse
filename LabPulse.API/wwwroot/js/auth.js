/**
 * LabPulse Authentication Service
 * Manages JWT token and user session in sessionStorage.
 */
const Auth = {
  STORAGE_KEY: 'labpulse_session',

  /**
   * Retrieve current session object from sessionStorage.
   * @returns {{ token: string, userName: string, expiresAt: string } | null}
   */
  getSession() {
    try {
      const raw = sessionStorage.getItem(this.STORAGE_KEY);
      if (!raw) return null;
      const session = JSON.parse(raw);
      if (!session || !session.token) {
        this.clearSession();
        return null;
      }
      // Check expiration if provided
      if (session.expiresAt) {
        const expiry = new Date(session.expiresAt).getTime();
        if (!isNaN(expiry) && expiry <= Date.now()) {
          this.clearSession();
          return null;
        }
      }
      return session;
    } catch {
      this.clearSession();
      return null;
    }
  },

  /**
   * Get raw JWT bearer token.
   * @returns {string|null}
   */
  getToken() {
    const session = this.getSession();
    return session ? session.token : null;
  },

  /**
   * Get currently authenticated username.
   * @returns {string}
   */
  getUserName() {
    const session = this.getSession();
    return session && session.userName ? session.userName : 'Operator';
  },

  /**
   * Returns true if a valid session exists.
   * @returns {boolean}
   */
  isAuthenticated() {
    return !!this.getToken();
  },

  /**
   * Store new login session and notify listeners.
   * @param {{ token: string, userName: string, expiresAt: string }} data
   */
  setSession(data) {
    if (!data || !data.token) return;
    sessionStorage.setItem(this.STORAGE_KEY, JSON.stringify({
      token: data.token,
      userName: data.userName || 'Operator',
      expiresAt: data.expiresAt || null
    }));
    window.dispatchEvent(new CustomEvent('auth:change', {
      detail: { isAuthenticated: true, userName: data.userName }
    }));
  },

  /**
   * Clear session from sessionStorage and notify listeners.
   * @param {string} [reason]
   */
  clearSession(reason) {
    sessionStorage.removeItem(this.STORAGE_KEY);
    window.dispatchEvent(new CustomEvent('auth:change', {
      detail: { isAuthenticated: false, reason: reason || null }
    }));
  }
};

window.Auth = Auth;
