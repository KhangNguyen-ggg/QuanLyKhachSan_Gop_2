using System.Net.Mail;
using DAL_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace BUS_QuanlyKhachSan.NhanSu;

public class BUS_NhanSu
{
    private readonly DAL_NhanSu dal = new();

    public T TheoId<T>(long ma) where T : class
    {
        BUS_KiemTra.Ma(ma, "dữ liệu");
        return BUS_KiemTra.Chon(dal.TheoId<T>(ma), "dữ liệu còn tồn tại");
    }

    public List<PhongBan> PhongBan() => dal.TatCa<PhongBan>();
    public List<ChucVu> ChucVu() => dal.TatCa<ChucVu>();
    public List<NhanVienHienThi> NhanVien(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.NhanVien(), tuKhoa, trangThai,
            x => $"{x.MaNhanVien} {x.HoTen} {x.PhongBan} {x.ChucVu} {x.SoDienThoai} {x.Email} {x.DiaChi}", x => x.TrangThai, "NhanVien");
    public List<CaLamViec> CaLamViec(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.TatCa<CaLamViec>().OrderBy(x => x.MaCa), tuKhoa, trangThai,
            x => $"{x.MaCa} {x.TenCa} {x.GhiChu}", x => x.TrangThai.ToString(), "CaLamViec");
    public List<LichLamViecHienThi> LichLamViec(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.Lich(), tuKhoa, trangThai,
            x => $"{x.MaLich} {x.NhanVien} {x.Ca} {x.NgayLam:dd MM yyyy} {x.GhiChu}", x => x.TrangThai, "LichLamViec");
    public List<ChamCongHienThi> ChamCong(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.ChamCong(), tuKhoa, trangThai,
            x => $"{x.MaChamCong} {x.MaLich} {x.NhanVien} {x.NgayChamCong:dd MM yyyy} {x.GhiChu}", x => x.TrangThai, "ChamCong");
    public List<BangLuongHienThi> BangLuong(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.BangLuong(), tuKhoa, trangThai,
            x => $"{x.MaBangLuong} {x.NhanVien} {x.Thang:00} {x.Nam} {x.GhiChu}", x => x.TrangThai, "BangLuong");
    public List<KhenThuongHienThi> KhenThuong(string tuKhoa = "", string trangThai = "") =>
        NhanSuQuyUoc.Loc(() => dal.KhenThuong(), tuKhoa, trangThai,
            x => $"{x.MaKhenThuong} {x.NhanVien} {x.NgayKhenThuong:dd MM yyyy} {x.LyDo} {x.GhiChu}", x => x.TrangThai, "KhenThuong");

    private static void KiemTraMa(long ma) => BUS_KiemTra.YeuCau(ma >= 0, "Mã dữ liệu không hợp lệ.");

    public void Luu(NhanVien duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "nhân viên");
        KiemTraMa(duLieu.MaNhanVien);
        duLieu.HoTen = BUS_KiemTra.Chuoi(duLieu.HoTen, "họ tên", 150, true);
        duLieu.DiaChi = BUS_KiemTra.Chuoi(duLieu.DiaChi, "địa chỉ", 300);
        duLieu.Email = BUS_KiemTra.Chuoi(duLieu.Email, "email", 150);
        BUS_KiemTra.YeuCau(duLieu.Email == "" || (MailAddress.TryCreate(duLieu.Email, out var email) && email.Address == duLieu.Email), "Email không đúng định dạng.");
        duLieu.SoDienThoai = BUS_KiemTra.Chuoi(duLieu.SoDienThoai, "số điện thoại", 20);
        if (duLieu.SoDienThoai != "") duLieu.SoDienThoai = BUS_KiemTra.DienThoai(duLieu.SoDienThoai);
        BUS_KiemTra.YeuCau(duLieu.GioiTinh is null or "" or "M" or "F" or "O", "Giới tính không hợp lệ.");
        if (duLieu.GioiTinh == "") duLieu.GioiTinh = null;
        BUS_KiemTra.Ma(duLieu.MaPhongBan, "phòng ban");
        BUS_KiemTra.Ma(duLieu.MaChucVu, "chức vụ");
        BUS_KiemTra.Tien(duLieu.LuongCoBan, "Lương cơ bản");
        BUS_KiemTra.YeuCau(!duLieu.NgaySinh.HasValue || duLieu.NgaySinh.Value.Date <= DateTime.Today, "Ngày sinh không được ở tương lai.");
        BUS_KiemTra.YeuCau(!duLieu.NgaySinh.HasValue || !duLieu.NgayVaoLam.HasValue || duLieu.NgayVaoLam >= duLieu.NgaySinh,
            "Ngày vào làm không được trước ngày sinh.");
        NhanSuQuyUoc.KiemTraTrangThai("NhanVien", duLieu.TrangThai);
        dal.LuuNhanVien(duLieu);
    }

    public void Luu(CaLamViec duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "ca làm việc");
        KiemTraMa(duLieu.MaCa);
        duLieu.TenCa = BUS_KiemTra.Chuoi(duLieu.TenCa, "tên ca", 100, true);
        duLieu.GhiChu = BUS_KiemTra.Chuoi(duLieu.GhiChu, "ghi chú", 300);
        BUS_KiemTra.YeuCau(duLieu.GioBatDau >= TimeSpan.Zero && duLieu.GioBatDau < TimeSpan.FromDays(1) &&
            duLieu.GioKetThuc >= TimeSpan.Zero && duLieu.GioKetThuc < TimeSpan.FromDays(1) &&
            duLieu.GioBatDau != duLieu.GioKetThuc, "Giờ bắt đầu/kết thúc phải hợp lệ và khác nhau.");
        // Khác Phòng ban: tính số giờ từ giờ ca; giờ kết thúc nhỏ hơn giờ bắt đầu là ca qua đêm.
        var soGio = duLieu.GioKetThuc - duLieu.GioBatDau;
        if (soGio < TimeSpan.Zero) soGio += TimeSpan.FromDays(1);
        duLieu.SoGioLam = decimal.Round((decimal)soGio.TotalHours, 2);
        dal.LuuCa(duLieu);
    }

    public void Luu(LichLamViec duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "lịch làm việc");
        KiemTraMa(duLieu.MaLich);
        BUS_KiemTra.Ma(duLieu.MaNhanVien, "nhân viên");
        BUS_KiemTra.Ma(duLieu.MaCa, "ca làm việc");
        duLieu.NgayLam = NhanSuQuyUoc.Ngay(duLieu.NgayLam, "ngày làm");
        duLieu.GhiChu = BUS_KiemTra.Chuoi(duLieu.GhiChu, "ghi chú", 300);
        NhanSuQuyUoc.KiemTraTrangThai("LichLamViec", duLieu.TrangThai);
        dal.LuuLich(duLieu);
    }

    public void Luu(ChamCong duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "chấm công");
        KiemTraMa(duLieu.MaChamCong);
        BUS_KiemTra.Ma(duLieu.MaLich, "lịch làm việc");
        duLieu.GhiChu = BUS_KiemTra.Chuoi(duLieu.GhiChu, "ghi chú", 300);
        NhanSuQuyUoc.KiemTraTrangThai("ChamCong", duLieu.TrangThai);
        BUS_KiemTra.YeuCau(duLieu.SoGioTangCa >= 0 && duLieu.SoGioTangCa <= 24 && decimal.Round(duLieu.SoGioTangCa, 2) == duLieu.SoGioTangCa,
            "Tăng ca phải từ 0 đến 24 giờ, tối đa 2 số lẻ.");
        if (duLieu.TrangThai is "Nghi" or "NghiPhep" or "KhongCham")
        {
            BUS_KiemTra.YeuCau(!duLieu.GioVao.HasValue && !duLieu.GioRa.HasValue && duLieu.SoGioTangCa == 0,
                "Vắng/nghỉ phép/không chấm không được có giờ vào, giờ ra hoặc tăng ca.");
            duLieu.SoGioLam = 0;
        }
        else
        {
            BUS_KiemTra.YeuCau(duLieu.GioVao.HasValue && duLieu.GioRa.HasValue, "Vui lòng nhập đủ ngày/giờ vào và ra.");
            var thoiLuong = (duLieu.GioRa.Value - duLieu.GioVao.Value).TotalHours;
            BUS_KiemTra.YeuCau(thoiLuong > 0 && thoiLuong <= 24, "Giờ ra phải sau giờ vào, mỗi lần chấm tối đa 24 giờ.");
            BUS_KiemTra.YeuCau(duLieu.SoGioTangCa <= (decimal)thoiLuong, "Tăng ca không được lớn hơn tổng giờ thực tế.");
            // Thêm: không nhập lặp giờ thường; tính từ thời gian thực tế sau khi trừ giờ tăng ca.
            duLieu.SoGioLam = decimal.Round((decimal)thoiLuong - duLieu.SoGioTangCa, 2);
            var lich = TheoId<LichLamViec>(duLieu.MaLich);
            var ca = TheoId<CaLamViec>(lich.MaCa);
            duLieu.TrangThai = duLieu.SoGioLam >= ca.SoGioLam ? "DuCong" : "ThieuCong";
        }
        dal.LuuChamCong(duLieu);
    }

    public static void TinhLuong(BangLuong duLieu)
    {
        foreach (var khoan in new[] { duLieu.LuongCoBan, duLieu.TienCong, duLieu.TienTangCa, duLieu.TienThuong,
            duLieu.TienPhuCap, duLieu.TienKhauTru, duLieu.Thue }) BUS_KiemTra.Tien(khoan, "Khoản lương");
        // Khác Phòng ban: lương cơ bản là thông tin tham chiếu, không cộng lần nữa vào tiền công.
        duLieu.TongLuong = duLieu.TienCong + duLieu.TienTangCa + duLieu.TienThuong + duLieu.TienPhuCap;
        duLieu.ThucLinh = duLieu.TongLuong - duLieu.TienKhauTru - duLieu.Thue;
        BUS_KiemTra.Tien(duLieu.TongLuong, "Tổng lương");
        BUS_KiemTra.Tien(duLieu.ThucLinh, "Thực lĩnh");
    }

    public void Luu(BangLuong duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "bảng lương");
        KiemTraMa(duLieu.MaBangLuong);
        BUS_KiemTra.Ma(duLieu.MaNhanVien, "nhân viên");
        BUS_KiemTra.YeuCau(duLieu.Thang is >= 1 and <= 12 && duLieu.Nam is >= 2000 and <= 2100, "Kỳ lương phải là tháng 1–12, năm 2000–2100.");
        duLieu.GhiChu = BUS_KiemTra.Chuoi(duLieu.GhiChu, "ghi chú", 500);
        NhanSuQuyUoc.KiemTraTrangThai("BangLuong", duLieu.TrangThai);
        TinhLuong(duLieu);
        if (duLieu.MaBangLuong == 0) duLieu.NgayLap = DateTime.Now;
        dal.LuuBangLuong(duLieu);
    }

    public decimal TongThuong(int maNhanVien, int thang, int nam)
    {
        BUS_KiemTra.Ma(maNhanVien, "nhân viên");
        BUS_KiemTra.YeuCau(thang is >= 1 and <= 12 && nam is >= 2000 and <= 2100, "Kỳ lương không hợp lệ.");
        return dal.TruyVan<decimal>("SELECT COALESCE(SUM(SoTien),0) FROM dbo.KhenThuong WHERE MaNhanVien=@maNhanVien " +
            "AND HinhThuc='TienThuong' AND TrangThai='DaDuyet' AND MONTH(NgayKhenThuong)=@thang AND YEAR(NgayKhenThuong)=@nam",
            new { maNhanVien, thang, nam }).Single();
    }

    public void Luu(KhenThuong duLieu)
    {
        BUS_KiemTra.Chon(duLieu, "khen thưởng");
        KiemTraMa(duLieu.MaKhenThuong);
        BUS_KiemTra.Ma(duLieu.MaNhanVien, "nhân viên");
        duLieu.NgayKhenThuong = NhanSuQuyUoc.Ngay(duLieu.NgayKhenThuong, "ngày khen thưởng");
        duLieu.LyDo = BUS_KiemTra.Chuoi(duLieu.LyDo, "lý do", 500, true);
        duLieu.GhiChu = BUS_KiemTra.Chuoi(duLieu.GhiChu, "ghi chú", 500);
        BUS_KiemTra.YeuCau(NhanSuQuyUoc.HinhThuc.Any(x => x.Key == duLieu.HinhThuc), "Hình thức khen thưởng không hợp lệ.");
        BUS_KiemTra.Tien(duLieu.SoTien, "Số tiền thưởng");
        BUS_KiemTra.YeuCau(duLieu.HinhThuc == "TienThuong" || duLieu.SoTien == 0, "Biểu dương/giấy khen có số tiền bằng 0.");
        NhanSuQuyUoc.KiemTraTrangThai("KhenThuong", duLieu.TrangThai);
        dal.Luu(duLieu, duLieu.MaKhenThuong);
    }

    public void Xoa<T>(long ma) where T : class
    {
        BUS_KiemTra.Ma(ma, "dữ liệu cần xóa");
        if (typeof(T) == typeof(BangLuong)) dal.XoaBangLuong(ma);
        else dal.Xoa<T>(ma);
    }
}
