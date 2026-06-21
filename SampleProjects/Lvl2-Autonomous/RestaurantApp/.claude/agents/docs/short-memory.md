# docs — Short-memory (tối đa 10 mục)

> **LUẬT:** giữ **≤10 mục**, mới nhất trên cùng. Cuối mỗi việc, **docs tự thêm 1 mục**.
> Khi vượt 10: nén mục cũ nhất vào `long-memory.md` rồi xoá khỏi đây. Ngắn, đúng sự thật.

## Mẫu
```
### [SỐ] YYYY-MM-DD — <tiêu đề>
- Việc: <cập nhật doc nào> · File: <file docs>
- Bài học: <nếu có>
```

---

### [1] 2026-06-21 — Tài liệu chức năng quản lý & tìm kiếm món
- Việc: thêm `docs/features/menu-management.md`, `menu-search.md`; cập nhật rag. File: docs/features/, rag/.
- Bài học: doc phải khớp code thật (đọc MenuService trước khi viết).
