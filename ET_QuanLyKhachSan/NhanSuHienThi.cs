namespace ET_QuanLyKhachSan;

// Thêm: dữ liệu JOIN chỉ để hiển thị; khi lưu vẫn dùng ET gốc để không ghi tên vào cột mã.
public class NhanVienHienThi : NhanVien
{
    public string PhongBan { get; set; } = "";
    public string ChucVu { get; set; } = "";
    public string LuaChon => $"{MaNhanVien} — {HoTen}";
}

public class LichLamViecHienThi : LichLamViec
{
    public string NhanVien { get; set; } = "";
    public string Ca { get; set; } = "";
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string LuaChon => $"{MaLich} — {NhanVien} — {NgayLam:dd/MM/yyyy} — {Ca}";
}

public class ChamCongHienThi : ChamCong
{
    public string NhanVien { get; set; } = "";
}

public class BangLuongHienThi : BangLuong
{
    public string NhanVien { get; set; } = "";
    public string Ky => $"{Thang:00}/{Nam}";
}

[Dapper.Contrib.Extensions.Table("KhenThuong")]
public class KhenThuong
{
    [Dapper.Contrib.Extensions.Key]
    public long MaKhenThuong { get; set; }
    public int MaNhanVien { get; set; }
    public DateTime NgayKhenThuong { get; set; }
    public string LyDo { get; set; } = "";
    public string HinhThuc { get; set; } = "TienThuong";
    public decimal SoTien { get; set; }
    public string TrangThai { get; set; } = "ChoDuyet";
    public string GhiChu { get; set; } = "";
}

public class KhenThuongHienThi : KhenThuong
{
    public string NhanVien { get; set; } = "";
}
