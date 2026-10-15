# Kết quả kiểm tra — bản giữ nguyên giao diện

Ngày 14/10/2026, Windows, .NET SDK 9.0.203, target .NET 8.

## Đã đạt

- Build solution WPF thành công, exit 0. Các cảnh báo CS0168 trong C# Gop được giữ nguyên theo yêu cầu.
- Sau khi bổ sung form đăng nhập: 131 file XAML gốc khớp SHA-256 từng byte với 5Sao. App.xaml chuyển việc chọn cửa sổ khởi động sang App; thêm LoginThuCongWindow riêng. Tổng 133 XAML và 137 C# giao diện đã đối chiếu. 106 file C# gốc giữ nguyên; 28 file Hệ thống/Lễ tân có comment nối/sửa; App và luồng đăng xuất dùng lựa chọn form chung.
- 33 file nguồn/project/metadata gốc thuộc BUS/DAL/ET Gop khớp từng byte. Các file SQL gốc cũng giữ nguyên.
- LeTanValidation: 109 trường hợp kiểm tra đầu vào và tính tiền đạt, không dùng SQL.
- InvoiceValidation: phép tính tiền phòng + nhiều dịch vụ/gói, bỏ dòng, làm tròn và đầu vào đạt, không dùng SQL.
- SqlIntegration chạy thật trên SQL Server Express trong database thử riêng: đặt/sửa/xác nhận/phân bổ/nhận/chuyển/trả phòng; lịch sử lưu trú; hóa đơn kèm nhiều dịch vụ/gói; thu tiền; lịch sử thu; chặn thu quá tiền và hóa đơn trùng.
- Hệ thống: thêm/sửa vai trò; lưu tài khoản qua form giao diện; giữ vai trò/hash; sửa để trống mật khẩu giữ hash cũ; khóa/mở khóa; tìm kiếm/làm mới tài khoản; binding các cột SQL.
- Double click dòng tài khoản mở đúng bản ghi và đủ các nút thao tác trong cửa sổ chi tiết; không có nút sửa/khóa lọt ra màn danh sách.
- Các cửa sổ Gán vai trò, Đặt lại mật khẩu, Phân quyền và Bộ lọc nhật ký mở theo luồng hiện tại. 65 checkbox phân quyền còn đủ. Chức năng lưu chưa triển khai không ghi dữ liệu hoặc báo lưu thành công.
- Các kiểm tra quyền truy cập chặn ghi khi vai trò không được phép. Không thay code đăng nhập/phân quyền demo của 5Sao.
- LoginValidation: khởi động mặc định vào form nhập tay; không ComboBox và không điền sẵn thông tin; Enter là nút mặc định; đăng nhập sai giữ form và báo lỗi; đăng nhập đúng mở màn chính với vai trò admin; đăng xuất trở lại form nhập tay để trống. Chuyển lựa chọn sang form test: ComboBox gốc giữ đủ tài khoản, đăng nhập và đăng xuất quay lại đúng form test. Các kiểm tra chạy ẩn, không dùng SQL. Ảnh: KiemTraGop2/LoginThuCong.png.
- Validation bổ sung cho login nhập tay đã đạt 9 trường hợp: tên trống, tên chỉ có khoảng trắng/Unicode space, mật khẩu trống, mật khẩu chỉ có khoảng trắng, tên 51 ký tự, mật khẩu 1001 ký tự, giới hạn đúng 50/1000, thông tin sai, và mật khẩu có khoảng trắng đầu/cuối không bị tự sửa. Đăng nhập thành công với tên có khoảng trắng đầu/cuối và luồng đăng xuất hai chế độ vẫn đạt. Giới hạn khớp form tài khoản; không thêm quy tắc độ mạnh mật khẩu vào màn đăng nhập.
- Build Debug và Release sau khi thêm login đều thành công. Không chạy lại SQL integration vì thay đổi này chỉ mở/chọn form đăng nhập; code nghiệp vụ vẫn khớp mã băm gốc.

Database thử cuối: QLKS_Gop2Check_bb98a4a5f122428590ceeb7d29a9a336, đã được xóa bởi finally. Database thử của các lần chạy trước cũng đã được xóa. Không chạy migration hay thay dữ liệu database đang dùng. Cửa sổ thử chạy ẩn, không đóng ứng dụng hiện tại của người dùng.

Ảnh các cửa sổ kiểm tra ở KiemTraGop2. DOI_CHIEU_GOP_2.json và KIEM_TRA_GOP_2.ps1 dùng để kiểm tra lại sự nguyên vẹn.

## Chưa kiểm tra

Máy in vật lý, các nhóm ngoài Hệ thống/Lễ tân và dữ liệu thật đang vận hành của người dùng. Những nhóm ngoài phạm vi được giữ nguyên từ 5Sao. Không triển khai thêm chức năng còn trống trong hai nguồn.
