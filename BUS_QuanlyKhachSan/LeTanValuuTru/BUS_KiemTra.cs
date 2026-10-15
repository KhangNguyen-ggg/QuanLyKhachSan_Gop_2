using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public static class BUS_KiemTra
{
    public static void YeuCau(bool hopLe, string thongBao)
    {
        if (!hopLe) throw new ArgumentException(thongBao);
    }
    public static string Chuoi(string giaTri, string ten, int toiDa, bool batBuoc = false)
    {
        string s = giaTri?.Trim() ?? "";
        YeuCau(!batBuoc || s.Length > 0, $"Vui lòng nhập {ten}.");
        YeuCau(s.Length <= toiDa, $"{ten} chỉ được tối đa {toiDa} ký tự.");
        return s;
    }
    public static void Ma(long ma, string ten) => YeuCau(ma > 0, $"Vui lòng chọn {ten}.");
    // Kiểm tra giá trị người dùng chọn trước khi View mở form sửa/chi tiết.
    public static T Chon<T>(T giaTri, string ten) where T : class
    {
        YeuCau(giaTri != null, $"Vui lòng chọn {ten}.");
        return giaTri;
    }
    public static DateTime Ngay(DateTime? ngay, string ten)
    {
        YeuCau(ngay.HasValue, $"Vui lòng chọn {ten}.");
        return ngay.Value;
    }
    public static int SoNguyen(string giaTri, string ten)
    {
        string s = Chuoi(giaTri, ten, 10, true);
        YeuCau(Regex.IsMatch(s, @"^[0-9]+$"), $"{ten} chỉ được nhập số nguyên không âm.");
        YeuCau(int.TryParse(s, out int so), $"{ten} vượt giới hạn số nguyên.");
        return so;
    }
    public static decimal SoTien(string giaTri, string ten = "Số tiền")
    {
        string s = Chuoi(giaTri, ten, 19, true);
        YeuCau(Regex.IsMatch(s, @"^[0-9]+([.,][0-9]{1,2})?$"),
            $"{ten} chỉ được nhập số không âm, tối đa 2 số lẻ; không dùng dấu phân cách hàng nghìn.");
        YeuCau(decimal.TryParse(s.Replace(',', '.'), NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out decimal so), $"{ten} không hợp lệ.");
        Tien(so, ten);
        return so;
    }
    public static void Tien(decimal so, string ten, bool phaiDuong = false)
    {
        YeuCau(phaiDuong ? so > 0 : so >= 0, $"{ten} phải {(phaiDuong ? "lớn hơn hoặc bằng 0.01" : "không âm")}.");
        YeuCau(so <= 9999999999999999.99m && decimal.Round(so, 2) == so,
            $"{ten} tối đa 16 chữ số phần nguyên và 2 chữ số thập phân.");
    }
    public static string DienThoai(string giaTri)
    {
        string s = Chuoi(giaTri, "số điện thoại", 20, true);
        YeuCau(Regex.IsMatch(s, @"^[0-9]{10,15}$"), "Số điện thoại phải gồm 10–15 chữ số, không chứa chữ, dấu cách hoặc ký tự khác.");
        return s;
    }
    public static string Gmail(string giaTri)
    {
        string s = Chuoi(giaTri, "email", 150, true);
        YeuCau(MailAddress.TryCreate(s, out var mail) && mail.Address == s &&
            mail.Host.Equals("gmail.com", StringComparison.OrdinalIgnoreCase) &&
            Regex.IsMatch(mail.User, @"^[A-Za-z0-9]+(?:[.][A-Za-z0-9]+)*$"),
            "Email phải đúng dạng ten@gmail.com, không có dấu cách hoặc dấu chấm liên tiếp.");
        return s.ToLowerInvariant();
    }
    // Kiểm tra giá trị người dùng chọn trước khi View mở form sửa/chi tiết.
    public static long MaDuocChon(object giaTri)
    {
        YeuCau(long.TryParse(Convert.ToString(giaTri), out long ma) && ma > 0, "Vui lòng chọn một dòng dữ liệu.");
        return ma;
    }
    public static object DongDuocChon(object dong)
    {
        YeuCau(dong != null, "Vui lòng chọn một dòng dữ liệu.");
        return dong;
    }
    public static bool KetQua(bool thanhCong)
    {
        YeuCau(thanhCong, "Không lưu được dữ liệu. Hãy kiểm tra trạng thái, dữ liệu liên quan và kết nối rồi thử lại.");
        return true;
    }
    // Lọc dữ liệu đầu ra trong BUS; View chỉ nhận bảng và đưa lên DataGrid.
    public static System.Data.DataView LocBang(System.Data.DataTable bang, string tuKhoa, string trangThai)
    {
        var ketQua = bang.Clone();
        tuKhoa = (tuKhoa ?? "").Trim();
        foreach (System.Data.DataRow dong in bang.Rows)
        {
            string noiDung = string.Join(" ", dong.ItemArray);
            bool dungTuKhoa = noiDung.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase);
            bool dungTrangThai = string.IsNullOrEmpty(trangThai) ||
                (bang.Columns.Contains("TrangThai") && Convert.ToString(dong["TrangThai"]) == trangThai);
            if (dungTuKhoa && dungTrangThai) ketQua.ImportRow(dong);
        }
        return ketQua.DefaultView;
    }

    public static void BoDong<T>(List<T> danhSach, int viTri)
    {
        YeuCau(viTri >= 0 && viTri < danhSach.Count, "Vui lòng chọn dòng cần bỏ.");
        danhSach.RemoveAt(viTri);
    }

}
