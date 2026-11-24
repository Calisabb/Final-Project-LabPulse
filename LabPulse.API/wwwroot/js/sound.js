/**
 * LabPulse Cyber-Terminal Synthesized Web Audio Engine
 * Pure Web Audio API synthesis: zero external audio files, zero latency, ultra-lightweight.
 */
(function (window, document) {
  'use strict';

  class SoundEngine {
    constructor() {
      this.ctx = null;
      this.enabled = localStorage.getItem('labpulse_sfx_enabled') !== 'false';
      this.masterVolume = 0.16; // Soft, comfortable ambient level
    }

    _initContext() {
      if (!this.ctx) {
        const AudioContextClass = window.AudioContext || window.webkitAudioContext;
        if (AudioContextClass) {
          this.ctx = new AudioContextClass();
        }
      }
      if (this.ctx && this.ctx.state === 'suspended') {
        this.ctx.resume().catch(() => {});
      }
    }

    get isEnabled() {
      return this.enabled;
    }

    toggle() {
      this.enabled = !this.enabled;
      localStorage.setItem('labpulse_sfx_enabled', this.enabled ? 'true' : 'false');
      if (this.enabled) {
        this._initContext();
        this.ping();
      }
      return this.enabled;
    }

    setEnabled(value) {
      this.enabled = !!value;
      localStorage.setItem('labpulse_sfx_enabled', this.enabled ? 'true' : 'false');
    }

    /* ------------------- Synthesized Sound Primitives ------------------- */

    /**
     * Tactile Cyber Mechanical Click (Buttons, Tabs, Segmented Filters)
     */
    click() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc.type = 'sine';
      osc.frequency.setValueAtTime(1400, now);
      osc.frequency.exponentialRampToValueAtTime(320, now + 0.035);

      gain.gain.setValueAtTime(this.masterVolume * 0.65, now);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.035);

      osc.connect(gain);
      gain.connect(this.ctx.destination);

      osc.start(now);
      osc.stop(now + 0.038);
    }

    /**
     * Subtle Keypress Tick (Terminal Search & Input Typing)
     */
    key() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc.type = 'triangle';
      osc.frequency.setValueAtTime(750 + Math.random() * 220, now);
      osc.frequency.exponentialRampToValueAtTime(260, now + 0.02);

      gain.gain.setValueAtTime(this.masterVolume * 0.28, now);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.02);

      osc.connect(gain);
      gain.connect(this.ctx.destination);

      osc.start(now);
      osc.stop(now + 0.022);
    }

    /**
     * High-Tech Double Chirp (Runbook Command Copied to Clipboard)
     */
    copy() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;

      // Chirp 1
      const osc1 = this.ctx.createOscillator();
      const gain1 = this.ctx.createGain();
      osc1.type = 'sine';
      osc1.frequency.setValueAtTime(1200, now);
      osc1.frequency.exponentialRampToValueAtTime(1650, now + 0.032);

      gain1.gain.setValueAtTime(this.masterVolume * 0.75, now);
      gain1.gain.exponentialRampToValueAtTime(0.001, now + 0.032);

      osc1.connect(gain1);
      gain1.connect(this.ctx.destination);
      osc1.start(now);
      osc1.stop(now + 0.035);

      // Chirp 2 (higher octave confirm)
      const osc2 = this.ctx.createOscillator();
      const gain2 = this.ctx.createGain();
      osc2.type = 'sine';
      osc2.frequency.setValueAtTime(1650, now + 0.045);
      osc2.frequency.exponentialRampToValueAtTime(2400, now + 0.09);

      gain2.gain.setValueAtTime(this.masterVolume * 0.95, now + 0.045);
      gain2.gain.exponentialRampToValueAtTime(0.001, now + 0.09);

      osc2.connect(gain2);
      gain2.connect(this.ctx.destination);
      osc2.start(now + 0.045);
      osc2.stop(now + 0.095);
    }

    /**
     * Telemetry Sonar Ping (Single node probe test [PING])
     */
    ping() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc.type = 'sine';
      osc.frequency.setValueAtTime(980, now);
      osc.frequency.exponentialRampToValueAtTime(860, now + 0.12);

      gain.gain.setValueAtTime(this.masterVolume * 0.65, now);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.12);

      osc.connect(gain);
      gain.connect(this.ctx.destination);

      osc.start(now);
      osc.stop(now + 0.13);
    }

    /**
     * Radar Grid Sweep (Batch Refresh / [SWEEP ALL])
     */
    sweep() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc.type = 'triangle';
      osc.frequency.setValueAtTime(320, now);
      osc.frequency.linearRampToValueAtTime(940, now + 0.12);
      osc.frequency.exponentialRampToValueAtTime(520, now + 0.22);

      gain.gain.setValueAtTime(0.01, now);
      gain.gain.linearRampToValueAtTime(this.masterVolume * 0.75, now + 0.08);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.22);

      osc.connect(gain);
      gain.connect(this.ctx.destination);

      osc.start(now);
      osc.stop(now + 0.23);
    }

    /**
     * Hazard Klaxon (Opening Delete Confirmation Modal)
     */
    warn() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc1 = this.ctx.createOscillator();
      const osc2 = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc1.type = 'sawtooth';
      osc2.type = 'sawtooth';

      osc1.frequency.setValueAtTime(260, now);
      osc2.frequency.setValueAtTime(276, now);

      gain.gain.setValueAtTime(this.masterVolume * 0.45, now);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.16);

      osc1.connect(gain);
      osc2.connect(gain);
      gain.connect(this.ctx.destination);

      osc1.start(now);
      osc2.start(now);
      osc1.stop(now + 0.17);
      osc2.stop(now + 0.17);
    }

    /**
     * Deep Electronic Purge Impact (Node Deleted Confirmation)
     */
    purge() {
      if (!this.enabled) return;
      this._initContext();
      if (!this.ctx) return;

      const now = this.ctx.currentTime;
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();

      osc.type = 'sine';
      osc.frequency.setValueAtTime(220, now);
      osc.frequency.exponentialRampToValueAtTime(38, now + 0.22);

      gain.gain.setValueAtTime(this.masterVolume * 0.9, now);
      gain.gain.exponentialRampToValueAtTime(0.001, now + 0.22);

      osc.connect(gain);
      gain.connect(this.ctx.destination);

      osc.start(now);
      osc.stop(now + 0.24);
    }
  }

  // Export singleton
  window.Sound = new SoundEngine();

  // Attach global unlock listener on first user interaction
  const unlockAudio = () => {
    window.Sound._initContext();
    window.removeEventListener('pointerdown', unlockAudio);
    window.removeEventListener('keydown', unlockAudio);
  };
  window.addEventListener('pointerdown', unlockAudio, { once: true });
  window.addEventListener('keydown', unlockAudio, { once: true });

  // Delegated UI Click Listener: automatic tactile clicks for all interactive buttons
  document.addEventListener('click', (e) => {
    const btn = e.target.closest('button, [role="button"], a.btn, .edex-cluster-btn, .edex-key-btn, .edex-tab-btn, .status-filter-btn, .quick-cmd-chip');
    if (!btn) return;
    if (btn.classList.contains('edex-sfx-toggle-btn')) return; // Has its own ping
    if (btn.classList.contains('edex-confirm-purge-btn')) return; // Handled by purge()
    if (btn.hasAttribute('data-no-sfx')) return;

    // Default tactile click
    window.Sound.click();
  }, { passive: true });

  // Delegated Typing Listener: mechanical keyboard tick for inputs
  document.addEventListener('keydown', (e) => {
    const input = e.target.closest('input[type="text"], input[type="search"], textarea');
    if (!input) return;
    if (['Shift', 'Control', 'Alt', 'Meta', 'CapsLock', 'Tab'].includes(e.key)) return;
    window.Sound.key();
  }, { passive: true });

})(window, document);
