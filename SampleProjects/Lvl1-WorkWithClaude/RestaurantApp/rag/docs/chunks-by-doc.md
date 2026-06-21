# Chunks theo từng file tài liệu (docs/)

> Liệt kê **mọi file trong `docs/`** và **chunk chưng cất của từng file** (ngắn, giàu từ khoá để
> Grep trúng). Mỗi mục ghi `Nguồn:` trỏ về file gốc. Khi `docs/` đổi, **subagent `docs`** phải
> cập nhật lại chunk tương ứng ở đây (xem `.claude/agents/docs.md`).
> Chunk theo **chủ đề** xem [chunks.md](chunks.md); bộ định tuyến: [../index.md](../index.md).
> Đường dẫn tính từ `rag/docs/`.

## 01-commons

### docs/01-commons/screen-list.md
Danh sách màn hình: **SCR-01** Quản lý & Tìm kiếm món (`MenuForm`), **SCR-02** Gọi món / Lập đơn
(`MainForm`). Mã `SCR-NN` bất biến; mỗi màn hình có 1 basic + 1 detail design; tên file = mã +
slug tiếng Việt không dấu (`SCR-01-quan-ly-mon.md`).
Nguồn: [../../docs/01-commons/screen-list.md](../../docs/01-commons/screen-list.md)

### docs/01-commons/feature-matrix.md
Ma trận chức năng × màn hình × tầng. SCR-01: CRUD + tìm kiếm món (`MenuService`, `JsonMenuRepository`,
`data/menu.json`). SCR-02: thêm món vào đơn + tính tiền (`OrderService`, `Order`). Nhóm quy tắc:
M1–M5 (quản lý món), S1–S5 (tìm kiếm), R1–R5 (tính tiền) — nơi thực thi tương ứng.
Nguồn: [../../docs/01-commons/feature-matrix.md](../../docs/01-commons/feature-matrix.md)

### docs/01-commons/coding-conventions.md
Quy ước code dùng chung: PascalCase (class/method), camelCase (biến), `_camelCase` (field private),
interface `I...`; Allman brace, indent 4 space, dòng ≤120; tiền dùng `decimal` (không double/float);
logic ở Service, không nhồi vào Form; test xUnit tên `Method_Condition_Expected`.
Nguồn: [../../docs/01-commons/coding-conventions.md](../../docs/01-commons/coding-conventions.md)

### docs/01-commons/glossary.md
Từ điển miền: Bàn (Table), Order, OrderLine, MenuItem (`Id/Name/Category/UnitPrice/IsAvailable`),
Subtotal, Discount (R3, ngưỡng 500.000đ), VAT (10%, sau giảm), Total, Invoice. Thuật ngữ kỹ thuật:
Repository, Service, composition root (`App/Program.cs`), seed data (`data/menu.json`).
Nguồn: [../../docs/01-commons/glossary.md](../../docs/01-commons/glossary.md) · liên quan: [chunks.md#thuật-ngữ-miền](chunks.md#thuật-ngữ-miền)

## 02-sad

### docs/02-sad/architecture-overview.md
Tổng quan kiến trúc: WinForms .NET 8, phân tầng monorepo. Tầng + trách nhiệm: App (composition
root) → Views (WinForms) → Controllers (điều phối) → Services (validate/tìm kiếm/tính tiền) →
Repositories (JSON) → Models (entity). Đồ thị phụ thuộc một chiều; có sequence "Thêm món vào đơn".
Nguồn: [../../docs/02-sad/architecture-overview.md](../../docs/02-sad/architecture-overview.md)

### docs/02-sad/layering-rules.md
Quy tắc & ranh giới tầng (vi phạm → fail build vì mỗi tầng 1 project). Bảng ĐƯỢC/KHÔNG ĐƯỢC mỗi
tầng; đồ thị ProjectReference: App→tất cả, Views→Controllers/Models, Controllers→Services/Models,
Services→Repositories/Models, Repositories→Models. Ví dụ đúng/sai; `verifier` grep vi phạm.
Nguồn: [../../docs/02-sad/layering-rules.md](../../docs/02-sad/layering-rules.md) · liên quan: [chunks.md#ranh-giới-các-tầng](chunks.md#ranh-giới-các-tầng)

### docs/02-sad/project-structure.md
Bố cục monorepo: `src/{Models,Repositories,Services,Controllers,Views,App}`, `tests/{UT,IT}`. Thư
mục tên ngắn, project/namespace tên dài (`RestaurantApp.Services`). Ánh xạ tầng ↔ file chính
(`MenuService.cs`, `JsonMenuRepository.cs`, `MenuForm.cs`...). UT nhắm Services/Repos/Controllers; IT/harness chạy cả View.
Nguồn: [../../docs/02-sad/project-structure.md](../../docs/02-sad/project-structure.md) · liên quan: [chunks.md#ranh-giới-các-tầng](chunks.md#ranh-giới-các-tầng)

### docs/02-sad/architecture-decisions.md
ADR rút gọn: ADR-001 tách nghiệp vụ khỏi UI; ADR-002 monorepo theo tầng; ADR-003 VAT 10% trong
`Order`; ADR-004 hai màn hình SCR-01/02; ADR-005 bộ nhớ theo sub-agent; ADR-006 bỏ MVC → tách
Models/Services/Repositories; ADR-007 lưu JSON (`JsonMenuRepository`, `data/menu.json`).
Nguồn: [../../docs/02-sad/architecture-decisions.md](../../docs/02-sad/architecture-decisions.md)

## 03-basic-designs

### docs/03-basic-designs/SCR-01-quan-ly-mon.md
Basic design SCR-01 (`MenuForm`/`MenuController`/`MenuService`): mục đích quản lý thực đơn; wireframe
ô tìm + lưới món + khu nhập liệu; chức năng F1 xem, F2 tìm, F3 thêm, F4 sửa, F5 xoá; ràng buộc tầng
(View chỉ gọi Controller). Thực thể `MenuItem`, nguồn `JsonMenuRepository`.
Nguồn: [../../docs/03-basic-designs/SCR-01-quan-ly-mon.md](../../docs/03-basic-designs/SCR-01-quan-ly-mon.md) · liên quan: [chunks.md#quản-lý-món-ăn](chunks.md#quản-lý-món-ăn)

### docs/03-basic-designs/SCR-02-goi-mon.md
Basic design SCR-02 (`MainForm`/`OrderController`/`OrderService`): lập đơn gọi món; wireframe combobox
món + số lượng + danh sách dòng + tổng tiền; chức năng F1 nạp thực đơn, F2 thêm vào đơn (gộp trùng),
F3 xem đơn, F4 tính tổng (R1–R5). `Order` là dữ liệu phiên, không lưu file.
Nguồn: [../../docs/03-basic-designs/SCR-02-goi-mon.md](../../docs/03-basic-designs/SCR-02-goi-mon.md) · liên quan: [chunks.md#quy-tắc-tính-tiền](chunks.md#quy-tắc-tính-tiền)

### docs/03-basic-designs/_template.md
Template Basic Design (copy → `SCR-NN-<slug>.md`). Khung mục: 1 Mục đích, 2 Wireframe, 3 Danh sách
chức năng, 4 Dữ liệu liên quan, 5 Ràng buộc tầng, 6 Liên kết. *(File mẫu — không có nội dung nghiệp vụ để chunk.)*
Nguồn: [../../docs/03-basic-designs/_template.md](../../docs/03-basic-designs/_template.md)

## 04-detail-designs

### docs/04-detail-designs/SCR-01-quan-ly-mon.md
Detail design SCR-01: controls (`txtSearch`, `chkOnlyAvailable`, `gridDishes`, `txtName/Category/Price`,
`btnAdd/Update/Delete`), sự kiện→handler; validate M1–M5 (tên không rỗng/không trùng/giá≥0/sửa-xoá theo
Id/loại trừ chính nó) và tìm kiếm S1–S5; API Add/Update/Delete/Search; sequence "Thêm món".
Nguồn: [../../docs/04-detail-designs/SCR-01-quan-ly-mon.md](../../docs/04-detail-designs/SCR-01-quan-ly-mon.md) · liên quan: [chunks.md#quản-lý-món-ăn](chunks.md#quản-lý-món-ăn), [chunks.md#tìm-kiếm-món-ăn](chunks.md#tìm-kiếm-món-ăn)

### docs/04-detail-designs/SCR-02-goi-mon.md
Detail design SCR-02: controls (`cboMenu`, `numQuantity`, `btnAdd`, `lstOrder`, `lblTotal`); tính tiền
R1 Subtotal, R2 VAT 10% sau giảm, R3 giảm 10% khi Subtotal≥500k, R4 Total không âm, R5 decimal;
O1 gộp món trùng, O2 SL≤0 lỗi, O3 Id sai lỗi; API GetMenu/AddItem/GetLines/GetTotalText.
Nguồn: [../../docs/04-detail-designs/SCR-02-goi-mon.md](../../docs/04-detail-designs/SCR-02-goi-mon.md) · liên quan: [chunks.md#quy-tắc-tính-tiền](chunks.md#quy-tắc-tính-tiền)

### docs/04-detail-designs/_template.md
Template Detail Design (copy → `SCR-NN-<slug>.md`). Khung mục: 1 Controls, 2 Sự kiện & xử lý, 3 Quy
tắc nghiệp vụ, 4 API, 5 Luồng tuần tự (Mermaid), 6 Xử lý lỗi, 7 Test bao phủ. *(File mẫu — không chunk nghiệp vụ.)*
Nguồn: [../../docs/04-detail-designs/_template.md](../../docs/04-detail-designs/_template.md)

## 05-data

### docs/05-data/data-model.md
Mô hình thực thể: `MenuItem` (`Id>0` bất biến, `Name` không rỗng/không trùng, `Category` mặc định
"Khác", `UnitPrice` decimal≥0, `IsAvailable`); `Order` (`Lines`, hằng `VatRate=0.10`,
`DiscountThreshold=500000`, `DiscountRate=0.10`; `Subtotal/Discount/Vat/CalculateTotal`); `OrderLine`
(`Item`, `Quantity>0`, `LineTotal`). Chỉ `MenuItem` lưu JSON; Order/OrderLine là phiên.
Nguồn: [../../docs/05-data/data-model.md](../../docs/05-data/data-model.md)

### docs/05-data/er-diagram.md
Sơ đồ ER: MENU_ITEM 1—n ORDER_LINE, ORDER 1—n ORDER_LINE. Trong một đơn cùng món chỉ 1 dòng (gộp O1);
`OrderLine` tham chiếu object `MenuItem` (không copy giá). Chỉ `MenuItem` là dữ liệu chủ (lưu JSON);
`Order`/`OrderLine` là dữ liệu giao dịch phiên (chưa lưu).
Nguồn: [../../docs/05-data/er-diagram.md](../../docs/05-data/er-diagram.md)

### docs/05-data/json-storage.md
Lưu trữ JSON: `JsonMenuRepository` đọc/ghi `data/menu.json` (mảng `MenuItem`). Load: thiếu/hỏng →
ghi seed mặc định; Save: serialize toàn bộ (ghi đè) sau mỗi Add/Update/Remove; `NextId()=max+1`;
`System.Text.Json` `WriteIndented=true`. Trừu tượng qua `IMenuRepository` → đổi sang DB không ảnh hưởng tầng trên.
Nguồn: [../../docs/05-data/json-storage.md](../../docs/05-data/json-storage.md)

## docs/README.md
Mục lục tài liệu, chia theo giai đoạn thiết kế: 01-commons, 02-sad, 03-basic-designs, 04-detail-designs,
05-data. Tri thức nghiệp vụ tách riêng ở `../rag/`; build/chạy/test ở `../README.md`.
Nguồn: [../../docs/README.md](../../docs/README.md)
