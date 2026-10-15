using System.Transactions;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_KhachTheoPhong
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly ThuThiLenhCoDieuKien lenh;
    public DAL_KhachTheoPhong()
    {
        hienThi = new HienThi(str);
        them = new Them(str);
        sua = new Sua(str);
        lenh = new ThuThiLenhCoDieuKien(str);
    }
    public System.Data.DataTable hienThiKTPAll() => lenh.TruyVanBang("EXEC dbo.sp_HienThiKhachLuuTru");
    public List<XepPhong> HienThiXepPhong() => hienThi.HienThiTatCa<XepPhong>();
    public List<Phong> HienThiPhong() => hienThi.HienThiTatCa<Phong>();
    public XepPhong HienThiXepPhong(XepPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public Phong HienThiPhong(Phong duLieu) => hienThi.HienThiTheoId(duLieu);
    public ChiTietDatPhong HienThiChiTietDatPhong(ChiTietDatPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public bool ChuyenPhong(long ma, int phongMoi, string ghiChu)
    {
        using var scope = new TransactionScope();
        var xp = hienThi.HienThiTheoId(new XepPhong { MaXepPhong = ma });
        var cu = hienThi.HienThiTheoId(new Phong { MaPhong = xp.MaPhong });
        var moi = hienThi.HienThiTheoId(new Phong { MaPhong = phongMoi });
        xp.NgayTraThucTe = DateTime.Now;
        xp.TrangThai = "DaCheckOut";
        cu.TrangThaiKhaiThac = "SanSang";
        cu.TrangThaiVeSinh = "CanDon";
        var luotMoi = new XepPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong, MaPhong = phongMoi,
            NgayNhanThucTe = DateTime.Now, TrangThai = "DaCheckIn", GhiChu = ghiChu };
        moi.TrangThaiKhaiThac = "DangSuDung";
        if (!sua.SuaTheoId(xp) || !sua.SuaTheoId(cu) || !them.ThemKhongDieuKien(luotMoi) || !sua.SuaTheoId(moi)) return false;
        scope.Complete();
        return true;
    }
    public bool SuaGhiChuLuuTru(long ma, string ghiChu)
    {
        var xp = hienThi.HienThiTheoId(new XepPhong { MaXepPhong = ma });
        xp.GhiChu = ghiChu;
        return sua.SuaTheoId(xp);
    }
    public bool NhanPhong(long ma)
    {
        using var scope = new TransactionScope();
        var xp = hienThi.HienThiTheoId(new XepPhong { MaXepPhong = ma });
        var ct = hienThi.HienThiTheoId(new ChiTietDatPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong });
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = ct.MaDatPhong });
        var p = hienThi.HienThiTheoId(new Phong { MaPhong = xp.MaPhong });
        xp.NgayNhanThucTe = DateTime.Now;
        xp.TrangThai = "DaCheckIn";
        p.TrangThaiKhaiThac = "DangSuDung";
        dp.TrangThai = "DaCheckIn";
        if (!sua.SuaTheoId(xp) || !sua.SuaTheoId(p) || !sua.SuaTheoId(dp)) return false;
        scope.Complete();
        return true;
    }
    public bool TraPhong(long ma)
    {
        using var scope = new TransactionScope();
        var xp = hienThi.HienThiTheoId(new XepPhong { MaXepPhong = ma });
        var p = hienThi.HienThiTheoId(new Phong { MaPhong = xp.MaPhong });
        xp.NgayTraThucTe = DateTime.Now;
        xp.TrangThai = "DaCheckOut";
        p.TrangThaiKhaiThac = "SanSang";
        p.TrangThaiVeSinh = "CanDon";
        if (!sua.SuaTheoId(xp) || !sua.SuaTheoId(p)) return false;
        var ct = hienThi.HienThiTheoId(new ChiTietDatPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong });
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = ct.MaDatPhong });
        var ids = hienThi.HienThiTatCa<ChiTietDatPhong>().Where(x => x.MaDatPhong == dp.MaDatPhong).Select(x => x.MaChiTietDatPhong).ToHashSet();
        var con = hienThi.HienThiTatCa<XepPhong>().Where(x => ids.Contains(x.MaChiTietDatPhong)).ToList();
        dp.TrangThai = con.Any(x => x.TrangThai == "DaCheckIn") ? "DaCheckIn"
            : con.Any(x => x.TrangThai == "DaXep") ? "DaXacNhan" : "DaCheckOut";
        if (!sua.SuaTheoId(dp)) return false;
        scope.Complete();
        return true;
    }
}
