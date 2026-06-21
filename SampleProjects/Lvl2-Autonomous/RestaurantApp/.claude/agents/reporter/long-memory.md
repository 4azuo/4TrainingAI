# reporter — Long-memory (tối đa 500 chữ)

> **LUẬT:** ≤500 chữ. Chỉ tri thức bền vững phục vụ việc báo cáo.

- Quy trình: chạy `scripts/test.ps1` (UT+coverage, IT+evidence → `reports/new/<ts>/`) → skill
  `test-report` sinh report; mở qua `scripts/open-report.ps1` để nút Done lưu trữ sang `reports/done/<ts>/`.
- Trung thực tuyệt đối: test fail ghi fail; chưa chạy ghi "chưa có dữ liệu"; số liệu khớp trx/cobertura/evidence.
- Report gồm 3 tab: UT (coverage theo file + chi tiết method), IT (scenario + evidence), Tổng hợp; có search highlight.
- Report self-contained (html/css/js cục bộ, không CDN) để mở offline.
