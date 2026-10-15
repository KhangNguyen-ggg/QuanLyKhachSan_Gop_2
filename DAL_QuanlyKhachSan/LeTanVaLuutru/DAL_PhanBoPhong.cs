using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_PhanBoPhong
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly Xoa xoa;
    private readonly ThuThiLenhCoDieuKien lenh;
    public DAL_PhanBoPhong()
    {
        hienThi = new HienThi(str);
        them = new Them(str);
        sua = new Sua(str);
        xoa = new Xoa(str);
        lenh = new ThuThiLenhCoDieuKien(str);
    }
    public System.Data.DataTable hienThiPBPAll() => lenh.TruyVanBang("EXEC dbo.sp_HienThiPhanBoPhong");
    public List<DatPhong> HienThiDatPhong() => hienThi.HienThiTatCa<DatPhong>().Where(x => x.TrangThai != "DaHuy").ToList();
    public List<Phong> HienThiPhong() => hienThi.HienThiTatCa<Phong>();
    public XepPhong HienThiXepPhong(XepPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public ChiTietDatPhong HienThiChiTietDatPhong(ChiTietDatPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public LoaiPhong HienThiLoaiPhong(LoaiPhong duLieu) => hienThi.HienThiTheoId(duLieu);
    public List<ChiTietDatPhong> ChiTiet(long ma) => hienThi.HienThiTatCa<ChiTietDatPhong>().Where(x => x.MaDatPhong == ma).ToList();
    public bool LuuPhanBo(XepPhong xp)
    {
        var ct = hienThi.HienThiTheoId(new ChiTietDatPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong });
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = ct.MaDatPhong });
        if (dp.TrangThai == "DaHuy") return false;
        if (xp.MaXepPhong != 0) return sua.SuaTheoId(xp);
        xp.TrangThai = "DaXep";
        return them.ThemKhongDieuKien(xp);
    }
    public bool BoPhanBo(long ma) => xoa.XoaTheoId(new XepPhong { MaXepPhong = ma });
}
