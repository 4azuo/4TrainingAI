# SAD — Quyết định kiến trúc (ADR rút gọn)

Ghi lại các quyết định bền vững của dự án.

## ADR-001 — Tách nghiệp vụ khỏi UI
- **Quyết định:** không nhồi logic vào code-behind của Form.
- **Lý do:** tách logic nghiệp vụ khỏi UI để test được và dễ bảo trì.
- **Hệ quả:** View chỉ gọi Controller; logic nằm ở Service/Model.

## ADR-002 — Tổ chức monorepo theo tầng
- **Quyết định:** mỗi tầng là 1 project riêng trong cùng solution.
- **Lý do:** ép buộc ranh giới phụ thuộc bằng compiler.
- **Hệ quả:** đồ thị: App → Views → Controllers → Services → Repositories → Models.

## ADR-003 — Thuế VAT
- **Quyết định:** VAT cố định 10%, tính trong `Order` (entity) qua `OrderService`.
- **Lý do:** quy định nghiệp vụ, không để rải rác ở UI.

## ADR-004 — Hai màn hình cốt lõi
- **Quyết định:** SCR-01 (Quản lý & Tìm kiếm món) và SCR-02 (Gọi món) là hai luồng chính.
- **Lý do:** đủ minh hoạ CRUD + tìm kiếm + tính tiền.
- **Hệ quả:** mỗi màn hình có 1 basic design + 1 detail design.

## ADR-005 — Bộ nhớ theo từng sub-agent
- **Quyết định:** mỗi sub-agent có `short-memory.md` (≤10 mục) và `long-memory.md` (≤500 chữ).
- **Lý do:** mỗi agent chỉ cần nhớ việc của mình → context gọn, không nhiễu chéo.

## ADR-006 — Bỏ MVC, chuyển kiến trúc phân tầng Model/Service/Repository
- **Quyết định:** tách project `Models` cũ (gộp entity + service + repository) thành **3
  project riêng**: `Models` (entity), `Services` (nghiệp vụ), `Repositories` (dữ liệu).
- **Lý do:** trách nhiệm đơn lẻ (SRP); đổi nguồn lưu trữ không ảnh hưởng nghiệp vụ.
- **Hệ quả:** thêm `RestaurantApp.Services`, `RestaurantApp.Repositories`; cập nhật
  ProjectReference toàn solution và cấu trúc `tests/UT`.

## ADR-007 — Lưu trữ bằng JSON file
- **Quyết định:** `JsonMenuRepository` đọc/ghi `data/menu.json` (System.Text.Json).
- **Lý do:** lưu trữ bền vững, đơn giản, dễ xem/sửa bằng tay; thay cho in-memory.
- **Hệ quả:** seed mẫu `data/menu.json` commit kèm; `InMemoryMenuRepository` giữ lại cho
  test nhanh. Đường dẫn file truyền từ composition root (`App`).
