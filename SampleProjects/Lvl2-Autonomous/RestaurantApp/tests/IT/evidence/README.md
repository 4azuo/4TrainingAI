# Evidence (bằng chứng IT)

Thư mục này chứa bằng chứng do tool IT **tự sinh** cho từng testcase:

```
evidence/
├── SUMMARY.md          # tổng hợp pass/fail mọi testcase (tool sinh)
└── <TC-id>/
    ├── output.log      # các bước, kỳ vọng vs thực tế, PASS/FAIL
    └── screenshot.png  # ảnh MenuForm tại trạng thái của testcase
```

> Các file `output.log`, `screenshot.png`, `SUMMARY.md` là **kết quả chạy** nên KHÔNG commit
> (đã loại trong `.gitignore`). Chỉ giữ `README.md` này. Chạy lại bằng:
>
> ```
> powershell -File ../tools/Run-IntegrationTests.ps1        # pwsh 7 cũng được
> ```
>
> Khi chạy qua `scripts/test.ps1`, evidence được ghi vào
> `reports/new/<ts>/it/evidence/` thay vì thư mục này (mỗi lần chạy một thư mục riêng).
