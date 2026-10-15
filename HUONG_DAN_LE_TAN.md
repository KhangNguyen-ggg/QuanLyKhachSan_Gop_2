# Lễ tân – Lưu trú

## Chạy project

1. Mở `QuanLyKhachSan_Demo/QuanLyKhachSan.sln` bằng Visual Studio có .NET 8 và WPF.
2. Trong SSMS, chọn database theo schema `Database_quanlykhachsan.sql` rồi chạy `letanvaluutru.sql`. File cập nhật cấu trúc giấy tờ rồi tạo/cập nhật 9 proc đọc nhiều bảng. Nếu đã có database và dữ liệu thì không cần chạy lại script tạo database/dữ liệu mẫu.
3. Sửa `QuanLyKhachSan_Demo/connection.txt` theo SQL Server của bạn. File được chép cạnh chương trình khi build; biến môi trường `QLKS_CONNECTION` có thể ghi đè cấu hình này.
4. Restore NuGet, Rebuild Solution và chạy project `QuanLyKhachSan`.

## Tổ chức chức năng

View gọi BUS, BUS gọi DAL của từng tab. DAL dùng nguyên 5 tiện ích có sẵn; không có DAL/BUS Lễ tân chung.

| Tab / chức năng | BUS | DAL |
|---|---|---|
| Đặt phòng: thêm, sửa, xác nhận, hủy, chi tiết | BUS_DatPhong | DAL_Datphong |
| Phân bổ phòng | Bus_PhanBoPhong | DAL_PhanBoPhong |
| Khách lưu trú: nhận, trả, chuyển phòng, sửa ghi chú | BUS_KhachTheoPhong | DAL_KhachTheoPhong |
| Lịch sử lưu trú | BUS_LichSuXepPhong | DAL_LichSuXepPhong |
| Khách hàng | BUS_KhachHang | DAL_Khachhang |
| Hóa đơn, ưu đãi, thu tiền | BUS_HoaDon | DAL_HoaDon |
| Lịch sử thu tiền | BUS_LichSuThuTien | DAL_LichSuThuTien |

- Đã bỏ nút Nhận phòng và Trả phòng khỏi tab Đặt phòng. Thực hiện ở Khách lưu trú.
- Đã bỏ cả 7 nút Chi tiết trong nhóm Lễ tân – Lưu trú. Vẫn có thể nhấp đúp vào dòng để xem thông tin.
- CBB lấy danh mục bằng `HienThiTatCa<T>()`, hiển thị tên/số phòng cần thiết bằng `DisplayMemberPath` và lấy mã bằng `SelectedValuePath` hoặc đối tượng đã chọn. Phòng được lọc theo loại đã chọn từ bảng Phong.
- Xem một bản ghi dùng `HienThiTheoId<T>()`. Ghi dữ liệu dùng `ThemKhongDieuKien`, `SuaTheoId`, `XoaTheoId`.
- Các bảng tổng hợp, chi tiết đặt phòng có tên loại và khoản thu dùng proc trong `letanvaluutru.sql`, gọi qua `ThuThiLenhCoDieuKien.TruyVan`. Không dùng proc thêm/sửa/xóa.
- Ràng buộc dữ liệu nằm trong BUS của từng chức năng. BUS_KiemTra chứa các kiểm tra dùng lại: chuỗi, số, tiền, điện thoại, Gmail, ngày và lựa chọn bắt buộc. View gọi BUS để đọc số và chỉ hiển thị thông báo; 5 tiện ích giữ nguyên; loại giấy tờ được kiểm tra tại BUS.
- Phần bắt lỗi vốn có trong 5 tiện ích vẫn giữ nguyên. DAL trả kết quả bool của tiện ích; form đóng sau khi lưu thành công. Các thao tác ghi nhiều bản ghi dùng TransactionScope để lưu cùng giao dịch.

## Luồng sử dụng cơ bản

1. Thêm khách hàng.
2. Đặt phòng: chọn khách và loại phòng bằng CBB; nhập ngày, số lượng, số khách, đơn giá; thêm dòng vào danh sách rồi lưu và xác nhận.
3. Phân bổ: chọn đơn, chi tiết đặt phòng và phòng cụ thể rồi lưu.
4. Khách lưu trú: nhận phòng từ lượt đã phân bổ; có thể sửa ghi chú hoặc chuyển phòng. Trả phòng tại đây; lịch sử giữ lượt cũ, phòng đã trả được đánh dấu cần dọn.
5. Tạo hóa đơn: chọn đơn, nhân viên và ưu đãi nếu có. Tiền phòng tính theo số phòng × số đêm dự kiến × đơn giá; dịch vụ tính các dòng đã sử dụng. Thu tiền sẽ thêm phiếu thu và cập nhật trạng thái hóa đơn.

Khách lưu trú hiển thị người đại diện đặt phòng theo schema hiện có. Các ràng buộc của database gốc vẫn áp dụng. Khi bấm Thêm/Lưu, dữ liệu nhập sai được BUS từ chối và form hiện thông báo; form không đóng và chưa gọi DAL để ghi dữ liệu.

Xem `KET_QUA_KIEM_TRA.md` về phạm vi đã kiểm tra. Có thể chạy kiểm tra ràng buộc và tính tiền không cần SQL Server bằng `dotnet run --project KiemTraLeTan/KiemTraLeTan.csproj`.

## Bản sửa hủy đặt phòng

- Hủy đơn chưa nhận phòng sẽ xóa các dòng XepPhong của đơn bằng tiện ích Xoa rồi đổi DatPhong thành DaHuy trong cùng giao dịch. Không xóa chi tiết đặt phòng.
- Đơn đã có lượt nhận/trả phòng không bị xóa lịch sử bằng thao tác hủy đặt phòng; xử lý lượt ở tab Khách lưu trú.
- CBB phân bổ lấy các đơn chưa hủy bằng HienThiTatCa rồi lọc trạng thái. CBB lập hóa đơn dùng sp_DatPhongChuaLapHoaDon để đối chiếu DatPhong với HoaDon, chỉ lấy đơn chưa hủy và chưa có hóa đơn. DAL kiểm tra lại khi lưu để form mở từ trước cũng không tạo trùng hoặc xử lý đơn đã hủy.
- Không thu thêm tiền cho hóa đơn thuộc đơn đã hủy. Các hóa đơn/phiếu thu đã tồn tại không tự bị xóa.
- Phân bổ cũ mang trạng thái Huy hoặc thuộc đơn DaHuy không còn hiện trong tab Phân bổ phòng. Chọn lại đơn đã hủy và bấm Hủy đặt phòng sẽ xóa các phân bổ cũ nếu chưa có lịch sử nhận/trả phòng.

**Bản này cần chạy lại toàn bộ letanvaluutru.sql trong SSMS trên đúng database ứng dụng đang kết nối.** Script bỏ unique loại–số giấy tờ, đổi LoaiGiayTo sang nvarchar(20) và cập nhật proc lưu trú trả về loại và mã số giấy tờ.

## Ràng buộc dữ liệu tại BUS

| Phần | Quy tắc đang áp dụng |
|---|---|
| Khách hàng | Họ tên, loại giấy tờ, điện thoại và email bắt buộc. Họ tên không chứa số; cho phép chữ Unicode, khoảng trắng, gạch nối và dấu nháy. |
| Giấy tờ | Chọn CBB Hộ chiếu, CCCD, CMND hoặc Khác. Số giấy tờ là mã số trên giấy tờ, được kiểm tra định dạng theo loại; giữ nguyên số 0 ở đầu. |
| Điện thoại | 10–15 chữ số; giữ số 0 đầu, không có chữ, dấu cách hoặc dấu cộng. |
| Email | Bắt buộc địa chỉ dạng ten@gmail.com, kiểm tra đúng tên miền gmail.com; không nhận các tên miền khác hoặc địa chỉ sai cấu trúc. Đây là kiểm tra định dạng, không kiểm tra hộp thư có tồn tại. |
| Thông tin khác | Ngày sinh không ở tương lai; giới tính hợp lệ; độ dài theo cột SQL: họ tên 150, loại giấy tờ 20, quốc tịch 80, địa chỉ 300, ghi chú khách 500. |
| Số nguyên | Không chấp nhận chữ, số âm, số lẻ, số dạng khoa học hoặc số vượt int. Số phòng và người lớn > 0; trẻ em >= 0. |
| Tiền | Không âm, tối đa 16 chữ số nguyên và 2 số lẻ. Nhập 500000 hoặc 500000.50 / 500000,50; không gõ dấu tách hàng nghìn. |
| Đặt phòng | Phải chọn khách, có ít nhất một chi tiết, chọn loại phòng; ngày trả sau ngày nhận ít nhất một ngày; đơn mới không nhận ngày quá khứ; ghi chú đơn tối đa 1000 ký tự. |
| Phân bổ/lưu trú | Phải chọn mã chi tiết, phòng và lượt lưu trú cần xử lý. Phòng chuyển đến phải khác phòng cũ. Ghi chú tối đa 500 ký tự. |
| Hóa đơn/thu tiền | Chọn đơn/hóa đơn và nhân viên; ưu đãi có thể để trống. Tiền thu > 0 và không vượt phần còn thiếu; không thu hóa đơn đã hủy. Ghi chú tối đa 500 ký tự. |

Kiểm tra dùng chung ở `BUS_QuanlyKhachSan/BUS_KiemTra.cs`; các quy tắc khách hàng, đặt phòng, phân bổ, lưu trú và hóa đơn nằm trong BUS tương ứng. Các hàm Luu/Tao/Thu gọi kiểm tra trước DAL; form gọi thêm kiểm tra chi tiết khi thêm dòng đặt phòng. Không có kiểm tra gọi SQL cho việc đọc số hoặc kiểm tra email.

Bản này bổ sung ràng buộc đầu vào, chưa bổ sung kiểm tra tồn phòng theo lịch, sức chứa từng loại hoặc hiệu lực ưu đãi. Các quy tắc hủy đơn của bản trước vẫn giữ nguyên.

## Loại giấy tờ và mã số giấy tờ

- Loại giấy tờ dùng CBB không cho nhập tự do. Số giấy tờ là mã số ghi trên giấy tờ, nhập bằng ô văn bản; thêm khách mới để trống, không mặc định 1.
- BUS_KhachHang.KiemTraSoGiayTo kiểm tra khi thêm/sửa. Đổi loại rồi lưu sẽ kiểm tra lại số theo loại vừa chọn. Form hiện hướng dẫn tương ứng và giữ nội dung người dùng đã nhập.

| Loại | Quy tắc nhập trong ứng dụng |
|---|---|
| CCCD | Đúng 12 chữ số ASCII 0–9. |
| CMND | Đúng 9 hoặc 12 chữ số ASCII 0–9. |
| Hộ chiếu | Kiểm tra cơ bản 6–9 ký tự chữ Latin không dấu hoặc số; không có khoảng trắng hay ký tự đặc biệt. Quy tắc nhập chung để hỗ trợ khách nước ngoài, không xác minh định dạng theo từng quốc gia. |
| Khác | 1–50 ký tự; bắt đầu bằng chữ Latin không dấu hoặc số, phần còn lại cho phép thêm dấu - và /. |

- Mọi loại đều bắt buộc nhập. Cắt khoảng trắng hai đầu, giữ nguyên chữ hoa/thường và số 0 đầu; không chuyển mã sang int hoặc số lượng. Không tự loại ký tự sai ở giữa.
- Quy tắc CCCD/CMND đối chiếu định dạng số từ Bộ Công an: https://bocongan.gov.vn/chinh-sach-phap-luat/chi-tiet-cau-hoi/f1feb6e4-403e-4a19-81dc-0273aa41224f và https://bocongan.gov.vn/hoi-dap/chi-tiet-cau-hoi/91aee2e8-fb7a-4e61-831e-e2944e07ccc9%3Fpage%3D/ . Đây là kiểm tra định dạng nhập, không xác minh giấy tờ có thật, còn hiệu lực hoặc thuộc về người nhập.
- ET và database giữ SoGiayTo kiểu chuỗi/varchar(50). DAL thêm/sửa cả LoaiGiayTo và SoGiayTo bằng 5 tiện ích gốc. Bản này không thêm quy tắc kiểm tra trùng giấy tờ.
- Khách hàng, Khách lưu trú và màn hình xem thông tin đều hiển thị nhãn Số giấy tờ. Proc lưu trú trả cột SoGiayTo, không còn tên SoLuongGiayTo.
- Chạy lại toàn bộ letanvaluutru.sql trên database đang dùng trước khi chạy ứng dụng để cập nhật proc. Không chạy lại script tạo database nếu đã có dữ liệu.
- Các giá trị cũ từng nhập dạng số lượng (1, 2...) không được tự xóa hoặc đoán thành mã giấy tờ. Mở khách tương ứng và nhập lại mã số thực tế trước khi lưu.

## Hóa đơn và thu tiền gộp

- Mỗi đặt phòng chỉ được lập hóa đơn một lần. Đơn đã có hóa đơn sẽ không xuất hiện trong CBB tạo hóa đơn; BUS và DAL kiểm tra lại khi lưu. Kể cả hóa đơn đã hủy vẫn được tính là đã lập. Không tự xóa các hóa đơn trùng từng có trong dữ liệu cũ.
- Proc mới sp_DatPhongChuaLapHoaDon nằm trong letanvaluutru.sql vì truy vấn đối chiếu hai bảng. Chạy lại toàn bộ file này trên database đang sử dụng trước khi mở ứng dụng.
- Thu tiền gộp phòng và dịch vụ trong một hóa đơn. Tổng phải thu = tiền phòng + dịch vụ/gói dịch vụ đã sử dụng − ưu đãi. Số còn thiếu = tổng phải thu − các phiếu thu thành công.
- sp_KhoanThuDatPhong lấy cả tiền phòng và các dòng SuDungDichVu của đúng đơn có TrangThai = DaSuDung; không thu dịch vụ chỉ đặt trước hoặc đã hủy. Tổng được làm tròn từng dòng theo cột tiền của SQL.
- Cửa sổ Thu tiền hiện tổng phải thu, đã thu và còn thiếu chung. Có thể thu một phần hoặc toàn bộ; không chọn riêng loại tiền. Thu đủ sẽ chuyển hóa đơn sang DaThanhToan và không thu thêm được.
- BUS kiểm tra số tiền; DAL đọc lại số đã thu trong giao dịch và từ chối số tiền vượt số còn thiếu, bằng 0, âm hoặc quá 2 số lẻ. Hóa đơn/đơn đặt phòng đã hủy không được thu.
- Chi tiết hóa đơn được chốt khi tạo. Hãy ghi nhận đầy đủ dịch vụ đã sử dụng trước khi lập hóa đơn. Dịch vụ thêm sau đó không tự được cộng vào hóa đơn cũ; bản sửa này không tự sửa các hóa đơn đã lưu.
