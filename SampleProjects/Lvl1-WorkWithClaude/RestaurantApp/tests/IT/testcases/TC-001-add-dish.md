# TC-001 — Thêm món mới hợp lệ

| | |
|--|--|
| **Chức năng** | Quản lý món ăn |
| **Mức** | Integration (Repository → Service → Controller → View) |
| **Tự động hoá bởi** | `harness` scenario `TC-001` |

## Tiền điều kiện
Thực đơn mặc định có 4 món (Phở bò, Cơm tấm, Cà phê sữa, Lẩu thái).

## Các bước
1. Gọi `MenuController.AddDish("Trà đào", 30000, "Đồ uống")`.
2. Lấy lại danh sách món.

## Kết quả mong đợi
- Số món tăng từ 4 → 5.
- Có món "Trà đào" với Id mới (> 0).

## Evidence (tự sinh)
- `evidence/TC-001/output.log` — các bước & kết quả PASS/FAIL.
- `evidence/TC-001/screenshot.png` — ảnh MenuForm hiển thị danh sách sau khi thêm.
