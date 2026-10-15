using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyKhachSan.Helpers
{
    public static class ThemeHelper
    {
        public static string Vnd(decimal amount) => amount.ToString("N0") + " ₫";

        public static string StatusText(string s) => s switch
        {
            "DaCheckIn" => "Đang ở",
            "DaCheckOut" => "Đã trả phòng",
            "DaXacNhan" => "Đã xác nhận",
            "ChoXacNhan" => "Chờ xác nhận",
            "DaHuy" => "Đã hủy",
            "Huy" => "Hủy",
            "DaThanhToan" => "Đã thanh toán",
            "ChuaThanhToan" => "Chưa thanh toán",
            "ThanhToanMotPhan" => "Thanh toán một phần",
            "ThanhCong" => "Thành công",
            "ChoXuLy" => "Chờ xử lý",
            "DangXuLy" => "Đang xử lý",
            "HoanThanh" => "Hoàn thành",
            "DangLam" => "Đang làm",
            "NghiViec" => "Nghỉ việc",
            "TamNghi" => "Tạm nghỉ",
            "SanSang" => "Sẵn sàng",
            "DangSuDung" => "Đang sử dụng",
            "BaoTri" => "Bảo trì",
            "NgungKhaiThac" => "Ngừng khai thác",
            "Sach" => "Sạch",
            "CanDon" => "Cần dọn",
            "DangDon" => "Đang dọn",
            "Khoa" => "Khóa",
            "HoatDong" => "Hoạt động",
            "DaXep" => "Đã xếp",
            "PhanTram" => "Phần trăm",
            "SoTien" => "Số tiền",
            "Thap" => "Thấp",
            "TrungBinh" => "Trung bình",
            "Cao" => "Cao",
            "KhanCap" => "Khẩn cấp",
            "DonPhong" => "Dọn phòng",
            "VeSinhDinhKy" => "Vệ sinh định kỳ",
            "BoSungVatDung" => "Bổ sung vật dụng",
            "ChoLam" => "Chờ làm",
            "DatTruoc" => "Đặt trước",
            "DaSuDung" => "Đã sử dụng",
            "ThanhLy" => "Thanh lý",
            "Hong" => "Hỏng",
            "TamTinh" => "Tạm tính",
            "DaDuyet" => "Đã duyệt",
            "DuCong" => "Đủ công",
            "ThieuCong" => "Thiếu công",
            "Nghi" => "Nghỉ",
            "NghiPhep" => "Nghỉ phép",
            "KhongCham" => "Không chấm",
            "M" => "Nam",
            "F" => "Nữ",
            "O" => "Khác",

            "DA_NHAN_PHONG"       => "Đang ở",
            "DA_TRA_PHONG"        => "Đã trả phòng",
            "DA_XAC_NHAN"         => "Đã xác nhận",
            "CHO_XAC_NHAN"        => "Chờ xác nhận",
            "HUY"                 => "Đã huỷ",
            "NO_SHOW"             => "Không đến",
            "DA_THANH_TOAN"       => "Đã thanh toán",
            "CHUA_THANH_TOAN"     => "Chưa thanh toán",
            "THANH_TOAN_MOT_PHAN" => "TT một phần",
            "DANG_LAM"            => "Đang làm việc",
            "NGHI_VIEC"           => "Nghỉ việc",
            "TAM_NGHI"            => "Tạm nghỉ",
            "SACH"                => "Sạch",
            "BAN"                 => "Bẩn",
            "DANG_DON"            => "Đang dọn",
            "DA_KIEM_TRA"         => "Đã kiểm tra",
            "DANG_HOAT_DONG"      => "Hoạt động",
            "BAO_TRI"             => "Bảo trì",
            "NGUNG_KHAI_THAC"     => "Ngừng KT",
            "DA_GHI_NHAN"         => "Đã ghi nhận",
            "DA_PHAN_BO"          => "Đã phân bổ",
            "PHAN_TRAM"           => "Phần trăm (%)",
            "SO_TIEN"             => "Số tiền (₫)",
            _                     => s
        };

        public static Color StatusColor(string s) => s switch
        {
            "DA_NHAN_PHONG" or "DA_THANH_TOAN" or "DANG_LAM" or "DA_GHI_NHAN" or "DANG_HOAT_DONG" or "SACH" or "DA_KIEM_TRA"
                => Color.FromRgb(52, 199, 89),
            "CHO_XAC_NHAN" or "CHUA_THANH_TOAN" or "THANH_TOAN_MOT_PHAN" or "DANG_DON" or "BAN"
                => Color.FromRgb(255, 165, 55),
            "HUY" or "NGHI_VIEC" or "NO_SHOW" or "BAO_TRI" or "NGUNG_KHAI_THAC"
                => Color.FromRgb(240, 70, 70),
            "DA_XAC_NHAN" or "DA_PHAN_BO" or "TAM_NGHI"
                => Color.FromRgb(75, 155, 255),
            "DA_TRA_PHONG" => Color.FromRgb(130, 148, 178),
            _              => Color.FromRgb(130, 148, 178)
        };

        public static SolidColorBrush StatusBrush(string s) => new(StatusColor(s));

        public static string FormatM(decimal v) =>
            v >= 1_000_000 ? $"{(double)v / 1_000_000:0.#}M" : v >= 1_000 ? $"{(double)v / 1_000:0}K" : v.ToString("N0");

        public static Window CreateDialog(string title, int width, int height)
        {
            return new Window
            {
                Title = title,
                Width = width,
                Height = height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = new SolidColorBrush(Color.FromRgb(22, 30, 52)),
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12.5
            };
        }
    }

    // ── Value Converters ──────────────────────────────────────────────

    public class GioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TimeSpan time ? time.ToString(@"hh\:mm") : "";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class StatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string s ? ThemeHelper.StatusText(s) : value;
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string s ? ThemeHelper.StatusBrush(s) : Brushes.Gray;
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class StatusBadgeBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string s) return Brushes.Transparent;
            var c = ThemeHelper.StatusColor(s);
            return new SolidColorBrush(Color.FromArgb(60, c.R, c.G, c.B));
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class VndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d) return ThemeHelper.Vnd(d);
            if (value is string s && decimal.TryParse(s, out var v)) return ThemeHelper.Vnd(v);
            return value?.ToString() ?? "";
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return b ? "DANG_HOAT_DONG" : "NGUNG_HOAT_DONG";
            if (value is string s && (s == "True" || s == "False")) return s == "True" ? "DANG_HOAT_DONG" : "NGUNG_HOAT_DONG";
            return value?.ToString() ?? "";
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class CustomerRankConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() switch
        {
            "KIM_CUONG" => "💎 Kim Cương",
            "VANG"      => "🥇 Vàng",
            "BAC"       => "🥈 Bạc",
            _           => "Thường"
        };
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class CustomerRankColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() switch
        {
            "KIM_CUONG" => new SolidColorBrush(Color.FromRgb(75, 155, 255)),
            "VANG"      => new SolidColorBrush(Color.FromRgb(201, 168, 76)),
            "BAC"       => new SolidColorBrush(Color.FromRgb(130, 148, 178)),
            _           => new SolidColorBrush(Color.FromRgb(130, 148, 178))
        };
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
