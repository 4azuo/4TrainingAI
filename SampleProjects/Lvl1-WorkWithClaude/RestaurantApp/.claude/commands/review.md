---
description: Rà soát thay đổi hiện tại — chất lượng code và tuân thủ ranh giới các tầng.
---

Rà soát các thay đổi chưa commit của RestaurantApp:

1. Chạy `git diff` để xem thay đổi.
2. Dùng sub-agent **verifier** kiểm tra:
   - Build pass, test pass.
   - Models không tham chiếu tầng trên; Repositories không chứa logic nghiệp vụ.
   - View/Controller không gọi thẳng Repository (View → Controller → Service → Repository).
   - Logic tiền/thuế không nằm trong Form.
3. Soát thêm: đặt tên rõ ràng, không lặp code, theo `docs/01-commons/coding-conventions.md`.

Đầu ra: danh sách phát hiện theo mức độ (Chặn / Nên sửa / Gợi ý), kèm `file:line`.
