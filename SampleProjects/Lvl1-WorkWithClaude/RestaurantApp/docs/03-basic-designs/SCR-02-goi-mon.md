# [SCR-02] Gọi món / Lập đơn — Basic Design

| Thuộc tính | Giá trị |
|------------|---------|
| Mã màn hình | SCR-02 |
| Form (Views) | `MainForm` |
| Controller | `OrderController` |
| Service | `OrderService` |
| Trạng thái | Done |

## 1. Mục đích
Cho nhân viên **lập đơn gọi món** cho khách: chọn món từ thực đơn, thêm số lượng vào đơn,
xem các dòng đã gọi và **tổng tiền** (đã gồm giảm giá theo ngưỡng + VAT).

## 2. Phác thảo bố cục (wireframe)
```
┌─ RestaurantApp — Gọi món ─────────────────────────┐
│ Món: [▼ combobox thực đơn]  SL: [  1 ▲▼]  [Thêm]   │
│ ┌───────────────────────────────────────────────┐ │
│ │ Phở bò x2 = 120.000 đ                          │ │  ← danh sách dòng đơn
│ │ Lẩu thái x1 = 250.000 đ                        │ │
│ └───────────────────────────────────────────────┘ │
│ Tổng: 407.000 đ                                    │
└─────────────────────────────────────────────────────┘
```

## 3. Danh sách chức năng
| # | Chức năng | Mô tả ngắn | Tầng xử lý |
|---|-----------|-----------|------------|
| F1 | Nạp thực đơn | Đổ danh sách món vào combobox | `OrderService.GetMenu` |
| F2 | Thêm vào đơn | Thêm món đã chọn + số lượng (gộp nếu trùng) | `OrderService.AddItem` |
| F3 | Xem đơn | Liệt kê từng dòng + thành tiền dòng | `OrderService.GetLines` |
| F4 | Tính tổng | Subtotal − Discount + VAT (R1–R5) | `OrderService` / `Order` |

## 4. Dữ liệu liên quan
- Thực thể: `Order`, `OrderLine`, `MenuItem` (xem [05-data](../05-data/data-model.md)).
- Nguồn món: `IMenuRepository`. Đơn (`Order`) là **dữ liệu phiên**, không lưu file.

## 5. Ràng buộc tầng
- View chỉ gọi `OrderController`; KHÔNG tính tiền, KHÔNG gọi Repository.
- Toàn bộ công thức tính tiền nằm ở `Order` (entity) + `OrderService`.

## 6. Liên kết
- Detail design: [SCR-02-goi-mon.md](../04-detail-designs/SCR-02-goi-mon.md)
- Test: UT `tests/UT/Services/OrderServiceTests.cs`, `tests/UT/Controllers/OrderControllerTests.cs`.
