using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using RestaurantApp.Controllers;
using RestaurantApp.Models;
using RestaurantApp.Repositories;
using RestaurantApp.Services;
using RestaurantApp.Views;

namespace RestaurantApp.ItHarness;

/// <summary>
/// Trình chạy Integration Test: với mỗi testcase, dựng MenuForm thật (Repository → Service →
/// Controller → View), CHỤP <b>before.png</b> (trạng thái trước), thực hiện luồng nghiệp vụ,
/// rồi CHỤP <b>after.png</b> (trạng thái sau) cùng <c>output.log</c> — evidence trước/sau để
/// CHỨNG MINH thay đổi.
///
/// Cách dùng: RestaurantApp.ItHarness &lt;TC-id|ALL&gt; &lt;thư-mục-evidence&gt;
/// Exit code 0 = mọi testcase đã chạy đều PASS; 1 = có FAIL hoặc lỗi.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = new CultureInfo("vi-VN");
        ApplicationConfiguration.Initialize();

        var which = args.Length > 0 ? args[0] : "ALL";
        var evidenceRoot = args.Length > 1
            ? Path.GetFullPath(args[1])
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "evidence"));

        var cases = new Dictionary<string, Func<Scenario>>(StringComparer.OrdinalIgnoreCase)
        {
            ["TC-001"] = Tc001_AddDish,
            ["TC-002"] = Tc002_RejectDuplicate,
            ["TC-003"] = Tc003_Search,
            ["TC-004"] = Tc004_Delete,
        };

        var toRun = which.Equals("ALL", StringComparison.OrdinalIgnoreCase)
            ? cases.Keys.ToList()
            : new List<string> { which };

        int failed = 0;
        foreach (var id in toRun)
        {
            if (!cases.TryGetValue(id, out var build))
            {
                Console.Error.WriteLine($"Unknown testcase: {id}");
                failed++;
                continue;
            }

            var sc = build();
            var pass = RunScenario(evidenceRoot, id, sc);
            Console.WriteLine($"{id}: {(pass ? "PASS" : "FAIL")} — {sc.Title}");
            if (!pass) failed++;
        }

        return failed == 0 ? 0 : 1;
    }

    // ---------------- Scenarios ----------------
    // Mỗi scenario chỉ mô tả THAO TÁC trên Controller + (tuỳ chọn) từ khoá lọc cho ảnh "sau".
    // Việc dựng form, chụp before/after và ghi log do RunScenario lo — tránh lặp code.

    private static Scenario Tc001_AddDish() => new()
    {
        Title = "TC-001 Thêm món mới hợp lệ",
        Act = c =>
        {
            var log = new List<string>();
            int before = c.GetAll().Count;
            var added = c.AddDish("Trà đào", 30_000m, "Đồ uống");
            int after = c.GetAll().Count;

            bool pass = after == before + 1 && c.GetAll().Any(m => m.Name == "Trà đào");
            log.Add("Bước: thêm 'Trà đào' (30.000, Đồ uống).");
            log.Add($"Kỳ vọng: số món tăng 1 và có 'Trà đào'. Thực tế: {before} → {after}, id mới = {added.Id}.");
            return (pass, log);
        },
    };

    private static Scenario Tc002_RejectDuplicate() => new()
    {
        Title = "TC-002 Chặn thêm món trùng tên",
        Act = c =>
        {
            var log = new List<string>();
            int before = c.GetAll().Count;
            bool rejected = false;
            try
            {
                c.AddDish("Phở bò", 10_000m, "Món chính");
            }
            catch (InvalidOperationException ex)
            {
                rejected = true;
                log.Add($"Bị từ chối đúng kỳ vọng: {ex.Message}");
            }
            int after = c.GetAll().Count;

            bool pass = rejected && after == before;
            log.Add("Bước: thêm món trùng tên 'Phở bò'.");
            log.Add($"Kỳ vọng: bị chặn, danh sách không đổi. Thực tế: rejected={rejected}, {before} → {after}.");
            return (pass, log);
        },
    };

    private static Scenario Tc003_Search() => new()
    {
        Title = "TC-003 Tìm kiếm món theo từ khoá",
        // Search != null → ảnh "after" thể hiện lưới đã lọc theo từ khoá.
        Search = ("phở", false),
        Act = c =>
        {
            var log = new List<string>();
            var result = c.Search("phở");
            bool pass = result.Count == 1 &&
                        result.All(m => m.Name.Contains("Phở", StringComparison.OrdinalIgnoreCase));

            log.Add("Bước: tìm với từ khoá 'phở'.");
            log.Add($"Kỳ vọng: 1 kết quả khớp tên. Thực tế: {result.Count} kết quả: " +
                    $"{string.Join(", ", result.Select(m => m.Name))}.");
            return (pass, log);
        },
    };

    private static Scenario Tc004_Delete() => new()
    {
        Title = "TC-004 Xoá món",
        Act = c =>
        {
            var log = new List<string>();
            int before = c.GetAll().Count;
            c.DeleteDish(1);
            int after = c.GetAll().Count;

            bool pass = after == before - 1 && c.GetAll().All(m => m.Id != 1);
            log.Add("Bước: xoá món id=1 (Phở bò).");
            log.Add($"Kỳ vọng: số món giảm 1, không còn id=1. Thực tế: {before} → {after}.");
            return (pass, log);
        },
    };

    // ---------------- Driver ----------------

    private static MenuController NewController()
    {
        IMenuRepository repo = new InMemoryMenuRepository();
        return new MenuController(new MenuService(repo));
    }

    /// <summary>
    /// Dựng form → chụp "before" → chạy thao tác → phản ánh lên View (lọc hoặc reload) →
    /// chụp "after" → ghi output.log. Trả về kết quả PASS/FAIL của thao tác.
    /// </summary>
    private static bool RunScenario(string root, string id, Scenario sc)
    {
        var dir = Path.Combine(root, id);
        Directory.CreateDirectory(dir);

        var controller = NewController();
        bool pass = false;
        var steps = new List<string>();
        var notes = new List<string>();

        try
        {
            using var form = new MenuForm(controller);
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-3000, -3000); // dựng ngoài màn hình, không làm phiền
            form.Show();
            Application.DoEvents();

            // (1) Trạng thái TRƯỚC thao tác.
            Shot(form, Path.Combine(dir, "before.png"));

            // (2) Luồng nghiệp vụ thật qua đủ các tầng.
            (pass, steps) = sc.Act(controller);

            // (3) Phản ánh thay đổi lên View rồi chụp trạng thái SAU.
            if (sc.Search is { } s) form.ApplySearch(s.Keyword, s.OnlyAvailable);
            else form.RefreshGrid();
            Application.DoEvents();
            Shot(form, Path.Combine(dir, "after.png"));

            notes.Add("Evidence: before.png (trước), after.png (sau), output.log (file này).");
        }
        catch (Exception ex)
        {
            notes.Add($"[screenshot error] {ex.Message}");
        }

        WriteLog(dir, id, sc.Title, pass, steps, notes);
        return pass;
    }

    private static void Shot(Form form, string path)
    {
        using var bmp = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
        bmp.Save(path, ImageFormat.Png);
    }

    private static void WriteLog(
        string dir, string id, string title, bool pass, List<string> steps, List<string> notes)
    {
        var lines = new List<string>
        {
            $"# {id} — {title}",
            $"Thời điểm: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            $"Kết quả: {(pass ? "PASS" : "FAIL")}",
            "",
            "## Các bước & kiểm chứng",
        };
        lines.AddRange(steps);
        lines.Add("");
        lines.AddRange(notes);
        File.WriteAllLines(Path.Combine(dir, "output.log"), lines);
    }

    /// <summary>Mô tả một testcase: thao tác nghiệp vụ + (tuỳ chọn) từ khoá lọc cho ảnh "sau".</summary>
    private sealed class Scenario
    {
        public string Title = string.Empty;
        public Func<MenuController, (bool Pass, List<string> Log)> Act = _ => (false, new List<string>());
        public (string Keyword, bool OnlyAvailable)? Search;
    }
}
