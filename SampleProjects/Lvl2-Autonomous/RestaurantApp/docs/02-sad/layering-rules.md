# SAD — Quy tắc phân tầng & ranh giới

> Quy tắc chung bắt buộc cho mọi tầng. Vi phạm sẽ **fail build** vì mỗi tầng là 1 project
> chỉ tham chiếu xuống dưới.

## Bảng trách nhiệm
| Tầng | Project | ĐƯỢC làm | KHÔNG được làm |
|------|---------|----------|----------------|
| **Model** | `RestaurantApp.Models` | Entity, bất biến nội tại (validate field) | Truy xuất dữ liệu; biết Repository/Service/View |
| **Repository** | `RestaurantApp.Repositories` | Đọc/ghi JSON, cấp Id, CRUD thô | Logic nghiệp vụ (validate trùng tên, tính tiền) |
| **Service** | `RestaurantApp.Services` | Validate, tìm kiếm, tính tiền, điều phối Repository | Tham chiếu View/Controller; biết WinForms |
| **Controller** | `RestaurantApp.Controllers` | Nhận lệnh View → gọi Service → trả dữ liệu | Logic nghiệp vụ; logic UI |
| **View** | `RestaurantApp.Views` | Hiển thị, bắt sự kiện, định dạng hiển thị | Gọi thẳng Repository/Service; tính tiền |
| **App** | `RestaurantApp.App` | Ráp các tầng (DI thủ công) | Chứa logic nghiệp vụ |

## Đồ thị tham chiếu project (ProjectReference)
```
App        → Views, Controllers, Services, Repositories, Models
Views      → Controllers, Models
Controllers→ Services, Models
Services   → Repositories, Models
Repositories → Models
Models     → (không tham chiếu gì)
```

## Ví dụ đúng/sai

❌ **Sai** — View tự gọi Repository & tự validate:
```csharp
// trong MenuForm.cs
var items = new JsonMenuRepository("data/menu.json").GetAll();   // View gọi thẳng repo
bool exists = items.Any(m => m.Name == name);                     // validate nằm trong Form
```

✅ **Đúng** — View → Controller → Service → Repository:
```csharp
// MenuForm.cs (View)
_controller.AddDish(txtName.Text, ParsePrice(), txtCategory.Text);  // chỉ uỷ quyền
```
```csharp
// MenuService.cs (Service) — logic ở đây
EnsureNameUnique(name, excludingId: null);
var item = new MenuItem(_repo.NextId(), name, unitPrice, category);
_repo.Add(item);                                                   // repo chỉ lưu
```

## Vì sao ranh giới do compiler ép buộc
Nếu ai đó viết `using RestaurantApp.Views;` trong `Services`, build **fail ngay** — vì
`RestaurantApp.Services.csproj` không tham chiếu `Views`. Sub-agent `verifier` dựa vào đây
để kiểm chứng ranh giới một cách máy móc, đáng tin.
