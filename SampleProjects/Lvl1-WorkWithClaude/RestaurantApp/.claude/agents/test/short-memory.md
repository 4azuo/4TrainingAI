# test — Short-memory (tối đa 10 mục)

> **LUẬT:** giữ **≤10 mục**, mới nhất trên cùng. Cuối mỗi việc, **test tự thêm 1 mục**.
> Khi vượt 10: nén mục cũ nhất vào `long-memory.md` rồi xoá khỏi đây. Ngắn, đúng sự thật.

## Mẫu
```
### [SỐ] YYYY-MM-DD — <tiêu đề>
- Việc: <test gì> · File: <file test>
- Kết quả: <pass/fail, số ca> · Bài học: <nếu có>
```

---

### [1] 2026-06-21 — Unit test cho MenuService
- Việc: thêm ca add/update/delete/search + chặn trùng tên. File: tests/UT/Models/MenuServiceTests.cs.
- Kết quả: chưa chạy (máy chưa có .NET). Bài học: assert chuỗi tiền phụ thuộc culture → cố định CultureInfo trong test.
