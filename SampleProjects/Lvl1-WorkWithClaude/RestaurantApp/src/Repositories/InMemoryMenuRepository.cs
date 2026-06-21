using RestaurantApp.Models;

namespace RestaurantApp.Repositories;

/// <summary>
/// Cài đặt in-memory cho <b>test nhanh</b> và demo (không chạm đĩa). Production dùng
/// <see cref="JsonMenuRepository"/>.
/// </summary>
public sealed class InMemoryMenuRepository : IMenuRepository
{
    private readonly List<MenuItem> _items;
    private int _seq;

    public InMemoryMenuRepository()
    {
        _items = new()
        {
            new MenuItem(1, "Phở bò", 60_000m, "Món chính"),
            new MenuItem(2, "Cơm tấm", 55_000m, "Món chính"),
            new MenuItem(3, "Cà phê sữa", 25_000m, "Đồ uống"),
            new MenuItem(4, "Lẩu thái", 250_000m, "Món chính"),
        };
        _seq = _items.Count;
    }

    public IReadOnlyList<MenuItem> GetAll() => _items;

    public MenuItem? FindById(int id) => _items.FirstOrDefault(m => m.Id == id);

    public void Add(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public void Update(MenuItem item)
    {
        // Mutate tại chỗ trên cùng tham chiếu — không cần thao tác lưu trữ.
        ArgumentNullException.ThrowIfNull(item);
    }

    public void Remove(int id)
    {
        var item = FindById(id);
        if (item is not null) _items.Remove(item);
    }

    public int NextId() => ++_seq;
}
