# Quy tắc nghiệp vụ — Tính tiền

## R1 — Subtotal (tổng trước thuế)
`Subtotal = Σ (đơn giá món × số lượng)` cho mọi dòng trong đơn.

## R2 — VAT
`VAT = Subtotal × 10%`. Tỷ lệ 10% cố định, khai báo hằng số trong Model.

## R3 — Giảm giá theo ngưỡng (tuỳ chọn)
- Đơn có `Subtotal >= 500.000đ` → giảm 10% trên Subtotal.
- Giảm giá tính TRƯỚC VAT: `Total = (Subtotal − Discount) + (Subtotal − Discount) × 10%`.

## R4 — Total
`Total = (Subtotal − Discount) + VAT_sau_giảm`. Total không bao giờ âm.

## R5 — Đơn vị tiền tệ
Mọi phép tính dùng `decimal` (VND). Làm tròn tới đồng (0 chữ số thập phân) khi hiển thị.

## Ví dụ
- 3 món × 200.000 = Subtotal 600.000 → đạt ngưỡng giảm 10% = 60.000.
- Sau giảm: 540.000; VAT 10% = 54.000 → **Total = 594.000đ**.
