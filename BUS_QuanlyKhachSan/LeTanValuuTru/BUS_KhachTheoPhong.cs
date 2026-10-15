using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_KhachTheoPhong
{
    private readonly DAL_KhachTheoPhong dal = new DAL_KhachTheoPhong();
    public System.Data.DataView hienThiKTPAll(string tuKhoa = "", string trangThai = "")
    {
        var bang = dal.hienThiKTPAll();
        foreach (System.Data.DataRow dong in bang.Rows)
            dong["LoaiGiayTo"] = BUS_KhachHang.ChuanHoaLoaiGiayTo(Convert.ToString(dong["LoaiGiayTo"]));
        return BUS_KiemTra.LocBang(bang, tuKhoa, trangThai);
    }
    public List<Phong> HienThiPhong() => dal.HienThiPhong();
    public XepPhong HienThiXepPhong(XepPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiXepPhong(duLieu), "dữ liệu còn tồn tại");
    public Phong HienThiPhong(Phong duLieu) => BUS_KiemTra.Chon(dal.HienThiPhong(duLieu), "dữ liệu còn tồn tại");
    public ChiTietDatPhong HienThiChiTietDatPhong(ChiTietDatPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiChiTietDatPhong(duLieu), "dữ liệu còn tồn tại");
    public bool ChuyenPhong(long ma, int phongMoi, string ghiChu)
    {
        BUS_KiemTra.Ma(ma, "lượt lưu trú");
        BUS_KiemTra.Ma(phongMoi, "phòng mới");
        ghiChu = BUS_KiemTra.Chuoi(ghiChu, "ghi chú", 500);
        var xp = dal.HienThiXepPhong(new XepPhong { MaXepPhong = ma });
        BUS_KiemTra.YeuCau(xp.MaPhong != phongMoi, "Phòng mới phải khác phòng đang ở.");
        return BUS_KiemTra.KetQua(dal.ChuyenPhong(ma, phongMoi, ghiChu));
    }
    public bool SuaGhiChuLuuTru(long ma, string ghiChu)
    {
        BUS_KiemTra.Ma(ma, "lượt lưu trú");
        ghiChu = BUS_KiemTra.Chuoi(ghiChu, "ghi chú", 500);
        return BUS_KiemTra.KetQua(dal.SuaGhiChuLuuTru(ma, ghiChu));
    }
    public List<XepPhong> HienThiXepPhong() => dal.HienThiXepPhong();
    public bool NhanPhong(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.NhanPhong(ma));
    }
    public bool TraPhong(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.TraPhong(ma));
    }
    public List<XepPhong> XepPhongTheoTrangThai(string trangThai)
    {
        return HienThiXepPhong().Where(x => x.TrangThai == trangThai).ToList();
    }
    public List<Phong> PhongChuyenDen(long ma)
    {
        var xp = HienThiXepPhong(new XepPhong { MaXepPhong = ma });
        BUS_KiemTra.Chon(xp, "lượt lưu trú còn tồn tại");
        var ct = HienThiChiTietDatPhong(new ChiTietDatPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong });
        return HienThiPhong().Where(x => x.MaLoaiPhong == ct.MaLoaiPhong && x.MaPhong != xp.MaPhong).ToList();
    }
    public Phong PhongCuaLuot(XepPhong xp)
    {
        if (xp == null) return null;
        return HienThiPhong(new Phong { MaPhong = xp.MaPhong });
    }
    public bool NhanPhongDuocChon(object ma) => NhanPhong(BUS_KiemTra.MaDuocChon(ma));
    public bool TraPhongDuocChon(object ma) => TraPhong(BUS_KiemTra.MaDuocChon(ma));
    public bool ChuyenPhongDuocChon(long ma, object phongMoi, string ghiChu)
    {
        return ChuyenPhong(ma, (int)BUS_KiemTra.MaDuocChon(phongMoi), ghiChu);
    }

}
