using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_Khachhang
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly Xoa xoa;
    public DAL_Khachhang()
    {
        hienThi = new HienThi(str);
        them = new Them(str);
        sua = new Sua(str);
        xoa = new Xoa(str);
    }
    public List<KhachHang> hienThiKHAll() => hienThi.HienThiTatCa<KhachHang>();
    public KhachHang HienThiKhachHang(KhachHang duLieu) => hienThi.HienThiTheoId(duLieu);
    public bool LuuKhachHang(KhachHang kh)
    {
        if (kh.MaKhachHang != 0) return sua.SuaTheoId(kh);
        kh.NgayTao = DateTime.Now;
        return them.ThemKhongDieuKien(kh);
    }
    public bool XoaKhachHang(int ma) => xoa.XoaTheoId(new KhachHang { MaKhachHang = ma });
}
