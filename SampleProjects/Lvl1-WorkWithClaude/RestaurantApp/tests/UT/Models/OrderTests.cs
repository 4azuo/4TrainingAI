using RestaurantApp.Models;
using Xunit;

namespace RestaurantApp.UnitTests;

public class OrderTests
{
    private static MenuItem Pho => new(1, "Phở bò", 60_000m);
    private static MenuItem Lau => new(4, "Lẩu thái", 250_000m);

    [Fact]
    public void Subtotal_TwoLines_SumsLineTotals()
    {
        var order = new Order();
        order.AddLine(Pho, 2);   // 120.000
        order.AddLine(Lau, 1);   // 250.000

        Assert.Equal(370_000m, order.Subtotal());
    }

    [Fact]
    public void AddLine_SameItemTwice_MergesQuantity()
    {
        var order = new Order();
        order.AddLine(Pho, 1);
        order.AddLine(Pho, 2);

        Assert.Single(order.Lines);
        Assert.Equal(3, order.Lines[0].Quantity);
    }

    [Fact]
    public void Discount_BelowThreshold_IsZero()
    {
        var order = new Order();
        order.AddLine(Pho, 2);   // 120.000 < 500.000

        Assert.Equal(0m, order.Discount());
    }

    [Fact]
    public void CalculateTotal_AboveThreshold_AppliesDiscountThenVat()
    {
        var order = new Order();
        order.AddLine(Lau, 3);   // Subtotal 750.000 >= 500.000

        // Discount 10% = 75.000 -> 675.000 ; VAT 10% = 67.500 -> Total 742.500
        Assert.Equal(75_000m, order.Discount());
        Assert.Equal(742_500m, order.CalculateTotal());
    }

    [Fact]
    public void AddLine_ZeroQuantity_Throws()
    {
        var order = new Order();
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddLine(Pho, 0));
    }
}
