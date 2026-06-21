using RestaurantApp.Models;

namespace RestaurantApp.Repositories;

/// <summary>
/// Nguồn dữ liệu thực đơn (CRUD). Trừu tượng hoá để dễ test &amp; thay nguồn lưu trữ
/// (JSON ⇄ DB) mà không ảnh hưởng tầng Service. KHÔNG chứa logic nghiệp vụ.
/// </summary>
public interface IMenuRepository
{
    IReadOnlyList<MenuItem> GetAll();
    MenuItem? FindById(int id);
    void Add(MenuItem item);
    /// <summary>Lưu lại thay đổi của một món đã sửa (entity được mutate tại chỗ).</summary>
    void Update(MenuItem item);
    void Remove(int id);
    /// <summary>Cấp Id mới chưa trùng cho món sắp thêm.</summary>
    int NextId();
}
