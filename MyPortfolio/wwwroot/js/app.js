/* ==========================================================================
   MyPortfolio — app.js
   Framework-free interaction layer. Every behaviour degrades gracefully:
   markup is usable with JS disabled, JS only adds polish.
   ========================================================================== */

(function () {
  'use strict';

  const root = document.documentElement;
  root.classList.add('js');

  const prefersReduced = () => window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const isMobile = () => window.matchMedia('(max-width: 63.99em)').matches;
  const lowEnd = () => (navigator.hardwareConcurrency || 4) <= 4;

  const qs = (sel, ctx) => (ctx || document).querySelector(sel);
  const qsa = (sel, ctx) => Array.from((ctx || document).querySelectorAll(sel));

  /* ------------------------------------------------------------------
   * Theme
   * ------------------------------------------------------------------ */
  const THEME_KEY = 'mp:theme';

  const Theme = {
    current() {
      return root.getAttribute('data-theme') || 'light';
    },
    apply(mode) {
      root.setAttribute('data-theme', mode);
      const meta = qs('meta[name="theme-color"]');
      if (meta) {
        meta.setAttribute('content', mode === 'dark' ? '#141311' : '#faf9f5');
      }
      qsa('[data-theme-toggle]').forEach((btn) => {
        btn.setAttribute('aria-pressed', String(mode === 'dark'));
        const label = qs('.theme-btn__state', btn);
        if (label) label.textContent = mode === 'dark' ? 'Dark' : 'Light';
      });
      window.dispatchEvent(new CustomEvent('mp:theme', { detail: { mode } }));
    },
    set(mode) {
      try { localStorage.setItem(THEME_KEY, mode); } catch { /* private mode */ }
      this.apply(mode);
    },
    toggle() {
      this.set(this.current() === 'dark' ? 'light' : 'dark');
    },
    init() {
      qsa('[data-theme-toggle]').forEach((btn) => {
        btn.addEventListener('click', () => Theme.toggle());
      });
      this.apply(this.current());
    }
  };

  const Reveal = {
    observer: null,
    ensure() {
      if (prefersReduced() || lowEnd()) {
        qsa('[data-reveal], [data-stagger]').forEach((el) => el.classList.add('is-revealed'));
        return;
      }
      if (!this.observer) {
        this.observer = new IntersectionObserver((entries) => {
          entries.forEach((entry) => {
            if (!entry.isIntersecting) return;
            entry.target.classList.add('is-revealed');
            this.observer.unobserve(entry.target);
          });
        }, { rootMargin: '0px 0px -8% 0px', threshold: 0.05 });
      }
      qsa('[data-reveal]:not(.is-revealed), [data-stagger]:not(.is-revealed)').forEach((el, idx) => {
        if (!el.style.getPropertyValue('--i')) {
          el.style.setProperty('--i', String(Math.min(idx % 8, 7)));
        }
        this.observer.observe(el);
      });
    },
    indexGroups(ctx) {
      qsa('[data-stagger]', ctx).forEach((group) => {
        Array.from(group.children).forEach((child, i) => {
          if (!child.style.getPropertyValue('--i')) child.style.setProperty('--i', String(Math.min(i, 12)));
        });
      });
    }
  };

  const Numbers = {
    io: null,
    ensure() {
      const targets = qsa('[data-bar],[data-count]');
      if (!targets.length) return;
      if (!this.io) {
        this.io = new IntersectionObserver((entries) => {
          entries.forEach((entry) => {
            if (!entry.isIntersecting) return;
            this.paint(entry.target);
            this.io.unobserve(entry.target);
          });
        }, { threshold: 0.25 });
      }
      targets.forEach((el) => { if (!el.dataset.painted) this.io.observe(el); });
    },
    paint(el) {
      el.dataset.painted = '1';
      if (el.hasAttribute('data-bar')) {
        const value = el.getAttribute('data-value') || '0';
        el.style.setProperty('--value', value + '%');
        return;
      }
      const to = parseFloat(el.getAttribute('data-count'));
      if (Number.isNaN(to)) return;
      const suffix = el.getAttribute('data-suffix') || '';
      if (prefersReduced()) {
        el.textContent = to + suffix;
        return;
      }
      const dur = 750;
      const t0 = performance.now();
      const step = (now) => {
        const p = Math.min((now - t0) / dur, 1);
        const eased = 1 - Math.pow(1 - p, 3);
        el.textContent = Math.round(to * eased) + suffix;
        if (p < 1) requestAnimationFrame(step);
      };
      requestAnimationFrame(step);
    }
  };

  const Dialogs = {
    open(id, params) {
      const el = qs(id);
      if (!el || typeof el.showModal !== 'function') return null;
      if (params) {
        qsa('[data-dst]', el).forEach((slot) => {
          const key = slot.getAttribute('data-dst');
          if (!(key in params)) return;
          const value = params[key];
          if (slot.matches('img')) {
            slot.src = value;
            slot.hidden = !value;
            return;
          }
          if (slot.matches('a')) {
            if (value) slot.href = value;
            slot.hidden = !value;
            return;
          }
          if (slot.hasAttribute('data-dst-split')) {
            slot.replaceChildren(...String(value).split(',').filter(Boolean).map((part) => {
              const chip = document.createElement('span');
              chip.className = 'tag';
              chip.textContent = part.trim();
              return chip;
            }));
            return;
          }
          slot.textContent = value;
        });
        const form = qs('form[data-dst-form]', el);
        if (form && params.action) form.action = params.action;
        qsa('input[data-dst-value]', el).forEach((input) => {
          const key = input.getAttribute('data-dst-value');
          if (key in params) input.value = params[key];
        });
      }
      el.showModal();
      return el;
    },
    close(el) {
      if (el && el.open) el.close();
    },
    init() {
      document.addEventListener('click', (e) => {
        const opener = e.target.closest('[data-dialog]');
        if (opener) {
          e.preventDefault();
          const payload = {};
          Object.keys(opener.dataset).forEach((key) => {
            if (key.startsWith('put')) {
              const name = key.charAt(3).toLowerCase() + key.slice(4);
              payload[name] = opener.dataset[key];
            }
          });
          const target = opener.getAttribute('data-dialog');
          const dlg = Dialogs.open(target, payload);
          if (dlg && dlg.dataset.form) {
            const form = qs('form', dlg);
            if (form) form.dataset.submitTrigger = JSON.stringify(payload);
          }
          return;
        }
        const closer = e.target.closest('[data-dialog-close]');
        if (closer) {
          const dlg = closer.closest('dialog');
          if (dlg) { e.preventDefault(); Dialogs.close(dlg); }
        }
      });

      qsa('dialog').forEach((dlg) => {
        // Click on the backdrop area (the dialog element itself) closes it.
        dlg.addEventListener('click', (e) => {
          if (e.target === dlg) {
            const rect = dlg.getBoundingClientRect();
            const inside = e.clientX >= rect.left && e.clientX <= rect.right &&
              e.clientY >= rect.top && e.clientY <= rect.bottom;
            if (!inside) Dialogs.close(dlg);
          }
        });
        dlg.addEventListener('close', () => {
          if (!qs('dialog[open]')) document.body.classList.remove('is-locked');
        });
        dlg.addEventListener('cancel', () => {
          if (!qs('dialog[open]')) document.body.classList.remove('is-locked');
        });
        const shown = () => document.body.classList.add('is-locked');
        dlg.addEventListener('show', shown);
      });
    }
  };

  const Confirm = {
    init() {
      document.addEventListener('submit', (e) => {
        const form = e.target.closest('form[data-confirm]');
        if (!form) return;
        if (form.dataset.confirmed === '1') return;
        e.preventDefault();
        Confirm.ask(form.getAttribute('data-confirm'), form.getAttribute('data-confirm-title') || 'Please confirm')
          .then((ok) => {
            if (!ok) return;
            form.dataset.confirmed = '1';
            form.submit();
          });
      });
    },
    ask(message, title) {
      const dlg = qs('#confirmDialog');
      if (!dlg) return Promise.resolve(window.confirm(message));
      qs('[data-dst="title"]', dlg).textContent = title;
      qs('[data-dst="message"]', dlg).textContent = message;
      return new Promise((resolve) => {
        const ok = qs('[data-confirm-ok]', dlg);
        let settled = false;
        const finish = (value) => {
          if (settled) return;
          settled = true;
          cleanup();
          resolve(value);
        };
        const onOk = () => finish(true);
        const onClose = () => finish(false);
        const cleanup = () => {
          ok.removeEventListener('click', onOk);
          dlg.removeEventListener('close', onClose);
        };
        ok.addEventListener('click', onOk);
        dlg.addEventListener('close', onClose);
        Dialogs.open('#confirmDialog');
      });
    }
  };

  /* ------------------------------------------------------------------
   * Toasts
   * ------------------------------------------------------------------ */
  const Toasts = {
    stack() {
      let el = qs('.toast-stack');
      if (!el) {
        el = document.createElement('div');
        el.className = 'toast-stack';
        el.setAttribute('role', 'region');
        el.setAttribute('aria-live', 'polite');
        el.setAttribute('aria-label', 'Notifications');
        document.body.appendChild(el);
      }
      return el;
    },
    dismiss(node, delay) {
      const ms = delay === undefined ? 5200 : delay;
      if (!ms) return;
      setTimeout(() => {
        if (!node.isConnected) return;
        node.dataset.state = 'closing';
        node.addEventListener('animationend', () => node.remove(), { once: true });
        if (prefersReduced()) setTimeout(() => node.remove(), 60);
      }, ms);
    },
    push({ title = 'Notice', message = '', tone = 'info' }) {
      const el = document.createElement('div');
      el.className = 'toast toast--' + tone;
      el.setAttribute('role', tone === 'danger' ? 'alert' : 'status');
      el.innerHTML =
        '<svg class="icon" aria-hidden="true"><use href="#i-' + (tone === 'success' ? 'check' : tone === 'danger' ? 'alert' : 'info') + '"></use></svg>' +
        '<div><p class="toast__title"></p><p class="toast__msg"></p></div>' +
        '<button type="button" class="btn btn--ghost btn--icon btn--sm" data-toast-close aria-label="Dismiss"><svg class="icon" aria-hidden="true"><use href="#i-close"></use></svg></button>';
      qs('.toast__title', el).textContent = title;
      qs('.toast__msg', el).textContent = message;
      this.stack().appendChild(el);
      this.dismiss(el);
      return el;
    },
    init() {
      document.addEventListener('click', (e) => {
        const closer = e.target.closest('[data-toast-close], .toast__close');
        if (!closer) return;
        const el = closer.closest('.toast');
        if (!el) return;
        el.dataset.state = 'closing';
        el.addEventListener('animationend', () => el.remove(), { once: true });
      });
      qsa('.toast-stack [data-toast], .toast-stack .toast').forEach((el) => {
        if (!el.dataset.state) Toasts.dismiss(el);
      });
    }
  };

  /* ------------------------------------------------------------------
   * Dropdown menus
   * ------------------------------------------------------------------ */
  const Menus = {
    closeAll(except) {
      qsa('.menu[data-open="true"]').forEach((m) => {
        if (m === except) return;
        m.dataset.open = 'false';
        const btn = qs('[data-menu-toggle]', m);
        if (btn) btn.setAttribute('aria-expanded', 'false');
      });
    },
    init() {
      document.addEventListener('click', (e) => {
        const toggle = e.target.closest('[data-menu-toggle]');
        if (toggle) {
          const menu = toggle.closest('.menu');
          const open = menu.dataset.open === 'true';
          Menus.closeAll(menu);
          menu.dataset.open = open ? 'false' : 'true';
          toggle.setAttribute('aria-expanded', String(!open));
          return;
        }
        if (!e.target.closest('.menu')) Menus.closeAll();
      });
      document.addEventListener('keydown', (e) => {
        if (e.key !== 'Escape') return;
        Menus.closeAll();
      });
      qsa('.menu__panel').forEach((panel) => {
        panel.addEventListener('keydown', (e) => {
          if (e.key !== 'ArrowDown' && e.key !== 'ArrowUp') return;
          const items = qsa('.menu__item', panel);
          const i = items.indexOf(document.activeElement);
          const next = e.key === 'ArrowDown' ? items[i + 1] : items[i - 1];
          if (next) { e.preventDefault(); next.focus(); }
        });
      });
    }
  };

  /* ------------------------------------------------------------------
   * Site nav (mobile) + admin rail
   * ------------------------------------------------------------------ */
  const Chrome = {
    init() {
      const navToggle = qs('[data-nav-toggle]');
      const navLinks = qs('#siteNavLinks');
      if (navToggle && navLinks) {
        navToggle.addEventListener('click', () => {
          const open = navLinks.dataset.open === 'true';
          navLinks.dataset.open = open ? 'false' : 'true';
          navToggle.setAttribute('aria-expanded', String(!open));
        });
        navLinks.addEventListener('click', (e) => {
          if (e.target.closest('a')) {
            navLinks.dataset.open = 'false';
            navToggle.setAttribute('aria-expanded', 'false');
          }
        });
        document.addEventListener('click', (e) => {
          if (!e.target.closest('.nav-bar')) {
            navLinks.dataset.open = 'false';
            navToggle.setAttribute('aria-expanded', 'false');
          }
        });
      }

      const shell = qs('.shell');
      const rail = qs('#rail');
      const railToggle = qs('[data-rail-toggle]');
      const railCollapse = qs('[data-rail-collapse]');
      const scrim = qs('#railScrim');

      const openRail = (state) => {
        if (!rail) return;
        rail.dataset.open = String(state);
        if (scrim) scrim.dataset.open = String(state);
        document.body.classList.toggle('is-locked', state && isMobile());
      };

      if (shell && railToggle) {
        railToggle.addEventListener('click', () => {
          if (isMobile()) openRail(rail.dataset.open !== 'true');
          else if (railCollapse) {
            const collapsed = shell.dataset.rail === 'collapsed';
            shell.dataset.rail = collapsed ? 'expanded' : 'collapsed';
            railCollapse.setAttribute('aria-pressed', String(!collapsed));
            try { localStorage.setItem('mp:rail', shell.dataset.rail); } catch { /* ignore */ }
          }
        });
        railCollapse && railCollapse.addEventListener('click', () => {
          shell.dataset.rail = shell.dataset.rail === 'collapsed' ? 'expanded' : 'collapsed';
          railCollapse.setAttribute('aria-pressed', String(shell.dataset.rail === 'collapsed'));
          try { localStorage.setItem('mp:rail', shell.dataset.rail); } catch { /* ignore */ }
        });
      }
      if (scrim) scrim.addEventListener('click', () => openRail(false));
      document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') openRail(false);
      });
      if (rail) {
        rail.addEventListener('click', (e) => {
          if (e.target.closest('.rail__link') && isMobile()) openRail(false);
        });
        window.addEventListener('resize', debounce(() => {
          if (!isMobile()) openRail(false);
        }, 180));
      }

      const scrollTop = qs('.scrolltop');
      const bar = qs('.nav-bar');
      const onScroll = () => {
        const y = window.scrollY;
        if (scrollTop) scrollTop.dataset.visible = String(y > 480);
        if (bar) bar.dataset.scrolled = String(y > 8);
      };
      window.addEventListener('scroll', throttle(onScroll, 90), { passive: true });
      onScroll();

      if (scrollTop) {
        scrollTop.addEventListener('click', () => {
          window.scrollTo({ top: 0, behavior: prefersReduced() ? 'auto' : 'smooth' });
        });
      }

      // Restore persisted rail state on desktop
      try {
        const stored = localStorage.getItem('mp:rail');
        if (shell && stored === 'collapsed' && !isMobile()) {
          shell.dataset.rail = 'collapsed';
          railCollapse && railCollapse.setAttribute('aria-pressed', 'true');
        }
      } catch { /* ignore */ }
    }
  };

  /* ------------------------------------------------------------------
   * Fold / show more
   * The server renders every item; JS collapses the tail so the page
   * stays complete and indexable without script.
   * ------------------------------------------------------------------ */
  const Folds = {
    apply(scope) {
      qsa('[data-fold]', scope).forEach((fold) => {
        const items = qsa(':scope > [data-fold-item]', fold);
        const limit = parseInt(fold.getAttribute('data-fold') || '0', 10);
        if (!items.length || !limit) return;
        const btn = qs('[data-fold-toggle]', fold.parentNode);
        const expanded = fold.dataset.open === 'true';
        items.forEach((item, i) => {
          item.hidden = !expanded && i >= limit;
        });
        const rest = Math.max(items.length - limit, 0);
        if (btn) {
          const more = btn.getAttribute('data-label-more') || 'Show more';
          const less = btn.getAttribute('data-label-less') || 'Show less';
          btn.hidden = rest === 0;
          btn.setAttribute('aria-expanded', String(expanded));
          const label = qs('[data-fold-label]', btn);
          if (label) label.textContent = expanded ? less : more + ' (' + rest + ')';
          if (!btn.dataset.wired) {
            btn.dataset.wired = '1';
            btn.addEventListener('click', () => {
              fold.dataset.open = fold.dataset.open === 'true' ? 'false' : 'true';
              Folds.apply(document);
              if (fold.dataset.open === 'false') {
                fold.scrollIntoView({ block: 'nearest', behavior: prefersReduced() ? 'auto' : 'smooth' });
              }
            });
          }
        }
      });
    },
    init() {
      this.apply(document);
    }
  };

  /* ------------------------------------------------------------------
   * Debounced table search over the existing GET endpoints
   * ------------------------------------------------------------------ */
  const Search = {
    init() {
      qsa('form[data-search]').forEach((form) => this.wire(form));
    },
    wire(form) {
      if (form.dataset.wired === '1') return;
      form.dataset.wired = '1';
      const target = form.getAttribute('data-search');
      const param = form.getAttribute('data-search-param') || (qs('input[name]', form) || {}).name;
      const input = qs('input[name="' + param + '"]', form) || qs('input', form);
      const url = form.getAttribute('action') || window.location.pathname;
      if (!target || !param || !input) return;

      let ctrl = null;
      const run = () => {
        const holder = qs(target);
        if (!holder) return;
        const query = new URLSearchParams(window.location.search);
        if (input.value) query.set(param, input.value); else query.delete(param);
        const href = url + (query.toString() ? '?' + query.toString() : '');
        holder.setAttribute('aria-busy', 'true');
        if (ctrl) ctrl.abort();
        ctrl = new AbortController();
        fetch(href, { headers: { 'X-Requested-With': 'fetch' }, signal: ctrl.signal })
          .then((r) => r.text())
          .then((html) => {
            const doc = new DOMParser().parseFromString(html, 'text/html');
            const fresh = doc.querySelector(target);
            if (!fresh) return;
            holder.innerHTML = fresh.innerHTML;
            window.history.replaceState({}, '', href);
            enhance(holder);
          })
          .catch((err) => {
            if (err.name !== 'AbortError') Toasts.push({ title: 'Search failed', message: 'Try again.', tone: 'danger' });
          })
          .then(() => holder.removeAttribute('aria-busy'));
      };

      input.addEventListener('input', debounce(run, 320));
      form.addEventListener('submit', (e) => {
        if (e.submitter && e.submitter.type === 'submit' && e.submitter.dataset.searchSubmit) return;
        e.preventDefault();
        run();
      });
    }
  };

  /* ------------------------------------------------------------------
   * Tabs, clipboard, password, glow, submit state
   * ------------------------------------------------------------------ */
  const Bits = {
    init() {
      document.addEventListener('click', (e) => {
        const tab = e.target.closest('[data-tab]');
        if (tab) {
          const group = tab.closest('[data-tabs]');
          if (!group) return;
          e.preventDefault();
          const name = tab.getAttribute('data-tab');
          qsa('[data-tab]', group).forEach((t) => t.setAttribute('aria-selected', String(t === tab)));
          qsa('[data-tabpanel]', group.parentNode).forEach((p) => {
            p.hidden = p.getAttribute('data-tabpanel') !== name;
          });
          return;
        }

        const copy = e.target.closest('[data-copy]');
        if (copy) {
          e.preventDefault();
          Bits.copy(copy.getAttribute('data-copy'), copy);
        }
      });

      document.addEventListener('click', (e) => {
        const pw = e.target.closest('[data-pw-toggle]');
        if (!pw) return;
        const input = document.getElementById(pw.getAttribute('data-pw-toggle'));
        if (!input) return;
        const show = input.type === 'password';
        input.type = show ? 'text' : 'password';
        pw.setAttribute('aria-pressed', String(show));
        const icon = qs('.icon use', pw);
        if (icon) icon.setAttribute('href', show ? '#i-eyeOff' : '#i-eye');
      });

      // Pointer-tracked glow on opted-in cards
      if (!prefersReduced() && window.matchMedia('(hover: hover)').matches) {
        document.addEventListener('pointermove', (e) => {
          const card = e.target.closest('[data-glow]');
          if (!card || card.dataset.glowed === '1') return;
          card.dataset.glowed = '1';
          requestAnimationFrame(() => {
            const r = card.getBoundingClientRect();
            card.style.setProperty('--px', ((e.clientX - r.left) / r.width * 100).toFixed(1) + '%');
            card.style.setProperty('--py', ((e.clientY - r.top) / r.height * 100).toFixed(1) + '%');
            setTimeout(() => { card.dataset.glowed = '0'; }, 40);
          });
        }, { passive: true });
      }

      // Busy state for the real submit of a valid form
      document.addEventListener('submit', (e) => {
        const form = e.target;
        if (!form.checkValidity || !form.checkValidity()) return;
        const btn = qsa('[type="submit"]', form).find((b) => !b.hidden && !b.disabled);
        if (!btn) return;
        btn.setAttribute('data-loading-state', 'busy');
        btn.disabled = true;
        setTimeout(() => {
          btn.removeAttribute('data-loading-state');
          btn.disabled = false;
        }, 12000);
      });

      // Live file preview
      qsa('[data-preview] input[type="file"]', document).forEach((input) => {
        input.addEventListener('change', () => {
          const img = qs(input.getAttribute('data-preview'));
          const file = input.files && input.files[0];
          if (!img || !file || !/^image\//.test(file.type)) return;
          img.src = URL.createObjectURL(file);
          img.hidden = false;
        });
      });

      // End-date control driven by "current / present" checkbox
      qsa('[data-until-now]').forEach((check) => {
        const end = qs(check.getAttribute('data-until-now'));
        if (!end) return;
        const sync = () => {
          end.disabled = check.checked;
          const wrap = end.closest('.field');
          if (wrap) wrap.dataset.muted = String(check.checked);
        };
        check.addEventListener('change', sync);
        sync();
      });
    },
    copy(text, btn) {
      const done = (ok) => {
        btn.dataset.copied = ok ? 'true' : 'false';
        const icon = qs('.icon use', btn);
        if (icon) icon.setAttribute('href', ok ? '#i-check' : '#i-alert');
        setTimeout(() => {
          btn.dataset.copied = 'false';
          if (icon) icon.setAttribute('href', '#i-copy');
        }, 1600);
      };
      if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(text).then(() => done(true), () => done(false));
      } else {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.setAttribute('readonly', '');
        ta.style.cssText = 'position:fixed;top:-1000px';
        document.body.appendChild(ta);
        ta.select();
        let ok = false;
        try { ok = document.execCommand('copy'); } catch { ok = false; }
        ta.remove();
        done(ok);
      }
    }
  };

  /* ------------------------------------------------------------------
   * Helpers
   * ------------------------------------------------------------------ */
  function debounce(fn, ms) {
    let t;
    return function (...args) {
      clearTimeout(t);
      t = setTimeout(() => fn.apply(this, args), ms);
    };
  }

  function throttle(fn, ms) {
    let last = 0, timer = null;
    return function (...args) {
      const now = Date.now();
      if (now - last >= ms) {
        last = now;
        fn.apply(this, args);
      } else {
        clearTimeout(timer);
        timer = setTimeout(() => { last = Date.now(); fn.apply(this, args); }, ms - (now - last));
      }
    };
  }

  /* Re-run the observers over freshly injected markup */
  function enhance(scope) {
    Reveal.indexGroups(scope);
    Reveal.ensure();
    Numbers.ensure();
    Folds.apply(scope);
  }

  /* ------------------------------------------------------------------
   * Boot
   * ------------------------------------------------------------------ */
  function boot() {
    Theme.init();
    Chrome.init();
    Dialogs.init();
    Confirm.init();
    Menus.init();
    Toasts.init();
    Search.init();
    Bits.init();
    Reveal.indexGroups(document);
    Reveal.ensure();
    Numbers.ensure();
    Folds.init();
    window.Portfolio = { theme: Theme, toast: Toasts, dialog: Dialogs, confirm: Confirm, enhance };
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', boot, { once: true });
  } else {
    boot();
  }
})();
