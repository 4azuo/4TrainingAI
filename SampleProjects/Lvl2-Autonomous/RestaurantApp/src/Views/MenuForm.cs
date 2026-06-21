using System.Globalization;
using RestaurantApp.Controllers;
using RestaurantApp.Models;

namespace RestaurantApp.Views;

/// <summary>
/// Màn hình QUẢN LÝ & TÌM KIẾM món ăn. View chỉ hiển thị và chuyển thao tác cho
/// <see cref="MenuController"/>; không tự validate, không tự lọc dữ liệu.
/// </summary>
public partial class MenuForm : Form
{
    private readonly MenuController _controller;

    public MenuForm(MenuController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        InitializeComponent();
        ReloadGrid(_controller.GetAll());
    }

    private void ReloadGrid(IReadOnlyList<MenuItem> items)
    {
        gridDishes.Rows.Clear();
        foreach (var m in items)
            gridDishes.Rows.Add(m.Id, m.Name, m.Category, m.UnitPrice.ToString("N0", CultureInfo.CurrentCulture), m.IsAvailable);
        lblCount.Text = $"{items.Count} món";
    }

    /// <summary>
    /// Hook cho Integration Test: áp dụng tìm kiếm y như khi người dùng gõ từ khoá và bấm nút.
    /// Cho phép tool IT tái hiện trạng thái UI để chụp evidence.
    /// </summary>
    public void ApplySearch(string keyword, bool onlyAvailable)
    {
        txtSearch.Text = keyword;
        chkOnlyAvailable.Checked = onlyAvailable;
        OnSearchClicked(this, EventArgs.Empty);
    }

    /// <summary>
    /// Hook cho Integration Test: nạp lại lưới theo dữ liệu hiện tại của Controller (sau khi
    /// thêm/sửa/xoá) để tool IT chụp evidence trạng thái "sau".
    /// </summary>
    public void RefreshGrid() => ReloadGrid(_controller.GetAll());

    // ---------- Tìm kiếm ----------
    private void OnSearchClicked(object? sender, EventArgs e)
        => ReloadGrid(_controller.Search(txtSearch.Text, chkOnlyAvailable.Checked));

    private void OnClearSearchClicked(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        chkOnlyAvailable.Checked = false;
        ReloadGrid(_controller.GetAll());
    }

    // ---------- Quản lý ----------
    private void OnAddClicked(object? sender, EventArgs e)
    {
        try
        {
            _controller.AddDish(txtName.Text, ParsePrice(), txtCategory.Text);
            AfterMutation();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void OnUpdateClicked(object? sender, EventArgs e)
    {
        if (SelectedId() is not int id) return;
        try
        {
            _controller.UpdateDish(id, txtName.Text, ParsePrice(), txtCategory.Text, chkAvailable.Checked);
            AfterMutation();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (SelectedId() is not int id) return;
        try
        {
            _controller.DeleteDish(id);
            AfterMutation();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void OnGridSelectionChanged(object? sender, EventArgs e)
    {
        if (gridDishes.CurrentRow is null) return;
        txtName.Text = Cell(1);
        txtCategory.Text = Cell(2);
        txtPrice.Text = Cell(3);
        chkAvailable.Checked = bool.TryParse(Cell(4), out var b) && b;
    }

    // ---------- Helpers ----------
    private void AfterMutation()
    {
        ReloadGrid(_controller.GetAll());
        ClearInputs();
    }

    private void ClearInputs()
    {
        txtName.Clear();
        txtCategory.Clear();
        txtPrice.Clear();
        chkAvailable.Checked = true;
    }

    private decimal ParsePrice()
        => decimal.TryParse(txtPrice.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var p)
            ? p
            : throw new FormatException("Đơn giá phải là số");

    private int? SelectedId()
        => gridDishes.CurrentRow is { } row && int.TryParse(row.Cells[0].Value?.ToString(), out var id)
            ? id
            : null;

    private string Cell(int index) => gridDishes.CurrentRow?.Cells[index].Value?.ToString() ?? string.Empty;

    private static void ShowError(Exception ex)
        => MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
