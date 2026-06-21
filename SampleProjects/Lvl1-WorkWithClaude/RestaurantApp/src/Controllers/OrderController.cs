using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

/// <summary>
/// Điều phối giữa View và <see cref="OrderService"/>. KHÔNG chứa logic UI, KHÔNG chứa logic
/// tính tiền (việc đó thuộc Order/OrderService). Chỉ nhận lệnh từ View, gọi Service, và
/// định dạng dữ liệu sẵn sàng hiển thị.
/// </summary>
public sealed class OrderController
{
    private readonly OrderService _service;

    public OrderController(OrderService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public IReadOnlyList<MenuItem> GetMenu() => _service.GetMenu();

    public void AddItem(int menuItemId, int quantity) => _service.AddItem(menuItemId, quantity);

    public IReadOnlyList<OrderLine> GetLines() => _service.GetLines();

    public decimal GetSubtotal() => _service.GetSubtotal();
    public decimal GetDiscount() => _service.GetDiscount();
    public decimal GetVat() => _service.GetVat();
    public decimal GetTotal() => _service.GetTotal();

    /// <summary>Chuỗi tổng tiền sẵn sàng cho View hiển thị (định dạng — không phải nghiệp vụ).</summary>
    public string GetTotalText() => $"{_service.GetTotal():N0} đ";
}
