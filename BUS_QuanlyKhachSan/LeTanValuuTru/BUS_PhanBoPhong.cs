using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class Bus_PhanBoPhong
{
    private readonly DAL_PhanBoPhong dal = new DAL_PhanBoPhong();
    public System.Data.DataView HienThiPBP(string tuKhoa = "", string trangThai = "")
    {
        return BUS_KiemTra.LocBang(dal.hienThiPBPAll(), tuKhoa, trangThai);
    }
    public List<DatPhong> HienThiDatPhong() => dal.HienThiDatPhong();
    public List<Phong> HienThiPhong() => dal.HienThiPhong();
    public XepPhong HienThiXepPhong(XepPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiXepPhong(duLieu), "dữ liệu còn tồn tại");
    public ChiTietDatPhong HienThiChiTietDatPhong(ChiTietDatPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiChiTietDatPhong(duLieu), "dữ liệu còn tồn tại");
    public LoaiPhong HienThiLoaiPhong(LoaiPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiLoaiPhong(duLieu), "dữ liệu còn tồn tại");
    public List<ChiTietDatPhong> ChiTiet(long ma) => dal.ChiTiet(ma);
    public static void KiemTraPhanBo(XepPhong xp)
    {
        BUS_KiemTra.Chon(xp, "phân bổ phòng");
        BUS_KiemTra.Ma(xp.MaChiTietDatPhong, "chi tiết đặt phòng");
        BUS_KiemTra.Ma(xp.MaPhong, "phòng");
        xp.GhiChu = BUS_KiemTra.Chuoi(xp.GhiChu, "ghi chú", 500);
    }
    public bool LuuPhanBo(XepPhong xp)
    {
        KiemTraPhanBo(xp);
        return BUS_KiemTra.KetQua(dal.LuuPhanBo(xp));
    }
    public bool BoPhanBo(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.BoPhanBo(ma));
    }
    public List<ChiTietDatPhong> ChiTietDuocChon(object maDat)
    {
        if (maDat == null) return new List<ChiTietDatPhong>();
        return ChiTiet(BUS_KiemTra.MaDuocChon(maDat));
    }
    public List<Phong> PhongTheoChiTiet(ChiTietDatPhong ct)
    {
        if (ct == null) return new List<Phong>();
        return HienThiPhong().Where(x => x.MaLoaiPhong == ct.MaLoaiPhong).ToList();
    }
    public bool LuuPhanBo(long ma, object maDat, object maChiTiet, object maPhong, string ghiChu)
    {
        long dat = BUS_KiemTra.MaDuocChon(maDat);
        long chiTiet = BUS_KiemTra.MaDuocChon(maChiTiet);
        int phong = (int)BUS_KiemTra.MaDuocChon(maPhong);
        var ct = HienThiChiTietDatPhong(new ChiTietDatPhong { MaChiTietDatPhong = chiTiet });
        BUS_KiemTra.YeuCau(ct != null && ct.MaDatPhong == dat, "Chi tiết không thuộc đơn đặt phòng đã chọn.");
        var p = HienThiPhong().FirstOrDefault(x => x.MaPhong == phong);
        BUS_KiemTra.YeuCau(p != null && p.MaLoaiPhong == ct.MaLoaiPhong, "Phòng không đúng loại phòng đã đặt.");
        var xp = ma == 0 ? new XepPhong() : HienThiXepPhong(new XepPhong { MaXepPhong = ma });
        BUS_KiemTra.Chon(xp, "phân bổ còn tồn tại");
        xp.MaChiTietDatPhong = chiTiet;
        xp.MaPhong = phong;
        xp.GhiChu = ghiChu;
        return LuuPhanBo(xp);
    }

    public object ThongTinChiTiet(ChiTietDatPhong ct)
    {
        if (ct == null) return null;
        var loai = HienThiLoaiPhong(new LoaiPhong { MaLoaiPhong = ct.MaLoaiPhong });
        return new { loai.TenLoaiPhong, ct.SoLuongPhong, ct.NgayNhanDuKien, ct.NgayTraDuKien };
    }

}
