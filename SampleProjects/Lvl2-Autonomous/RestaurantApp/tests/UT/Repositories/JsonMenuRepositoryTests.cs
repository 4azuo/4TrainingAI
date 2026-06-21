using RestaurantApp.Models;
using RestaurantApp.Repositories;
using Xunit;

namespace RestaurantApp.UnitTests;

/// <summary>Kiểm chứng đọc/ghi JSON: dùng file tạm, dọn dẹp sau mỗi test.</summary>
public class JsonMenuRepositoryTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"menu-{Guid.NewGuid():N}.json");

    public void Dispose()
    {
        if (File.Exists(_path)) File.Delete(_path);
    }

    [Fact]
    public void Ctor_NoFile_CreatesSeedFile()
    {
        var repo = new JsonMenuRepository(_path);

        Assert.True(File.Exists(_path));
        Assert.NotEmpty(repo.GetAll());
    }

    [Fact]
    public void Roundtrip_PreservesAllFields()
    {
        var first = new JsonMenuRepository(_path).GetAll()[0];

        // Nạp lại từ đĩa: constructor-binding phải khớp tên (Id, Name…) không phân biệt hoa thường.
        var reloaded = new JsonMenuRepository(_path).GetAll()[0];

        Assert.Equal(first.Id, reloaded.Id);
        Assert.Equal(first.Name, reloaded.Name);
        Assert.Equal(first.UnitPrice, reloaded.UnitPrice);
        Assert.Equal(first.Category, reloaded.Category);
        Assert.Equal(first.IsAvailable, reloaded.IsAvailable);
    }

    [Fact]
    public void Add_PersistsToDisk()
    {
        var repo = new JsonMenuRepository(_path);
        repo.Add(new MenuItem(repo.NextId(), "Trà đào", 30_000m, "Đồ uống"));

        var reloaded = new JsonMenuRepository(_path);
        Assert.Contains(reloaded.GetAll(), m => m.Name == "Trà đào");
    }

    [Fact]
    public void Remove_PersistsToDisk()
    {
        var repo = new JsonMenuRepository(_path);
        repo.Remove(1);

        var reloaded = new JsonMenuRepository(_path);
        Assert.DoesNotContain(reloaded.GetAll(), m => m.Id == 1);
    }

    [Fact]
    public void NextId_ContinuesFromMaxId()
    {
        var repo = new JsonMenuRepository(_path);   // seed Id 1..4
        Assert.Equal(5, repo.NextId());
    }
}
