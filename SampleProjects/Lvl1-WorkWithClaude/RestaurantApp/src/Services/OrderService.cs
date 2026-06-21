using RestaurantApp.Models;
using RestaurantApp.Repositories;

namespace RestaurantApp.Services;

/// <summary>
/// Nghiệp vụ LẬP ĐƠN gọi món: tra món từ Repository, thêm vào <see cref="Order"/> (gộp nếu
/// trùng) và cung cấp các số liệu tính tiền (R1–R5). Giữ một đơn đang mở trong phiên.
/// </summary>
public sealed class OrderService
{
    private readonly IMenuRepository _menu;
    private readonly Order _order = new();

    public OrderService(IMenuRepository menu)
    {
        _menu = menu ?? throw new ArgumentNullException(nameof(menu));
    }

    /// <summary>Thực đơn để hiển thị chọn món.</summary>
    public IReadOnlyList<MenuItem> GetMenu() => _menu.GetAll();

    /// <summary>Thêm món vào đơn theo Id. Ném lỗi nếu món không tồn tại.</summary>
    public void AddItem(int menuItemId, int quantity)
    {
        var item = _menu.FindById(menuItemId)
                   ?? throw new InvalidOperationException($"Không tìm thấy món #{menuItemId}");
        _order.AddLine(item, quantity);
    }

    public IReadOnlyList<OrderLine> GetLines() => _order.Lines;

    public decimal GetSubtotal() => _order.Subtotal();
    public decimal GetDiscount() => _order.Discount();
    public decimal GetVat() => _order.Vat();
    public decimal GetTotal() => _order.CalculateTotal();
}
