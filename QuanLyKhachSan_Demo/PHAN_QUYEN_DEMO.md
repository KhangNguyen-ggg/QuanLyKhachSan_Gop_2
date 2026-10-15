# Phân quyền demo 5 vai trò

Chạy ứng dụng, chọn tài khoản trên màn hình đăng nhập rồi nhấn Đăng nhập.
Tên đăng nhập và mật khẩu được điền sẵn để dễ trình bày. Đăng xuất để thử vai trò khác.

| Vai trò | Tài khoản | Mật khẩu | Phạm vi |
|---|---|---|---|
| Quản trị hệ thống | admin | admin123 | Toàn bộ màn hình và thao tác |
| Quản lý khách sạn | quanly | demo123 | Toàn bộ nghiệp vụ, tổng quan, báo cáo, nhật ký; không vào tài khoản/phân quyền |
| Lễ tân | letan | demo123 | Đặt phòng, phân bổ phòng, tư vấn AI, lưu trú, khách hàng, ghi nhận sử dụng dịch vụ; xem phòng, gói và ưu đãi |
| Thu ngân | thungan | demo123 | Lập hóa đơn, thu tiền, áp dụng ưu đãi, xem bản in; xem khách hàng và gói/ưu đãi; không hủy hóa đơn |
| Nhân viên buồng phòng | buongphong | demo123 | Xem phòng, công việc dọn, bảo trì, thiết bị; bắt đầu/hoàn thành dọn và tạo yêu cầu bảo trì |

Menu và tab ngoài phạm vi được ẩn. Nút ngoài quyền thao tác bị vô hiệu hóa, có chú thích khi rê chuột.
Tổng quan có doanh thu nên chỉ Admin và Quản lý được xem. Lễ tân không được vào thu tiền.
Buồng phòng không được phân công nhân viên, sửa danh mục phòng hay thay đổi hồ sơ thiết bị.

Các vai trò và tài khoản demo được định nghĩa trong `DemoSession.cs`, sử dụng trong bộ nhớ.
Đây là phân quyền giao diện của bản demo; các nút nghiệp vụ và màn hình chỉnh sửa quyền trước đó vẫn là mô phỏng.
Chưa kết nối SQL Server. Khi triển khai dữ liệu thật, cần kiểm tra quyền ở lớp xử lý nghiệp vụ.
