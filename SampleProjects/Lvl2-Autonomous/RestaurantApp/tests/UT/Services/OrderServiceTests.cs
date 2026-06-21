using RestaurantApp.Repositories;
using RestaurantApp.Services;
using Xunit;

namespace RestaurantApp.UnitTests;

public class OrderServiceTests
{
    private static OrderService NewService() => new(new InMemoryMenuRepository());

    [Fact]
    public void AddItem_ValidId_AppearsInOrder()
    {
        var s = NewService();
        s.AddItem(menuItemId: 1, quantity: 2);

        Assert.Single(s.GetLines());
        Assert.Equal(2, s.GetLines()[0].Quantity);
    }

    [Fact]
    public void AddItem_UnknownId_Throws()
    {
        var s = NewService();
        Assert.Throws<InvalidOperationException>(() => s.AddItem(menuItemId: 999, quantity: 1));
    }

    [Fact]
    public void AddItem_SameItemTwice_MergesQuantity()   // O1
    {
        var s = NewService();
        s.AddItem(1, 1);
        s.AddItem(1, 2);

        Assert.Single(s.GetLines());
        Assert.Equal(3, s.GetLines()[0].Quantity);
    }

    [Fact]
    public void AddItem_ZeroQuantity_Throws()   // O2
    {
        var s = NewService();
        Assert.Throws<ArgumentOutOfRangeException>(() => s.AddItem(1, 0));
    }

    [Fact]
    public void Totals_AboveThreshold_ApplyDiscountThenVat()   // R1–R4
    {
        var s = NewService();
        s.AddItem(4, 3);   // Lẩu thái 250.000 × 3 = Subtotal 750.000

        Assert.Equal(750_000m, s.GetSubtotal());
        Assert.Equal(75_000m, s.GetDiscount());   // giảm 10%
        Assert.Equal(742_500m, s.GetTotal());     // (750k-75k) + VAT 10%
    }

    [Fact]
    public void Totals_BelowThreshold_NoDiscount()   // R3
    {
        var s = NewService();
        s.AddItem(1, 2);   // Phở bò 60.000 × 2 = 120.000 < 500.000

        Assert.Equal(0m, s.GetDiscount());
        Assert.Equal(132_000m, s.GetTotal());   // 120k + VAT 10%
    }
}
