using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.NhanSu;

public class DAL_PhongBan
{
    // Dùng lại các tiện ích SQL của dự án; kiểm tra dữ liệu đầu vào nằm ở BUS.
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly Xoa xoa;
    private readonly ThuThiLenhCoDieuKien lenh;

    // Khởi tạo các tiện ích SQL với chuỗi kết nối từ QLKS_CONNECTION hoặc connection.txt.
    public DAL_PhongBan()
    {
        // Các tiện ích cùng dùng chuỗi kết nối từ QLKS_CONNECTION hoặc connection.txt.
        string chuoi = CauHinhKetNoi.Chuoi;
        hienThi = new HienThi(chuoi);
        sua = new Sua(chuoi);
        xoa = new Xoa(chuoi);
        them = new Them(chuoi);
        lenh = new ThuThiLenhCoDieuKien(chuoi);
    }

    // Đọc toàn bộ bảng PhongBan và ánh xạ mỗi dòng SQL thành một đối tượng ET PhongBan.
    public List<PhongBan> HienThiTatCa() => hienThi.HienThiTatCa<PhongBan>();

    // Tiện ích đọc thuộc tính có [Key] trong ET để tìm bản ghi theo MaPhongBan.
    public PhongBan HienThiTheoId(int ma) => hienThi.HienThiTheoId(new PhongBan { MaPhongBan = ma });

    // SQL tự sinh MaPhongBan; tiện ích thêm trả false nếu gặp lỗi, chẳng hạn trùng tên.
    public bool Them(PhongBan phongBan) => them.ThemKhongDieuKien(phongBan);

    public bool Xoa(PhongBan phongBan) => xoa.XoaTheoId(phongBan);

    public bool Sua(PhongBan phongBan)
    {
        // Các tham số @... lấy giá trị từ thuộc tính ET, không ghép nội dung người dùng vào SQL.
        // Kiểm tra số dòng cập nhật để không báo lưu thành công khi bản ghi đã bị xóa.
        return lenh.ThucThiLenh("UPDATE dbo.PhongBan SET TenPhongBan = @TenPhongBan, MoTa = @MoTa, " +
            "TrangThai = @TrangThai WHERE MaPhongBan = @MaPhongBan", phongBan) == 1;
    }
}
