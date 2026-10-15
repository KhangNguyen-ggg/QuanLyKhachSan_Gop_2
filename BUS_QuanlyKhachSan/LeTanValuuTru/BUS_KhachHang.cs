using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_KhachHang
{
    public static IReadOnlyList<string> LoaiGiayToChoPhep { get; } = Array.AsReadOnly(new[] { "Hộ chiếu", "CCCD", "CMND", "Khác" });
    public static string ChuanHoaLoaiGiayTo(string loai)
    {
        return loai?.Trim().ToUpperInvariant() switch
        {
            "PASSPORT" or "HOCHIEU" or "HO CHIEU" or "HỘ CHIẾU" => "Hộ chiếu",
            "CCCD" => "CCCD",
            "CMND" => "CMND",
            "OTHER" or "KHAC" or "KHÁC" => "Khác",
            _ => loai?.Trim() ?? ""
        };
    }
    public static string HuongDanSoGiayTo(string loai) => ChuanHoaLoaiGiayTo(loai) switch
    {
        "CCCD" => "CCCD phải gồm đúng 12 chữ số (0–9).",
        "CMND" => "CMND phải gồm 9 hoặc 12 chữ số (0–9).",
        "Hộ chiếu" => "Hộ chiếu: nhập 6–9 ký tự chữ không dấu hoặc số, không có khoảng trắng.",
        "Khác" => "Giấy tờ khác: 1–50 ký tự chữ không dấu, số, dấu - hoặc /; bắt đầu bằng chữ hoặc số.",
        _ => "Chọn loại giấy tờ rồi nhập mã số ghi trên giấy tờ."
    };
    public static string KiemTraSoGiayTo(string loai, string so)
    {
        loai = ChuanHoaLoaiGiayTo(loai);
        BUS_KiemTra.YeuCau(LoaiGiayToChoPhep.Contains(loai), "Vui lòng chọn loại giấy tờ.");
        so = BUS_KiemTra.Chuoi(so, "số giấy tờ", 50, true);
        string mau = loai switch
        {
            "CCCD" => @"\A[0-9]{12}\z",
            "CMND" => @"\A(?:[0-9]{9}|[0-9]{12})\z",
            "Hộ chiếu" => @"\A[A-Za-z0-9]{6,9}\z",
            _ => @"\A[A-Za-z0-9][A-Za-z0-9/-]{0,49}\z"
        };
        BUS_KiemTra.YeuCau(System.Text.RegularExpressions.Regex.IsMatch(so, mau), HuongDanSoGiayTo(loai));
        return so; // Giữ nguyên số 0 đầu và ký tự mã; không chuyển sang số nguyên.
    }
    private readonly DAL_Khachhang dal = new DAL_Khachhang();
    public List<KhachHang> hienThiKHAll(string tuKhoa = "", string trangThai = "")
    {
        var ds = dal.hienThiKHAll();
        foreach (var kh in ds) kh.LoaiGiayTo = ChuanHoaLoaiGiayTo(kh.LoaiGiayTo);
        return ds.Where(kh => ($"{kh.MaKhachHang} {kh.HoTen} {kh.LoaiGiayTo} {kh.SoGiayTo} {kh.SoDienThoai} {kh.Email} {kh.GioiTinh} {kh.NgaySinh} {kh.QuocTich} {kh.DiaChi} {kh.GhiChu} {kh.TrangThai}")
            .Contains((tuKhoa ?? "").Trim(), StringComparison.CurrentCultureIgnoreCase)
            && (string.IsNullOrEmpty(trangThai) || kh.TrangThai.ToString() == trangThai))
            .OrderByDescending(kh => kh.MaKhachHang).ToList();
    }
    public KhachHang HienThiKhachHang(KhachHang duLieu) => BUS_KiemTra.Chon(dal.HienThiKhachHang(duLieu), "dữ liệu còn tồn tại");
    public static void KiemTraKhachHang(KhachHang kh)
    {
        BUS_KiemTra.Chon(kh, "khách hàng");
        kh.HoTen = BUS_KiemTra.Chuoi(kh.HoTen, "họ tên", 150, true);
        BUS_KiemTra.YeuCau(System.Text.RegularExpressions.Regex.IsMatch(kh.HoTen, @"^[\p{L}\p{M}]+(?:[ '’-][\p{L}\p{M}]+)*$"),
            "Họ tên chỉ được chứa chữ, khoảng trắng, dấu gạch nối hoặc dấu nháy.");
        kh.LoaiGiayTo = ChuanHoaLoaiGiayTo(kh.LoaiGiayTo);
        BUS_KiemTra.YeuCau(LoaiGiayToChoPhep.Contains(kh.LoaiGiayTo), "Vui lòng chọn loại giấy tờ: Hộ chiếu, CCCD, CMND hoặc Khác.");
        kh.SoGiayTo = KiemTraSoGiayTo(kh.LoaiGiayTo, kh.SoGiayTo);
        kh.SoDienThoai = BUS_KiemTra.DienThoai(kh.SoDienThoai);
        kh.Email = BUS_KiemTra.Gmail(kh.Email);
        BUS_KiemTra.YeuCau(!kh.NgaySinh.HasValue || kh.NgaySinh.Value.Date <= DateTime.Today, "Ngày sinh không được ở tương lai.");
        BUS_KiemTra.YeuCau(kh.GioiTinh is "M" or "F" or "O", "Vui lòng chọn giới tính.");
        kh.QuocTich = BUS_KiemTra.Chuoi(kh.QuocTich, "quốc tịch", 80);
        kh.DiaChi = BUS_KiemTra.Chuoi(kh.DiaChi, "địa chỉ", 300);
        kh.GhiChu = BUS_KiemTra.Chuoi(kh.GhiChu, "ghi chú", 500);
    }
    public bool LuuKhachHang(KhachHang kh)
    {
        KiemTraKhachHang(kh);
        return BUS_KiemTra.KetQua(dal.LuuKhachHang(kh));
    }
    public bool XoaKhachHang(int ma)
    {
        BUS_KiemTra.Ma(ma, "dòng dữ liệu cần thao tác");
        return BUS_KiemTra.KetQua(dal.XoaKhachHang(ma));
    }
    public KhachHang DuLieuForm(int ma)
    {
        if (ma == 0) return new KhachHang { GioiTinh = "O", TrangThai = true };
        var kh = HienThiKhachHang(new KhachHang { MaKhachHang = ma });
        BUS_KiemTra.Chon(kh, "khách hàng còn tồn tại");
        kh.LoaiGiayTo = ChuanHoaLoaiGiayTo(kh.LoaiGiayTo);
        if (!LoaiGiayToChoPhep.Contains(kh.LoaiGiayTo)) kh.LoaiGiayTo = "Khác";
        return kh;
    }

}
