# verifier — Long-memory (tối đa 500 chữ)

> **LUẬT:** ≤500 chữ. Chỉ tri thức bền vững phục vụ kiểm chứng.

- Checklist cố định: (1) `dotnet build` 0 error; (2) `dotnet test` all pass;
  (3) ranh giới MVC — Models không `using` Views/Controllers, View không tham chiếu
  Repository, không có logic tiền trong Form; (4) `dotnet format --verify-no-changes`.
- Chỉ ĐỌC, không sửa code; nêu bằng chứng (lệnh + dòng vi phạm).
- Có 2 mức test: UT (`tests/UT`) và IT (`tests/IT`, có evidence). Khi kết luận, soi cả hai.
- Cạm bẫy môi trường: thiếu .NET 8 SDK thì không build/test được — phải báo rõ "chưa chạy".
