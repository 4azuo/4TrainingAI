/* ════════════════════════════════════════════════════════════
   INDEX (SHELL) JS — điều phối "vỏ": dựng tab cấp cao + nạp trang con
   vào iframe. Khác với common.js (vốn lo tab + search TRONG mỗi trang),
   nên vỏ có logic riêng ở đây.
   Thêm trang mới = thêm 1 dòng trong mảng PAGES.
   ════════════════════════════════════════════════════════════ */
var PAGES = [
  { id: 'vibe',     icon: '🌀', label: 'Vibe Coding',           file: 'VibeCoding.html' },
  { id: 'enduser',  icon: '🧠', label: 'AI Concepts',     file: 'AI-Concepts.html' },
  { id: 'workflow', icon: '🔁', label: 'AI Engineering',  file: 'AI-Engineering.html' }
];

var STORAGE_KEY = 'activePage:index';
var nav   = document.getElementById('shell-nav');
var frame = document.getElementById('shell-frame');

function getInitialPageId() {
  var saved = localStorage.getItem(STORAGE_KEY);
  var ok = PAGES.some(function (p) { return p.id === saved; });
  return ok ? saved : PAGES[0].id;
}

function activate(id) {
  var page = PAGES.find(function (p) { return p.id === id; });
  if (!page) return;
  localStorage.setItem(STORAGE_KEY, id);
  document.querySelectorAll('.shell-tab').forEach(function (b) {
    b.classList.toggle('active', b.dataset.page === id);
  });
  // Chỉ đổi src khi khác trang hiện tại (tránh reload thừa).
  var target = page.file;
  var current = frame.getAttribute('data-file');
  if (current !== target) {
    frame.setAttribute('data-file', target);
    frame.src = target;
  }
}

PAGES.forEach(function (p) {
  var btn = document.createElement('button');
  btn.className = 'shell-tab';
  btn.dataset.page = p.id;
  btn.innerHTML = '<span class="ico">' + p.icon + '</span>' +
                  '<span class="label-full">' + p.label + '</span>';
  btn.addEventListener('click', function () { activate(p.id); });
  nav.appendChild(btn);
});

activate(getInitialPageId());
