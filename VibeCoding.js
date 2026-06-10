/* ════════════════════════════════════════════════════════════
   TAB MANIFEST — nguồn duy nhất định nghĩa các tab của trang này.
   Thêm tab mới = thêm 1 dòng ở đây + 1 file trong thư mục VibeCoding/.
   Logic chung (nav, search, tabs, mermaid) nằm ở common.js.
   ════════════════════════════════════════════════════════════ */
var TABS = [
  { id: 'what',       label: '1. Vibe Coding là gì',      file: 'VibeCoding/01-what.html' },
  { id: 'tools',      label: '2. Bản đồ công cụ',         file: 'VibeCoding/02-tools.html' },
  { id: 'thirdparty', label: '3. 3rd-party: Ưu & Nhược',  file: 'VibeCoding/03-thirdparty.html' },
  { id: 'vscode',     label: '4. AI trên VSCode/IDE',     file: 'VibeCoding/04-vscode.html' },
  { id: 'scope',      label: '5. Personal vs Enterprise', file: 'VibeCoding/05-scope.html' },
  { id: 'terms',      label: '6. Vì sao cần học?',        file: 'VibeCoding/06-terms.html' },
  { id: 'beyond',     label: '7. Ngoài làm app',          file: 'VibeCoding/07-beyond.html' },
  { id: 'summary',    label: '8. Tổng kết',               file: 'VibeCoding/08-summary.html' },
  { id: 'history',    label: '📋 Lịch sử',                file: 'VibeCoding/09-history.html' }
];
