/* ════════════════════════════════════════════════════════════
   COMMON JS — logic dùng chung cho mọi trang dạng "tab + search".
   Mỗi trang chỉ cần định nghĩa biến toàn cục `TABS` (mảng manifest)
   TRƯỚC khi nạp file này. Ví dụ:
     <script src="VibeCoding.js"></script>  // định nghĩa TABS
     <script src="common.js"></script>             // logic chung
   Mỗi phần tử TABS: { id, label, file }.
   ════════════════════════════════════════════════════════════ */

/* ── MERMAID: khởi tạo + lazy-render theo tab (tránh blank khi display:none) ── */
mermaid.initialize({
  startOnLoad: false,
  theme: 'default',
  flowchart: { curve: 'basis', htmlLabels: true },
  sequence: { mirrorActors: false, messageAlign: 'center' }
});

function renderDiagramsIn(panel) {
  var nodes = Array.from(panel.querySelectorAll('.mermaid')).filter(function (n) {
    return !n.hasAttribute('data-processed');
  });
  if (nodes.length) mermaid.run({ nodes: nodes });
}

/* ── SEARCH ── */
function initSearch() {
  var allMarks = [];
  var currentIdx = -1;

  function getTabId(el) {
    var panel = el.closest('.tab-panel');
    return panel ? panel.id.replace('tab-', '') : null;
  }

  function clearHighlights() {
    document.querySelectorAll('mark.search-hl').forEach(function (m) {
      if (m.parentNode) m.parentNode.replaceChild(document.createTextNode(m.textContent), m);
    });
    var main = document.querySelector('main');
    if (main) main.normalize();
    allMarks = [];
    currentIdx = -1;
  }

  function highlightAll(query) {
    clearHighlights();
    if (!query) return 0;
    var q = query.toLowerCase();

    var walker = document.createTreeWalker(
      document.querySelector('main'),
      NodeFilter.SHOW_TEXT,
      {
        acceptNode: function (node) {
          if (!node.textContent.toLowerCase().includes(q)) return NodeFilter.FILTER_REJECT;
          var el = node.parentElement;
          if (!el || el.closest('.mermaid, svg, script, style')) return NodeFilter.FILTER_REJECT;
          return NodeFilter.FILTER_ACCEPT;
        }
      }
    );

    var nodes = [];
    var n;
    while ((n = walker.nextNode())) nodes.push(n);

    var marks = [];
    nodes.forEach(function (textNode) {
      var text = textNode.textContent;
      var lower = text.toLowerCase();
      var frag = document.createDocumentFragment();
      var last = 0, idx;
      while ((idx = lower.indexOf(q, last)) !== -1) {
        if (idx > last) frag.appendChild(document.createTextNode(text.slice(last, idx)));
        var m = document.createElement('mark');
        m.className = 'search-hl';
        m.textContent = text.slice(idx, idx + query.length);
        frag.appendChild(m);
        marks.push(m);
        last = idx + query.length;
      }
      if (last < text.length) frag.appendChild(document.createTextNode(text.slice(last)));
      if (textNode.parentNode) textNode.parentNode.replaceChild(frag, textNode);
    });

    allMarks = marks;
    return marks.length;
  }

  function updateCount() {
    var el = document.getElementById('search-count');
    var btnPrev = document.getElementById('btn-prev');
    var btnNext = document.getElementById('btn-next');
    if (!el) return;
    var total = allMarks.length;
    if (total === 0) {
      el.className = 'search-count';
      el.textContent = '';
    } else {
      el.className = 'search-count';
      el.textContent = (currentIdx + 1) + ' / ' + total;
    }
    if (btnPrev) btnPrev.disabled = total === 0;
    if (btnNext) btnNext.disabled = total === 0;
  }

  function activateMark(idx) {
    if (!allMarks.length) return;
    allMarks.forEach(function (m) { m.classList.remove('active'); });
    currentIdx = ((idx % allMarks.length) + allMarks.length) % allMarks.length;
    var mark = allMarks[currentIdx];
    mark.classList.add('active');
    var tabId = getTabId(mark);
    if (tabId) {
      var btn = document.querySelector('[data-tab="' + tabId + '"]');
      if (btn && !btn.classList.contains('active')) btn.click();
    }
    mark.scrollIntoView({ behavior: 'smooth', block: 'center' });
    updateCount();
  }

  function next() { if (allMarks.length) activateMark(currentIdx + 1); }
  function prev() { if (allMarks.length) activateMark(currentIdx - 1); }

  var input = document.getElementById('search-input');
  var btnNext = document.getElementById('btn-next');
  var btnPrev = document.getElementById('btn-prev');
  if (!input) return;

  btnNext.disabled = true;
  btnPrev.disabled = true;

  var timer;
  input.addEventListener('input', function () {
    clearTimeout(timer);
    timer = setTimeout(function () {
      var q = input.value.trim();
      var countEl = document.getElementById('search-count');
      if (!q) {
        clearHighlights();
        if (countEl) { countEl.textContent = ''; countEl.className = 'search-count'; }
        if (btnNext) btnNext.disabled = true;
        if (btnPrev) btnPrev.disabled = true;
        return;
      }
      var count = highlightAll(q);
      if (count === 0) {
        if (countEl) { countEl.textContent = 'Không tìm thấy'; countEl.className = 'search-count no-match'; }
        if (btnNext) btnNext.disabled = true;
        if (btnPrev) btnPrev.disabled = true;
        return;
      }
      currentIdx = -1;
      activateMark(0);
    }, 200);
  });

  input.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
      e.preventDefault();
      e.shiftKey ? prev() : next();
    }
    if (e.key === 'Escape') {
      input.value = '';
      clearHighlights();
      var countEl = document.getElementById('search-count');
      if (countEl) { countEl.textContent = ''; countEl.className = 'search-count'; }
      if (btnNext) btnNext.disabled = true;
      if (btnPrev) btnPrev.disabled = true;
      input.blur();
    }
  });

  if (btnNext) btnNext.addEventListener('click', next);
  if (btnPrev) btnPrev.addEventListener('click', prev);
}

/* ── PERSIST: nhớ tab đang xem qua refresh (key riêng theo từng trang) ── */
function storageKey() { return 'activeTab:' + location.pathname; }

function getInitialTabId() {
  var saved = localStorage.getItem(storageKey());
  var ok = TABS.some(function (t) { return t.id === saved; }); // tab còn tồn tại?
  return ok ? saved : TABS[0].id;
}

/* ── TABS: chuyển tab + lazy render ── */
function initTabs() {
  document.querySelectorAll('.tab-btn').forEach(function (btn) {
    btn.addEventListener('click', function () {
      localStorage.setItem(storageKey(), btn.dataset.tab);
      document.querySelectorAll('.tab-btn').forEach(function (t) { t.classList.remove('active'); });
      document.querySelectorAll('.tab-panel').forEach(function (p) { p.classList.remove('active'); });
      btn.classList.add('active');
      var panel = document.getElementById('tab-' + btn.dataset.tab);
      if (panel) {
        panel.classList.add('active');
        renderDiagramsIn(panel);
      }
    });
  });
}

/* ── LOADER: sinh nav + nạp các fragment tab ── */
function buildNav() {
  var nav = document.getElementById('tab-nav');
  if (!nav) return;
  var activeId = getInitialTabId();
  TABS.forEach(function (t) {
    var btn = document.createElement('button');
    btn.className = 'tab-btn' + (t.id === activeId ? ' active' : '');
    btn.dataset.tab = t.id;
    btn.textContent = t.label;
    nav.appendChild(btn);
  });
}

function loadTabs() {
  var main = document.getElementById('tab-main');
  if (!main) return Promise.resolve();

  return Promise.all(TABS.map(function (t, i) {
    return fetch(t.file)
      .then(function (r) {
        if (!r.ok) throw new Error('HTTP ' + r.status);
        return r.text();
      })
      .then(function (html) { return { i: i, t: t, html: html, ok: true }; })
      .catch(function (err) { return { i: i, t: t, err: err, ok: false }; });
  })).then(function (results) {
    var activeId = getInitialTabId();
    results.forEach(function (res) {
      var panel = document.createElement('div');
      panel.className = 'tab-panel' + (res.t.id === activeId ? ' active' : '');
      panel.id = 'tab-' + res.t.id;
      if (res.ok) {
        panel.innerHTML = res.html;
      } else {
        panel.innerHTML =
          '<h2>⚠️ Không tải được nội dung</h2>' +
          '<p class="tagline">Không nạp được <code>' + res.t.file + '</code> (' + res.err.message + ').</p>' +
          '<p>Trang nạp nội dung động qua <code>fetch</code>, nên cần mở qua một server cục bộ ' +
          '(ví dụ <strong>VSCode Live Server</strong>) thay vì mở thẳng file <code>file://</code>.</p>';
      }
      main.appendChild(panel);
    });
  });
}

document.addEventListener('DOMContentLoaded', function () {
  if (typeof TABS === 'undefined' || !Array.isArray(TABS)) {
    console.error('common.js: cần định nghĩa biến toàn cục `TABS` trước khi nạp file này.');
    return;
  }
  buildNav();
  loadTabs().then(function () {
    initTabs();
    initSearch();
    var activePanel = document.querySelector('.tab-panel.active');
    if (activePanel) renderDiagramsIn(activePanel);
  });
});
