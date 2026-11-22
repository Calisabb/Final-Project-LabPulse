/**
 * LabPulse Formatting Utilities
 * Handles formatting of timestamps, durations, telemetry, and status indicators.
 */
const Format = {
  /**
   * Parse an API date value as UTC. The API returns ISO strings without a
   * trailing 'Z', so JavaScript would otherwise interpret them as local time.
   * @param {string|Date|null} dateVal
   * @returns {Date}
   */
  parseApiDate(dateVal) {
    if (dateVal instanceof Date) return dateVal;
    if (typeof dateVal === 'string' && dateVal.length > 0) {
      const hasOffset = dateVal.endsWith('Z') || /[+\-]\d{2}:\d{2}$/.test(dateVal);
      return new Date(hasOffset ? dateVal : dateVal + 'Z');
    }
    return new Date(dateVal);
  },

  /**
   * Format ISO date string into readable date and time.
   * @param {string|Date|null} dateVal
   * @returns {string}
   */
  dateTime(dateVal) {
    if (!dateVal) return 'Never';
    const date = this.parseApiDate(dateVal);
    if (isNaN(date.getTime())) return 'Invalid date';
    return date.toLocaleString(undefined, {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  },

  /**
   * Format relative time (e.g., "just now", "5m ago", "2h ago").
   * @param {string|Date|null} dateVal
   * @returns {string}
   */
  relativeTime(dateVal) {
    if (!dateVal) return 'Never';
    const date = this.parseApiDate(dateVal);
    if (isNaN(date.getTime())) return 'Unknown';

    const now = Date.now();
    const diffMs = now - date.getTime();
    if (diffMs < 0) return 'Just now';

    const diffSec = Math.floor(diffMs / 1000);
    if (diffSec < 15) return 'Just now';
    if (diffSec < 60) return `${diffSec}s ago`;

    const diffMin = Math.floor(diffSec / 60);
    if (diffMin < 60) return `${diffMin}m ago`;

    const diffHours = Math.floor(diffMin / 60);
    if (diffHours < 24) return `${diffHours}h ago`;

    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays}d ago`;
  },

  /**
   * Format duration in seconds into human-readable string.
   * @param {number} seconds
   * @returns {string}
   */
  duration(seconds) {
    if (seconds == null || isNaN(seconds) || seconds < 0) return '0s';
    const sec = Math.floor(seconds);
    if (sec < 60) return `${sec}s`;

    const mins = Math.floor(sec / 60);
    const remSec = sec % 60;
    if (mins < 60) {
      return remSec > 0 ? `${mins}m ${remSec}s` : `${mins}m`;
    }

    const hours = Math.floor(mins / 60);
    const remMins = mins % 60;
    if (hours < 24) {
      return remMins > 0 ? `${hours}h ${remMins}m` : `${hours}h`;
    }

    const days = Math.floor(hours / 24);
    const remHours = hours % 24;
    return remHours > 0 ? `${days}d ${remHours}h` : `${days}d`;
  },

  /**
   * Format response time in milliseconds.
   * @param {number|null} ms
   * @returns {string}
   */
  responseTime(ms) {
    if (ms == null) return '—';
    return `${Math.round(ms)} ms`;
  },

  /**
   * Format percentage to 1 decimal place.
   * @param {number|null} pct
   * @returns {string}
   */
  percentage(pct) {
    if (pct == null || isNaN(pct)) return '0.0%';
    return `${Number(pct).toFixed(1)}%`;
  },

  /**
   * Normalize ServiceStatus enum (string or integer) to canonical string.
   * Unknown = 0, Online = 1, Offline = 2
   * @param {string|number|null} status
   * @returns {'online'|'offline'|'unknown'}
   */
  statusKey(status) {
    if (status == null) return 'unknown';
    if (status === 1 || String(status).toLowerCase() === 'online') return 'online';
    if (status === 2 || String(status).toLowerCase() === 'offline') return 'offline';
    return 'unknown';
  },

  /**
   * Human-readable label for status.
   * @param {string|number|null} status
   * @returns {string}
   */
  statusLabel(status) {
    const key = this.statusKey(status);
    switch (key) {
      case 'online': return 'Online';
      case 'offline': return 'Offline';
      default: return 'Unknown';
    }
  },

  /**
   * ShellType to readable name and badge label.
   * Docker = 1, PowerShell = 2, Bash = 3
   * @param {string|number|null} shell
   * @returns {string}
   */
  shellTypeName(shell) {
    if (shell == null) return 'Bash';
    if (shell === 1 || String(shell).toLowerCase() === 'docker') return 'Docker';
    if (shell === 2 || String(shell).toLowerCase() === 'powershell') return 'PowerShell';
    if (shell === 3 || String(shell).toLowerCase() === 'bash') return 'Bash';
    return String(shell);
  },

  /**
   * Build complete endpoint URL string.
   * @param {Object} service
   * @returns {string}
   */
  endpoint(service) {
    if (!service) return '';
    const protocol = service.useHttps ? 'https' : 'http';
    const port = service.port ? `:${service.port}` : '';
    const path = service.healthPath ? (service.healthPath.startsWith('/') ? service.healthPath : `/${service.healthPath}`) : '';
    return `${protocol}://${service.host}${port}${path}`;
  }
};

window.Format = Format;
