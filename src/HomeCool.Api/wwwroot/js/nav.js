function renderNav() {
  document.body.insertAdjacentHTML('afterbegin', `
    <nav>
      <button class="hamburger" id="hamburger">☰</button>
      <span class="logo">🧊 HomeCool</span>
      <div class="nav-user">
        <span id="nav-user"></span>
        <button id="logout-btn">Abmelden</button>
      </div>
    </nav>
    <div class="layout">
      <aside class="sidebar" id="sidebar">
        <div class="section-label">Navigation</div>
        <a href="/dashboard.html"><span class="icon">🏠</span> Dashboard</a>
        <a href="/kiosk.html"><span class="icon">📷</span> Kiosk</a>
        <a href="/consumption.html"><span class="icon">🥤</span> Konsum buchen</a>
        <a href="/billing.html"><span class="icon">💰</span> Abrechnung</a>
        <div class="admin-section" data-admin-only>
          <div class="section-label">Admin</div>
          <a href="/purchases.html" data-admin-only><span class="icon">🛒</span> Einkauf erfassen</a>
          <a href="/products.html" data-admin-only><span class="icon">📦</span> Produkte</a>
          <a href="/admin.html" data-admin-only><span class="icon">⚙️</span> Verwaltung</a>
        </div>
      </aside>
      <main class="main" id="main-content">
  `);
  // Close layout after main content
  document.body.insertAdjacentHTML('beforeend', `</main></div>`);
  initNav();
}
