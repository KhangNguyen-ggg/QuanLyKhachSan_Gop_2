# Đối chiếu giao diện bản đã sửa

131 file XAML gốc giữ nguyên từng byte từ QuanLyKhachSan5Sao/QuanLyKhachSan_Demo_0.3. App.xaml chỉ bỏ StartupUri để App chọn form đăng nhập; form gốc có ComboBox giữ nguyên. Theo yêu cầu mới, thêm LoginThuCongWindow riêng. Không khôi phục các thanh thao tác của Gop.

Giữ nguyên 106 file C# giao diện; 28 file trong Hệ thống/Lễ tân & lưu trú có thay đổi nối nghiệp vụ như sau:

- 24 file chỉ nối namespace BUS_QuanlyKhachSan.LeTanValuuTru, có comment.
- Views/TaiKhoanView.xaml.cs: dùng hienThiTKAll() gốc và giữ bộ lọc của giao diện.
- Views/Dialogs/TaiKhoan/TaiKhoanForm.xaml.cs: nối lệnh lưu với ThemTK/SuaTK; giữ cách xác nhận/băm mật khẩu hiện có của 5Sao, giữ hash khi sửa để trống mật khẩu.
- Views/Dialogs/TaiKhoan/TaiKhoanChiTietWindow.xaml.cs: kiểm tra kết quả khóa/mở khóa từ BUS gốc, không báo thành công khi BUS trả lỗi.
- Views/Dialogs/TaiKhoan/VaiTroQuyenChiTietWindow.xaml.cs: kiểm tra kết quả xóa từ BUS gốc.

Code BUS/DAL/ET của Gop giữ nguyên. BUS_BangLuong, DAL/NhanSu và cấu hình giao diện được mang nguyên từ 5Sao để giữ các phần giao diện còn lại; không sửa chúng.

Đăng nhập nhập tay bổ sung: LoginThuCongWindow.xaml/.cs giữ style chung, gọi DemoSession.SignIn như form cũ; App.xaml.cs chọn form bằng DungDangNhapDemo, MainWindow chỉ gọi cùng lựa chọn khi đăng xuất. Không thay cơ chế xác thực hoặc quyền truy cập.

DOI_CHIEU_GOP_2.json lưu SHA-256 nguồn và đích cho từng file. KIEM_TRA_GOP_2.ps1 kiểm tra cả số file XAML/C#, mã băm và comment. Danh sách file có kind=ui-binding trong JSON là toàn bộ các chỗ nối/sửa của giao diện.
