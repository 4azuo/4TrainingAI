# Index RAG — RestaurantApp

Bộ định tuyến truy hồi tri thức nghiệp vụ. Tìm từ khoá → đọc mục trong `docs/chunks.md` → mở nguồn nếu cần.

| Chủ đề | Từ khoá để Grep | Chunk (docs/) | Nguồn đầy đủ |
|--------|-----------------|---------------|--------------|
| Quản lý món (CRUD) | thêm món, sửa món, xoá món, trùng tên, validate, M1, M2, M3, M4, M5 | [docs/chunks.md#quản-lý-món-ăn](docs/chunks.md#quản-lý-món-ăn) | [knowledge/menu-rules.md](knowledge/menu-rules.md) |
| Tìm kiếm món | tìm kiếm, search, từ khoá, danh mục, onlyAvailable, S1, S2, S3, S4, S5 | [docs/chunks.md#tìm-kiếm-món-ăn](docs/chunks.md#tìm-kiếm-món-ăn) | [knowledge/menu-rules.md](knowledge/menu-rules.md) |
| Tính tiền / thuế / giảm giá | subtotal, vat, 10%, discount, 500k, total, R1, R2, R3, R4, R5 | [docs/chunks.md#quy-tắc-tính-tiền](docs/chunks.md#quy-tắc-tính-tiền) | [knowledge/business-rules.md](knowledge/business-rules.md) |
| Ranh giới các tầng | model, repository, service, controller, view, ranh giới, phụ thuộc một chiều | [docs/chunks.md#ranh-giới-các-tầng](docs/chunks.md#ranh-giới-các-tầng) | [../docs/02-sad/layering-rules.md](../docs/02-sad/layering-rules.md) |
| Thuật ngữ miền | order, orderline, menuitem, bàn, hoá đơn, invoice | [docs/chunks.md#thuật-ngữ-miền](docs/chunks.md#thuật-ngữ-miền) | [knowledge/domain-glossary.md](knowledge/domain-glossary.md) |
| Build / chạy / test | dotnet build, dotnet test, dotnet run, sdk, net8.0-windows | [docs/chunks.md#build--chạy--test](docs/chunks.md#build--chạy--test) | [../README.md](../README.md) |

> Mẹo: `grep -ri "<từ khoá>" rag/docs` để nhảy thẳng tới mẩu trả lời.
>
> Xem chunk theo **tài liệu nguồn** (tổng hợp ngược): [docs/chunks-by-doc.md](docs/chunks-by-doc.md).
