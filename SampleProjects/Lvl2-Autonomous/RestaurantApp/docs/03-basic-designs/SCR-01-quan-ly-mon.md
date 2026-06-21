# [SCR-01] Quản lý & Tìm kiếm món — Basic Design

| Thuộc tính | Giá trị |
|------------|---------|
| Mã màn hình | SCR-01 |
| Form (Views) | `MenuForm` |
| Controller | `MenuController` |
| Service | `MenuService` |
| Trạng thái | Done |

## 1. Mục đích
Cho nhân viên quản lý **thực đơn**: thêm / sửa / xoá món và **tìm kiếm** nhanh theo tên
hoặc danh mục. Đây là màn hình thao tác dữ liệu gốc (`MenuItem`) của hệ thống.

## 2. Phác thảo bố cục (wireframe)
```
┌─ RestaurantApp — Quản lý & Tìm kiếm món ─────────────────────┐
│ [Tìm theo tên/danh mục...] [□ Chỉ món đang bán] [Tìm] [Xoá lọc]│
│ ┌──────────────────────────────────────────────────────────┐ │
│ │ Id │ Tên món │ Danh mục │ Đơn giá │ Đang bán             │ │  ← lưới món
│ └──────────────────────────────────────────────────────────┘ │
│ "N món"                                                       │
│ [Tên món] [Danh mục] [Đơn giá] [□ Đang bán]                  │  ← khu nhập liệu
│ [Thêm món] [Cập nhật] [Xoá món]                              │
└───────────────────────────────────────────────────────────────┘
```

## 3. Danh sách chức năng
| # | Chức năng | Mô tả ngắn | Tầng xử lý |
|---|-----------|-----------|------------|
| F1 | Xem danh sách | Hiển thị toàn bộ món trong lưới | `MenuService.GetAll` |
| F2 | Tìm kiếm | Lọc theo từ khoá (tên/danh mục) + "chỉ đang bán" | `MenuService.Search` |
| F3 | Thêm món | Thêm món mới (validate trùng tên, giá ≥ 0) | `MenuService.AddDish` |
| F4 | Sửa món | Cập nhật theo Id đang chọn | `MenuService.UpdateDish` |
| F5 | Xoá món | Xoá theo Id đang chọn | `MenuService.DeleteDish` |

## 4. Dữ liệu liên quan
- Thực thể: `MenuItem` (xem [05-data/data-model.md](../05-data/data-model.md)).
- Nguồn dữ liệu: `IMenuRepository` → `JsonMenuRepository` (`data/menu.json`).

## 5. Ràng buộc tầng
- View **không** tự validate, **không** gọi Repository — chỉ gọi `MenuController`.
- Validate trùng tên / giá ≥ 0 và logic lọc nằm ở `MenuService`.

## 6. Liên kết
- Detail design: [SCR-01-quan-ly-mon.md](../04-detail-designs/SCR-01-quan-ly-mon.md)
- Test: UT `tests/UT/Services/MenuServiceTests.cs`; IT TC-001/002/003/004.
