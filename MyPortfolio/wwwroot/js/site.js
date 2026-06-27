/* ============================================
   MyPortfolio — Site JavaScript
   Animations, Interactions, Utilities
   ============================================ */

(function () {
  'use strict';

  /* ---------- Scroll Reveal (IntersectionObserver) ---------- */
  function initScrollReveal() {
    const elements = document.querySelectorAll('.fade-up, .fade-left, .fade-right, .fade-scale, .stagger-children');
    if (!elements.length) return;

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

    elements.forEach(el => observer.observe(el));
  }

  /* ---------- Navbar Scroll Effect ---------- */
  function initNavbarScroll() {
    const navbar = document.querySelector('.main-navbar');
    if (!navbar) return;

    const observer = new IntersectionObserver(
      ([e]) => navbar.classList.toggle('scrolled', !e.isIntersecting),
      { rootMargin: '-1px 0px 0px 0px', threshold: 1 }
    );
    observer.observe(document.body);
  }

  /* ---------- Scroll to Top ---------- */
  function initScrollToTop() {
    const btn = document.getElementById('scrollTopBtn');
    if (!btn) return;
    window.addEventListener('scroll', () => {
      btn.classList.toggle('visible', window.scrollY > 400);
    }, { passive: true });
  }

  /* ---------- Theme Toggle ---------- */
  function initThemeToggle() {
    const root = document.documentElement;
    const btns = document.querySelectorAll('[id^="themeToggle"], [id^="footerThemeToggle"]');
    const icon = document.getElementById('themeIcon');

    const stored = localStorage.getItem('theme');
    if (stored) root.setAttribute('data-bs-theme', stored);

    function syncIcon() {
      if (!icon) return;
      const mode = root.getAttribute('data-bs-theme');
      icon.className = mode === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars';
    }
    syncIcon();

    btns.forEach(btn => {
      btn?.addEventListener('click', () => {
        const current = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-bs-theme', current);
        localStorage.setItem('theme', current);
        syncIcon();
      });
    });
  }

  /* ---------- Button Ripple ---------- */
  function initRipple() {
    document.addEventListener('click', function (e) {
      const btn = e.target.closest('.btn');
      if (!btn) return;
      const rect = btn.getBoundingClientRect();
      const ripple = document.createElement('span');
      ripple.className = 'ripple';
      const size = Math.max(rect.width, rect.height);
      ripple.style.width = ripple.style.height = size + 'px';
      ripple.style.left = (e.clientX - rect.left - size / 2) + 'px';
      ripple.style.top = (e.clientY - rect.top - size / 2) + 'px';
      btn.appendChild(ripple);
      ripple.addEventListener('animationend', () => ripple.remove());
    });
  }

  /* ---------- Button Loading State ---------- */
  function initLoadingButtons() {
    document.addEventListener('click', function (e) {
      const btn = e.target.closest('[data-loading]');
      if (!btn || btn.disabled) return;
      const form = btn.closest('form');
      if (form && !form.checkValidity()) return;
      btn.disabled = true;
      btn.classList.add('loading');
      if (!btn.querySelector('.btn-loader')) {
        const original = btn.innerHTML;
        btn.dataset.originalHtml = original;
        btn.innerHTML = '<span class="spinner"></span>';
        btn.classList.add('btn-loading');
      }
    });
  }

  /* ---------- Read More / Show Less ---------- */
  function initReadMore() {
    document.querySelectorAll('.toggle-btn').forEach(btn => {
      btn.addEventListener('click', function () {
        const container = this.closest('.read-more-container');
        if (!container) return;
        const isExpanded = container.classList.toggle('expanded');
        const textSpan = this.querySelector('.toggle-text');
        if (textSpan) textSpan.textContent = isExpanded ? 'Show Less' : 'Show More';
        if (isExpanded) {
          setTimeout(() => this.scrollIntoView({ behavior: 'smooth', block: 'nearest' }), 150);
        }
      });
    });
  }

  /* ---------- Copy to Clipboard ---------- */
  function initClipboard() {
    document.addEventListener('click', async e => {
      const btn = e.target.closest('.btn-copy-contact');
      if (!btn) return;
      const originalHtml = btn.innerHTML;
      const text = btn.getAttribute('data-copy');
      if (!text) return;
      try {
        await navigator.clipboard.writeText(text);
        btn.innerHTML = '<i class="bi bi-check-lg"></i>';
        btn.classList.add('copied');
        setTimeout(() => { btn.innerHTML = originalHtml; btn.classList.remove('copied'); }, 1600);
      } catch {
        btn.innerHTML = '<i class="bi bi-exclamation-triangle"></i>';
        setTimeout(() => btn.innerHTML = originalHtml, 1600);
      }
    });
  }

  /* ---------- Toast Auto-dismiss ---------- */
  function initToasts() {
    document.querySelectorAll('.toast').forEach(t => {
      setTimeout(() => {
        t.classList.remove('show');
        setTimeout(() => t.remove(), 300);
      }, 5000);
    });
  }

  /* ---------- Password Visibility Toggle ---------- */
  function initPasswordToggle() {
    document.querySelectorAll('[data-pw-toggle]').forEach(btn => {
      btn.addEventListener('click', function () {
        const input = document.getElementById(this.getAttribute('data-pw-toggle'));
        if (!input) return;
        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        this.querySelector('i').className = isPassword ? 'bi bi-eye-slash' : 'bi bi-eye';
      });
    });
  }

  /* ---------- Smooth Back to Top ---------- */
  function initSmoothScroll() {
    document.querySelectorAll('.back-to-top-link').forEach(a => {
      a.addEventListener('click', (e) => {
        if (a.getAttribute('href') === '#top') {
          e.preventDefault();
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      });
    });
  }

  /* ---------- Sidebar (Admin) ---------- */
  function initSidebar() {
    var toggle = document.getElementById('sidebarToggle');
    var sidebar = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebarOverlay');
    var content = document.getElementById('adminContent');
    if (!toggle || !sidebar) return;

    function isMobile() {
      return window.innerWidth < 768;
    }

    function closeMobileSidebar() {
      sidebar.classList.remove('show');
      if (overlay) overlay.classList.remove('active');
    }

    toggle.addEventListener('click', function (e) {
      e.stopPropagation();
      if (isMobile()) {
        sidebar.classList.toggle('show');
        if (overlay) overlay.classList.toggle('active');
      } else {
        sidebar.classList.toggle('collapsed');
        if (content) content.classList.toggle('collapsed');
      }
    });

    if (overlay) {
      overlay.addEventListener('click', closeMobileSidebar);
    }

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && sidebar.classList.contains('show')) {
        closeMobileSidebar();
      }
    });

    var resizeTimer;
    window.addEventListener('resize', function () {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function () {
        if (!isMobile()) {
          closeMobileSidebar();
          sidebar.classList.remove('show');
        }
      }, 200);
    });

    if (content) {
      content.addEventListener('click', function () {
        if (isMobile() && sidebar.classList.contains('show')) {
          closeMobileSidebar();
        }
      });
    }
  }

  /* ---------- Initialize ---------- */
  document.addEventListener('DOMContentLoaded', function () {
    initScrollReveal();
    initNavbarScroll();
    initScrollToTop();
    initThemeToggle();
    initRipple();
    initLoadingButtons();
    initReadMore();
    initClipboard();
    initToasts();
    initPasswordToggle();
    initSmoothScroll();
    initSidebar();
  });

})();
