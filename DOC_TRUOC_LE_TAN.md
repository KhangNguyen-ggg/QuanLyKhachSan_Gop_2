# Bản sửa Lễ tân trên QuanLyKhachSan(6)

## Chạy bản mới

1. Trong SSMS, chọn đúng database đang dùng và chạy `CapNhat_LeTan_HoaDon.sql`. Script chỉ tạo/cập nhật proc; không tạo lại database, không xóa dữ liệu và không sửa hóa đơn cũ.
2. Mở `QuanLyKhachSan_Demo/QuanLyKhachSan.sln`, chọn Rebuild Solution rồi chạy.
3. Kiểm tra `connection.txt` đang trỏ tới đúng database nếu máy bạn có tên SQL Server khác.

Cần chạy script vì cột truy vấn Đặt phòng đã đổi từ `KhachHang`, `NgayNhan`, `NgayTra` thành `HoTen`, `NgayNhanDuKien`, `NgayTraDuKien` để khớp XAML trực tiếp.

## Cách viết đã đổi

Áp dụng cho nhóm Lễ tân: Đặt phòng, Phân bổ phòng, Khách hàng, Khách lưu trú, Lịch sử lưu trú, Hóa đơn và Lịch sử thu tiền, cùng các form con.

- Các nút có hàm sự kiện bình thường, từng câu lệnh trong `{ }`, giống cách bạn đang viết ở Hệ thống.
- Mở form trực tiếp bằng `new ...`, `Owner`, `ShowDialog()` ở toàn bộ dự án; hai file helper mở form cũ đã được xóa.
- Không tạo class `DongDat`, không thêm ViewModel, không cấu hình Binding bằng C#.
- Binding các cột, ô hiển thị, SelectedValuePath và mẫu dòng nằm trực tiếp trong XAML.
- Các bảng truy vấn nhiều bảng trả DataTable từ DAL và DataView từ BUS. View gán `ItemsSource` trực tiếp, không còn `.Select(x => new { ... })` để tạo lại cột.
- Kiểm tra số, ngày, lựa chọn bắt buộc, dữ liệu cần lưu và kết quả DAL nằm ở BUS. View chỉ gọi BUS, đóng form sau khi lưu thành công và hiển thị thông báo khi BUS báo lỗi. `try/catch` trong View chỉ để hiện thông báo, không chứa quy tắc kiểm tra dữ liệu.
- Các form Hệ thống, Phòng, Dịch vụ, Kho, Nhân sự cũng được chuyển sang mở trực tiếp. Form nhận bản nháp ET qua constructor; nguồn ComboBox và dữ liệu bổ sung được chỉ rõ bằng binding XAML tới cửa sổ. Không dùng reflection, JSON hoặc quét control để cấu hình binding.

Binding thuộc XAML; SQL chỉ trả cột dữ liệu tương ứng. Ví dụ:

```sql
SELECT dp.MaDatPhong, kh.HoTen AS HoTen
FROM DatPhong dp
JOIN KhachHang kh ON dp.MaKhachHangDat = kh.MaKhachHang;
```

```xml
<DataGrid x:Name="dgvDatPhong" SelectedValuePath="MaDatPhong">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Khách đặt" Binding="{Binding HoTen}"/>
    </DataGrid.Columns>
</DataGrid>
```

```csharp
// View chỉ gọi BUS để lấy bảng rồi hiển thị.
dgvDatPhong.ItemsSource = bus.hienThiDPAll();
```

Các dòng đặt phòng/dịch vụ đang thêm nhưng chưa lưu chưa có trong SQL. Chúng được giữ tạm trong List của ET; BUS chuẩn bị dữ liệu xem trước, XAML hiển thị. Không ghi dữ liệu nháp vào database khi chỉ chọn dịch vụ hay khi đóng form.

## Hóa đơn

Vào Hóa đơn → Tạo hóa đơn → chọn đơn đặt phòng và nhân viên.

Bảng đầu hiển thị tiền phòng và các lượt dịch vụ/gói đã ghi nhận `DaSuDung`. Dịch vụ `DatTruoc` hoặc `Huy` không được tính. Chọn thêm dịch vụ hoặc gói và số lượng ở phía dưới; có thể thêm nhiều dòng. Chọn dòng mới rồi Bỏ dòng vừa thêm nếu muốn đổi lựa chọn/số lượng.

Tổng tiền = tiền phòng + tiền dịch vụ + tiền gói dịch vụ. Phải thu = tổng tiền − ưu đãi. Gói chỉ tính giá gói; không tự cộng lại giá từng dịch vụ thành phần. Chỉ thêm lượt phát sinh mới, tránh nhập lại lượt đã có ở bảng đầu.

Các lượt mới và hóa đơn được lưu trong cùng giao dịch. Đóng form không lưu nháp; đổi đơn đặt phòng xóa danh sách đang thêm để không gán sang khách khác. Chi tiết, in hóa đơn và thu tiền sử dụng tổng đã lưu. Bản sửa áp dụng cho lập hóa đơn mới; không tự tính lại hóa đơn đã lập.

## Kiểm tra và giới hạn

Đã kiểm tra tĩnh: 21 file XAML, tên control và event, resource được dùng, tên/số tham số các lời gọi BUS, các cột SQL/Binding và phạm vi thay đổi. Phần Vai trò được sửa thêm để chọn đúng dòng, sửa không chèn thêm bản ghi, kiểm tra tại BUS và tải lại danh sách sau khi đóng form.

Chưa build/chạy WPF hoặc kết nối SQL Server trong môi trường chỉnh sửa do không có .NET SDK/SQL Server. Kiểm tra tĩnh không thay thế việc build và chạy trên Windows.

Có kiểm thử BUS không cần database, chạy trên máy có .NET 8:

```powershell
dotnet run --project KiemTraHoaDon/KiemTraHoaDon.csproj
```

Các ca kiểm thử bao gồm: nhiều dịch vụ/gói, cộng cả các khoản cũ và mới, làm tròn từng dòng, bỏ dòng, số lượng/đơn giá sai, lựa chọn trống, ngày và số nhập sai, DAL báo lưu thất bại, lọc bảng có ngày NULL.

Khi chạy ứng dụng, kiểm tra thêm: tạo/sửa đặt phòng; thêm/bỏ chi tiết; chọn phân bổ; tìm kiếm/lọc trạng thái; xem chi tiết; nhận/trả/chuyển phòng; thêm dịch vụ/gói và đóng form hoặc đổi đơn; lưu hóa đơn rồi xem lại chi tiết/in/thu tiền. Ví dụ phòng 1.000.000 + dịch vụ 170.000 + gói 750.000 phải có tổng 1.920.000; giảm 10% còn 1.728.000.

## Bổ sung ngày 14/10

Đã bỏ toàn bộ lời gọi tới hai helper cũ và xóa hai file đó khỏi mã nguồn. Không tạo helper khác để thay tên. Các chức năng CRUD ngoài Lễ tân vốn chưa được triển khai vẫn giữ nguyên phạm vi; lần này chỉ đổi cách mở form, truyền dữ liệu và cấp nguồn binding. Khi tải bản mới, giải nén vào thư mục mới rồi Rebuild Solution để không còn các file cũ do chép đè.
