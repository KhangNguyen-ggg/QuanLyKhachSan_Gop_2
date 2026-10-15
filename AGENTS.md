# Quy định làm việc cho Codex

## Kiểm tra và dọn dẹp tự động

- **Không tạo thư mục `checks/`, `check/`, `tests-temp/` hoặc các tệp kiểm tra tạm (`*.ps1`, `*.bat`, `*.cmd`, v.v.) trong dự án**, trừ khi người dùng yêu cầu rõ ràng.
- Khi cần kiểm tra, ưu tiên dùng lệnh trực tiếp trong terminal (`dotnet build`, `dotnet test` nếu có bài kiểm thử sẵn, v.v.) và tận dụng các công cụ, bài kiểm tra hiện có.
- Nếu bắt buộc phải viết script kiểm tra tạm, tạo trong thư mục tạm của hệ điều hành (`$env:TEMP` trên Windows), **không đặt trong repository**. Tự xóa script tạm sau khi chạy, kể cả khi kiểm tra thất bại (dùng `try/finally`).
- Trước khi kết thúc mỗi tác vụ, **xóa toàn bộ tệp/thư mục kiểm tra tạm do chính tác vụ đó tạo ra**. Không xóa hoặc sửa các tệp kiểm tra đã tồn tại từ trước, tệp người dùng tạo, hoặc tệp được Git theo dõi khi chưa có yêu cầu.
- Không tự tạo thêm file log, tài liệu hay báo cáo kiểm thử trong dự án nếu người dùng không yêu cầu. Chỉ báo kết quả kiểm tra ngắn gọn trong chat.
- Nếu gặp khó khăn khi dọn dẹp, thông báo rõ tên tệp còn sót thay vì xóa bừa.

## Giữ nguyên code hiện có

- Chỉ sửa các tệp cần thiết cho yêu cầu hiện tại; ưu tiên tái sử dụng hàm, lớp, tiện ích và mô hình sẵn có.
- Không tự ý xóa hay thay đổi logic nghiệp vụ ngoài phạm vi yêu cầu.
