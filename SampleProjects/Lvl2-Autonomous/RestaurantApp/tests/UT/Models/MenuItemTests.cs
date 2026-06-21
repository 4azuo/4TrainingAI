using RestaurantApp.Models;
using Xunit;

namespace RestaurantApp.UnitTests;

public class MenuItemTests
{
    [Fact]
    public void Ctor_Defaults_CategoryAndAvailability()
    {
        var item = new MenuItem(1, "Phở bò", 60_000m);

        Assert.Equal("Khác", item.Category);
        Assert.True(item.IsAvailable);
    }

    [Fact]
    public void Ctor_TrimsName()
    {
        var item = new MenuItem(1, "  Phở bò  ", 60_000m);
        Assert.Equal("Phở bò", item.Name);
    }

    [Fact]
    public void Ctor_NegativePrice_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MenuItem(1, "X", -5m));
    }

    [Fact]
    public void Ctor_BlankName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MenuItem(1, "   ", 1m));
    }

    [Fact]
    public void Update_KeepsId_ChangesRest()
    {
        var item = new MenuItem(7, "Cũ", 10_000m, "A");
        item.Update("Mới", 20_000m, "B", isAvailable: false);

        Assert.Equal(7, item.Id);
        Assert.Equal("Mới", item.Name);
        Assert.Equal(20_000m, item.UnitPrice);
        Assert.Equal("B", item.Category);
        Assert.False(item.IsAvailable);
    }
}
