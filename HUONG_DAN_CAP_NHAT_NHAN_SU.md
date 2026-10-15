# Nâng cấp DB cho Nhân sự

Mở **CapNhat_NhanSu.sql** trong SQL Server Management Studio và nhấn **Execute** toàn bộ file. Script mặc định dùng `QuanLyKhachSan5Sao2`; nếu DB có tên khác, sửa dòng `USE` ở đầu file.

Không chạy lại file tạo database hoặc nhập lại dữ liệu mẫu. Script nâng cấp trực tiếp các bảng hiện có, có thể chạy lại và không xóa dữ liệu nhân viên, lịch, chấm công hay lương.

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

Kiểm thử tự động bằng DB tạm riêng:

```powershell
powershell -ExecutionPolicy Bypass -File checks/NhanSuDatabaseMigration.ps1
```

Bộ kiểm tra dựng cấu trúc Nhân sự gốc từ file SQL trong dự án, kiểm tra giữ nguyên dữ liệu, rollback khi lỗi, chạy lại nhiều lần, sinh mã, CRUD, khóa ngoại và các điều kiện nhập liệu; sau đó xóa đúng DB tạm do nó tạo. Tài khoản chạy kiểm thử cần quyền tạo/xóa DB tạm.

Script này chỉ chuẩn bị DB. Các tab còn dùng dữ liệu mẫu trong `NhanSuView` vẫn cần được nối DAL → BUS → binding; chạy SQL không tự thay đổi các handler C#.
