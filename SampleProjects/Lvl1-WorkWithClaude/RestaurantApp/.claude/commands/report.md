---
description: Chạy test và sinh báo cáo Test HTML (UT coverage + IT evidence).
argument-hint: [tiêu đề báo cáo]
---

Tạo báo cáo Test cho RestaurantApp.

Tiêu đề: $ARGUMENTS (nếu trống, dùng "Báo cáo Test RestaurantApp").

Dùng sub-agent **reporter** + skill **test-report**:
1. Chạy `powershell -File scripts/test.ps1 -Title "$ARGUMENTS"` — chạy UT (có coverage) + IT
   (evidence), gom vào `reports/new/<yyyyMMdd-HHmmss>/` rồi sinh `report.html` + `report-data.js`.
2. (Tuỳ chọn) Lấy kết luận kiểm chứng từ sub-agent **verifier**.
3. Mở report kèm nút Done: `powershell -File scripts/open-report.ps1 -RunDir reports/new/<ts>`.
4. Trả về đường dẫn `reports/new/<ts>/report.html`.
