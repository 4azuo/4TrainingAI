# RestaurantApp 🍽️ — Ví dụ Lvl1: Làm việc cùng Claude

Project mẫu cho **Lvl1 — Work With Claude**: minh hoạ cách tổ chức một dự án thật để
Claude Code phát huy tối đa, sử dụng đầy đủ các thành phần:

| Thành phần | Vị trí | Vai trò |
|-----------|--------|---------|
| **Sub-agents** | `.claude/agents/` | dev, test, docs, verifier, reporter |
| **Skills** | `.claude/skills/` | `test-report` — sinh báo cáo Test (UT coverage + IT evidence) |
| **Commands** | `.claude/commands/` | `/feature`, `/review`, `/report` |
| **Hooks** | `.claude/hooks/` | format-on-save, protect-paths |
| **Memory** | `CLAUDE.md` + `.claude/agents/<tên>/` | bộ nhớ dự án + bộ nhớ riêng từng sub-agent |
| **RAG** | `rag/` | kho tri thức nghiệp vụ để truy hồi |
| **Scripts** | `scripts/` | build, test, generate-report |
| **Docs** | `docs/` | SAD, basic/detail design theo màn hình, dữ liệu |
| **Data** | `data/` | lưu trữ JSON (seed mẫu) |
| **Src** | `src/` | mã nguồn monorepo (.NET WinForms, phân tầng) |
| **Settings** | `.claude/settings.json`, `settings.local.json`, `.vscode/settings.json` | cấu hình team & cá nhân |

## Ứng dụng demo
RestaurantApp — quản lý gọi món: quản lý & tìm kiếm món, lập đơn, tính tổng tiền (gồm thuế
VAT 10% và giảm giá theo ngưỡng). Viết bằng **C# .NET 8 Windows Forms**, theo **kiến trúc
phân tầng** (Model / Repository / Service / Controller / View), tổ chức **monorepo**.

## Yêu cầu môi trường
- **.NET 8 SDK** (`dotnet --version` → 8.x).
- Windows (dùng Windows Forms — `net8.0-windows`).
- (Tuỳ chọn) VS Code + extension `ms-dotnettools.csharp`, `anthropic.claude-code`.

## Bắt đầu nhanh
```bash
dotnet restore RestaurantApp.sln          # tải package (xUnit, test sdk)
dotnet build   RestaurantApp.sln          # build cả monorepo
dotnet test    tests/UT                   # unit test (Models/Repositories/Services/Controllers)
dotnet run --project src/App              # chạy app WinForms
powershell -File tests/IT/tools/Run-IntegrationTests.ps1   # integration test + evidence
```
Hoặc dùng script tiện lợi (chạy được dưới Windows PowerShell 5.1 lẫn pwsh 7):
```bash
powershell -File scripts/build.ps1             # build
powershell -File scripts/test.ps1              # UT(+coverage) + IT(+evidence) → reports/new/<ts>/ + report.html
powershell -File scripts/open-report.ps1 -RunDir reports/new/<ts>   # mở report + nút Done (lưu trữ)
```
> `scripts/test.ps1` gom mọi kết quả vào `reports/new/<yyyyMMdd-HHmmss>/` rồi sinh report qua
> skill **test-report**. Nút **Done** trên report chuyển thư mục sang `reports/done/<ts>/`.

## Lần chạy đầu nên thấy gì
- Build: `Build succeeded. 0 Error(s)`.
- Unit test: toàn bộ pass (MenuItem, Order, JsonMenuRepository, MenuService, OrderService,
  MenuController, OrderController).
- Integration test: TC-001..TC-004 đều PASS, có evidence trong `tests/IT/evidence/<TC>/`.
- App: cửa sổ **"RestaurantApp — Quản lý & Tìm kiếm món"** với ô tìm kiếm, lưới món, khu
  nhập liệu và các nút Thêm/Cập nhật/Xoá; dữ liệu nạp từ `data/menu.json`.

## Làm việc với Claude
```text
> /feature thêm chức năng giảm giá 10% cho đơn trên 500k
> dùng sub-agent verifier kiểm tra ranh giới các tầng
> /report tạo báo cáo HTML cho sprint này
```

## Lưu ý
- Nếu `dotnet` báo không tìm thấy SDK → cài .NET 8 trước.
- Assert tiền tệ (`"66.000 đ"`) phụ thuộc culture máy; UT đã cố định `vi-VN`, harness IT cũng vậy.

Xem [docs/02-sad/architecture-overview.md](docs/02-sad/architecture-overview.md) để hiểu kiến trúc phân tầng,
và [docs/README.md](docs/README.md) cho mục lục tài liệu thiết kế.
