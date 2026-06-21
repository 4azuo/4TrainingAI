#!/usr/bin/env python3
# Server nền chỉ để xử lý nút Done của report (POST /api/done) + (tuỳ chọn) phục vụ reports/.
#
# Report MỞ TRỰC TIẾP bằng file:// vẫn gọi được Done nhờ CORS (trả Access-Control-Allow-Origin).
#   POST /api/done?runId=<ts>  hoặc body JSON {"runId": "<ts>"}
#        -> chuyển reports/new/<ts> sang reports/done/<ts>.
#
# Tự khởi động qua .vscode/tasks.json (runOn: folderOpen). Cổng cố định, chỉ loopback.
# Console log tiếng Anh, comment tiếng Việt.

import json
import os
import shutil
import sys
import time
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from urllib.parse import urlparse, parse_qs

PORT = 8770
# reports/ nằm cạnh scripts/ (script ở scripts/, lùi 1 cấp)
ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "reports"))


class Handler(SimpleHTTPRequestHandler):
    # Phục vụ tĩnh từ ROOT (reports/) — tuỳ chọn, report cũng mở được bằng file://
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=ROOT, **kwargs)

    # Thêm CORS cho mọi phản hồi để trang file:// gọi được /api/done
    def end_headers(self):
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        super().end_headers()

    def _send_json(self, code, obj):
        body = json.dumps(obj).encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(body)

    # Trả lời preflight CORS
    def do_OPTIONS(self):
        self.send_response(204)
        self.send_header("Content-Length", "0")
        self.end_headers()

    def do_POST(self):
        parsed = urlparse(self.path)
        if parsed.path != "/api/done":
            self._send_json(404, {"ok": False, "error": "not found"})
            return

        # runId lấy từ query trước, nếu không có thì đọc body JSON
        run_id = (parse_qs(parsed.query).get("runId") or [""])[0]
        length = int(self.headers.get("Content-Length") or 0)
        if not run_id and length:
            try:
                payload = json.loads(self.rfile.read(length).decode("utf-8")) or {}
                run_id = payload.get("runId", "")
            except Exception:
                run_id = ""

        # Chặn path traversal: chỉ giữ tên thư mục cuối
        run_id = os.path.basename(run_id.strip())
        src = os.path.join(ROOT, "new", run_id)
        if not run_id or not os.path.isdir(src):
            self._send_json(400, {"ok": False, "error": "run not found: %s" % run_id})
            return

        done_dir = os.path.join(ROOT, "done")
        os.makedirs(done_dir, exist_ok=True)
        dest = os.path.join(done_dir, run_id)
        if os.path.exists(dest):
            dest = "%s-%d" % (dest, int(time.time()))
        shutil.move(src, dest)
        rel = "reports/done/" + os.path.basename(dest)
        print("[report-server] archived %s -> %s" % (run_id, rel))
        self._send_json(200, {"ok": True, "dest": rel})

    def log_message(self, fmt, *args):
        sys.stderr.write("[report-server] %s\n" % (fmt % args))


def main():
    os.makedirs(ROOT, exist_ok=True)
    try:
        httpd = ThreadingHTTPServer(("127.0.0.1", PORT), Handler)
    except OSError:
        # Cổng đã bận: nhiều khả năng server đang chạy sẵn -> thoát êm
        print("[report-server] port %d already in use, assuming server is already running." % PORT)
        return
    print("[report-server] listening on http://127.0.0.1:%d/  (Done API: POST /api/done)" % PORT)
    print("[report-server] reports root: %s" % ROOT)
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("[report-server] stopped.")


if __name__ == "__main__":
    main()
