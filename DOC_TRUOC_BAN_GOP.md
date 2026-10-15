# QuanLyKhachSan_Gop_2 — giữ nguyên giao diện 5Sao

Mở `QuanLyKhachSan_Demo/QuanLyKhachSan.sln`, chọn project QuanLyKhachSan làm Startup Project.

Bản này thay cho cách gộp trước: lấy nguyên giao diện và luồng thao tác của QuanLyKhachSan5Sao, nối chức năng BUS/DAL/ET từ QuanLyKhachSan_Gop.

## Phần giữ nguyên

- 131 file XAML gốc khớp từng byte với QuanLyKhachSan5Sao. App.xaml chỉ chuyển việc chọn cửa sổ khởi động sang App.xaml.cs; thêm LoginThuCongWindow riêng theo yêu cầu.
- 106 file C# giao diện khớp từng byte với 5Sao. Form đăng nhập có ComboBox, xác thực DemoSession, phân quyền demo và điều hướng nhóm tab giữ nguyên; App và luồng đăng xuất được nối vào lựa chọn form dùng chung.
- Toàn bộ 33 file có sẵn trong ba project BUS/DAL/ET của Gop giữ nguyên byte, gồm code, project và metadata. Không viết lại công thức, giao dịch hoặc thủ tục nghiệp vụ.
- File SQL gốc Gop giữ nguyên. connection.txt lấy từ bản giao diện hiện tại; không sửa chuỗi kết nối.
- Hai thư mục nguồn không bị sửa. Bản Gop_2 trước đã được sao lưu trong BackUp/QuanLyKhachSan_Gop_2_TruocKhiGiuNguyenGiaoDien_20261014_203617.

## Phần nối trong Hệ thống và Lễ tân & lưu trú

- 24 file C# chỉ đổi using sang đúng namespace BUS gốc, có comment tại dòng nối.
- TaiKhoanView lọc kết quả BUS gốc bằng bộ lọc đã có trong giao diện.
- TaiKhoanForm gọi ThemTK/SuaTK gốc; giữ xác nhận mật khẩu, định dạng PBKDF2, salt, số vòng và quy tắc để trống khi sửa đang có trong 5Sao. Không chuyển đổi mật khẩu đã lưu, không sửa code đăng nhập.
- TaiKhoanChiTietWindow kiểm tra kết quả false/thông báo lỗi từ BUS gốc trước khi đóng sau khóa/mở khóa.
- VaiTroQuyenChiTietWindow kiểm tra kết quả xóa trước khi đóng.
- Các chỗ nối/sửa đều có comment Ghép Gop hoặc Ghép Hệ thống. Không đưa nút của màn danh sách Gop vào giao diện.

Tài khoản giữ đúng cách dùng: Thêm, Tìm kiếm, Làm mới ở danh sách; double click một dòng mở cửa sổ chứa các nút thao tác. Những phần chưa có xử lý trong nguồn, như Gán vai trò riêng, Đặt lại mật khẩu riêng và Lưu phân quyền, vẫn giữ trạng thái hiện tại; không thêm nghiệp vụ mới.

## Chọn form đăng nhập

Mặc định mở `Views/LoginThuCongWindow.xaml`: tên đăng nhập và mật khẩu để trống, không ComboBox, không hiển thị tài khoản mẫu. Nhấn Enter hoặc nút Đăng nhập để dùng cơ chế `DemoSession.SignIn` hiện tại.

Form nhập tay chặn tên đăng nhập/mật khẩu trống hoặc chỉ có khoảng trắng; tên đăng nhập tối đa 50 ký tự sau khi bỏ khoảng trắng đầu/cuối, mật khẩu tối đa 1000 ký tự. Mật khẩu không bị cắt hoặc bỏ khoảng trắng. Lỗi hiển thị ngay trên form và focus về ô cần sửa; thông tin sai dùng thông báo chung, không tiết lộ tài khoản có tồn tại hay không.

Đổi một dòng trong `QuanLyKhachSan_Demo/App.xaml.cs`, sau đó build lại:

```csharp
public static bool DungDangNhapDemo = false; // nhập tay để trình bày
// Đổi false thành true để dùng form có ComboBox khi test.
```

Khởi động và đăng xuất đều gọi `App.TaoFormDangNhap()`. Đây là chuyển giao diện đăng nhập; chưa chuyển xác thực sang SQL.

## Kiểm tra

```powershell
powershell -ExecutionPolicy Bypass -File .\KIEM_TRA_GOP_2.ps1
dotnet build .\QuanLyKhachSan_Demo\QuanLyKhachSan.sln
dotnet run --project .\checks\LeTanValidation\LeTanValidation.csproj
dotnet run --project .\checks\InvoiceValidation\InvoiceValidation.csproj
dotnet run --project .\checks\LoginValidation\LoginValidation.csproj -c Release
```

Kiểm tra SQL/WPF ở checks/SqlIntegration dùng database mới tên QLKS_Gop2Check_<GUID>, tự xóa đúng database thử trong finally. Không dùng database đang có. Schema.sql chỉ dùng để tạo database thử trống; procedure cuối cùng lấy từ các script Gop gốc. Không chạy SQL tự động khi mở ứng dụng.

DOI_CHIEU_GOP_2.json và KIEM_TRA_GOP_2.ps1 là đối chiếu của bản hiện tại. DOI_CHIEU_CODE_GOC.json, ZIP trong BanGoc và các project KiemTra/KiemTraLeTan/KiemTraHoaDon ở thư mục gốc là tài liệu/code kiểm tra lịch sử của Gop, không dùng để kết luận giao diện bản này. Xem KET_QUA_KIEM_TRA.md và THAY_DOI_GIAO_DIEN.md.
