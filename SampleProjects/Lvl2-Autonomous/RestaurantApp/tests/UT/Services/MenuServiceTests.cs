using RestaurantApp.Repositories;
using RestaurantApp.Services;
using Xunit;

namespace RestaurantApp.UnitTests;

public class MenuServiceTests
{
    private static MenuService NewService() => new(new InMemoryMenuRepository());

    // ---------- Quản lý (M1–M5) ----------

    [Fact]
    public void AddDish_Valid_IncreasesCountAndAssignsId()
    {
        var svc = NewService();
        int before = svc.GetAll().Count;

        var added = svc.AddDish("Trà đào", 30_000m, "Đồ uống");

        Assert.Equal(before + 1, svc.GetAll().Count);
        Assert.True(added.Id > 0);
        Assert.Contains(svc.GetAll(), m => m.Name == "Trà đào");
    }

    [Fact]
    public void AddDish_DuplicateName_Throws()
    {
        var svc = NewService();
        Assert.Throws<InvalidOperationException>(() => svc.AddDish("Phở bò", 10_000m, "Món chính"));
    }

    [Fact]
    public void AddDish_DuplicateNameDifferentCase_Throws()
    {
        var svc = NewService();
        Assert.Throws<InvalidOperationException>(() => svc.AddDish("phở BÒ", 10_000m, "Món chính"));
    }

    [Fact]
    public void AddDish_BlankName_Throws()
    {
        var svc = NewService();
        Assert.Throws<ArgumentException>(() => svc.AddDish("  ", 10_000m, "Khác"));
    }

    [Fact]
    public void AddDish_NegativePrice_Throws()
    {
        var svc = NewService();
        Assert.Throws<ArgumentOutOfRangeException>(() => svc.AddDish("Nước ép", -1m, "Đồ uống"));
    }

    [Fact]
    public void UpdateDish_Valid_ChangesFields()
    {
        var svc = NewService();
        svc.UpdateDish(1, "Phở đặc biệt", 75_000m, "Món chính", isAvailable: false);

        var item = svc.GetAll().First(m => m.Id == 1);
        Assert.Equal("Phở đặc biệt", item.Name);
        Assert.Equal(75_000m, item.UnitPrice);
        Assert.False(item.IsAvailable);
    }

    [Fact]
    public void UpdateDish_UnknownId_Throws()
    {
        var svc = NewService();
        Assert.Throws<InvalidOperationException>(
            () => svc.UpdateDish(999, "X", 1m, "Khác", true));
    }

    [Fact]
    public void DeleteDish_Valid_RemovesItem()
    {
        var svc = NewService();
        int before = svc.GetAll().Count;

        svc.DeleteDish(1);

        Assert.Equal(before - 1, svc.GetAll().Count);
        Assert.DoesNotContain(svc.GetAll(), m => m.Id == 1);
    }

    // ---------- Tìm kiếm (S1–S5) ----------

    [Fact]
    public void Search_ByName_ReturnsMatches()
    {
        var svc = NewService();
        var result = svc.Search("phở");

        Assert.Single(result);
        Assert.Equal("Phở bò", result[0].Name);
    }

    [Fact]
    public void Search_ByCategory_ReturnsMatches()
    {
        var svc = NewService();
        var result = svc.Search("đồ uống");

        Assert.Single(result);
        Assert.Equal("Cà phê sữa", result[0].Name);
    }

    [Fact]
    public void Search_EmptyKeyword_ReturnsAll()
    {
        var svc = NewService();
        Assert.Equal(svc.GetAll().Count, svc.Search("").Count);
    }

    [Fact]
    public void Search_OnlyAvailable_FiltersOutUnavailable()
    {
        var svc = NewService();
        svc.UpdateDish(1, "Phở bò", 60_000m, "Món chính", isAvailable: false);

        var all = svc.Search("phở", onlyAvailable: false);
        var available = svc.Search("phở", onlyAvailable: true);

        Assert.Single(all);
        Assert.Empty(available);
    }
}
