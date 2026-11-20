/**
 * LabPulse API Client
 * Centralized fetch wrapper handling JWT authentication, ProblemDetails parsing, and 401 interceptor.
 */
class ApiError extends Error {
  constructor(message, status, title = '', detail = '', errors = null) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.errors = errors || {};
  }

  /**
   * Get first error message for a specific field name (case-insensitive).
   * @param {string} fieldName
   * @returns {string|null}
   */
  getFieldError(fieldName) {
    if (!this.errors) return null;
    const lower = fieldName.toLowerCase();
    for (const key of Object.keys(this.errors)) {
      if (key.toLowerCase() === lower) {
        const msgs = this.errors[key];
        return Array.isArray(msgs) ? msgs[0] : String(msgs);
      }
    }
    return null;
  }
}

const Api = {
  /**
   * Low-level fetch wrapper.
   * @param {string} url
   * @param {RequestInit} [options]
   * @returns {Promise<any>}
   */
  async request(url, options = {}) {
    const headers = new Headers(options.headers || {});

    // Ensure content type if body is present
    if (options.body && !headers.has('Content-Type') && typeof options.body === 'string') {
      headers.set('Content-Type', 'application/json');
    }

    // Attach JWT bearer token if available
    const token = window.Auth ? window.Auth.getToken() : null;
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }

    const config = {
      ...options,
      headers
    };

    let response;
    try {
      response = await fetch(url, config);
    } catch (networkErr) {
      throw new ApiError(
        'Unable to connect to the server. Check your network connection.',
        0,
        'Network Error',
        networkErr.message
      );
    }

    // Handle 401 Unauthorized globally
    if (response.status === 401) {
      if (window.Auth) {
        window.Auth.clearSession('Session expired or unauthorized. Please sign in again.');
      }
      throw new ApiError('Session expired or unauthorized. Please log in again.', 401, 'Unauthorized');
    }

    // Parse response body
    const contentType = response.headers.get('content-type') || '';
    const isJson = contentType.includes('application/json') || contentType.includes('application/problem+json');

    let bodyData = null;
    if (response.status !== 204) {
      try {
        bodyData = isJson ? await response.json() : await response.text();
      } catch {
        bodyData = null;
      }
    }

    if (!response.ok) {
      let title = response.statusText || 'Error';
      let detail = '';
      let errors = null;
      let displayMessage = `Request failed (${response.status})`;

      if (bodyData && typeof bodyData === 'object') {
        title = bodyData.title || title;
        detail = bodyData.detail || '';
        errors = bodyData.errors || null;

        if (errors && typeof errors === 'object') {
          // ValidationProblemDetails
          const messages = [];
          for (const key of Object.keys(errors)) {
            const errList = errors[key];
            if (Array.isArray(errList)) {
              messages.push(...errList);
            } else if (errList) {
              messages.push(String(errList));
            }
          }
          if (messages.length > 0) {
            displayMessage = messages.join('. ');
          }
        } else if (detail) {
          displayMessage = detail;
        } else if (title) {
          displayMessage = title;
        }
      }

      throw new ApiError(displayMessage, response.status, title, detail, errors);
    }

    return bodyData;
  },

  /**
   * Request with automatic URL fallback (e.g. for backend controller routes vs spec routes).
   * @param {string} primaryUrl
   * @param {string} fallbackUrl
   * @param {RequestInit} [options]
   */
  async requestWithFallback(primaryUrl, fallbackUrl, options = {}) {
    try {
      return await this.request(primaryUrl, options);
    } catch (err) {
      if (err.status === 404 && fallbackUrl && fallbackUrl !== primaryUrl) {
        return await this.request(fallbackUrl, options);
      }
      throw err;
    }
  },

  /* ------------------- Auth ------------------- */
  async login(userName, password) {
    return await this.request('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ userName, password })
    });
  },

  /* ------------------- Categories ------------------- */
  async getCategories() {
    return await this.request('/api/Categories');
  },

  /* ------------------- Monitored Services ------------------- */
  async getServices(params = {}) {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    if (params.categoryId) query.set('categoryId', params.categoryId);
    if (params.search) query.set('search', params.search);

    const qs = query.toString();
    const url = qs ? `/api/Services?${qs}` : '/api/Services';
    return await this.request(url);
  },

  async getService(id) {
    return await this.request(`/api/Services/${id}`);
  },

  async createService(dto) {
    return await this.request('/api/Services', {
      method: 'POST',
      body: JSON.stringify(dto)
    });
  },

  async updateService(id, dto) {
    return await this.request(`/api/Services/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto)
    });
  },

  async deleteService(id) {
    return await this.request(`/api/Services/${id}`, {
      method: 'DELETE'
    });
  },

  /* ------------------- Health Checks ------------------- */
  async checkService(serviceId) {
    // HealthChecksController route is /api/HealthChecks/services/{id}/check; spec route is /api/services/{id}/check
    return await this.requestWithFallback(
      `/api/HealthChecks/services/${serviceId}/check`,
      `/api/services/${serviceId}/check`,
      { method: 'POST' }
    );
  },

  async checkAll() {
    // HealthChecksController route is /api/HealthChecks/checks; spec route is /api/checks
    return await this.requestWithFallback(
      '/api/HealthChecks/checks',
      '/api/checks',
      { method: 'POST' }
    );
  },

  /* ------------------- Runbooks ------------------- */
  async getServiceRunbooks(serviceId) {
    return await this.request(`/api/services/${serviceId}/runbooks`);
  },

  async createRunbook(dto) {
    return await this.request('/api/runbooks', {
      method: 'POST',
      body: JSON.stringify(dto)
    });
  },

  async updateRunbook(id, dto) {
    return await this.request(`/api/runbooks/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto)
    });
  },

  async deleteRunbook(id) {
    return await this.request(`/api/runbooks/${id}`, {
      method: 'DELETE'
    });
  },

  /* ------------------- Incidents ------------------- */
  async getIncidents(onlyOpen = false) {
    const qs = onlyOpen ? '?onlyOpen=true' : '';
    // IncidentsController route is /api/Incidents/incidents; spec route is /api/incidents
    return await this.requestWithFallback(
      `/api/Incidents/incidents${qs}`,
      `/api/incidents${qs}`
    );
  },

  async getServiceIncidents(serviceId) {
    // IncidentsController route is /api/Incidents/services/{id}/incidents; spec route is /api/services/{id}/incidents
    return await this.requestWithFallback(
      `/api/Incidents/services/${serviceId}/incidents`,
      `/api/services/${serviceId}/incidents`
    );
  },

  /* ------------------- Health Logs & Uptime ------------------- */
  async getServiceLogs(serviceId, count = 50) {
    return await this.request(`/api/services/${serviceId}/logs?count=${count}`);
  },

  async getServicePeriodLogs(serviceId, from, to) {
    const fromIso = typeof from === 'string' ? from : from.toISOString();
    const toIso = typeof to === 'string' ? to : to.toISOString();
    return await this.request(`/api/services/${serviceId}/logs/period?from=${encodeURIComponent(fromIso)}&to=${encodeURIComponent(toIso)}`);
  },

  async getServiceUptime(serviceId, days = 30) {
    return await this.request(`/api/services/${serviceId}/uptime?days=${days}`);
  }
};

window.Api = Api;
window.ApiError = ApiError;
