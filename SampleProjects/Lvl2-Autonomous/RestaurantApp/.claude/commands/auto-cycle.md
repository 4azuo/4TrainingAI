---
description: Chu kỳ tự trị — check token, lấy task từ USER_TODO, đối ứng 1 task nhỏ, test, PR, merge vào lastest.
allowed-tools: Bash(*), Read(*), Edit(*), Write(*), Glob(*), Grep(*)
---

# /auto-cycle — một vòng làm việc tự trị

Bạn đang chạy **không có người giám sát** (headless, kích bởi cron 10 phút/lần qua
`.claude/hooks/autonomous-tick.sh`). Làm **đúng và đủ một vòng** theo các bước dưới đây rồi
**dừng**. Quy ước nhánh tích hợp tên là **`lastest`** (đúng chính tả người dùng đặt), PR luôn
nhắm vào **`main`**.

> Nguyên tắc sống còn: **mỗi vòng chỉ làm MỘT task nhỏ** trong một group để tránh hết token
> giữa chừng. Nếu bước nào fail, ghi log vào `AI_DONE.md` mục "Sự cố" rồi dừng — đừng cố làm tiếp.

---

## Bước 0 — Cổng token (BẮT BUỘC, không qua thì dừng)
1. Chạy skill **usage**: `python .claude/skills/usage/check_usage.py`
2. Đọc `.claude/skills/usage/output/last-usage-check.json`, lấy `session_pct` và `weekly_pct`.
3. **Điều kiện đi tiếp:** `session_pct < 90` **VÀ** `weekly_pct < 95`.
   - Không thỏa → in `"[auto-cycle] Token cao (session=…%, weekly=…%) — bỏ lượt."` rồi **DỪNG**.

## Bước 1 — Đồng bộ nhánh `lastest`
1. Nếu đang ở nhánh khác `lastest`:
   - `git fetch origin`
   - Nếu chưa có `lastest`: `git checkout -b lastest origin/lastest` (nếu remote có) hoặc
     `git checkout -b lastest main` (tạo mới từ main).
   - Nếu đã có: `git checkout lastest`.
2. `git pull --ff-only origin lastest` (bỏ qua nếu remote chưa có nhánh này).

## Bước 2 — Thu nhận yêu cầu người dùng → sinh task
1. Đọc `USER_TODO.md`. Nếu **không có** yêu cầu mới (chỉ còn template) → bỏ qua phần sinh task.
2. Với mỗi yêu cầu: tách thành các task nhỏ làm được trong ~1 vòng. Ghi vào `AI_TODO.md`,
   mỗi task có **ID `TSK-{groupid:0000}-{taskid:0000}`** (group = một yêu cầu/đợt, task = việc con).
   Ví dụ: `TSK-0001-0001`, `TSK-0001-0002`.
3. **Clear `USER_TODO.md`** về template rỗng (xem mẫu trong chính file đó) để **không tạo lại
   task cũ** ở vòng sau.

## Bước 3 — Chọn 1 task, đưa vào tiến trình
1. Đọc `AI_TODO.md`, chọn **task nhỏ kế tiếp** (ưu tiên theo thứ tự, cùng group làm dần).
2. Chuyển task đó sang `AI_PROGRESS.md` (kèm thời điểm bắt đầu), **xóa khỏi `AI_TODO.md`**.
3. Commit trên `lastest`: `git add -A && git commit -m "chore(auto): nhận TSK-xxxx-xxxx vào tiến trình"`.

## Bước 4 — Đối ứng task trên nhánh riêng
1. Tạo nhánh: `git checkout -b task/TSK-xxxx-xxxx`.
2. Đối ứng nội dung task theo kiến trúc phân tầng + convention trong `CLAUDE.md`.
   Có test thì viết test (xem `tests/UT`).
3. Commit: `git add -A && git commit -m "feat(TSK-xxxx-xxxx): <mô tả ngắn>"`.
4. Xuất report để người dùng đọc & feedback sau:
   `pwsh -File scripts/14-test.ps1` (Windows) hoặc `powershell -File scripts/14-test.ps1`.
5. Đẩy nhánh và tạo PR vào `main`:
   `git push -u origin task/TSK-xxxx-xxxx`
   `gh pr create --base main --head task/TSK-xxxx-xxxx --title "TSK-xxxx-xxxx: <tiêu đề>" --body "<tóm tắt + link report>"`

## Bước 5 — Hợp nhất vào `lastest`, cập nhật sổ
1. `git checkout lastest`
2. `git merge --no-ff task/TSK-xxxx-xxxx`
3. Ghi task vào `AI_DONE.md` (ID, mô tả, thời điểm, link PR), **xóa khỏi `AI_PROGRESS.md`**.
4. Commit: `git add -A && git commit -m "chore(auto): hoàn tất TSK-xxxx-xxxx, merge vào lastest"`.
5. (Tùy chọn) `git push origin lastest`.

## Bước 6 — Chốt token
1. Chạy lại skill **usage**: `python .claude/skills/usage/check_usage.py`.
2. In tóm tắt: task đã làm, PR đã tạo, token còn lại. **DỪNG** (vòng sau cron sẽ kích lại).
