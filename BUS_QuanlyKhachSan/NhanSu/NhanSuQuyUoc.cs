using System.Globalization;

namespace BUS_QuanlyKhachSan.NhanSu;

// Thêm: một nơi dùng chung mã SQL, tên hiển thị, tìm không dấu và nhập giờ cho toàn bộ Nhân sự.
public static class NhanSuQuyUoc
{
    public static KeyValuePair<string, string>[] TrangThai(string bang) => bang switch
    {
        "NhanVien" => [new("DangLam", "Đang làm"), new("TamNghi", "Tạm nghỉ"), new("NghiViec", "Đã nghỉ")],
        "CaLamViec" => [new("True", "Đang hoạt động"), new("False", "Ngừng hoạt động")],
        "LichLamViec" => [new("DaXep", "Đã xếp"), new("Nghi", "Nghỉ"), new("Huy", "Hủy")],
        "ChamCong" => [new("DuCong", "Đủ công"), new("ThieuCong", "Thiếu công"), new("Nghi", "Vắng"), new("NghiPhep", "Nghỉ phép"), new("KhongCham", "Không chấm")],
        "BangLuong" => [new("TamTinh", "Tạm tính"), new("DaDuyet", "Đã duyệt"), new("DaThanhToan", "Đã thanh toán")],
        "KhenThuong" => [new("ChoDuyet", "Chờ duyệt"), new("DaDuyet", "Đã duyệt"), new("TuChoi", "Từ chối")],
        _ => throw new ArgumentException("Danh mục trạng thái không hợp lệ.")
    };

    public static KeyValuePair<string, string>[] HinhThuc =>
        [new("TienThuong", "Tiền thưởng"), new("BieuDuong", "Biểu dương"), new("GiayKhen", "Giấy khen")];

    public static void KiemTraTrangThai(string bang, string giaTri) => BUS_KiemTra.YeuCau(
        TrangThai(bang).Any(x => x.Key == giaTri), "Vui lòng chọn trạng thái hợp lệ.");

    public static string TenTrangThai(string bang, string giaTri) => TrangThai(bang).FirstOrDefault(x => x.Key == giaTri).Value ?? giaTri;

    public static List<T> Loc<T>(Func<IEnumerable<T>> nguon, string tuKhoa, string trangThai,
        Func<T, string> noiDung, Func<T, string> layTrangThai, string bang)
    {
        tuKhoa = (tuKhoa ?? "").Trim();
        BUS_KiemTra.YeuCau(BUS_PhongBan.TuKhoaTimKiemHopLe(tuKhoa), "Từ khóa chỉ được chứa chữ, số và khoảng trắng.");
        string BoChuD(string s) => (s ?? "").Replace('đ', 'd').Replace('Đ', 'D');
        return nguon().Where(x => (string.IsNullOrEmpty(trangThai) || layTrangThai(x) == trangThai) &&
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(BoChuD(noiDung(x) + " " + TenTrangThai(bang, layTrangThai(x))),
                BoChuD(tuKhoa), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0).ToList();
    }

    public static TimeSpan Gio(string giaTri, string ten)
    {
        BUS_KiemTra.YeuCau(TimeSpan.TryParseExact((giaTri ?? "").Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out var gio) &&
            gio >= TimeSpan.Zero && gio < TimeSpan.FromDays(1), $"{ten}: nhập giờ theo HH:mm (00:00–23:59).");
        return gio;
    }

    public static DateTime Ngay(DateTime? giaTri, string ten)
    {
        var ngay = BUS_KiemTra.Ngay(giaTri, ten).Date;
        BUS_KiemTra.YeuCau(ngay.Year is >= 2000 and <= 2100, $"{ten} phải trong năm 2000–2100.");
        return ngay;
    }
}
