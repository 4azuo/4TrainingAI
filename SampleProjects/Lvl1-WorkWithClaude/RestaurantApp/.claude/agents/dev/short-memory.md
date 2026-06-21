# dev — Short-memory (tối đa 10 mục)

> **LUẬT:** giữ **≤10 mục**, mới nhất trên cùng. Cuối mỗi việc, **dev tự thêm 1 mục**.
> Khi vượt 10: nén mục cũ nhất vào `long-memory.md` rồi xoá khỏi đây. Ngắn, đúng sự thật.

## Mẫu
```
### [SỐ] YYYY-MM-DD — <tiêu đề>
- Việc: <làm gì> · File: <file chính>
- Kết quả: <build/verifier> · Bài học: <nếu có>
```

---

### [1] 2026-06-21 — Thêm chức năng quản lý & tìm kiếm món
- Việc: thêm `MenuService` (Models), `MenuController`, `MenuForm` (View). File: src/Models, src/Controllers, src/Views.
- Kết quả: chưa build (máy chưa có .NET). Bài học: giữ validate (trùng tên, giá ≥0) trong MenuService, View không tự lọc.
