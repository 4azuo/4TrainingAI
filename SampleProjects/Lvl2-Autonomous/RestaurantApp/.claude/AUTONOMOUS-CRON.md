# Cài đặt cron 10 phút cho `autonomous-tick.sh`

Hướng dẫn cài **chu kỳ tự trị** chạy trên **WSL (Ubuntu)**: cron gọi
[`autonomous-tick.sh`](autonomous-tick.sh) mỗi **10 phút**, mỗi lượt chạy `claude -p "/auto-cycle"`
một vòng rồi thoát. Cơ chế khóa bằng file `.claude/.autonomous.lock` đảm bảo **không có hai lượt
chồng nhau** (lượt sau thấy khóa thì bỏ qua).

> Mọi lệnh dưới đây chạy trong **shell WSL** (Ubuntu), trừ khi ghi rõ là PowerShell/Windows.
> Thư mục gốc dự án (đặt là `$PROJECT`) ví dụ: `~/projects/RestaurantApp`.

---

## 0. Đặt biến cho tiện

```bash
PROJECT="$HOME/projects/RestaurantApp"   # sửa cho đúng đường dẫn của bạn
cd "$PROJECT"
```

---

## 1. Kiểm tra công cụ bắt buộc

Chạy khối này — mọi dòng phải in ra đường dẫn/version, **không** được "not found":

```bash
echo "WSL distro : $WSL_DISTRO_NAME"            # phải có tên distro (vd: Ubuntu)
command -v cron     || echo "❌ thiếu cron"      # dịch vụ cron
command -v claude   || echo "❌ thiếu claude"    # CLI Claude Code (npm-global của Windows)
command -v gh       || echo "❌ thiếu gh"        # GitHub CLI (tạo PR)
command -v git      || echo "❌ thiếu git"
command -v python3  || echo "❌ thiếu python3"   # cho skill usage (cổng token)
command -v node     || echo "❌ thiếu node"      # cho pipeline test
command -v flock    >/dev/null; true            # KHÔNG còn cần flock (đã bỏ) — chỉ tham khảo
```

Ghi chú từng cái:

| Công cụ | Vì sao cần | Cài thế nào (WSL) |
|---|---|---|
| **WSL** | Toàn bộ workflow chạy trong Ubuntu | Đã có sẵn nếu bạn đang đọc file này trong WSL |
| **cron** | Bộ hẹn giờ gọi script mỗi 10' | `sudo apt-get update && sudo apt-get install -y cron` |
| **claude** | Chạy `/auto-cycle` headless | Bản **npm-global của Windows**, gọi qua interop. Xem mục 2. |
| **gh** | Tạo PR vào `main` | `sudo apt-get install -y gh` (hoặc theo trang chính thức) |
| **git** | Thao tác nhánh/commit/merge | `sudo apt-get install -y git` |
| **python3** | Skill `usage` kiểm token (cổng bắt buộc trong `/auto-cycle`) | `sudo apt-get install -y python3` |
| **node** | `scripts/14-test.ps1` → harness render headless | cài Node LTS trong WSL |
| **Chrome/Edge** | Harness IT render WebGL headless | dùng Chrome/Edge trên Windows (interop) hoặc cài trong WSL |

> `pwsh`/`powershell` cho `scripts/14-test.ps1`: bước test trong `/auto-cycle` gọi PowerShell.
> Trên WSL có thể gọi `powershell.exe`/`pwsh.exe` của Windows qua interop. Kiểm tra:
> `command -v powershell.exe || command -v pwsh.exe`.

---

## 2. `claude` phải tìm thấy được dưới cron

**Quan trọng:** cron chạy với PATH tối thiểu (`/usr/bin:/bin`) và **không** nạp `~/.bashrc`/`~/.profile`.
`claude` là wrapper npm-global của **Windows**, chỉ có trên PATH khi shell tương tác kế thừa PATH của
Windows. Script đã tự thêm thư mục npm-global vào PATH:

```bash
export PATH="$PATH:/mnt/c/Users/PHONG HUYNH/AppData/Roaming/npm"
```

- Nếu đường dẫn npm-global của bạn **khác**, sửa lại dòng đó trong
  [`autonomous-tick.sh`](autonomous-tick.sh), hoặc đặt biến môi trường `CLAUDE_BIN` trỏ thẳng tới
  binary (xem mục 5).
- Kiểm tra nhanh đúng cách cron sẽ thấy:
  ```bash
  env -i PATH="/usr/bin:/bin:/mnt/c/Users/PHONG HUYNH/AppData/Roaming/npm" \
    bash -lc 'command -v claude'
  ```

**Đăng nhập** (làm 1 lần, trong shell tương tác):

```bash
claude            # đảm bảo đã đăng nhập tài khoản (chạy thử rồi /exit)
gh auth status || gh auth login    # đăng nhập GitHub để tạo PR được
git config --global user.name  "Tên Bạn"
git config --global user.email "email@cua.ban"
```

---

## 3. Phân quyền & chuẩn hoá xuống dòng cho script

```bash
chmod +x "$PROJECT/.claude/hooks/autonomous-tick.sh"

# Nếu file từng bị lưu kiểu Windows (CRLF) → cron báo "bad interpreter". Khử CRLF:
sed -i 's/\r$//' "$PROJECT/.claude/hooks/autonomous-tick.sh"

# Kiểm tra cú pháp trước khi giao cho cron:
bash -n "$PROJECT/.claude/hooks/autonomous-tick.sh" && echo "✅ cú pháp OK"
```

---

## 4. Bật dịch vụ cron trong WSL

WSL **không** tự bật cron. Bật thủ công mỗi lần mở WSL:

```bash
sudo service cron start
service cron status      # phải thấy "is running"
```

**Tự bật khi mở WSL** (khỏi gõ tay) — thêm vào cuối `~/.bashrc`:

```bash
# tự khởi động cron nếu chưa chạy (không hỏi mật khẩu nhờ NOPASSWD bên dưới)
if ! pgrep -x cron >/dev/null 2>&1; then sudo service cron start >/dev/null 2>&1; fi
```

Cho phép `sudo service cron start` **không hỏi mật khẩu** (để dòng trên chạy êm):

```bash
echo "$USER ALL=(root) NOPASSWD: /usr/sbin/service cron start" | \
  sudo tee /etc/sudoers.d/cron-nopasswd >/dev/null
sudo chmod 440 /etc/sudoers.d/cron-nopasswd
```

> Muốn cron chạy **ngay cả khi chưa mở cửa sổ WSL nào**: bật `systemd` cho WSL
> (`/etc/wsl.conf` → `[boot]\nsystemd=true`, rồi `wsl --shutdown` từ PowerShell), hoặc tạo
> **Task Scheduler** trên Windows gọi `wsl -d Ubuntu -e bash -lc '<script>'`. Mặc định cron chỉ
> sống khi WSL còn ít nhất một phiên mở.

---

## 5. Cài lịch crontab (mỗi 10 phút)

Mở crontab của user:

```bash
crontab -e        # lần đầu chọn editor (nano cho dễ)
```

Thêm dòng (sửa đường dẫn cho khớp `$PROJECT` của bạn):

```cron
# Xế Nổ Đường Phố — chu kỳ tự trị mỗi 10 phút
*/10 * * * * /home/phonghc/projects/RestaurantApp/.claude/hooks/autonomous-tick.sh >> /home/phonghc/projects/RestaurantApp/.claude/cron.out 2>&1
```

- Script **đã tự ghi** mọi thứ vào `.claude/autonomous-tick.log`; phần `>> cron.out 2>&1` chỉ để
  bắt lỗi *trước khi* script kịp mở log (vd. sai quyền, CRLF). Có thể bỏ nếu không cần.
- **Tùy chọn settings qua biến môi trường** — đặt ngay đầu dòng cron nếu muốn ghi đè:
  ```cron
  */10 * * * * CLAUDE_BIN=/mnt/c/Users/PHONG\ HUYNH/AppData/Roaming/npm/claude /home/phonghc/projects/RestaurantApp/.claude/hooks/autonomous-tick.sh
  ```
  Các biến script hiểu:
  - `CLAUDE_BIN` — đường dẫn binary `claude` (nếu auto-detect không ra).

Lưu lại. Kiểm tra đã nạp:

```bash
crontab -l
```

---

## 6. Chạy thử (không chờ tới mốc 10 phút)

```bash
# Chạy tay đúng như cron sẽ chạy (PATH tối thiểu, không nạp .bashrc):
env -i PATH="/usr/bin:/bin" "$PROJECT/.claude/hooks/autonomous-tick.sh"
echo "exit=$?"
```

Sau đó xem log:

```bash
tail -n 40 "$PROJECT/.claude/autonomous-tick.log"
```

Mong đợi thấy `[run] bắt đầu /auto-cycle` → … → `[done] kết thúc lượt`. Nếu token cao, `/auto-cycle`
sẽ in dòng "Token cao… bỏ lượt" và dừng — đó là **bình thường**.

---

## 7. Theo dõi realtime

```bash
tail -f "$PROJECT/.claude/autonomous-tick.log"
```

(Đã bỏ phần tự mở cửa sổ Windows — cron không có desktop session nên dùng `tail -f` là chắc ăn nhất.)

---

## 8. Cờ trạng thái & khóa

- **Khóa**: `.claude/.autonomous.lock` — tồn tại = đang có lượt chạy. `/auto-cycle` xóa nó khi xong
  hoặc lỗi; wrapper có trap dọn nốt khi thoát. Nếu lỡ kẹt (máy tắt đột ngột), xóa tay:
  ```bash
  rm -f "$PROJECT/.claude/.autonomous.lock"
  ```
- **Sổ tiến trình**: `AI_TODO.md` / `AI_PROGRESS.md` / `AI_DONE.md` (mục "Sự cố" ghi lỗi từng vòng).

---

## 9. Gỡ lỗi thường gặp

| Triệu chứng (trong log) | Nguyên nhân | Cách xử lý |
|---|---|---|
| Không có dòng `[run]` nào | cron chưa chạy | `sudo service cron start`; `crontab -l` xem đã nạp chưa |
| `bad interpreter: ^M` | file CRLF | `sed -i 's/\r$//' autonomous-tick.sh` |
| `Permission denied` | thiếu quyền chạy | `chmod +x autonomous-tick.sh` |
| `[error] không tìm thấy 'claude'` | PATH npm-global sai | sửa dòng `export PATH` hoặc đặt `CLAUDE_BIN` |
| Mọi lượt đều `[skip] đang bận` | khóa kẹt | `rm -f .claude/.autonomous.lock` |
| `gh: not found` / PR fail | thiếu/chưa login gh | `sudo apt-get install -y gh && gh auth login` |
| Token luôn "bỏ lượt" | session/weekly cao | đợi reset, hoặc xem `.claude/skills/usage/output/last-usage-check.json` |

---

## 10. Gỡ cài đặt

```bash
crontab -e            # xóa dòng */10 … autonomous-tick.sh, lưu lại
rm -f "$PROJECT/.claude/.autonomous.lock"
sudo service cron stop   # nếu muốn tắt hẳn cron
```
