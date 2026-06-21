# reporter — Short-memory (tối đa 10 mục)

> **LUẬT:** giữ **≤10 mục**, mới nhất trên cùng. Cuối mỗi báo cáo, **reporter tự thêm 1 mục**.
> Khi vượt 10: nén mục cũ nhất vào `long-memory.md` rồi xoá khỏi đây. Ngắn, đúng sự thật.

## Mẫu
```
### [SỐ] YYYY-MM-DD — <tiêu đề>
- Báo cáo: <chủ đề> · File: reports/<...>.html
- Số liệu: <test pass/fail, verifier ĐẠT?> · Bài học: <nếu có>
```

---

### [1] 2026-06-21 — Báo cáo khởi tạo chức năng quản lý & tìm kiếm món
- Báo cáo: tiến độ feature. File: reports/ (chưa tạo do chưa chạy được test).
- Bài học: chỉ ghi số liệu lấy từ output thật; nếu chưa chạy test thì ghi "chưa có dữ liệu".
