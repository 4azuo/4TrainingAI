# Tests

Hai mức kiểm thử, tách rõ:

```
tests/
├── UT/                         # Unit Test — nhanh, không UI, mirror cấu trúc src/
│   ├── Models/                 #   test entity (MenuItem, Order, OrderLine)
│   ├── Repositories/           #   test lưu trữ JSON (JsonMenuRepository)
│   ├── Services/               #   test nghiệp vụ (MenuService, OrderService)
│   ├── Controllers/            #   test điều phối (MenuController, OrderController)
│   └── RestaurantApp.UnitTests.csproj
└── IT/                         # Integration Test — chạy luồng thật + chụp evidence
    ├── testcases/              #   đặc tả từng testcase (+ testcases.json manifest)
    ├── evidence/               #   output.log + screenshot.png (tool tự sinh)
    ├── harness/                #   chương trình chạy testcase qua đủ các tầng
    └── tools/Run-IntegrationTests.ps1   # tool tự động chạy & chụp evidence
```

## Chạy
```bash
dotnet test tests/UT                                          # unit test
powershell -File tests/IT/tools/Run-IntegrationTests.ps1      # integration + evidence (pwsh cũng được)

# Hoặc chạy cả UT + IT và sinh report gom vào reports/new/<yyyyMMdd-HHmmss>/:
powershell -File scripts/test.ps1
powershell -File scripts/open-report.ps1 -RunDir reports/new/<ts>   # mở report + nút Done
```
> Script không hard-code `pwsh`; chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7.

## Khác nhau
| | UT | IT |
|--|----|----|
| Phạm vi | một lớp/hàm | luồng qua nhiều tầng (Repo→Service→Controller→View) |
| Tốc độ | rất nhanh | chậm hơn (dựng Form, chụp ảnh) |
| Bằng chứng | pass/fail của xUnit | `output.log` + `screenshot.png` cho từng testcase |
| Cấu trúc | mirror `src/` | testcases + evidence |
