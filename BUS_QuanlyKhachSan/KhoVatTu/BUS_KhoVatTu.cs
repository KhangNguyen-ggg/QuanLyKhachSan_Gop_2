using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using DAL_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using static BUS_QuanlyKhachSan.BUS_KiemTra;

namespace BUS_QuanlyKhachSan.KhoVatTu;

public class BUS_KhoVatTu
{
    private readonly DAL_KhoVatTu dal = new();
    public List<T> DanhMuc<T>() where T : class => dal.TatCa<T>();
    public T TheoId<T>(long ma) where T : class { Ma(ma, "dữ liệu"); return Chon(dal.TheoId<T>(ma), "dữ liệu còn tồn tại"); }
    public List<VatTuHienThi> VatTu() => dal.VatTu();
    public List<DinhMucHienThi> DinhMuc() => dal.DinhMuc();
    public List<NhaCungCapVatTuHienThi> NguonCung(int ma) => dal.NguonCung(ma);
    public List<TonKhoHienThi> Ton() => dal.Ton();
    public List<NhatKyKhoHienThi> NhatKy(int kho, int vatTu) => dal.NhatKy(kho, vatTu);
    public List<PhieuKhoHienThi> Phieu() => dal.Phieu();
    public List<CapNoiBoHienThi> Cap() => dal.Cap();
    public List<ChiTietPhieuKhoHienThi> ChiTiet(long ma) => dal.ChiTiet(ma);
    public List<ChiTietCapHienThi> ChiTietCap(long ma) => dal.ChiTietCap(ma);
    public List<ThamChieuKho> ThamChieu(bool dichVu) => dal.ThamChieu(dichVu);

    // Thêm: dùng chung tìm không dấu, chặn ký tự đặc biệt như Nhân sự, giữ thứ tự từ DAL.
    public static List<T> Loc<T>(Func<IEnumerable<T>> nguon, string tuKhoa, string trangThai, Func<T, string> noiDung, Func<T, string> layTrangThai)
    {
        tuKhoa = (tuKhoa ?? "").Trim();
        YeuCau(NhanSu.BUS_PhongBan.TuKhoaTimKiemHopLe(tuKhoa), "Từ khóa chỉ được chứa chữ, số và khoảng trắng.");
        string BoD(string x) => (x ?? "").Replace('đ', 'd').Replace('Đ', 'D');
        return nguon().Where(x => CultureInfo.InvariantCulture.CompareInfo.IndexOf(BoD(noiDung(x)), BoD(tuKhoa), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0 &&
            (string.IsNullOrEmpty(trangThai) || layTrangThai(x) == trangThai)).ToList();
    }
    public static Dictionary<string, string> TrangThai => new() { ["True"] = "Đang hoạt động", ["False"] = "Ngừng hoạt động" };
    public static Dictionary<string, string> TrangThaiPhieu => new() { ["MOI"] = "Mới", ["DA_XAC_NHAN"] = "Đã xác nhận", ["DA_HUY"] = "Đã hủy" };
    public static Dictionary<string, string> LoaiPhieu => new() { ["NHAP"] = "Nhập kho", ["XUAT"] = "Xuất kho", ["DIEU_CHINH_TANG"] = "Điều chỉnh tăng", ["DIEU_CHINH_GIAM"] = "Điều chỉnh giảm" };
    public static Dictionary<string, string> MucDichCap => new() { ["DON_PHONG"] = "Dọn phòng", ["DICH_VU"] = "Dịch vụ", ["KHAC"] = "Khác" };

    public static decimal SoLuong(string chuoi, string ten, bool choPhepKhong = false, int soChuSoNguyen = 14)
    {
        string s = chuoi?.Trim() ?? "";
        YeuCau(Regex.IsMatch(s, @"\A[0-9]+(?:[.,][0-9]{1,4})?\z") &&
            decimal.TryParse(s.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _), ten + " phải là số, tối đa 4 số lẻ; dùng dấu phẩy hoặc chấm, không phân cách hàng nghìn.");
        decimal so = decimal.Parse(s.Replace(',', '.'), CultureInfo.InvariantCulture);
        KiemTraSoLuong(so, ten, choPhepKhong, soChuSoNguyen);
        return so;
    }
    private static void KiemTraSoLuong(decimal so, string ten, bool choPhepKhong = false, int chuSo = 14) =>
        YeuCau((choPhepKhong ? so >= 0 : so > 0) && so < (chuSo == 8 ? 100000000m : 100000000000000m) && decimal.Round(so, 4) == so, ten + (choPhepKhong ? " phải từ 0" : " phải lớn hơn 0") + $", tối đa {chuSo} chữ số nguyên và 4 số lẻ.");
    private static void TienHang(decimal soLuong, decimal? gia)
    {
        if (gia.HasValue) Tien(gia.Value, "Đơn giá");
        YeuCau(!gia.HasValue || gia == 0 || soLuong <= 9999999999999999.99m / gia.Value, "Thành tiền một dòng vượt giới hạn 16 chữ số nguyên.");
    }

    public void Luu(Kho x)
    {
        Chon(x, "kho"); YeuCau(x.MaKho >= 0, "Mã kho không hợp lệ.");
        x.TenKho = Chuoi(x.TenKho, "tên kho", 100, true); x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 500);
        dal.LuuDanhMuc<Kho>(x, x.MaKho);
    }
    public void Luu(NhomVatTu x)
    {
        Chon(x, "nhóm vật tư"); YeuCau(x.MaNhomVatTu >= 0, "Mã nhóm không hợp lệ.");
        x.TenNhomVatTu = Chuoi(x.TenNhomVatTu, "tên nhóm vật tư", 100, true); x.MoTa = Chuoi(x.MoTa, "mô tả", 500);
        dal.LuuDanhMuc<NhomVatTu>(x, x.MaNhomVatTu);
    }
    public void Luu(VatTu x)
    {
        Chon(x, "vật tư"); YeuCau(x.MaLoaiVatTu >= 0, "Mã vật tư không hợp lệ.");
        Ma(x.MaNhomVatTu, "nhóm vật tư"); if (x.MaKhoMacDinh.HasValue) Ma(x.MaKhoMacDinh.Value, "kho mặc định");
        x.TenVatTu = Chuoi(x.TenVatTu, "tên vật tư", 150, true); x.DonViTinh = Chuoi(x.DonViTinh, "đơn vị tính", 50, true); x.MoTa = Chuoi(x.MoTa, "mô tả", 500);
        KiemTraSoLuong(x.SoLuongToiThieu, "Tồn tối thiểu", true); dal.LuuDanhMuc<VatTu>(x, x.MaLoaiVatTu);
    }
    public void Luu(NhaCungCap x)
    {
        Chon(x, "nhà cung cấp"); YeuCau(x.MaNhaCungCap >= 0, "Mã nhà cung cấp không hợp lệ.");
        x.TenNhaCungCap = Chuoi(x.TenNhaCungCap, "tên nhà cung cấp", 200, true);
        x.NguoiDaiDien = Chuoi(x.NguoiDaiDien, "người đại diện", 150);
        x.LoaiCungCap = Chuoi(x.LoaiCungCap, "loại cung cấp", 100);
        x.SoDienThoai = Chuoi(x.SoDienThoai, "số điện thoại", 20);
        if (x.SoDienThoai.Length > 0) DienThoai(x.SoDienThoai);
        x.Email = Chuoi(x.Email, "email", 150);
        YeuCau(x.Email.Length == 0 || (MailAddress.TryCreate(x.Email, out var email) && email.Address == x.Email && x.Email.All(c => c < 128)), "Email nhà cung cấp không hợp lệ.");
        x.DiaChi = Chuoi(x.DiaChi, "địa chỉ", 300); x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 500);
        dal.LuuDanhMuc<NhaCungCap>(x, x.MaNhaCungCap);
    }
    public void Xoa<T>(long ma) where T : class { Ma(ma, "danh mục"); dal.XoaDanhMuc<T>(ma); }
    public void Luu(TieuHaoVatTu x, bool them)
    {
        Chon(x, "định mức"); Ma(x.MaDichVu, "dịch vụ"); Ma(x.MaVatTu, "vật tư");
        KiemTraSoLuong(x.SoLuongTieuHao, "Số lượng mỗi lần", false, 8);
        x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 300); dal.LuuDinhMuc(x, them);
    }
    public void Xoa(TieuHaoVatTu x) { Chon(x, "định mức"); Ma(x.MaDichVu, "dịch vụ"); Ma(x.MaVatTu, "vật tư"); dal.XoaDinhMuc(x.MaDichVu, x.MaVatTu); }
    public void Luu(VatTu_NhaCungCap x, bool them)
    {
        Chon(x, "nguồn cung"); Ma(x.MaNhaCungCap, "nhà cung cấp"); Ma(x.MaVatTu, "vật tư");
        x.MaSanPhamNCC = Chuoi(x.MaSanPhamNCC, "mã sản phẩm NCC", 100); YeuCau(x.MaSanPhamNCC.All(c => c >= 32 && c <= 126), "Mã sản phẩm NCC chỉ nhận ký tự ASCII.");
        if (x.DonGiaNhap.HasValue) Tien(x.DonGiaNhap.Value, "Đơn giá nhập");
        x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 300); dal.LuuNguonCung(x, them);
    }
    public void Xoa(VatTu_NhaCungCap x) { Chon(x, "nguồn cung"); Ma(x.MaNhaCungCap, "nhà cung cấp"); Ma(x.MaVatTu, "vật tư"); dal.XoaNguonCung(x.MaVatTu, x.MaNhaCungCap); }

    public void Luu(PhieuNhapXuat x)
    {
        Chon(x, "phiếu"); YeuCau(x.MaPhieu >= 0, "Mã phiếu không hợp lệ.");
        YeuCau(LoaiPhieu.ContainsKey(x.LoaiPhieu ?? ""), "Loại phiếu không hợp lệ.");
        Ma(x.MaKho, "kho"); Ma(x.MaNhanVien, "nhân viên");
        if (x.MaNhaCungCap.HasValue) Ma(x.MaNhaCungCap.Value, "nhà cung cấp");
        YeuCau(x.NgayPhieu >= new DateTime(1900, 1, 1) && x.NgayPhieu.Date <= DateTime.Today, "Ngày phiếu phải từ năm 1900 đến hôm nay.");
        x.MucDich = Chuoi(x.MucDich, "mục đích", 150); x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 500);
        dal.LuuPhieu(x);
    }
    public void Luu(ChiTietPhieuKho x)
    {
        Chon(x, "chi tiết phiếu"); Ma(x.MaPhieu, "phiếu"); Ma(x.MaVatTu, "vật tư"); YeuCau(x.MaChiTietPhieuKho >= 0, "Mã dòng không hợp lệ.");
        KiemTraSoLuong(x.SoLuong, "Số lượng"); TienHang(x.SoLuong, x.DonGia);
        x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 300); dal.LuuChiTiet(x);
    }
    public void Luu(ChiTietCapVatTuNoiBo x)
    {
        Chon(x, "chi tiết cấp"); Ma(x.MaPhieuCap, "phiếu cấp"); Ma(x.MaVatTu, "vật tư"); YeuCau(x.MaChiTietCap >= 0, "Mã dòng không hợp lệ.");
        KiemTraSoLuong(x.SoLuong, "Số lượng"); dal.LuuChiTietCap(x);
    }
    public void XoaChiTiet(long ma, long phieu, bool cap = false) { Ma(ma, "chi tiết"); Ma(phieu, "phiếu"); dal.XoaChiTiet(ma, phieu, cap); }
    public void TaoCap(PhieuCapVatTuNoiBo x, int vatTu, decimal soLuong)
    {
        Chon(x, "phiếu cấp"); Ma(x.MaKho, "kho"); Ma(x.MaNhanVien, "nhân viên"); Ma(vatTu, "vật tư"); KiemTraSoLuong(soLuong, "Số lượng");
        YeuCau(MucDichCap.ContainsKey(x.MucDich ?? ""), "Mục đích cấp không hợp lệ.");
        YeuCau((x.MaDatPhong == null || x.MucDich == "DON_PHONG") && (x.MaSuDungDichVu == null || x.MucDich == "DICH_VU"), "Tham chiếu không khớp mục đích cấp.");
        if (x.MaDatPhong.HasValue) Ma(x.MaDatPhong.Value, "đặt phòng");
        if (x.MaSuDungDichVu.HasValue) Ma(x.MaSuDungDichVu.Value, "sử dụng dịch vụ");
        x.GhiChu = Chuoi(x.GhiChu, "ghi chú", 500); dal.TaoCap(x, vatTu, soLuong);
    }
    public void XuLyPhieu(long ma, bool cap, bool huy, int? nguoiXacNhan = null)
    {
        Ma(ma, "phiếu");
        if (nguoiXacNhan.HasValue) Ma(nguoiXacNhan.Value, "nhân viên xác nhận");
        dal.XuLyPhieu(ma, cap, huy, nguoiXacNhan);
    }
    public void NhapXuat(string loai, int kho, int vatTu, decimal soLuong, int nhanVien, int? ncc, decimal? gia, string mucDich, string ghiChu)
    {
        YeuCau(loai is "NHAP" or "XUAT", "Chọn Nhập hoặc Xuất.");
        Ma(kho, "kho"); Ma(vatTu, "vật tư"); Ma(nhanVien, "nhân viên");
        if (ncc.HasValue) Ma(ncc.Value, "nhà cung cấp");
        KiemTraSoLuong(soLuong, "Số lượng"); TienHang(soLuong, gia);
        dal.NhapXuat(loai, kho, vatTu, soLuong, nhanVien, ncc, gia, Chuoi(mucDich, "mục đích", 150), Chuoi(ghiChu, "ghi chú", 500));
    }
    public void KiemKe(int kho, int vatTu, decimal ton, int nhanVien, string lyDo)
    {
        Ma(kho, "kho"); Ma(vatTu, "vật tư"); Ma(nhanVien, "nhân viên"); KiemTraSoLuong(ton, "Tồn thực tế", true);
        dal.KiemKe(kho, vatTu, ton, nhanVien, Chuoi(lyDo, "lý do kiểm kê", 500, true));
    }
    public void ChuyenKho(int xuat, int nhap, int vatTu, decimal soLuong, int nhanVien, string ghiChu)
    {
        Ma(xuat, "kho xuất"); Ma(nhap, "kho nhập"); Ma(vatTu, "vật tư"); Ma(nhanVien, "nhân viên");
        YeuCau(xuat != nhap, "Kho xuất và kho nhập phải khác nhau.");
        KiemTraSoLuong(soLuong, "Số lượng chuyển");
        dal.ChuyenKho(xuat, nhap, vatTu, soLuong, nhanVien, Chuoi(ghiChu, "ghi chú", 500));
    }
}

