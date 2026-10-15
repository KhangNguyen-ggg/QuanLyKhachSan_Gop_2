# Binding trực tiếp theo tên thuộc tính

Mở `QuanLyKhachSan_Demo/QuanLyKhachSan.sln`, chọn project giao diện làm Startup Project. Bản này giữ nguyên ET, DAL, BUS và lệnh gọi proc Đặt phòng của project bạn gửi.

## Quy tắc binding

Mọi binding dùng tên trực tiếp: `{Binding MaDatPhong}`, `{Binding HoTen}`, `{Binding SoPhong}`, `{Binding TenLoaiPhong}`. Không còn tiền tố `DuLieu.`, `Dong.`, `Nguon.` hoặc `KhachHang.` trong XAML.

- Bảng chỉ cần một ET: nguồn danh sách là đối tượng ET trực tiếp.
- Bảng cần ghép tên danh mục: ViewModel tạo dòng có các thuộc tính trực tiếp, gồm toàn bộ thuộc tính ET chính và tên từ bảng liên quan. Không sửa cấu trúc ET hoặc database.
- Form nhập nhận trực tiếp bản nháp ET. Binding TwoWay như `{Binding SoPhong}` cập nhật bản nháp đó. Form chưa ghi database; đóng/hủy không sửa dòng trong danh sách.
- Nguồn ComboBox được bind trực tiếp trong XAML qua thuộc tính Nguon của Window, ví dụ `ItemsSource="{Binding Nguon.LoaiPhongs, RelativeSource={RelativeSource AncestorType=Window}}"`. SelectedValue vẫn bind vào ET.
- Cửa sổ chi tiết nhận dòng đang chọn và dùng đúng các tên trực tiếp của dòng.

## Đặt phòng đang gọi proc của bạn

Không cần sửa proc hiện có. Đoạn nhận kết quả trong `DatPhongView.xaml.cs` ánh xạ:

| Cột proc | Thuộc tính dùng để binding |
|---|---|
| MaDatPhong | MaDatPhong |
| KhachHang | HoTen |
| NgayNhan | NgayNhanDuKien |
| NgayTra | NgayTraDuKien |
| SoPhong | SoPhong |
| SoKhach | SoKhach |
| TrangThai | TrangThai |

Các tên tổng hợp như `SoKhach`, `DaThu`, `ConThieu` không phải thuộc tính của ET chính; giữ tên trực tiếp để hiển thị dữ liệu ghép/tổng hợp. Khi tự viết proc mới, đặt alias theo tên binding hoặc ánh xạ ở đoạn nhận kết quả như ví dụ Đặt phòng.

Proc Đặt phòng hiện chỉ cấp bảy cột cho danh sách. Muốn form sửa điền đủ khách đặt, nhân viên tạo, ngày đặt, ghi chú và chi tiết, bạn cần bổ sung nguồn cho `MaKhachHangDat`, `MaNhanVienTao`, `NgayDat`, `GhiChu`, `ChiTiets` khi làm chức năng. Bản sửa binding không tự truy vấn thêm hoặc suy ra mã khách hàng từ họ tên. Các danh mục ComboBox vẫn cần được bạn cấp dữ liệu vào `DuLieuKhachSan`.

## Kiểm tra

Đã kiểm tra tĩnh 126 file XAML, 1.000 đường dẫn binding/lookup, tên thuộc tính ET, handler sự kiện và đối chiếu DAL/BUS không bị sửa. Không còn đường dẫn binding có dấu chấm.

Chương trình kiểm tra C# đã cập nhật theo binding trực tiếp, chỉ dùng dữ liệu trong bộ nhớ:

```powershell
dotnet run --project .\KiemTra\KiemTra.csproj -- .
```

Môi trường thực hiện không có .NET SDK nên chưa biên dịch/chạy lại chương trình kiểm tra C# hoặc chạy WPF trên Windows. Phần SQL Server hiện có vẫn dùng cấu hình kết nối của bạn trong DAL.
