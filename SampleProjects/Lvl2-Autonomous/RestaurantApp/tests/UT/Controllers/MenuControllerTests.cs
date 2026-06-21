using RestaurantApp.Controllers;
using RestaurantApp.Repositories;
using RestaurantApp.Services;
using Xunit;

namespace RestaurantApp.UnitTests;

public class MenuControllerTests
{
    private static MenuController NewController() =>
        new(new MenuService(new InMemoryMenuRepository()));

    [Fact]
    public void AddDish_DelegatesToService()
    {
        var c = NewController();
        int before = c.GetAll().Count;

        c.AddDish("Sinh tố bơ", 35_000m, "Đồ uống");

        Assert.Equal(before + 1, c.GetAll().Count);
    }

    [Fact]
    public void AddDish_Duplicate_Throws()
    {
        var c = NewController();
        Assert.Throws<InvalidOperationException>(() => c.AddDish("Cơm tấm", 1m, "Món chính"));
    }

    [Fact]
    public void Search_ReturnsFiltered()
    {
        var c = NewController();
        var result = c.Search("cơm");

        Assert.Single(result);
        Assert.Equal("Cơm tấm", result[0].Name);
    }

    [Fact]
    public void DeleteDish_RemovesItem()
    {
        var c = NewController();
        int before = c.GetAll().Count;

        c.DeleteDish(2);

        Assert.Equal(before - 1, c.GetAll().Count);
    }
}
