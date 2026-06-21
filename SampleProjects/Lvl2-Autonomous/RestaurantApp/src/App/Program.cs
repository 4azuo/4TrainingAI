using System.Globalization;
using RestaurantApp.Controllers;
using RestaurantApp.Repositories;
using RestaurantApp.Services;
using RestaurantApp.Views;

namespace RestaurantApp.App;

internal static class Program
{
    /// <summary>Điểm vào ứng dụng — nơi duy nhất "ráp" các tầng lại với nhau.</summary>
    [STAThread]
    private static void Main()
    {
        // Cố định culture để hiển thị/nhập tiền tệ nhất quán (vi-VN dùng '.' phân nhóm).
        CultureInfo.CurrentCulture = new CultureInfo("vi-VN");
        ApplicationConfiguration.Initialize();

        // Composition root: dựng dependency từ dưới lên.
        // Repository JSON đọc/ghi data/menu.json cạnh file thực thi (seed được copy khi build).
        var dataPath = Path.Combine(AppContext.BaseDirectory, "data", "menu.json");
        IMenuRepository repo = new JsonMenuRepository(dataPath);

        // Chức năng cốt lõi: quản lý & tìm kiếm món (SCR-01).
        var menuService = new MenuService(repo);
        var menuController = new MenuController(menuService);

        Application.Run(new MenuForm(menuController));
    }
}
