# RestaurantApp — Hướng dẫn cho Claude

> Đây là **bộ nhớ dự án (project memory)**. Claude Code tự nạp file này mỗi phiên làm việc.
> Dùng cú pháp `@đường-dẫn` để import thêm file memory khác.

## Bối cảnh dự án
- **RestaurantApp**: phần mềm quản lý gọi món cho nhà hàng, viết bằng **C# / .NET 8 Windows Forms**.
- Kiến trúc: **phân tầng** (layered) — Model / Repository / Service / Controller / View. (Bỏ MVC cũ.)
- Lưu trữ: **JSON file** (`data/menu.json`) qua tầng Repository.
- Tổ chức mã nguồn: **monorepo** — nhiều project con trong cùng 1 solution (`RestaurantApp.sln`).

## Cấu trúc monorepo
```
data/            # lưu trữ JSON (seed mẫu: menu.json)
src/
  Models/        # RestaurantApp.Models — entity thuần (MenuItem, Order, OrderLine)
  Repositories/  # RestaurantApp.Repositories — truy xuất dữ liệu, lưu JSON
  Services/      # RestaurantApp.Services — logic nghiệp vụ (validate, tìm kiếm, tính tiền)
  Controllers/   # RestaurantApp.Controllers — điều phối View <-> Service
  Views/         # RestaurantApp.Views — WinForms (Form, control)
  App/           # RestaurantApp.App — entry point: Program.cs, composition root
tests/
  UT/            # Unit test (xUnit) — mirror src/ (Models/, Repositories/, Services/, Controllers/)
  IT/            # Integration test — testcases + evidence + tool tự động
```
> Đồ thị phụ thuộc một chiều: App → Views → Controllers → Services → Repositories → Models.

## Chức năng cốt lõi (mỗi màn hình có basic + detail design trong docs/)
1. **SCR-01 Quản lý & Tìm kiếm món** (CRUD + lọc) — xem @docs/04-detail-designs/SCR-01-quan-ly-mon.md
2. **SCR-02 Gọi món / Lập đơn** (tính tiền) — xem @docs/04-detail-designs/SCR-02-goi-mon.md

## Quy tắc bắt buộc
1. **Tôn trọng ranh giới tầng**: View chỉ gọi Controller; Controller chỉ gọi Service; Service
   gọi Repository. View/Controller KHÔNG gọi thẳng Repository.
2. Tầng dưới KHÔNG tham chiếu tầng trên (Models không biết gì về Repository/Service/View).
3. Logic nghiệp vụ (validate, tìm kiếm) nằm ở **Service**; tính tiền nằm ở **Model** (`Order`);
   truy xuất dữ liệu nằm ở **Repository**. KHÔNG nhồi logic vào Form.
4. Code mới phải có unit test trong `tests/UT` (đặt đúng tầng) và testcase IT nếu là luồng người dùng.
5. Tuân thủ coding convention: xem @docs/01-commons/coding-conventions.md

## Lệnh thường dùng
Pipeline test đã đơn giản hoá thành các script đánh số trong `scripts/`
(log tiếng Anh, comment tiếng Việt):
- Build: `powershell -File scripts/01-build.ps1` (hoặc `dotnet build RestaurantApp.sln`)
- Unit test: `powershell -File scripts/11-unit-test.ps1` — xoá `tests/UT/report` rồi chạy UT
  (trx + coverage) ra `tests/UT/report`.
- Integration test: `powershell -File scripts/12-integration-test.ps1` — xoá `tests/IT/evidence`
  rồi chạy IT ra `tests/IT/evidence`.
- Sinh report: `powershell -File scripts/13-generate-report.ps1` — copy `tests/UT/report` +
  `tests/IT/evidence` vào `reports/new/<yyyyMMdd-HHmmss>/` rồi sinh report HTML.
- Chạy toàn bộ (build → UT → IT → report): `powershell -File scripts/14-test.ps1`
- Chạy app: `dotnet run --project src/App`
- Tạo report: dùng skill **test-report** hoặc command `/report`

> **Xem report**: mở thẳng `reports/new/<ts>/report.html` bằng file:// (dữ liệu nhúng trong
> `report-data.js`). Nút **Done** gọi `scripts/report-server.py` qua URL tuyệt đối
> `http://127.0.0.1:8770/api/done` (CORS bật nên chạy cả khi file://) để chuyển
> `reports/new/<ts>` sang `reports/done/<ts>`. Server tự chạy khi mở workspace (xem `.vscode/tasks.json`).

## Bộ nhớ làm việc (memory protocol)
Bộ nhớ tiến trình KHÔNG dùng chung — **mỗi sub-agent có bộ nhớ riêng** trong thư mục của nó:
`.claude/agents/<tên>/short-memory.md` (≤10 mục) và `long-memory.md` (≤500 chữ).
- **Đầu việc**: agent đọc memory riêng của mình để khỏi lặp lỗi cũ.
- **Cuối việc**: agent tự ghi 1 mục vào short-memory của mình; khi tràn 10 → nén mục cũ nhất
  sang long-memory rồi xoá khỏi short. (Rule chi tiết nằm trong từng agent.)
- Quyết định kiến trúc bền vững → ghi vào @docs/02-sad/architecture-decisions.md (không phải memory).

## Sub-agents (xem .claude/agents/)
- **dev** — viết & sửa code theo kiến trúc phân tầng (Model/Repository/Service/Controller/View).
- **test** — viết và chạy unit test trong `tests/UT`.
- **docs** — cập nhật tài liệu trong `docs/`.
- **verifier** — kiểm chứng build, UT, IT (evidence), ranh giới kiến trúc.
- **reporter** — tổng hợp kết quả, gọi skill tạo report HTML.
