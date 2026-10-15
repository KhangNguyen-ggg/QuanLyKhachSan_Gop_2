using System.Transactions;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_HoaDon
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private readonly HienThi hienThi;
    private readonly Them them;
    private readonly Sua sua;
    private readonly ThuThiLenhCoDieuKien lenh;
    public DAL_HoaDon()
    {
        hienThi = new HienThi(str);
        them = new Them(str);
        sua = new Sua(str);
        lenh = new ThuThiLenhCoDieuKien(str);
    }
    public System.Data.DataTable hienThiHDAll() => lenh.TruyVanBang("EXEC dbo.sp_HienThiHoaDon");
    public List<DatPhong> HienThiDatPhong() => lenh.TruyVan<DatPhong>("EXEC dbo.sp_DatPhongChuaLapHoaDon");
    public bool DaLapHoaDon(long maDat) => hienThi.HienThiTatCa<HoaDon>().Any(x => x.MaDatPhong == maDat);
    public List<NhanVien> HienThiNhanVien() => hienThi.HienThiTatCa<NhanVien>();
    public List<UuDai> HienThiUuDai() => hienThi.HienThiTatCa<UuDai>();
    public List<DichVu> HienThiDichVu() => hienThi.HienThiTatCa<DichVu>().Where(x => x.TrangThai).ToList();
    public List<GoiDichVu> HienThiGoiDichVu() => hienThi.HienThiTatCa<GoiDichVu>().Where(x => x.TrangThai).ToList();
    public HoaDon HienThiHoaDon(HoaDon duLieu) => hienThi.HienThiTheoId(duLieu);
    public KhachHang HienThiKhachHang(KhachHang duLieu) => hienThi.HienThiTheoId(duLieu);
    public List<ChiTietHoaDon> ChiTietHoaDon(long ma) => hienThi.HienThiTatCa<ChiTietHoaDon>().Where(x => x.MaHoaDon == ma).ToList();
    public decimal DaThu(long ma) => hienThi.HienThiTatCa<ThanhToan>().Where(x => x.MaHoaDon == ma && x.TrangThai == "ThanhCong").Sum(x => x.SoTien);
    public List<ChiTietHoaDon> KhoanThu(long ma) =>
        lenh.TruyVan<ChiTietHoaDon>("EXEC dbo.sp_KhoanThuDatPhong @MaDatPhong", new { MaDatPhong = ma });
    public static decimal TinhGiam(decimal tong, UuDai uu)
    {
        if (uu == null) return 0;
        return Math.Round(uu.LoaiGiam == "PhanTram" ? tong * uu.GiaTriGiam / 100 : uu.GiaTriGiam,
            2, MidpointRounding.AwayFromZero);
    }
    public static decimal TinhTong(IEnumerable<ChiTietHoaDon> chiTiet) =>
        chiTiet.Sum(x => Math.Round(x.SoLuong * x.DonGia, 2, MidpointRounding.AwayFromZero));
    public bool TaoHoaDon(long maDat, int? nhanVien, int? uuDai, string ghiChu, List<SuDungDichVu> dichVuThem = null)
    {
        using var scope = new TransactionScope();
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = maDat });
        if (dp.TrangThai == "DaHuy" || DaLapHoaDon(maDat)) return false;
        // Lưu lượt sử dụng và hóa đơn cùng giao dịch: lỗi thì hoàn tác tất cả.
        foreach (var sd in dichVuThem ?? new List<SuDungDichVu>())
        {
            if (sd.MaDichVu.HasValue && !HienThiDichVu().Any(x => x.MaDichVu == sd.MaDichVu)) return false;
            if (sd.MaGoiDichVu.HasValue && !HienThiGoiDichVu().Any(x => x.MaGoiDichVu == sd.MaGoiDichVu)) return false;
            var luot = new SuDungDichVu { MaDatPhong = maDat, MaKhachHang = dp.MaKhachHangDat,
                MaNhanVien = nhanVien, MaDichVu = sd.MaDichVu, MaGoiDichVu = sd.MaGoiDichVu,
                SoLuong = sd.SoLuong, DonGia = sd.DonGia, ThoiGianSuDung = DateTime.Now,
                TrangThai = "DaSuDung" };
            if (!them.ThemKhongDieuKien(luot)) return false;
        }
        var ct = KhoanThu(maDat);
        var tong = TinhTong(ct);
        var giam = TinhGiam(tong, uuDai.HasValue ? hienThi.HienThiTheoId(new UuDai { MaUuDai = uuDai.Value }) : null);
        var hd = new HoaDon { MaDatPhong = maDat, MaKhachHang = dp.MaKhachHangDat,
            MaNhanVienLap = nhanVien, MaUuDai = uuDai, NgayLap = DateTime.Now,
            TongTien = tong, TienGiam = giam, ThanhTien = tong - giam,
            TrangThai = tong == giam ? "DaThanhToan" : "ChuaThanhToan", GhiChu = ghiChu };
        if (!them.ThemKhongDieuKien(hd)) return false;
        foreach (var dong in ct)
        {
            dong.MaHoaDon = hd.MaHoaDon;
            if (!them.ThemKhongDieuKien(dong)) return false;
        }
        scope.Complete();
        return true;
    }
    public bool ApDungUuDai(long ma, int? uuDai, string ghiChu)
    {
        var hd = hienThi.HienThiTheoId(new HoaDon { MaHoaDon = ma });
        hd.MaUuDai = uuDai;
        hd.TienGiam = TinhGiam(hd.TongTien, uuDai.HasValue ? hienThi.HienThiTheoId(new UuDai { MaUuDai = uuDai.Value }) : null);
        hd.ThanhTien = hd.TongTien - hd.TienGiam;
        var daThu = DaThu(ma);
        hd.TrangThai = daThu >= hd.ThanhTien ? "DaThanhToan" : daThu > 0 ? "ThanhToanMotPhan" : "ChuaThanhToan";
        hd.GhiChu = ghiChu;
        return sua.SuaTheoId(hd);
    }
    public bool ThuTien(long ma, decimal tien, int? nhanVien, string ghiChu)
    {
        using var scope = new TransactionScope();
        var hd = hienThi.HienThiTheoId(new HoaDon { MaHoaDon = ma });
        var dp = hienThi.HienThiTheoId(new DatPhong { MaDatPhong = hd.MaDatPhong });
        if (dp.TrangThai == "DaHuy" || hd.TrangThai == "Huy") return false;
        decimal daThu = DaThu(ma);
        if (tien <= 0 || decimal.Round(tien, 2) != tien || tien > hd.ThanhTien - daThu) return false;
        var thu = new ThanhToan { MaHoaDon = ma, MaNhanVien = nhanVien, SoTien = tien,
            ThoiGianThanhToan = DateTime.Now, TrangThai = "ThanhCong", GhiChu = ghiChu,
            MaGiaoDich = Guid.NewGuid().ToString("N") };
        if (!them.ThemKhongDieuKien(thu)) return false;
        hd.TrangThai = daThu + tien >= hd.ThanhTien ? "DaThanhToan" : "ThanhToanMotPhan";
        if (!sua.SuaTheoId(hd)) return false;
        scope.Complete();
        return true;
    }
    public bool HuyHoaDon(long ma)
    {
        var hd = hienThi.HienThiTheoId(new HoaDon { MaHoaDon = ma });
        hd.TrangThai = "Huy";
        return sua.SuaTheoId(hd);
    }
}
