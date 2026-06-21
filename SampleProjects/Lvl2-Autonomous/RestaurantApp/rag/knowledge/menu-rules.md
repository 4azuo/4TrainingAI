# Quy tắc nghiệp vụ — Quản lý & Tìm kiếm món

## Quản lý món (validate ở MenuService)
- **M1** Tên món không được rỗng (sau khi trim).
- **M2** Tên món không trùng món khác — không phân biệt hoa thường.
- **M3** Đơn giá ≥ 0 (kiểu `decimal`).
- **M4** Sửa/xoá theo Id; Id không tồn tại → `InvalidOperationException`.
- **M5** Khi sửa, kiểm tra trùng tên loại trừ chính món đang sửa.

## Tìm kiếm món (MenuService.Search)
- **S1** Khớp khi tên HOẶC danh mục chứa từ khoá.
- **S2** Không phân biệt hoa thường.
- **S3** Từ khoá rỗng/trắng → trả toàn bộ.
- **S4** `onlyAvailable = true` → loại món `IsAvailable = false`.
- **S5** Trim khoảng trắng đầu/cuối từ khoá trước khi so khớp.

## Thực đơn mặc định (InMemoryMenuRepository)
| Id | Tên | Danh mục | Giá |
|----|-----|----------|-----|
| 1 | Phở bò | Món chính | 60.000 |
| 2 | Cơm tấm | Món chính | 55.000 |
| 3 | Cà phê sữa | Đồ uống | 25.000 |
| 4 | Lẩu thái | Món chính | 250.000 |
