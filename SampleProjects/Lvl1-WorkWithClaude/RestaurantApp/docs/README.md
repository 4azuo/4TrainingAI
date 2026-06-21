# Tài liệu RestaurantApp

Tài liệu nội bộ, chia theo **giai đoạn thiết kế**. Tri thức nghiệp vụ tách riêng ở `../rag/`.

## Cấu trúc thư mục
| Thư mục | Nội dung |
|---------|----------|
| [01-commons/](01-commons/) | File **dùng chung**: danh sách màn hình, ma trận chức năng, quy ước code, từ điển |
| [02-sad/](02-sad/) | **Thiết kế kiến trúc** (SAD): tổng quan, quy tắc phân tầng, bố cục, ADR |
| [03-basic-designs/](03-basic-designs/) | **Thiết kế cơ bản** — 1 file / 1 màn hình (+ template) |
| [04-detail-designs/](04-detail-designs/) | **Thiết kế chi tiết** — 1 file / 1 màn hình (+ template) |
| [05-data/](05-data/) | **Dữ liệu**: mô hình, sơ đồ ER, lưu trữ JSON |

## Mục lục nhanh
### 01 — Commons
- [screen-list.md](01-commons/screen-list.md) — danh sách màn hình (SCR-NN)
- [feature-matrix.md](01-commons/feature-matrix.md) — ma trận chức năng × màn hình × tầng
- [coding-conventions.md](01-commons/coding-conventions.md) — quy ước đặt tên, định dạng, test
- [glossary.md](01-commons/glossary.md) — từ điển miền

### 02 — SAD (Software Architecture Design)
- [architecture-overview.md](02-sad/architecture-overview.md) — kiến trúc phân tầng & luồng
- [layering-rules.md](02-sad/layering-rules.md) — quy tắc & ranh giới các tầng
- [project-structure.md](02-sad/project-structure.md) — bố cục monorepo
- [architecture-decisions.md](02-sad/architecture-decisions.md) — ADR

### 03 — Basic Designs
- [_template.md](03-basic-designs/_template.md) — template thiết kế cơ bản
- [SCR-01 Quản lý & Tìm kiếm món](03-basic-designs/SCR-01-quan-ly-mon.md)
- [SCR-02 Gọi món / Lập đơn](03-basic-designs/SCR-02-goi-mon.md)

### 04 — Detail Designs
- [_template.md](04-detail-designs/_template.md) — template thiết kế chi tiết
- [SCR-01 Quản lý & Tìm kiếm món](04-detail-designs/SCR-01-quan-ly-mon.md)
- [SCR-02 Gọi món / Lập đơn](04-detail-designs/SCR-02-goi-mon.md)

### 05 — Data
- [data-model.md](05-data/data-model.md) — thực thể & thuộc tính
- [er-diagram.md](05-data/er-diagram.md) — sơ đồ quan hệ
- [json-storage.md](05-data/json-storage.md) — lưu trữ JSON

## Liên kết nhanh
- Bắt đầu nhanh (build/chạy/test): [../README.md](../README.md)
- Quy tắc tính tiền (R1–R5): `../rag/knowledge/business-rules.md`
- Bộ định tuyến RAG: `../rag/index.md`
