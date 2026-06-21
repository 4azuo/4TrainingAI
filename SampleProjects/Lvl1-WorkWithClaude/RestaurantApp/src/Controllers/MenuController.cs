using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

/// <summary>
/// Điều phối giữa View quản lý/tìm kiếm món và <see cref="MenuService"/>.
/// KHÔNG chứa logic nghiệp vụ (validate/lọc nằm ở MenuService) lẫn logic UI.
/// </summary>
public sealed class MenuController
{
    private readonly MenuService _service;

    public MenuController(MenuService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public IReadOnlyList<MenuItem> GetAll() => _service.GetAll();

    // Quản lý món
    public MenuItem AddDish(string name, decimal unitPrice, string category)
        => _service.AddDish(name, unitPrice, category);

    public void UpdateDish(int id, string name, decimal unitPrice, string category, bool isAvailable)
        => _service.UpdateDish(id, name, unitPrice, category, isAvailable);

    public void DeleteDish(int id) => _service.DeleteDish(id);

    // Tìm kiếm món
    public IReadOnlyList<MenuItem> Search(string? keyword, bool onlyAvailable = false)
        => _service.Search(keyword, onlyAvailable);
}
