const API_BASE = '/api';

function getToken() { return localStorage.getItem('hc_token'); }
function getUser() { return JSON.parse(localStorage.getItem('hc_user') || 'null'); }
function isAdmin() { return getUser()?.role === 'Admin'; }

async function apiFetch(path, options = {}) {
  const token = getToken();
  const res = await fetch(API_BASE + path, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {})
    }
  });

  if (res.status === 401) {
    localStorage.removeItem('hc_token');
    localStorage.removeItem('hc_user');
    window.location.href = '/index.html';
    throw new Error('Unauthorized');
  }

  if (res.status === 204) return null;

  const text = await res.text();
  let data;
  try { data = JSON.parse(text); } catch { data = text; }

  if (!res.ok) {
    throw new Error(data?.error || data || `HTTP ${res.status}`);
  }
  return data;
}

const api = {
  login: (name, password) => apiFetch('/auth/login', { method: 'POST', body: JSON.stringify({ name, password }) }),
  me: () => apiFetch('/auth/me'),

  users: {
    list: () => apiFetch('/users'),
    get: (id) => apiFetch(`/users/${id}`),
    create: (data) => apiFetch('/users', { method: 'POST', body: JSON.stringify(data) }),
    update: (id, data) => apiFetch(`/users/${id}`, { method: 'PATCH', body: JSON.stringify(data) }),
  },

  products: {
    list: (onlyActive = false) => apiFetch(`/products?onlyActive=${onlyActive}`),
    get: (id) => apiFetch(`/products/${id}`),
    create: (data) => apiFetch('/products', { method: 'POST', body: JSON.stringify(data) }),
    update: (id, data) => apiFetch(`/products/${id}`, { method: 'PATCH', body: JSON.stringify(data) }),
    adjustStock: (id, adjustment) => apiFetch(`/products/${id}/stock`, { method: 'PATCH', body: JSON.stringify({ adjustment, note: null }) }),
  },

  purchases: {
    list: () => apiFetch('/purchases'),
    create: (data) => apiFetch('/purchases', { method: 'POST', body: JSON.stringify(data) }),
  },

  consumptions: {
    list: (userId) => apiFetch(`/consumptions${userId ? `?userId=${userId}` : ''}`),
    create: (data) => apiFetch('/consumptions', { method: 'POST', body: JSON.stringify(data) }),
    delete: (id) => apiFetch(`/consumptions/${id}`, { method: 'DELETE' }),
  },

  payments: {
    list: (userId) => apiFetch(`/payments${userId ? `?userId=${userId}` : ''}`),
    create: (data) => apiFetch('/payments', { method: 'POST', body: JSON.stringify(data) }),
  },

  reports: {
    stock: () => apiFetch('/reports/stock'),
    userBalance: () => apiFetch('/reports/user-balance'),
    monthlySummary: (year, month) => apiFetch(`/reports/monthly-summary?year=${year}&month=${month}`),
    exportCsv: (year, month) => `/api/reports/export/monthly-summary?year=${year}&month=${month}`,
  }
};

function requireAuth() {
  if (!getToken()) window.location.href = '/index.html';
}

function requireAdmin() {
  requireAuth();
  if (!isAdmin()) window.location.href = '/dashboard.html';
}

function fmt(n) {
  return Number(n).toFixed(2).replace('.', ',') + ' €';
}

function fmtDate(d) {
  return new Date(d).toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function fmtDatetime(d) {
  return new Date(d).toLocaleString('de-DE', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
}

function showAlert(el, msg, type = 'error') {
  el.className = `alert alert-${type}`;
  el.textContent = msg;
  el.classList.remove('hidden');
  setTimeout(() => el.classList.add('hidden'), 4000);
}

function productEmoji(name) {
  const n = name.toLowerCase();
  if (n.includes('cola') || n.includes('softdrink')) return '🥤';
  if (n.includes('bier') || n.includes('beer')) return '🍺';
  if (n.includes('wasser') || n.includes('water')) return '💧';
  if (n.includes('saft') || n.includes('juice')) return '🧃';
  if (n.includes('kaffee') || n.includes('coffee')) return '☕';
  if (n.includes('wein') || n.includes('wine')) return '🍷';
  if (n.includes('milch') || n.includes('milk')) return '🥛';
  if (n.includes('energy')) return '⚡';
  return '🍾';
}

function initNav() {
  const user = getUser();
  if (!user) return;

  const navUser = document.getElementById('nav-user');
  if (navUser) navUser.textContent = `${user.name} (${user.role})`;

  const logoutBtn = document.getElementById('logout-btn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', () => {
      localStorage.removeItem('hc_token');
      localStorage.removeItem('hc_user');
      window.location.href = '/index.html';
    });
  }

  const hamburger = document.getElementById('hamburger');
  const sidebar = document.getElementById('sidebar');
  if (hamburger && sidebar) {
    hamburger.addEventListener('click', () => sidebar.classList.toggle('open'));
    document.addEventListener('click', (e) => {
      if (!sidebar.contains(e.target) && e.target !== hamburger) {
        sidebar.classList.remove('open');
      }
    });
  }

  // Highlight active nav link
  const currentPage = window.location.pathname.split('/').pop() || 'index.html';
  document.querySelectorAll('.sidebar a').forEach(a => {
    if (a.getAttribute('href') === currentPage) a.classList.add('active');
  });

  // Hide admin links for non-admins
  if (user.role !== 'Admin') {
    document.querySelectorAll('[data-admin-only]').forEach(el => el.style.display = 'none');
  }
}
