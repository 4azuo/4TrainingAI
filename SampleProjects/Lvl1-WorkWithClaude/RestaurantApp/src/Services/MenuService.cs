using RestaurantApp.Models;
using RestaurantApp.Repositories;

namespace RestaurantApp.Services;

/// <summary>
/// Nghiệp vụ QUẢN LÝ và TÌM KIẾM món ăn. Toàn bộ validate (tên không rỗng/không trùng,
/// giá ≥ 0) và logic lọc tìm kiếm tập trung tại đây — View/Controller không tự xử lý.
/// </summary>
public sealed class MenuService
{
    private readonly IMenuRepository _repo;

    public MenuService(IMenuRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public IReadOnlyList<MenuItem> GetAll() => _repo.GetAll();

    // ---------- Quản lý món (CRUD) ----------

    /// <summary>Thêm món mới. Ném lỗi nếu tên trùng (không phân biệt hoa thường).</summary>
    public MenuItem AddDish(string name, decimal unitPrice, string category)
    {
        EnsureNameUnique(name, excludingId: null);
        var item = new MenuItem(_repo.NextId(), name, unitPrice, category);
        _repo.Add(item);
        return item;
    }

    /// <summary>Sửa thông tin món theo Id.</summary>
    public void UpdateDish(int id, string name, decimal unitPrice, string category, bool isAvailable)
    {
        var item = _repo.FindById(id)
                   ?? throw new InvalidOperationException($"Không tìm thấy món #{id}");
        EnsureNameUnique(name, excludingId: id);
        item.Update(name, unitPrice, category, isAvailable);
        _repo.Update(item);   // lưu lại thay đổi (JSON repo ghi đĩa)
    }

    /// <summary>Xoá món theo Id.</summary>
    public void DeleteDish(int id)
    {
        if (_repo.FindById(id) is null)
            throw new InvalidOperationException($"Không tìm thấy món #{id}");
        _repo.Remove(id);
    }

    // ---------- Tìm kiếm món ----------

    /// <summary>
    /// Tìm món theo từ khoá: khớp tên HOẶC danh mục, không phân biệt hoa thường.
    /// keyword rỗng → trả toàn bộ. onlyAvailable = true → chỉ món đang bán.
    /// </summary>
    public IReadOnlyList<MenuItem> Search(string? keyword, bool onlyAvailable = false)
    {
        IEnumerable<MenuItem> query = _repo.GetAll();

        if (onlyAvailable)
            query = query.Where(m => m.IsAvailable);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim().ToLowerInvariant();
            query = query.Where(m =>
                m.Name.ToLowerInvariant().Contains(k) ||
                m.Category.ToLowerInvariant().Contains(k));
        }

        return query.ToList();
    }

    // ---------- Helpers ----------

    private void EnsureNameUnique(string name, int? excludingId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên món không được rỗng", nameof(name));

        var trimmed = name.Trim();
        bool duplicated = _repo.GetAll().Any(m =>
            (excludingId is null || m.Id != excludingId.Value) &&
            string.Equals(m.Name, trimmed, StringComparison.OrdinalIgnoreCase));

        if (duplicated)
            throw new InvalidOperationException($"Món '{trimmed}' đã tồn tại");
    }
}
