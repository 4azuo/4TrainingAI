#!/usr/bin/env bash
# Cron tick cho workflow tự trị — cài trên WSL, chạy MỖI 10 PHÚT.
#
#   crontab -e   →   */10 * * * * /path/RestaurantApp/.claude/hooks/autonomous-tick.sh
#
#   - Nếu lượt trước (hoặc một task khác) còn giữ khóa  → flock fail → ghi log "skip" rồi thoát.
# Cơ chế "bận thì chờ lượt sau": khóa bằng SỰ TỒN TẠI của file `.autonomous.lock` (không dùng flock).
#   - Nếu file khóa đã có  → một lượt cron khác đang chạy → ghi log "skip" rồi thoát.
#   - Nếu chưa có          → tạo file khóa → gọi Claude headless chạy /auto-cycle.
# Việc XÓA khóa do /auto-cycle đảm nhiệm (chạy xong hoặc lỗi đều xóa). Trap EXIT dưới đây chỉ là
# LƯỚI AN TOÀN: nếu tiến trình chết bất thường mà /auto-cycle chưa kịp xóa, wrapper dọn nốt khi thoát.
set -euo pipefail

# Thư mục gốc dự án = hai cấp trên file này (.claude/hooks → .claude → root)
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
LOCK="$PROJECT_DIR/.claude/.autonomous.lock"
LOG="$PROJECT_DIR/.claude/autonomous-tick.log"
ts() { date '+%F %T'; }

# Tạo khóa theo kiểu nguyên tử: `noclobber` khiến `> file` THẤT BẠI nếu file đã tồn tại,
# nên tránh được kẽ hở hai lượt cron cùng vượt qua một phép kiểm tra "tồn tại?" rời rạc.
if ! ( set -o noclobber; : > "$LOCK" ) 2>/dev/null; then
  echo "$(ts) [skip] đang bận (đã có .autonomous.lock) — chờ lượt sau" >> "$LOG"
  exit 0
fi
# Lưới an toàn: bảo đảm khóa được dọn khi wrapper thoát, kể cả khi Claude chết câm.
trap 'rm -f "$LOCK"' EXIT

cd "$PROJECT_DIR"

# QUAN TRỌNG: cron chạy với PATH tối thiểu (/usr/bin:/bin) và KHÔNG nạp ~/.bashrc/~/.profile.
# `claude` là bản npm-global của Windows, chỉ nằm trên PATH khi shell tương tác kế thừa PATH
# của Windows qua interop. Dưới cron nó "command not found" → lượt tick chết câm.
# Bổ sung thư mục npm-global của Windows vào PATH để cron tìm thấy wrapper claude.
export PATH="$PATH:/mnt/c/Users/PHONG HUYNH/AppData/Roaming/npm"

# Cho ghi đè qua biến môi trường; nếu vẫn không tìm thấy thì báo lỗi rõ rồi bỏ lượt.
CLAUDE_BIN="${CLAUDE_BIN:-$(command -v claude || true)}"
if [ -z "$CLAUDE_BIN" ]; then
  echo "$(ts) [error] không tìm thấy 'claude' trên PATH — kiểm tra đường dẫn npm-global của Windows" >> "$LOG"
  exit 0
fi

echo "$(ts) [run] bắt đầu /auto-cycle" >> "$LOG"
# Không mở cửa sổ GUI (cron không có desktop session). Mọi thứ ghi vào "$LOG"; xem realtime bằng:
#   tail -f .claude/autonomous-tick.log

# Headless: -p chạy một lượt rồi thoát. Không hỏi quyền vì sẽ chạy không người trực trên WSL.
# (Cổng token nằm TRONG /auto-cycle: session<90% và weekly<95% mới làm tiếp.)
"$CLAUDE_BIN" -p "/auto-cycle" \
  --permission-mode bypassPermissions \
  --dangerously-skip-permissions \
  >> "$LOG" 2>&1 || echo "$(ts) [warn] /auto-cycle thoát mã $?" >> "$LOG"

echo "$(ts) [done] kết thúc lượt" >> "$LOG"
# trap EXIT ở trên xóa nốt `.autonomous.lock` nếu /auto-cycle chưa kịp xóa → khóa được nhả.
