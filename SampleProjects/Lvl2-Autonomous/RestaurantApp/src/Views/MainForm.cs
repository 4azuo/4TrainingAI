using RestaurantApp.Controllers;
using RestaurantApp.Models;

namespace RestaurantApp.Views;

/// <summary>
/// Màn hình chính. View chỉ làm 2 việc: hiển thị dữ liệu và chuyển thao tác người dùng
/// cho Controller. KHÔNG tính tiền, KHÔNG gọi Repository ở đây.
/// </summary>
public partial class MainForm : Form
{
    private readonly OrderController _controller;

    public MainForm(OrderController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        InitializeComponent();
        LoadMenu();
    }

    private void LoadMenu()
    {
        cboMenu.DisplayMember = nameof(MenuItem.Name);
        cboMenu.ValueMember = nameof(MenuItem.Id);
        cboMenu.DataSource = _controller.GetMenu().ToList();
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        if (cboMenu.SelectedValue is not int menuItemId) return;
        var qty = (int)numQuantity.Value;

        try
        {
            _controller.AddItem(menuItemId, qty);   // ủy quyền cho Controller
            RefreshOrder();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void RefreshOrder()
    {
        lstOrder.Items.Clear();
        foreach (var line in _controller.GetLines())
            lstOrder.Items.Add($"{line.Item.Name} x{line.Quantity} = {line.LineTotal():N0} đ");

        lblTotal.Text = $"Tổng: {_controller.GetTotalText()}";
    }
}
