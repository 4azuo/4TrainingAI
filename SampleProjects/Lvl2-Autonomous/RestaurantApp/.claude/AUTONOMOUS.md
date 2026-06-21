# Chế độ tự trị (Autonomous) — cách lắp ráp

> **Sample project — chỉ định nghĩa workflow, KHÔNG tự chạy.** Các file dưới đây mô tả một vòng
> làm việc không người giám sát, dự kiến deploy trên **WSL** (môi trường cô lập nên cho AI full quyền).

## Các mảnh ghép
| File | Vai trò |
|------|---------|
| `.claude/hooks/autonomous-tick.sh` | Cron tick 10'/lần; `flock` để **bận thì bỏ lượt, rảnh thì chạy**; gọi `claude -p /auto-cycle`. |
| `.claude/commands/auto-cycle.md` | Định nghĩa **một vòng**: cổng token → đồng bộ `lastest` → sinh task → làm 1 task → test → PR → merge. |
| `.claude/skills/usage/check_usage.py` | Skill `usage`: in % token và ghi `output/last-usage-check.json` (xóa & tạo lại mỗi lần) cho cổng token đọc. |
| `USER_TODO.md` | Người dùng ghi yêu cầu; vòng chạy đọc rồi clear. |
| `AI_TODO.md` / `AI_PROGRESS.md` / `AI_DONE.md` | Sổ task: hàng đợi → đang làm → đã xong (ID `TSK-{group:0000}-{task:0000}`). |
| `.claude/.claude/settings.json` | Hồ sơ quyền "bypass tất cả" cho chế độ tự trị (xem lưu ý bên dưới). |

## Vòng đời (1 tick)
```
cron 10' → autonomous-tick.sh
  ├─ flock bận?  → log "skip", thoát (chờ lượt sau)
  └─ rảnh → claude -p /auto-cycle
       0. usage: session<90% & weekly<95%?  (không → dừng)
       1. checkout/fetch/pull nhánh `lastest`
       2. USER_TODO.md → sinh task vào AI_TODO.md (có ID) → clear USER_TODO.md
       3. lấy 1 task nhỏ → AI_PROGRESS.md, xóa khỏi AI_TODO.md, commit `lastest`
       4. nhánh task/TSK-… → đối ứng → commit → scripts/14-test.ps1 → PR vào `main`
       5. về `lastest` → merge task → AI_DONE.md, xóa khỏi AI_PROGRESS.md, commit
       6. usage lần nữa → dừng
```

## Cài trên WSL
```bash
chmod +x .claude/hooks/autonomous-tick.sh
crontab -e
# thêm dòng (đổi /path cho đúng):
*/10 * * * * /path/to/RestaurantApp/.claude/hooks/autonomous-tick.sh
# theo dõi:
tail -f .claude/autonomous-tick.log
```
Yêu cầu: đã đăng nhập `claude` (có `~/.claude/.credentials.json`), có `git`, `gh` (đã `gh auth login`),
`python3`, và `pwsh`/`powershell` để chạy `scripts/14-test.ps1`.

## Lưu ý về quyền (quan trọng)
- Claude Code **chỉ tự nạp** `.claude/settings.json` và `.claude/settings.local.json`.
  File `.claude/.claude/settings.json` (đúng đường dẫn người dùng yêu cầu) là **hồ sơ tham chiếu**;
  để nó có hiệu lực thật, một trong các cách:
  1. Tick đã truyền sẵn `--permission-mode bypassPermissions --dangerously-skip-permissions` (đang dùng).
  2. Hoặc copy nội dung sang `.claude/settings.local.json`.
  3. Hoặc trỏ `CLAUDE_CONFIG_DIR` tới `.claude/.claude` khi chạy headless.
- Chỉ bật full-quyền trong môi trường cô lập (WSL/CI). Không bật trên máy có dữ liệu nhạy cảm.
