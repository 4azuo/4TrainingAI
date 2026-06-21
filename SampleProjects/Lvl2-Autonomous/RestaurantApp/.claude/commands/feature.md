---
description: Hiện thực một tính năng mới end-to-end theo quy trình kiến trúc phân tầng + multi-agent.
argument-hint: <mô tả tính năng>
---

Triển khai tính năng sau cho RestaurantApp theo đúng kiến trúc phân tầng (Model/Repository/Service/Controller/View):

**Yêu cầu:** $ARGUMENTS

Quy trình bắt buộc (điều phối các sub-agent):
1. Dùng sub-agent **dev** để hiện thực code trong `src/` (giữ ranh giới các tầng).
2. Dùng sub-agent **test** để viết & chạy unit test cho phần nghiệp vụ mới.
3. Dùng sub-agent **docs** cập nhật `docs/` nếu kiến trúc/hành vi thay đổi.
4. Dùng sub-agent **verifier** để xác minh build + test + ranh giới kiến trúc.
5. Báo lại tóm tắt: file đã đổi, test thêm, kết quả verifier.

Chỉ kết luận "hoàn tất" khi verifier báo **ĐẠT**.
