# Data — Lưu trữ JSON

> `JsonMenuRepository` (tầng Repository) đọc/ghi dữ liệu món ở định dạng JSON.

## Vị trí file
| File | Vai trò |
|------|---------|
| `data/menu.json` | **Seed mẫu** (commit kèm) + dữ liệu món. Đường dẫn truyền từ `App`. |

> App tính đường dẫn `data/menu.json` cạnh file thực thi (output) và copy seed khi build.
> Ghi đè khi thêm/sửa/xoá; seed gốc trong source không bị thay đổi.

## Lược đồ (schema) `menu.json`
Mảng các `MenuItem`:
```json
[
  { "Id": 1, "Name": "Phở bò",     "Category": "Món chính", "UnitPrice": 60000, "IsAvailable": true },
  { "Id": 2, "Name": "Cơm tấm",    "Category": "Món chính", "UnitPrice": 55000, "IsAvailable": true },
  { "Id": 3, "Name": "Cà phê sữa", "Category": "Đồ uống",   "UnitPrice": 25000, "IsAvailable": true },
  { "Id": 4, "Name": "Lẩu thái",   "Category": "Món chính", "UnitPrice": 250000, "IsAvailable": true }
]
```

| Trường | Kiểu JSON | Bắt buộc | Ghi chú |
|--------|-----------|----------|---------|
| `Id` | number | ✅ | > 0, duy nhất |
| `Name` | string | ✅ | không rỗng |
| `Category` | string | ✅ | rỗng → "Khác" khi nạp |
| `UnitPrice` | number | ✅ | ≥ 0 (VND) |
| `IsAvailable` | bool | ✅ | còn bán |

## Hành vi đọc/ghi
- **Load**: nếu file tồn tại → deserialize; nếu thiếu/hỏng → khởi tạo seed mặc định rồi ghi ra.
- **Save**: serialize toàn bộ danh sách (ghi đè) sau mỗi `Add` / `Update` / `Remove`.
- `NextId()`: cấp Id = max(Id hiện có) + 1.
- Tuần tự hoá bằng `System.Text.Json`, `WriteIndented = true` (dễ đọc tay).

## Vì sao JSON (thay in-memory cũ)
- Lưu **bền vững** giữa các lần chạy, vẫn đủ đơn giản để xem/sửa bằng tay.
- Trừu tượng qua `IMenuRepository` → sau này đổi sang DB chỉ cần thêm 1 implementation,
  Service/Controller/View không đổi. `InMemoryMenuRepository` giữ lại cho **test nhanh**.
