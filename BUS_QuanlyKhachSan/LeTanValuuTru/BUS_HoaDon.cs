using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_HoaDon
{
    private readonly DAL_HoaDon dal = new DAL_HoaDon();
    public System.Data.DataView hienThiHDAll(string tuKhoa = "", string trangThai = "")
    {
        return BUS_KiemTra.LocBang(dal.hienThiHDAll(), tuKhoa, trangThai);
    }
    public List<DatPhong> HienThiDatPhong() => dal.HienThiDatPhong();
    public List<NhanVien> HienThiNhanVien() => dal.HienThiNhanVien();
    public List<UuDai> HienThiUuDai() => dal.HienThiUuDai();
    public List<DichVu> HienThiDichVu() => dal.HienThiDichVu();
    public List<GoiDichVu> HienThiGoiDichVu() => dal.HienThiGoiDichVu();
    public HoaDon HienThiHoaDon(HoaDon duLieu) => BUS_KiemTra.Chon(dal.HienThiHoaDon(duLieu), "dữ liệu còn tồn tại");
    public KhachHang HienThiKhachHang(KhachHang duLieu) => BUS_KiemTra.Chon(dal.HienThiKhachHang(duLieu), "dữ liệu còn tồn tại");
    public List<ChiTietHoaDon> ChiTietHoaDon(long ma) => dal.ChiTietHoaDon(ma);
    public decimal DaThu(long ma) => dal.DaThu(ma);
    public List<ChiTietHoaDon> KhoanThu(long ma) => dal.KhoanThu(ma);
    public static decimal TinhTong(IEnumerable<ChiTietHoaDon> chiTiet) => DAL_HoaDon.TinhTong(chiTiet);
    public static decimal TinhGiam(decimal tong, UuDai uu) => DAL_HoaDon.TinhGiam(tong, uu);
    public static void KiemTraDichVu(SuDungDichVu sd)
    {
        BUS_KiemTra.Chon(sd, "lượt dịch vụ");
        BUS_KiemTra.YeuCau(sd.MaDichVu.HasValue != sd.MaGoiDichVu.HasValue, "Mỗi dòng chỉ chọn một dịch vụ hoặc một gói dịch vụ.");
        BUS_KiemTra.Ma(sd.MaDichVu ?? sd.MaGoiDichVu ?? 0, "dịch vụ hoặc gói dịch vụ");
        BUS_KiemTra.YeuCau(sd.SoLuong > 0 && sd.SoLuong <= 9999999999.99m && decimal.Round(sd.SoLuong, 2) == sd.SoLuong,
            "Số lượng phải lớn hơn 0, tối đa 10 chữ số phần nguyên và 2 số lẻ.");
        BUS_KiemTra.Tien(sd.DonGia, "Đơn giá");
        BUS_KiemTra.Tien(Math.Round(sd.SoLuong * sd.DonGia, 2, MidpointRounding.AwayFromZero), "Thành tiền dịch vụ");
    }
    public static List<ChiTietHoaDon> KhoanThuDichVu(IEnumerable<SuDungDichVu> dichVu) =>
        dichVu.Select(x => new ChiTietHoaDon { LoaiChiPhi = x.MaGoiDichVu.HasValue ? "GoiDichVu" : "DichVu",
            SoLuong = x.SoLuong, DonGia = x.DonGia }).ToList();
    public bool TaoHoaDon(long maDat, int? nhanVien, int? uuDai, string ghiChu, List<SuDungDichVu> dichVuThem = null)
    {
        BUS_KiemTra.Ma(maDat, "đơn đặt phòng");
        BUS_KiemTra.Ma(nhanVien ?? 0, "nhân viên lập hóa đơn");
        if (uuDai.HasValue) BUS_KiemTra.Ma(uuDai.Value, "ưu đãi");
        ghiChu = BUS_KiemTra.Chuoi(ghiChu, "ghi chú", 500);
        foreach (var sd in dichVuThem ?? new List<SuDungDichVu>()) KiemTraDichVu(sd);
        BUS_KiemTra.YeuCau(!dal.DaLapHoaDon(maDat), "Đơn đặt phòng đã có hóa đơn.");
        decimal tong = TinhTong(KhoanThu(maDat)) + TinhTong(KhoanThuDichVu(dichVuThem ?? new List<SuDungDichVu>()));
        BUS_KiemTra.Tien(tong, "Tổng tiền hóa đơn");
        var uu = uuDai.HasValue ? HienThiUuDai().FirstOrDefault(x => x.MaUuDai == uuDai.Value) : null;
        BUS_KiemTra.YeuCau(!uuDai.HasValue || uu != null, "Ưu đãi không còn tồn tại.");
        BUS_KiemTra.YeuCau(TinhGiam(tong, uu) <= tong, "Tiền giảm không được vượt tổng hóa đơn.");
        return BUS_KiemTra.KetQua(dal.TaoHoaDon(maDat, nhanVien, uuDai, ghiChu, dichVuThem));
    }
    public bool ApDungUuDai(long ma, int? uuDai, string ghiChu)
    {
        BUS_KiemTra.Ma(ma, "hóa đơn");
        if (uuDai.HasValue) BUS_KiemTra.Ma(uuDai.Value, "ưu đãi");
        ghiChu = BUS_KiemTra.Chuoi(ghiChu, "ghi chú", 500);
        var hd = HienThiHoaDon(new HoaDon { MaHoaDon = ma });
        var uu = uuDai.HasValue ? HienThiUuDai().FirstOrDefault(x => x.MaUuDai == uuDai.Value) : null;
        BUS_KiemTra.YeuCau(!uuDai.HasValue || uu != null, "Ưu đãi không còn tồn tại.");
        decimal phaiThu = hd.TongTien - TinhGiam(hd.TongTien, uu);
        BUS_KiemTra.YeuCau(phaiThu >= 0 && phaiThu >= DaThu(ma), "Ưu đãi làm số phải thu âm hoặc thấp hơn số tiền đã thu.");
        return BUS_KiemTra.KetQua(dal.ApDungUuDai(ma, uuDai, ghiChu));
    }
    public bool ThuTien(long ma, decimal tien, int? nhanVien, string ghiChu)
    {
        BUS_KiemTra.Ma(ma, "hóa đơn");
        BUS_KiemTra.Ma(nhanVien ?? 0, "nhân viên thu tiền");
        BUS_KiemTra.Tien(tien, "Số tiền thu", true);
        ghiChu = BUS_KiemTra.Chuoi(ghiChu, "ghi chú", 500);
        var hd = HienThiHoaDon(new HoaDon { MaHoaDon = ma });
        BUS_KiemTra.YeuCau(hd.TrangThai != "Huy", "Hóa đơn đã hủy, không thể thu tiền.");
        BUS_KiemTra.YeuCau(tien <= hd.ThanhTien - dal.DaThu(ma), "Số tiền thu không được vượt số tiền còn thiếu.");
        return BUS_KiemTra.KetQua(dal.ThuTien(ma, tien, nhanVien, ghiChu));
    }
    public bool HuyHoaDon(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.HuyHoaDon(ma));
    }
    public void ThemDichVu(List<SuDungDichVu> ds, object maDat, DichVu dv, string soLuong)
    {
        BUS_KiemTra.MaDuocChon(maDat);
        BUS_KiemTra.Chon(dv, "dịch vụ");
        var sd = new SuDungDichVu { MaDichVu = dv.MaDichVu, DonGia = dv.DonGia,
            SoLuong = BUS_KiemTra.SoTien(soLuong, "Số lượng dịch vụ") };
        KiemTraDichVu(sd);
        ds.Add(sd);
    }
    public void ThemGoiDichVu(List<SuDungDichVu> ds, object maDat, GoiDichVu goi, string soLuong)
    {
        BUS_KiemTra.MaDuocChon(maDat);
        BUS_KiemTra.Chon(goi, "gói dịch vụ");
        var sd = new SuDungDichVu { MaGoiDichVu = goi.MaGoiDichVu, DonGia = goi.DonGia,
            SoLuong = BUS_KiemTra.SoTien(soLuong, "Số lượng gói dịch vụ") };
        KiemTraDichVu(sd);
        ds.Add(sd);
    }
    public List<object> DichVuDaChon(List<SuDungDichVu> ds, List<DichVu> dichVu, List<GoiDichVu> goi)
    {
        return ds.Select(x => (object)new {
            Loai = x.MaGoiDichVu.HasValue ? "Gói dịch vụ" : "Dịch vụ",
            NoiDung = x.MaGoiDichVu.HasValue
                ? goi.First(g => g.MaGoiDichVu == x.MaGoiDichVu).TenGoiDichVu
                : dichVu.First(d => d.MaDichVu == x.MaDichVu).TenDichVu,
            x.SoLuong, x.DonGia,
            ThanhTien = Math.Round(x.SoLuong * x.DonGia, 2, MidpointRounding.AwayFromZero)
        }).ToList();
    }
    public List<ChiTietHoaDon> KhoanThuDuocChon(DatPhong don)
    {
        if (don == null) return new List<ChiTietHoaDon>();
        var ds = KhoanThu(don.MaDatPhong);
        foreach (var ct in ds) ct.ThanhTien = TinhTong(new[] { ct });
        return ds;
    }
    public object XemTruocHoaDon(List<ChiTietHoaDon> daCo, List<SuDungDichVu> them, UuDai uu)
    {
        var ds = daCo.Concat(KhoanThuDichVu(them)).ToList();
        decimal tong = TinhTong(ds);
        decimal giam = TinhGiam(tong, uu);
        return new {
            TienPhong = TinhTong(ds.Where(x => x.LoaiChiPhi == "TienPhong")),
            TienDichVu = TinhTong(ds.Where(x => x.LoaiChiPhi == "DichVu")),
            TienGoi = TinhTong(ds.Where(x => x.LoaiChiPhi == "GoiDichVu")),
            TongTien = tong, TienGiam = giam, PhaiThu = tong - giam,
            DieuKien = uu?.DieuKien ?? ""
        };
    }
    public KhachHang KhachCuaDon(DatPhong don)
    {
        if (don == null) return null;
        return HienThiKhachHang(new KhachHang { MaKhachHang = don.MaKhachHangDat });
    }
    public bool LuuHoaDon(object maDat, object nhanVien, UuDai uu, string ghiChu, List<SuDungDichVu> them)
    {
        return TaoHoaDon(BUS_KiemTra.MaDuocChon(maDat), (int)BUS_KiemTra.MaDuocChon(nhanVien), uu?.MaUuDai, ghiChu, them);
    }
    public object ThongTinThuTien(long ma)
    {
        var hd = HienThiHoaDon(new HoaDon { MaHoaDon = ma });
        BUS_KiemTra.Chon(hd, "hóa đơn còn tồn tại");
        decimal daThu = DaThu(ma);
        decimal conThieu = Math.Max(0, hd.ThanhTien - daThu);
        return new { MaHoaDon = ma, PhaiThu = hd.ThanhTien, DaThu = daThu, ConThieu = conThieu,
            SoTien = conThieu.ToString("0.##"), DuocThu = conThieu > 0 && hd.TrangThai != "Huy" };
    }
    public bool ThuTienNhap(long ma, string soTien, object nhanVien, string ghiChu)
    {
        return ThuTien(ma, BUS_KiemTra.SoTien(soTien), (int)BUS_KiemTra.MaDuocChon(nhanVien), ghiChu);
    }

    public List<string> NoiDungIn(long ma)
    {
        var hd = HienThiHoaDon(new HoaDon { MaHoaDon = ma });
        var kh = HienThiKhachHang(new KhachHang { MaKhachHang = hd.MaKhachHang });
        var dong = new List<string> {
            $"HÓA ĐƠN #{ma} — {hd.NgayLap:dd/MM/yyyy HH:mm}",
            $"Khách hàng: {kh.HoTen} · Đặt phòng: {hd.MaDatPhong}"
        };
        foreach (var ct in ChiTietHoaDon(ma))
            dong.Add($"{ct.NoiDung} | {ct.SoLuong} × {ct.DonGia:N2} = {ct.ThanhTien:N2} đ");
        dong.Add($"Tổng: {hd.TongTien:N2} đ | Giảm: {hd.TienGiam:N2} đ | Phải thu: {hd.ThanhTien:N2} đ");
        dong.Add($"Đã thu: {DaThu(ma):N2} đ | Trạng thái: {hd.TrangThai}");
        return dong;
    }

}
