#!/usr/bin/env bash
# Cron tick cho workflow tự trị — cài trên WSL, chạy MỖI 10 PHÚT.
#
#   crontab -e   →   */10 * * * * /path/RestaurantApp/.claude/hooks/autonomous-tick.sh
#
# Cơ chế "bận thì chờ lượt sau": dùng flock không chặn (-n) trên một file khóa.
#   - Nếu lượt trước (hoặc một task khác) còn giữ khóa  → flock fail → ghi log "skip" rồi thoát.
#   - Nếu rảnh → lấy khóa → gọi Claude headless chạy /auto-cycle → nhả khóa khi xong.
# Nhờ vậy hai lượt cron không bao giờ chồng nhau, và một vòng dài sẽ "nuốt" các lượt 10' kế tiếp.
set -euo pipefail

# Thư mục gốc dự án = hai cấp trên file này (.claude/hooks → .claude → root)
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
LOCK="$PROJECT_DIR/.claude/.autonomous.lock"
LOG="$PROJECT_DIR/.claude/autonomous-tick.log"
ts() { date '+%F %T'; }

# Mở fd 9 trỏ tới file khóa rồi thử lấy khóa độc quyền, không chặn.
exec 9>"$LOCK"
if ! flock -n 9; then
  echo "$(ts) [skip] đang bận (lượt trước chưa xong) — chờ lượt sau" >> "$LOG"
  exit 0
fi

cd "$PROJECT_DIR"
echo "$(ts) [run] bắt đầu /auto-cycle" >> "$LOG"

# Headless: -p chạy một lượt rồi thoát. Không hỏi quyền vì sẽ chạy không người trực trên WSL.
# (Cổng token nằm TRONG /auto-cycle: session<90% và weekly<95% mới làm tiếp.)
claude -p "/auto-cycle" \
  --permission-mode bypassPermissions \
  --dangerously-skip-permissions \
  >> "$LOG" 2>&1 || echo "$(ts) [warn] /auto-cycle thoát mã $?" >> "$LOG"

echo "$(ts) [done] kết thúc lượt" >> "$LOG"
# fd 9 đóng khi script thoát → khóa tự nhả.
