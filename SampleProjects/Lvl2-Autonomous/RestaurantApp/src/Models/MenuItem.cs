namespace RestaurantApp.Models;

/// <summary>Một món trong thực đơn. Id bất biến; thông tin khác sửa qua <see cref="Update"/>.</summary>
public sealed class MenuItem
{
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = "Khác";
    public decimal UnitPrice { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public MenuItem(int id, string name, decimal unitPrice, string category = "Khác", bool isAvailable = true)
    {
        Id = id;
        Update(name, unitPrice, category, isAvailable);
    }

    /// <summary>Cập nhật thông tin món (validate tại đây). Id không đổi.</summary>
    public void Update(string name, decimal unitPrice, string category, bool isAvailable)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên món không được rỗng", nameof(name));
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Đơn giá không được âm");

        Name = name.Trim();
        UnitPrice = unitPrice;
        Category = string.IsNullOrWhiteSpace(category) ? "Khác" : category.Trim();
        IsAvailable = isAvailable;
    }
}
