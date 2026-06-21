<!--
  TEMPLATE — Basic Design (Thiết kế cơ bản) cho MỘT màn hình.
  Copy file này, đổi tên thành SCR-NN-<slug>.md, điền các mục dưới.
  Basic design = "nhìn từ ngoài vào": màn hình làm gì, bố cục, chức năng, ràng buộc tầng.
  Chi tiết điều khiển/sự kiện/validate → để ở Detail Design (04-detail-designs).
-->
# [SCR-NN] <Tên màn hình> — Basic Design

| Thuộc tính | Giá trị |
|------------|---------|
| Mã màn hình | SCR-NN |
| Form (Views) | `<TênForm>` |
| Controller | `<TênController>` |
| Service | `<TênService>` |
| Trạng thái | Draft / Done |

## 1. Mục đích
<Một đoạn ngắn: màn hình này phục vụ ai, giải quyết việc gì.>

## 2. Phác thảo bố cục (wireframe)
```
┌─ <Tiêu đề> ───────────────────────────────┐
│  <vùng 1>                                  │
│  <vùng 2>                                  │
└────────────────────────────────────────────┘
```

## 3. Danh sách chức năng
| # | Chức năng | Mô tả ngắn | Tầng xử lý |
|---|-----------|-----------|------------|
| F1 | <...> | <...> | Service/... |

## 4. Dữ liệu liên quan
- Thực thể: `<MenuItem / Order ...>` (xem [05-data](../05-data/data-model.md)).
- Nguồn dữ liệu: `<Repository / JSON>`.

## 5. Ràng buộc tầng
- View chỉ gọi Controller; KHÔNG gọi thẳng Service/Repository.
- Logic nằm ở Service (xem [layering-rules](../02-sad/layering-rules.md)).

## 6. Liên kết
- Detail design: [04-detail-designs/SCR-NN-<slug>.md](../04-detail-designs/)
- Test: <UT/IT liên quan>
