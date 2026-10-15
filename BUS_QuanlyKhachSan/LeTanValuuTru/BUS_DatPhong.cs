using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_DatPhong
{
    private readonly DAL_Datphong dal = new DAL_Datphong();
    public System.Data.DataView hienThiDPAll(string tuKhoa = "", string trangThai = "")
    {
        return BUS_KiemTra.LocBang(dal.hienThiDPAll(), tuKhoa, trangThai);
    }
    public List<KhachHang> HienThiKhachHang() => dal.HienThiKhachHang();
    public List<LoaiPhong> HienThiLoaiPhong() => dal.HienThiLoaiPhong();
    public List<XepPhong> HienThiXepPhong() => dal.HienThiXepPhong();
    public DatPhong HienThiDatPhong(DatPhong duLieu) => BUS_KiemTra.Chon(dal.HienThiDatPhong(duLieu), "dữ liệu còn tồn tại");
    public Phong HienThiPhong(Phong duLieu) => BUS_KiemTra.Chon(dal.HienThiPhong(duLieu), "dữ liệu còn tồn tại");
    public List<ChiTietDatPhong> ChiTiet(long ma) => dal.ChiTiet(ma);
    public System.Data.DataView ChiTietHienThi(long ma) => dal.ChiTietHienThi(ma).DefaultView;
    public static void KiemTraChiTiet(ChiTietDatPhong ct)
    {
        BUS_KiemTra.Chon(ct, "chi tiết đặt phòng");
        BUS_KiemTra.Ma(ct.MaLoaiPhong, "loại phòng");
        BUS_KiemTra.YeuCau(ct.NgayNhanDuKien != default && ct.NgayTraDuKien.Date > ct.NgayNhanDuKien.Date,
            "Ngày trả phải sau ngày nhận ít nhất một ngày.");
        BUS_KiemTra.YeuCau(ct.SoLuongPhong > 0, "Số lượng phòng phải lớn hơn 0.");
        BUS_KiemTra.YeuCau(ct.NguoiLon > 0, "Số người lớn phải lớn hơn 0.");
        BUS_KiemTra.YeuCau(ct.TreEm >= 0, "Số trẻ em không được âm.");
        BUS_KiemTra.Tien(ct.DonGiaDuKien, "Đơn giá");
        ct.GhiChu = BUS_KiemTra.Chuoi(ct.GhiChu, "ghi chú chi tiết", 500);
    }
    public static void KiemTraDatPhong(DatPhong dp, List<ChiTietDatPhong> chiTiet)
    {
        BUS_KiemTra.Chon(dp, "đơn đặt phòng");
        BUS_KiemTra.Ma(dp.MaKhachHangDat, "khách đặt phòng");
        dp.GhiChu = BUS_KiemTra.Chuoi(dp.GhiChu, "ghi chú đặt phòng", 1000);
        BUS_KiemTra.YeuCau(chiTiet != null && chiTiet.Count > 0, "Hãy thêm ít nhất một loại phòng trước khi lưu.");
        foreach (var ct in chiTiet)
        {
            KiemTraChiTiet(ct);
            if (dp.MaDatPhong == 0)
                BUS_KiemTra.YeuCau(ct.NgayNhanDuKien.Date >= DateTime.Today, "Ngày nhận của đơn mới không được ở quá khứ.");
        }
    }
    public bool LuuDatPhong(DatPhong dp, List<ChiTietDatPhong> chiTiet)
    {
        KiemTraDatPhong(dp, chiTiet);
        return BUS_KiemTra.KetQua(dal.LuuDatPhong(dp, chiTiet));
    }
    public bool XacNhan(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.XacNhan(ma));
    }
    public bool HuyDatPhong(long ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.HuyDatPhong(ma));
    }
    public ChiTietDatPhong TaoChiTiet(object maLoai, DateTime? ngayNhan, DateTime? ngayTra,
        string soPhong, string nguoiLon, string treEm, string donGia)
    {
        var ct = new ChiTietDatPhong
        {
            MaLoaiPhong = (int)BUS_KiemTra.MaDuocChon(maLoai),
            NgayNhanDuKien = BUS_KiemTra.Ngay(ngayNhan, "ngày nhận").Date.AddHours(14),
            NgayTraDuKien = BUS_KiemTra.Ngay(ngayTra, "ngày trả").Date.AddHours(12),
            SoLuongPhong = BUS_KiemTra.SoNguyen(soPhong, "Số phòng"),
            NguoiLon = BUS_KiemTra.SoNguyen(nguoiLon, "Người lớn"),
            TreEm = BUS_KiemTra.SoNguyen(treEm, "Trẻ em"),
            DonGiaDuKien = BUS_KiemTra.SoTien(donGia, "Đơn giá")
        };
        KiemTraChiTiet(ct);
        return ct;
    }
    public List<object> HienThiChiTietTam(List<ChiTietDatPhong> chiTiet, List<LoaiPhong> loaiPhong)
    {
        return chiTiet.Select(ct => (object)new {
            TenLoaiPhong = loaiPhong.FirstOrDefault(x => x.MaLoaiPhong == ct.MaLoaiPhong)?.TenLoaiPhong,
            ct.NgayNhanDuKien, ct.NgayTraDuKien, ct.SoLuongPhong, ct.NguoiLon, ct.TreEm, ct.DonGiaDuKien
        }).ToList();
    }
    public bool LuuDatPhong(long ma, object maKhach, string ghiChu, List<ChiTietDatPhong> chiTiet)
    {
        var don = ma == 0 ? new DatPhong() : HienThiDatPhong(new DatPhong { MaDatPhong = ma });
        BUS_KiemTra.Chon(don, "đơn đặt phòng còn tồn tại");
        don.MaKhachHangDat = (int)BUS_KiemTra.MaDuocChon(maKhach);
        don.GhiChu = ghiChu;
        return LuuDatPhong(don, chiTiet);
    }

}
