# Chunks — Mẩu tri thức chưng cất (RestaurantApp)

> File gộp các mẩu (chunk) ngắn, giàu từ khoá, tối ưu để **Grep trúng**. Mỗi mục là một chủ đề,
> kèm `Nguồn:` để truy về tri thức đầy đủ trong `../knowledge/` hoặc tài liệu `../../docs/`.
> Bộ định tuyến: [../index.md](../index.md).

## Quản lý món ăn

Thêm/sửa/xoá món; validate tập trung ở `MenuService` (Services). Quy tắc: M1 tên không rỗng;
M2 không trùng tên (không phân biệt hoa thường); M3 giá ≥ 0 (`decimal`); M4 sửa/xoá theo Id,
Id sai → InvalidOperationException; M5 khi sửa, check trùng loại trừ chính nó. API:
`MenuController.AddDish(name, price, category)`, `UpdateDish(id, name, price, category,
isAvailable)`, `DeleteDish(id)`. Ranh giới: View không validate/không gọi Repository, chỉ gọi
Controller; Controller uỷ quyền MenuService. Test: UT MenuServiceTests/MenuControllerTests; IT
TC-001/TC-002/TC-004.

Nguồn: ../../docs/04-detail-designs/SCR-01-quan-ly-mon.md, ../knowledge/menu-rules.md

## Tìm kiếm món ăn

Lọc món theo từ khoá ở `MenuService.Search(keyword, onlyAvailable)`. Quy tắc: S1 khớp tên
HOẶC danh mục; S2 không phân biệt hoa thường; S3 từ khoá rỗng → trả toàn bộ; S4
`onlyAvailable=true` loại món `IsAvailable=false`; S5 trim từ khoá. API:
`MenuController.Search(keyword, onlyAvailable=false)`; hook UI cho IT
`MenuForm.ApplySearch(keyword, onlyAvailable)`. Ranh giới: logic lọc ở Service, View chỉ
truyền từ khoá và hiển thị. Test: UT các ca `Search_*`; IT TC-003
(tìm "phở" → 1 kết quả + screenshot lưới đã lọc).

Nguồn: ../../docs/04-detail-designs/SCR-01-quan-ly-mon.md, ../knowledge/menu-rules.md

## Quy tắc tính tiền

R1 **Subtotal** = Σ(đơn giá × số lượng) mọi dòng. R2 **VAT** = 10% cố định, tính trên số
tiền **sau giảm giá**. R3 **Giảm giá**: đơn có Subtotal ≥ 500.000đ → giảm 10% trên Subtotal,
tính TRƯỚC VAT. R4 **Total** = (Subtotal − Discount) + VAT_sau_giảm, không bao giờ âm. R5
tiền dùng `decimal` (VND), làm tròn tới đồng khi hiển thị. Ví dụ: Lẩu thái 250k × 3 =
Subtotal 750k → giảm 75k → 675k → VAT 67.5k → **Total 742.500đ**. Toàn bộ logic này nằm ở
Model `Order` (`Subtotal()`, `Discount()`, `Vat()`, `CalculateTotal()`), không nằm ở Form.

Nguồn: ../knowledge/business-rules.md

## Ranh giới các tầng

Phụ thuộc một chiều: App → Views → Controllers → Services → Repositories → Models.
**Models** (`src/Models`): entity thuần (MenuItem, Order, OrderLine); KHÔNG truy xuất dữ liệu,
KHÔNG `using` tầng trên. **Repositories** (`src/Repositories`): đọc/ghi dữ liệu (JSON), KHÔNG
chứa logic nghiệp vụ. **Services** (`src/Services`): logic nghiệp vụ (validate, tìm kiếm, tính
tiền). **Controllers** (`src/Controllers`): điều phối, không chứa logic UI lẫn nghiệp vụ.
**Views** (`src/Views`, WinForms): chỉ gọi Controller, KHÔNG gọi thẳng Repository/Service,
KHÔNG tính tiền trong Form. Vì mỗi tầng là một project, compiler tự chặn vi phạm
(Models.csproj không tham chiếu Views). Sub-agent `verifier` grep các vi phạm này.

Nguồn: ../../docs/02-sad/layering-rules.md, ../../docs/02-sad/project-structure.md

## Thuật ngữ miền

**Order** (đơn) = một lượt gọi món của bàn, gồm nhiều **OrderLine** (dòng món: 1 MenuItem +
số lượng). **MenuItem** (món) có `Id`, `Name`, `UnitPrice`. **Subtotal** = tổng trước thuế;
**Discount** = giảm giá theo ngưỡng (R3); **VAT** = thuế GTGT 10%; **Total** = thành tiền
cuối khách trả. **Invoice** (hoá đơn) = bản in tổng kết khi thanh toán. **Bàn (Table)** = vị
trí khách ngồi, gắn một Order đang mở. Thêm cùng một món hai lần thì gộp số lượng (không tạo
dòng mới).

Nguồn: ../knowledge/domain-glossary.md

## Build / chạy / test

Cần **.NET 8 SDK** (WinForms dùng `net8.0-windows`). Lệnh: `dotnet build RestaurantApp.sln`,
`dotnet test tests/UT` (test các tầng Models/Repositories/Services/Controllers),
`dotnet run --project src/App` (mở cửa sổ "RestaurantApp — Quản lý & Tìm kiếm món"; dữ liệu
nạp từ `data/menu.json`). Script đánh số trong `scripts/`: `01-build.ps1` (build solution),
`11-unit-test.ps1` (xoá `tests/UT/report` rồi chạy UT → trx + coverage), `12-integration-test.ps1`
(xoá `tests/IT/evidence` rồi chạy IT → evidence), `13-generate-report.ps1` (copy UT report +
IT evidence vào `reports/new/<yyyyMMdd-HHmmss>/` rồi sinh report HTML qua skill `test-report`),
`14-test.ps1` (chạy cả 4 bước trên). Lưu ý: assert chuỗi tiền `"66.000 đ"` phụ thuộc culture máy
nên có thể vỡ giữa các máy.

Nguồn: ../../README.md
