using System.Globalization;
using RestaurantApp.Controllers;
using RestaurantApp.Repositories;
using RestaurantApp.Services;
using Xunit;

namespace RestaurantApp.UnitTests;

public class OrderControllerTests
{
    public OrderControllerTests()
    {
        // Cố định culture để assert chuỗi tiền không phụ thuộc máy chạy (vi-VN dùng '.').
        CultureInfo.CurrentCulture = new CultureInfo("vi-VN");
    }

    private static OrderController NewController() =>
        new(new OrderService(new InMemoryMenuRepository()));

    [Fact]
    public void AddItem_ValidId_AppearsInOrder()
    {
        var c = NewController();
        c.AddItem(menuItemId: 1, quantity: 2);

        Assert.Single(c.GetLines());
        Assert.Equal(2, c.GetLines()[0].Quantity);
    }

    [Fact]
    public void AddItem_UnknownId_Throws()
    {
        var c = NewController();
        Assert.Throws<InvalidOperationException>(() => c.AddItem(menuItemId: 999, quantity: 1));
    }

    [Fact]
    public void GetTotalText_FormatsWithCurrency()
    {
        var c = NewController();
        c.AddItem(1, 1);   // Phở bò 60.000

        // 60.000 + VAT 10% = 66.000
        Assert.Equal("66.000 đ", c.GetTotalText());
    }
}
