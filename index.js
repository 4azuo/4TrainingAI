/* ════════════════════════════════════════════════════════════
   INDEX (SHELL) JS — điều phối "vỏ": dựng tab cấp cao + nạp trang con
   vào iframe. Khác với common.js (vốn lo tab + search TRONG mỗi trang),
   nên vỏ có logic riêng ở đây.
   Thêm trang mới = thêm 1 dòng trong mảng PAGES.

   Mỗi trang con có 1 iframe RIÊNG, tạo lazy lần đầu mở rồi GIỮ SỐNG.
   Đổi tab = bật/tắt class .is-active (ẩn/hiện) — KHÔNG đổi src, nên
   không reload, không nháy, giữ nguyên scroll + sub-tab + ô tìm kiếm.
   ════════════════════════════════════════════════════════════ */
var PAGES = [
  { id: 'vibe',     icon: '🌀', label: 'Vibe Coding',           file: 'VibeCoding.html' },
  { id: 'enduser',  icon: '🧠', label: 'AI Concepts',     file: 'AI-Concepts.html' },
  { id: 'workflow', icon: '🔁', label: 'AI Engineering',  file: 'AI-Engineering.html' }
];

var STORAGE_KEY = 'activePage:index';
var nav    = document.getElementById('shell-nav');
var main   = document.getElementById('shell-main');
var frames = {}; // id -> <iframe> đã tạo (giữ sống)

function getInitialPageId() {
  var saved = localStorage.getItem(STORAGE_KEY);
  var ok = PAGES.some(function (p) { return p.id === saved; });
  return ok ? saved : PAGES[0].id;
}

/* Tạo iframe cho 1 trang (chỉ 1 lần), rồi tái dùng ở các lần đổi tab sau. */
function getFrame(page) {
  if (frames[page.id]) return frames[page.id];
  var f = document.createElement('iframe');
  f.className = 'shell-frame';
  f.title = page.label;
  f.src = page.file;
  main.appendChild(f);
  frames[page.id] = f;
  return f;
}

function activate(id) {
  var page = PAGES.find(function (p) { return p.id === id; });
  if (!page) return;
  localStorage.setItem(STORAGE_KEY, id);
  document.querySelectorAll('.shell-tab').forEach(function (b) {
    b.classList.toggle('active', b.dataset.page === id);
  });
  getFrame(page); // tạo lazy nếu là lần đầu mở trang này
  Object.keys(frames).forEach(function (key) {
    frames[key].classList.toggle('is-active', key === id);
  });
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
