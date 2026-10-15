using System.Transactions;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_Datphong
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly Xoa xoa;
    private readonly ThuThiLenhCoDieuKien lenh;

    public DAL_Datphong()
    {
        hienThi = new HienThi(str);
        them = new Them(str);
        sua = new Sua(str);
        xoa = new Xoa(str);
        lenh = new ThuThiLenhCoDieuKien(str);
    }

    public System.Data.DataTable hienThiDPAll() => lenh.TruyVanBang("EXEC dbo.sp_HienThiDatPhong");

    public List<KhachHang> HienThiKhachHang() => hienThi.HienThiTatCa<KhachHang>();
    public List<LoaiPhong> HienThiLoaiPhong() => hienThi.HienThiTatCa<LoaiPhong>();
    public List<XepPhong> HienThiXepPhong() => hienThi.HienThiTatCa<XepPhong>();
    public DatPhong HienThiDatPhong(DatPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public Phong HienThiPhong(Phong duLieu) => hienThi.HienThiTheoId(duLieu);
    public List<ChiTietDatPhong> ChiTiet(long ma) => hienThi.HienThiTatCa<ChiTietDatPhong>().Where(x => x.MaDatPhong == ma).ToList();
    public System.Data.DataTable ChiTietHienThi(long ma) => lenh.TruyVanBang("EXEC dbo.sp_ChiTietDatPhong @MaDatPhong", new { MaDatPhong = ma });
    public bool LuuDatPhong(DatPhong dp, List<ChiTietDatPhong> chiTiet)
    {
        using var scope = new TransactionScope();
        var chiTietCu = ChiTiet(dp.MaDatPhong);
        if (dp.MaDatPhong == 0)
        {
            dp.NgayDat = DateTime.Now;
            dp.TrangThai = "ChoXacNhan";
            if (!them.ThemKhongDieuKien(dp)) return false;
        }
        else if (!sua.SuaTheoId(dp)) return false;

        // Giữ mã của các dòng cũ để phân bổ phòng vẫn tham chiếu đúng chi tiết.
        foreach (var cu in chiTietCu.Where(x => !chiTiet.Any(c => c.MaChiTietDatPhong == x.MaChiTietDatPhong)))
            if (!xoa.XoaTheoId(cu)) return false;
        foreach (var ct in chiTiet)
        {
            ct.MaDatPhong = dp.MaDatPhong;
            bool daLuu = ct.MaChiTietDatPhong == 0 ? them.ThemKhongDieuKien(ct) : sua.SuaTheoId(ct);
            if (!daLuu) return false;
        }
        scope.Complete();
        return true;
    }
    public bool XacNhan(long ma)
    {
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = ma });
        if (dp.TrangThai == "DaHuy") return false;
        dp.TrangThai = "DaXacNhan";
        return sua.SuaTheoId(dp);
    }
    public bool HuyDatPhong(long ma)
    {
        using var scope = new TransactionScope();
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = ma });
        var ids = ChiTiet(ma).Select(x => x.MaChiTietDatPhong).ToHashSet();
        var phanBo = hienThi.HienThiTatCa<XepPhong>().Where(x => ids.Contains(x.MaChiTietDatPhong)).ToList();
        // Lượt đã nhận/trả phòng là lịch sử lưu trú, không xóa khi hủy đặt phòng.
        if (dp.TrangThai is "DaCheckIn" or "DaCheckOut" ||
            phanBo.Any(x => x.NgayNhanThucTe.HasValue || x.NgayTraThucTe.HasValue ||
                x.TrangThai is "DaCheckIn" or "DaCheckOut")) return false;
        foreach (var xp in phanBo)
        {
            if (!xoa.XoaTheoId(xp)) return false;
        }
        dp.TrangThai = "DaHuy";
        if (!sua.SuaTheoId(dp)) return false;
        scope.Complete();
        return true;
    }
}
