# data/ — Lưu trữ JSON

Dữ liệu **bền vững** của RestaurantApp ở định dạng JSON. Đây là **seed mẫu** (commit kèm).

| File | Thực thể | Repository |
|------|----------|------------|
| `menu.json` | `MenuItem[]` | `JsonMenuRepository` |

- Khi build, `src/App` copy `menu.json` ra `bin/.../data/menu.json`; app đọc/ghi tại bản copy
  đó (seed gốc trong source **không** bị thay đổi khi chạy).
- Lược đồ chi tiết: [docs/05-data/json-storage.md](../docs/05-data/json-storage.md).
