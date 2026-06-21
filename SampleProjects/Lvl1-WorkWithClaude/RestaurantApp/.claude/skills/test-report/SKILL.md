---
name: test-report
description: Sinh báo cáo Test HTML (self-contained, html/css/js tách riêng) từ kết quả một lần chạy — Unit Test (coverage theo file) và Integration Test (evidence). Có tab UT/IT/Tổng hợp, search highlight + next/prev, nút Done để lưu trữ. Dùng sau khi chạy scripts/test.ps1 hoặc khi user gọi /report.
---

# Skill: Báo cáo Test (test-report)

Sinh một **báo cáo Test** mở được offline cho RestaurantApp, gồm 2 phần: **Unit Test** (theo
file nguồn + coverage) và **Integration Test** (theo scenario + evidence). Template tách riêng
**html / css / js**; dữ liệu nạp qua `report-data.js`.

## Khi nào dùng
- Sau khi chạy `scripts/test.ps1` (đã tự gọi skill này).
- User gọi command `/report`.
- Sub-agent `reporter` cần render kết quả test.

## Mô hình thư mục một lần chạy
Mỗi lần chạy gom kết quả vào `reports/new/<yyyyMMdd-HHmmss>/`:
```
reports/new/<ts>/
├── ut/test-results.trx          # kết quả UT theo method (logger trx)
├── ut/coverage.cobertura.xml    # coverage theo file (coverlet)
├── it/evidence/<TC>/output.log  # evidence IT: các bước + PASS/FAIL
├── it/evidence/<TC>/screenshot.png
├── report-data.js               # SINH RA: window.REPORT_DATA = {...}
├── report.html / report.css / report.js   # copy từ skill này
```
Khi bấm **Done**, cả thư mục được chuyển sang `reports/done/<ts>/`.

## Đầu vào → dữ liệu
| Phần | Nguồn |
|------|-------|
| UT: test nào, pass/fail, thời gian | `ut/test-results.trx` |
| UT: coverage % mỗi file + tổng | `ut/coverage.cobertura.xml` |
| IT: scenario, chức năng, các bước, mong đợi | `tests/IT/testcases/*.md` + `testcases.json` |
| IT: kết quả + evidence | `it/evidence/<TC>/output.log` + `screenshot.png` |

## Các bước thực hiện
1. Chạy `scripts/test.ps1` (chạy UT có coverage + IT, rồi gọi
   `scripts/generate-report.ps1 -RunDir reports/new/<ts>`).
2. `generate-report.ps1` parse trx + cobertura + evidence → ghi `report-data.js`, rồi copy
   `report.html`, `report.css`, `report.js` từ skill này vào thư mục run.
3. Mở report kèm nút Done hoạt động:
   `powershell -File scripts/open-report.ps1 -RunDir reports/new/<ts>`.

## Nội dung báo cáo
- **Tab Unit Test**: bảng theo file nguồn (`src/...`) — số test, pass/fail, **coverage % từng
  file**; mở rộng một dòng để xem **chi tiết từng method** (tên, Passed/Failed, thời gian).
  Có **coverage tổng**.
- **Tab Integration Test**: liệt kê theo **scenario** (file testcase); mở rộng một testcase để
  xem các bước, kết quả mong đợi, **output.log** và **screenshot** (evidence). Hiển thị các
  **chức năng** được phủ.
- **Tab Tổng hợp**: thẻ số liệu UT (tổng/pass/fail + coverage tổng) và IT (tổng/pass/fail) +
  danh sách chức năng được phủ.
- **Search**: gõ từ khoá → tô màu (`<mark>`) mọi chỗ khớp trong tab hiện tại; **Next/Prev**
  (hotkey **Enter** = sau, **Shift+Enter** = trước); match hiện tại đổi màu + cuộn tới.
- **Done**: gọi `POST /api/done` (do `open-report.ps1` phục vụ) để chuyển run sang
  `reports/done/<ts>/`. Nếu mở trực tiếp bằng `file://`, nút Done hiện lệnh lưu trữ thủ công.

## Quy ước
- KHÔNG dùng CDN/script ngoài → report mở được offline (css/js là file cục bộ cạnh html).
- Số liệu phải lấy đúng từ trx/cobertura/evidence — **không bịa**.
- Badge xanh (#1b9e4b) khi pass, đỏ (#d93025) khi fail; coverage ≥80% xanh, ≥50% vàng, còn lại đỏ.
