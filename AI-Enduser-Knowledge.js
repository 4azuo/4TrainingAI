/* ── SEARCH ── */
(function () {
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

  document.addEventListener('DOMContentLoaded', function () {
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
  });
})();

/* Mermaid: lazy-render per tab (fixes display:none blank diagram issue) */
mermaid.initialize({
  startOnLoad: false,
  theme: 'default',
  flowchart: { curve: 'basis', htmlLabels: true },
  sequence: { mirrorActors: false, messageAlign: 'center' }
});

function renderDiagramsIn(panel) {
  var nodes = Array.from(panel.querySelectorAll('.mermaid')).filter(function(n) {
    return !n.hasAttribute('data-processed');
  });
  if (nodes.length) mermaid.run({ nodes: nodes });
}

document.addEventListener('DOMContentLoaded', function () {
  var activePanel = document.querySelector('.tab-panel.active');
  if (activePanel) renderDiagramsIn(activePanel);

  document.querySelectorAll('.tab-btn').forEach(function (btn) {
    btn.addEventListener('click', function () {
      document.querySelectorAll('.tab-btn').forEach(function (t) { t.classList.remove('active'); });
      document.querySelectorAll('.tab-panel').forEach(function (p) { p.classList.remove('active'); });
      btn.classList.add('active');
      var panel = document.getElementById('tab-' + btn.dataset.tab);
      panel.classList.add('active');
      renderDiagramsIn(panel);
    });
  });
});
