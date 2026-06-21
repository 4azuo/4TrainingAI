# TC-004 — Xoá món

| | |
|--|--|
| **Chức năng** | Quản lý món ăn |
| **Mức** | Integration |
| **Tự động hoá bởi** | `harness` scenario `TC-004` |

## Tiền điều kiện
Thực đơn mặc định 4 món, có món Id = 1 (Phở bò).

## Các bước
1. Gọi `MenuController.DeleteDish(1)`.

## Kết quả mong đợi
- Số món giảm 4 → 3.
- Không còn món Id = 1.

## Evidence (tự sinh)
- `evidence/TC-004/output.log` — số món trước/sau.
- `evidence/TC-004/screenshot.png` — ảnh MenuForm sau khi xoá.
