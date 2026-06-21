# test — Long-memory (tối đa 500 chữ)

> **LUẬT:** ≤500 chữ. Chỉ tri thức bền vững về kiểm thử dự án này.

- Dùng **xUnit**; tên ca `Method_Condition_Expected`; mỗi ca độc lập.
- Tập trung test **Models** (logic tiền/thuế/validate/search) và **Controllers** (điều phối);
  KHÔNG unit-test trực tiếp WinForms — hành vi UI để IT lo.
- Unit test mirror cấu trúc `src/` trong `tests/UT/` (Models/, Controllers/).
- Cạm bẫy: định dạng tiền `N0` phụ thuộc culture máy → ép `CultureInfo` khi assert chuỗi tiền.
- Mỗi business rule mới (R1–R5, validate trùng tên) phải có ca tương ứng.
