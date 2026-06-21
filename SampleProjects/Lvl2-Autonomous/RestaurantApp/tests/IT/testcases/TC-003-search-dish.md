# TC-003 — Tìm kiếm món theo từ khoá

| | |
|--|--|
| **Chức năng** | Tìm kiếm món ăn |
| **Mức** | Integration |
| **Tự động hoá bởi** | `harness` scenario `TC-003` |

## Tiền điều kiện
Thực đơn mặc định 4 món.

## Các bước
1. Gọi `MenuController.Search("phở")`.
2. Trên UI: tool gọi `MenuForm.ApplySearch("phở", false)` để lưới hiển thị kết quả lọc.

## Kết quả mong đợi
- Trả về đúng 1 món có tên chứa "Phở" (Phở bò).
- Lưới trên screenshot chỉ còn dòng khớp.

## Evidence (tự sinh)
- `evidence/TC-003/output.log` — số kết quả & tên món khớp.
- `evidence/TC-003/screenshot.png` — ảnh MenuForm đã lọc theo "phở".
