# RAG — Kho tri thức RestaurantApp

Thư mục này chứa tri thức nghiệp vụ dạng văn bản để **truy hồi (retrieval)** khi cần,
thay vì nhồi hết vào context. Khi làm việc, Claude (hoặc sub-agent) sẽ **đọc đúng file
liên quan** tới câu hỏi thay vì đọc toàn bộ.

## Cấu trúc
```
rag/
├── README.md       # bạn đang đọc
├── index.md        # bộ định tuyến: chủ đề / từ khoá → mục trong docs/chunks.md + nguồn
├── docs/           # mẩu chưng cất ngắn, tối ưu để Grep trúng
│   ├── chunks.md        # gộp mọi chunk, mỗi chủ đề là một mục `## ...`
│   └── chunks-by-doc.md # tổng hợp ngược: gom chunk theo từng tài liệu nguồn
└── knowledge/      # tri thức đầy đủ (nguồn gốc)
    ├── menu-rules.md
    ├── business-rules.md
    └── domain-glossary.md
```

## Cách dùng (manual RAG loop)
1. Có câu hỏi → mở `index.md`, tìm dòng từ khoá khớp.
2. Đọc mục được trỏ tới trong `docs/chunks.md` (câu trả lời cô đọng).
3. Cần đầy đủ hơn → mở file `knowledge/` nguồn ghi kèm.

Ví dụ:
- Quy tắc tính tiền/thuế/giảm giá → `docs/chunks.md#quy-tắc-tính-tiền` → `knowledge/business-rules.md`.
- Thuật ngữ nghiệp vụ → `docs/chunks.md#thuật-ngữ-miền` → `knowledge/domain-glossary.md`.

> RAG "thủ công": grep/đọc đúng mục theo chủ đề thay vì nhồi toàn bộ vào context.

## Quy ước
- `docs/chunks.md` — mỗi mục ngắn (≤ ~150 chữ), một chủ đề, giàu từ khoá; ghi `Nguồn:` để truy vết.
- `knowledge/` là bản đầy đủ; khi đổi phải cập nhật lại mục chunk tương ứng.
- Đặt mã định danh (R1, R2…) cho quy tắc để trích dẫn chính xác trong code/test.
