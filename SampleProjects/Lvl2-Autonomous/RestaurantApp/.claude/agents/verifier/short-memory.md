# verifier — Short-memory (tối đa 10 mục)

> **LUẬT:** giữ **≤10 mục**, mới nhất trên cùng. Cuối mỗi lần kiểm chứng, **verifier tự thêm 1 mục**.
> Khi vượt 10: nén mục cũ nhất vào `long-memory.md` rồi xoá khỏi đây. Ngắn, đúng sự thật.

## Mẫu
```
### [SỐ] YYYY-MM-DD — <tiêu đề>
- Kiểm: build / test / ranh giới · Kết quả: ĐẠT / CHƯA ĐẠT (<lý do>)
- Bài học: <nếu có>
```

---

### [1] 2026-06-21 — Kiểm chứng khung quản lý & tìm kiếm món
- Kiểm: build/test/ranh giới MVC. Kết quả: CHƯA CHẠY (máy chưa có .NET 8 SDK).
- Bài học: phải cài SDK mới kết luận được; trước mắt soát ranh giới bằng grep `using RestaurantApp.Views` trong Models.
