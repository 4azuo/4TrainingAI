using System.Text.Json;
using RestaurantApp.Models;

namespace RestaurantApp.Repositories;

/// <summary>
/// Lưu trữ thực đơn bằng <b>JSON file</b> (System.Text.Json). Đọc khi khởi tạo, ghi đè
/// toàn bộ danh sách sau mỗi thao tác thay đổi. Thiếu/hỏng file → khôi phục seed mặc định.
/// </summary>
public sealed class JsonMenuRepository : IMenuRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _path;
    private readonly List<MenuItem> _items;
    private int _seq;

    /// <param name="path">Đường dẫn file JSON, ví dụ <c>data/menu.json</c>.</param>
    public JsonMenuRepository(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _items = Load();
        _seq = _items.Count == 0 ? 0 : _items.Max(m => m.Id);
    }

    public IReadOnlyList<MenuItem> GetAll() => _items;

    public MenuItem? FindById(int id) => _items.FirstOrDefault(m => m.Id == id);

    public void Add(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        Save();
    }

    public void Update(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        // Entity được sửa tại chỗ (cùng tham chiếu đang nằm trong _items) → chỉ cần ghi lại.
        Save();
    }

    public void Remove(int id)
    {
        var item = FindById(id);
        if (item is null) return;
        _items.Remove(item);
        Save();
    }

    public int NextId() => ++_seq;

    // ---------- JSON I/O ----------

    private List<MenuItem> Load()
    {
        if (!File.Exists(_path))
        {
            var seed = DefaultSeed();
            WriteToDisk(seed);
            return seed;
        }

        try
        {
            var json = File.ReadAllText(_path);
            var items = JsonSerializer.Deserialize<List<MenuItem>>(json, Options);
            return items is { Count: > 0 } ? items : DefaultSeed();
        }
        catch (JsonException)
        {
            var seed = DefaultSeed();
            WriteToDisk(seed);
            return seed;
        }
    }

    private void Save() => WriteToDisk(_items);

    private void WriteToDisk(List<MenuItem> items)
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_path, JsonSerializer.Serialize(items, Options));
    }

    /// <summary>Seed mẫu khi chưa có file (khớp với <c>data/menu.json</c> commit kèm).</summary>
    private static List<MenuItem> DefaultSeed() => new()
    {
        new MenuItem(1, "Phở bò", 60_000m, "Món chính"),
        new MenuItem(2, "Cơm tấm", 55_000m, "Món chính"),
        new MenuItem(3, "Cà phê sữa", 25_000m, "Đồ uống"),
        new MenuItem(4, "Lẩu thái", 250_000m, "Món chính"),
    };
}
