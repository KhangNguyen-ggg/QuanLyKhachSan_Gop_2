# Nâng cấp DB cho Nhân sự

Nếu muốn chạy **một file DB tổng duy nhất**, dùng **Database_QuanLyKhachSan_Tong.sql**: mở trong SSMS và Execute toàn bộ. File tự tạo DB/bảng và nạp mẫu khi khởi tạo mới; với DB đã có đủ bảng, file giữ dữ liệu và chỉ cập nhật cấu trúc/thủ tục cần thiết. Bản tổng đã gộp phần Nhân sự bên dưới, Lễ tân và Hệ thống; không cần chạy thêm các file SQL riêng.

Mở **CapNhat_NhanSu.sql** trong SQL Server Management Studio và nhấn **Execute** toàn bộ file. Script mặc định dùng `QuanLyKhachSan5Sao2`; nếu DB có tên khác, sửa dòng `USE` ở đầu file.

`CapNhat_NhanSu.sql` vẫn dùng được nếu chỉ muốn cập nhật phần Nhân sự. Không chạy lại **Database_quanlykhachsan.sql** gốc vì file đó có bước xóa DB cũ. Không cần nhập lại dữ liệu mẫu khi nâng cấp DB đang có dữ liệu.

## Những gì được bổ sung

- `KhenThuong`: mã tự tăng, nhân viên, ngày khen thưởng, lý do, hình thức, số tiền, trạng thái duyệt, ghi chú.
- `LichLamViec.MaLich`: giữ nguyên khóa cũ; khóa mới được cấp bằng `DEFAULT` và `Seq_LichLamViec`. Không phải dựng lại bảng để đổi sang `IDENTITY`.
- Bỏ khóa ngoại ngược từ `LichLamViec.MaChamCong` sang `ChamCong`. Cột cũ được giữ để tương thích ET/dữ liệu, không còn là liên kết nghiệp vụ.
- Thêm khóa ngoại từ chấm công đến lịch, kiểm tra đồng thời mã lịch, nhân viên và ngày. Mỗi lịch vẫn chỉ có một bản chấm công theo ràng buộc sẵn có.
- Chặn giờ ra trước giờ vào; ca qua đêm được lưu bằng ngày/giờ thực tế của hai ngày.
- `usp_ThemLichLamViec`: thêm lịch trước khi có chấm công và trả về `MaLich`.
- `usp_HienThiBangLuong`: bổ sung đúng thủ tục mà `DAL_BangLuong.HienThiBangLuongAll()` hiện đang gọi. Có thể lọc theo nhân viên, tháng, năm.

Không có trigger tự cộng tiền thưởng. `KhenThuong.SoTien` là chi tiết; `BangLuong.TienThuong` là tổng thưởng của kỳ lương, cần được chốt trong nghiệp vụ lập lương để tránh cộng hai lần. Các khoản lương cũ giữ nguyên.

## Quy ước khi nối DAL/BUS/giao diện

Để thêm lịch, gọi `dbo.usp_ThemLichLamViec` với `@MaNhanVien`, `@MaCa`, `@NgayLam`, `@TrangThai`, `@GhiChu`. Kết quả là một dòng có cột `MaLich`. Có thể dùng `ThuThiLenhCoDieuKien.TruyVan<long>()` để nhận khóa và gán vào ET.

Nếu viết INSERT trực tiếp, bỏ cột `MaLich` để DEFAULT sinh mã và dùng `OUTPUT INSERTED.MaLich` nhận kết quả. `MaLich` dùng SEQUENCE, không lấy bằng `SCOPE_IDENTITY()` hoặc trông chờ tiện ích Dapper.Contrib tự gán khóa này. ET hiện dùng `[ExplicitKey]`, nên vẫn phải gán mã nếu tiếp tục thêm qua tiện ích generic cũ.

Để lấy mã chấm công thực tế, JOIN `ChamCong` theo `MaLich`; không đọc cột `LichLamViec.MaChamCong` cũ làm liên kết. Mã tự sinh có thể có khoảng trống, không cần liên tục.

| Trường | Giá trị lưu trong SQL |
|---|---|
| `LichLamViec.TrangThai` | `DaXep`, `Nghi`, `Huy` |
| `ChamCong.TrangThai` | `DuCong`, `ThieuCong`, `Nghi`, `NghiPhep`, `KhongCham` |
| `BangLuong.TrangThai` | `TamTinh`, `DaDuyet`, `DaThanhToan` |
| `KhenThuong.TrangThai` | `ChoDuyet`, `DaDuyet`, `TuChoi` |
| `KhenThuong.HinhThuc` | `TienThuong`, `BieuDuong`, `GiayKhen` |

Tên hiển thị tiếng Việt cần ánh xạ về các mã trên khi lưu. Các bảng Phòng ban, Chức vụ, Nhân viên và Ca làm việc tiếp tục dùng cấu trúc hiện có; cột số tiền trong bảng `ChucVu` hiện vẫn tên `Chucvu`.

## Kiểm tra và giới hạn

Script kiểm tra chấm công cũ phải có lịch tương ứng, khớp nhân viên/ngày và có thứ tự giờ hợp lệ. Nếu phát hiện sai lệch hoặc gặp lỗi nâng cấp, script báo lỗi và rollback các thay đổi cấu trúc; không tự sửa hay xóa các dòng đang có.

Hai file `ThemDuLieu_QuanLyKhachSan*.sql` là dữ liệu mẫu cho cấu trúc cũ. Khi thiết lập DB mới từ đầu, nhập dữ liệu mẫu trước rồi chạy nâng cấp này; không dùng chúng để tạo thêm lịch/chấm công sau nâng cấp vì thứ tự chèn cũ là chấm công trước, lịch sau.

Giao diện Nhân viên và năm tab trong `NhanSuView` đã nối SQL qua `DAL_NhanSu` → `BUS_NhanSu`. Phòng ban/Chức vụ giữ BUS riêng; các ô tìm kiếm dùng chung `NhanSuGiaoDien`. Các mã trạng thái, tìm kiếm không dấu và nhập giờ dùng `NhanSuQuyUoc`.

Form Nhân viên/Ca/Lịch/Chấm công/Lương/Khen thưởng dùng lại cho thêm và sửa. Từ cửa sổ chi tiết, bấm **Sửa** để mở form có bản nháp; đóng form không lưu sẽ giữ dữ liệu cũ. Những chỗ thêm/bớt/khác Phòng ban có comment ngay trong C# và XAML.

Lập lương vẫn nhập tiền công, tăng ca, phụ cấp, khấu trừ và thuế theo kỳ; chưa có công thức quy đổi công thành tiền tự động. Nút **Nạp lương cơ bản và thưởng đã duyệt** lấy lương tham chiếu và gán tổng thưởng tiền mặt đã duyệt trong tháng/năm đang chọn, không cộng dồn khi bấm lại. Tổng lương = tiền công + tăng ca + thưởng + phụ cấp; thực lĩnh = tổng lương − khấu trừ − thuế. Lương cơ bản không cộng lần thứ hai. Bảng lương đã thanh toán chỉ xem, không sửa/xóa; thay đổi khen thưởng không tự sửa số tiền của kỳ đã chốt.
