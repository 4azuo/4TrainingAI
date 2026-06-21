# Từ điển miền — Nhà hàng (Glossary)

> Tài liệu **dùng chung**: thuật ngữ nghiệp vụ. Tri thức chi tiết đặt ở `../../rag/`.

- **Bàn (Table)**: vị trí khách ngồi, gắn với một Order đang mở.
- **Order (Đơn gọi món)**: tập hợp các món khách gọi trong một lượt.
- **OrderLine (Dòng món)**: một MenuItem kèm số lượng trong Order.
- **MenuItem (Món)**: món trong thực đơn — có `Id`, `Name`, `Category`, `UnitPrice`, `IsAvailable`.
- **Subtotal**: tổng tiền hàng trước thuế và giảm giá.
- **Discount (Giảm giá)**: khoản trừ theo quy tắc R3 (ngưỡng 500.000đ → giảm 10%).
- **VAT**: thuế GTGT 10% (tính sau giảm giá).
- **Total (Thành tiền)**: số tiền khách phải trả cuối cùng.
- **Invoice (Hoá đơn)**: bản in tổng kết của một Order khi thanh toán.

## Thuật ngữ kỹ thuật
- **Repository**: đối tượng truy xuất dữ liệu (ở đây lưu **JSON**).
- **Service**: đối tượng chứa logic nghiệp vụ.
- **Composition root**: nơi duy nhất ráp các tầng (project `App`, `Program.cs`).
- **Seed data**: dữ liệu mẫu khởi tạo (`data/menu.json`).
