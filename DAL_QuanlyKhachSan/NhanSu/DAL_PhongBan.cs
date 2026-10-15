using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;

namespace DAL_QuanlyKhachSan.NhanSu;

public class DAL_PhongBan
{
    // Dùng lại các tiện ích SQL của dự án; kiểm tra dữ liệu đầu vào nằm ở BUS.
    // SuaTheoId() bỏ qua kết quả connection.Update() rồi trả true nên có thể báo thành công dù không cập nhật được dòng nào.
    //XoaTheoId() bắt mọi lỗi rồi trả false
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly ThuThiLenhCoDieuKien lenh;
    // Khởi tạo các tiện ích SQL với chuỗi kết nối từ QLKS_CONNECTION hoặc connection.txt.
    public DAL_PhongBan()
    {
        string chuoi = CauHinhKetNoi.Chuoi;
        hienThi = new HienThi(chuoi);
        them = new Them(chuoi);
        lenh = new ThuThiLenhCoDieuKien(chuoi);
    }

    // Đọc toàn bộ bảng PhongBan và ánh xạ mỗi dòng SQL thành một đối tượng ET PhongBan.
    public List<PhongBan> HienThiTatCaPB() => hienThi.HienThiTatCa<PhongBan>();

    // Tiện ích đọc thuộc tính có [Key] trong ET để tìm bản ghi theo MaPhongBan.
    public PhongBan HienThiTheoId(int ma) => hienThi.HienThiTheoId(new PhongBan { MaPhongBan = ma });

    // SQL tự sinh MaPhongBan; tiện ích thêm trả false nếu gặp lỗi, chẳng hạn trùng tên.
    public bool Them(PhongBan phongBan) => them.ThemKhongDieuKien(phongBan);

    // Kiểm tra tên phòng ban đã tồn tại hay chưa
    public bool TenDaTonTai(PhongBan phongBan) 
    {
        return lenh.TruyVan<int>(
        "SELECT COUNT(1) FROM dbo.PhongBan WHERE TenPhongBan = @TenPhongBan AND MaPhongBan <> @MaPhongBan",phongBan).Single() > 0;
        //> 0 biến số đó thành true/false.
    }

    //XÓA 
    public bool Xoa(PhongBan phongBan)
    {
        try
        {
            return lenh.ThucThiLenh("DELETE FROM dbo.PhongBan WHERE MaPhongBan = @MaPhongBan", phongBan) == 1;//chỉ báo thành công khi tác động đúng một dòng
        }
        // Lỗi vi phạm các ràng buộc dữ liệu (Constraint Violation) trong cơ sở dữ liệu SQL Server
        catch (SqlException ex) when (ex.Number == 547)
        {
            throw new InvalidOperationException(
                "Không thể xóa phòng ban đang được nhân viên sử dụng. Có thể đổi trạng thái thành Ngừng hoạt động.", ex);
        }
        // Nhận các lỗi SQL còn lại.
        catch (SqlException ex)
        {
            throw new InvalidOperationException(
                "Không thể xóa phòng ban. Vui lòng thử lại.", ex);
        }
    }

    // CẬP NHẬT
    public bool Sua(PhongBan phongBan)
    {
        // Các tham số @... lấy giá trị từ thuộc tính ET, không ghép nội dung người dùng vào SQL.
        // Kiểm tra số dòng cập nhật để không báo lưu thành công khi bản ghi đã bị xóa.
        return lenh.ThucThiLenh("UPDATE dbo.PhongBan SET TenPhongBan = @TenPhongBan, MoTa = @MoTa, " +
            "TrangThai = @TrangThai WHERE MaPhongBan = @MaPhongBan", phongBan) == 1;
    }
}
