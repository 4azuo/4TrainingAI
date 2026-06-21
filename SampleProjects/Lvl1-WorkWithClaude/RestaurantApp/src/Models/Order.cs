namespace RestaurantApp.Models;

/// <summary>
/// Đơn gọi món. Chứa TOÀN BỘ logic tính tiền (R1–R5 trong rag/knowledge/business-rules.md).
/// </summary>
public sealed class Order
{
    /// <summary>Tỷ lệ VAT cố định 10% (R2).</summary>
    public const decimal VatRate = 0.10m;

    /// <summary>Ngưỡng và mức giảm giá (R3).</summary>
    public const decimal DiscountThreshold = 500_000m;
    public const decimal DiscountRate = 0.10m;

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    /// <summary>Thêm một món vào đơn (gộp nếu món đã có).</summary>
    public void AddLine(MenuItem item, int quantity)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var existing = _lines.FirstOrDefault(l => l.Item.Id == item.Id);
        if (existing is null)
            _lines.Add(new OrderLine(item, quantity));
        else
            existing.AddQuantity(quantity);
    }

    /// <summary>R1 — tổng tiền trước thuế & giảm giá.</summary>
    public decimal Subtotal() => _lines.Sum(l => l.LineTotal());

    /// <summary>R3 — giảm giá theo ngưỡng đơn.</summary>
    public decimal Discount()
    {
        var subtotal = Subtotal();
        return subtotal >= DiscountThreshold ? subtotal * DiscountRate : 0m;
    }

    /// <summary>R2 — VAT tính trên số tiền SAU giảm giá.</summary>
    public decimal Vat() => (Subtotal() - Discount()) * VatRate;

    /// <summary>R4 — thành tiền cuối cùng khách phải trả.</summary>
    public decimal CalculateTotal()
    {
        var total = (Subtotal() - Discount()) + Vat();
        return total < 0 ? 0m : total;
    }
}
