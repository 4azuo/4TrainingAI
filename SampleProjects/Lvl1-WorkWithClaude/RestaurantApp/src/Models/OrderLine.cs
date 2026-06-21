namespace RestaurantApp.Models;

/// <summary>Một dòng trong đơn: một món kèm số lượng.</summary>
public sealed class OrderLine
{
    public MenuItem Item { get; }
    public int Quantity { get; private set; }

    public OrderLine(MenuItem item, int quantity)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity = quantity;
    }

    public void AddQuantity(int more)
    {
        if (more <= 0) throw new ArgumentOutOfRangeException(nameof(more));
        Quantity += more;
    }

    /// <summary>Thành tiền của dòng = đơn giá × số lượng.</summary>
    public decimal LineTotal() => Item.UnitPrice * Quantity;
}
