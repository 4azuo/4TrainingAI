# Quy ước code RestaurantApp

> Tài liệu **dùng chung** cho mọi tầng.

## Đặt tên
- Class / method / property: `PascalCase`.
- Biến cục bộ / tham số: `camelCase`.
- Field private: `_camelCase`.
- 1 file = 1 class công khai; tên file trùng tên class.
- Interface bắt đầu bằng `I` (`IMenuRepository`).

## Định dạng
- Indent 4 space, dấu `{` xuống dòng mới (Allman) — theo `.editorconfig`.
- Dòng tối đa ~120 ký tự.
- `using` sắp xếp, `System.*` lên đầu.

## Nguyên tắc thiết kế (kiến trúc phân tầng)
- **Model**: chỉ là thực thể (entity) — dữ liệu + bất biến nội tại; KHÔNG truy xuất dữ liệu.
- **Repository**: đọc/ghi dữ liệu (JSON). KHÔNG chứa logic nghiệp vụ.
- **Service**: toàn bộ logic nghiệp vụ (validate, tìm kiếm, tính tiền).
- **Controller**: điều phối View ↔ Service. KHÔNG chứa logic nghiệp vụ lẫn logic UI.
- **View**: hiển thị + nhận thao tác; chỉ gọi Controller.
- Tránh lặp code; tách hàm khi một khối làm >1 việc.
- Method nên ngắn, đặt tên theo hành vi (`AddLine`, `CalculateTotal`).
- Tiền tệ dùng `decimal`, KHÔNG dùng `double`/`float`.

## Test
- xUnit, tên `Method_Condition_Expected`.
- Mỗi business rule mới phải có test (đặt đúng tầng trong `tests/UT`).

## Commit
- Tiếng Việt hoặc Anh đều được, ở thể mệnh lệnh: "Thêm giảm giá theo ngưỡng đơn".
