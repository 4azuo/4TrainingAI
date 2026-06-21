# Data — Mô hình dữ liệu (Model)

> Mô tả các thực thể (entity) trong `RestaurantApp.Models` và thuộc tính của chúng.

## MenuItem — Món trong thực đơn
| Thuộc tính | Kiểu | Ràng buộc | Ghi chú |
|------------|------|-----------|---------|
| `Id` | `int` | > 0, **bất biến** | Khoá định danh, do Repository cấp (`NextId`) |
| `Name` | `string` | không rỗng, không trùng (M1, M2) | Trim khi gán |
| `Category` | `string` | mặc định "Khác" | Trim; rỗng → "Khác" |
| `UnitPrice` | `decimal` | ≥ 0 (M3) | Tiền VND |
| `IsAvailable` | `bool` | mặc định `true` | Còn bán hay không |

## Order — Đơn gọi món
| Thuộc tính | Kiểu | Ghi chú |
|------------|------|---------|
| `Lines` | `IReadOnlyList<OrderLine>` | Các dòng món; thêm qua `AddLine` (gộp nếu trùng) |
| `VatRate` | hằng `decimal` = 0.10 | R2 |
| `DiscountThreshold` | hằng `decimal` = 500.000 | R3 |
| `DiscountRate` | hằng `decimal` = 0.10 | R3 |

Phương thức tính: `Subtotal()`, `Discount()`, `Vat()`, `CalculateTotal()` (R1–R5).

## OrderLine — Dòng món
| Thuộc tính | Kiểu | Ràng buộc | Ghi chú |
|------------|------|-----------|---------|
| `Item` | `MenuItem` | không null | Món được gọi |
| `Quantity` | `int` | > 0 (O2) | Tăng qua `AddQuantity` |

Phương thức: `LineTotal() = Item.UnitPrice × Quantity`.

## Tính bền vững (persistence)
| Thực thể | Lưu trữ | Nguồn |
|----------|---------|-------|
| `MenuItem` | **JSON** | `data/menu.json` qua `JsonMenuRepository` |
| `Order` / `OrderLine` | **không lưu** (dữ liệu phiên) | tạo mới mỗi lần lập đơn |
