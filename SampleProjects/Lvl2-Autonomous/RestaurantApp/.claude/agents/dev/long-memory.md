# dev — Long-memory (tối đa 500 chữ)

> **LUẬT:** ≤500 chữ. Chỉ tri thức bền vững phục vụ việc code: quy ước, cạm bẫy, ranh giới.

- Ranh giới MVC là bất biến: Models không `using` Views/Controllers; View gọi Controller,
  không gọi Repository; logic tiền/thuế/validate nằm ở Models.
- Tiền dùng `decimal`, không double/float.
- Mỗi tầng là một project → tên thư mục rút gọn (`Models`, `App`…) nhưng project/namespace
  vẫn đầy đủ (`RestaurantApp.Models`).
- Code mới phải kèm unit test (việc của agent test) và qua verifier mới coi là xong.
