# TC-002 — Chặn thêm món trùng tên

| | |
|--|--|
| **Chức năng** | Quản lý món ăn (validate) |
| **Mức** | Integration |
| **Tự động hoá bởi** | `harness` scenario `TC-002` |

## Tiền điều kiện
Thực đơn đã có "Phở bò".

## Các bước
1. Gọi `MenuController.AddDish("Phở bò", 10000, "Món chính")`.

## Kết quả mong đợi
- Ném `InvalidOperationException` ("Món 'Phở bò' đã tồn tại").
- Danh sách món KHÔNG đổi (vẫn 4 món).

## Evidence (tự sinh)
- `evidence/TC-002/output.log` — xác nhận bị từ chối + danh sách không đổi.
- `evidence/TC-002/screenshot.png` — ảnh MenuForm (danh sách nguyên trạng).
