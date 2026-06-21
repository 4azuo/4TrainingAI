# Ma trận chức năng (Feature Matrix)

> Tài liệu **dùng chung**: ánh xạ **chức năng ↔ màn hình ↔ tầng xử lý**. Dùng để rà soát
> độ phủ và tìm nhanh nơi sửa khi đổi nghiệp vụ.

## Ma trận chức năng × màn hình
| Chức năng | SCR-01 Quản lý món | SCR-02 Gọi món |
|-----------|:------------------:|:--------------:|
| Thêm món (Create) | ✅ | — |
| Sửa món (Update) | ✅ | — |
| Xoá món (Delete) | ✅ | — |
| Xem danh sách món (Read) | ✅ | ✅ (combobox) |
| Tìm kiếm / lọc món | ✅ | — |
| Thêm món vào đơn | — | ✅ |
| Tính tiền (subtotal/VAT/giảm/total) | — | ✅ |

## Ma trận CRUD × thực thể × tầng
| Thực thể | Tầng nghiệp vụ (Service) | Tầng dữ liệu (Repository) | Lưu trữ |
|----------|--------------------------|---------------------------|---------|
| `MenuItem` | `MenuService` (CRUD + Search) | `IMenuRepository` → `JsonMenuRepository` | `data/menu.json` |
| `Order` / `OrderLine` | `OrderService` (logic tính tiền) | *(phiên làm việc — không lưu)* | trong bộ nhớ |

## Ma trận quy tắc nghiệp vụ × nơi thực thi
| Nhóm quy tắc | Mã | Thực thi tại |
|--------------|----|--------------|
| Quản lý món | M1–M5 | `MenuService` ([menu-management](../04-detail-designs/SCR-01-quan-ly-mon.md)) |
| Tìm kiếm món | S1–S5 | `MenuService.Search` |
| Tính tiền | R1–R5 | `Order` (entity) + `OrderService` |
