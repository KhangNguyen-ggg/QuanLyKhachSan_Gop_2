using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyKhachSan.Views
{
    public partial class TuVanAIView : UserControl
    {
        public TuVanAIView()
        {
            InitializeComponent();
        }

        private async void btnPhanTichAI_Click(object sender, RoutedEventArgs e)
        {
            string yeuCau = txtYeuCauAI.Text.Trim();
            if (string.IsNullOrEmpty(yeuCau))
            {
                MessageBox.Show("Vui lòng nhập yêu cầu của khách hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Giả lập đang phân tích
            btnPhanTichAI.IsEnabled = false;
            btnPhanTichAI.Content = "⏳ Đang phân tích...";
            prgAI.Visibility = Visibility.Visible;
            txtPhanHoiAI.Text = "AI đang xử lý yêu cầu, vui lòng chờ...";
            borderKetQuaPhanTich.Visibility = Visibility.Collapsed;

            await Task.Delay(1500); // Giả lập thời gian AI xử lý

            // ── Phân tích từ khóa đơn giản (demo) ──
            string lower = yeuCau.ToLower();

            // Bóc tách số khách
            string soKhach = "Chưa xác định";
            var matchKhach = System.Text.RegularExpressions.Regex.Match(lower, @"(\d+)\s*(người|khách|nguoi|khach)");
            if (matchKhach.Success) soKhach = matchKhach.Groups[1].Value + " người";

            // Bóc tách số phòng
            string soPhong = "Chưa xác định";
            var matchPhong = System.Text.RegularExpressions.Regex.Match(lower, @"(\d+)\s*(phòng|phong)");
            if (matchPhong.Success)
            {
                soPhong = matchPhong.Groups[1].Value + " phòng";
                if (lower.Contains("liền kề") || lower.Contains("lien ke")) soPhong += " (liền kề)";
            }

            // Bóc tách thời gian
            string thoiGian = "Chưa xác định";
            var matchDem = System.Text.RegularExpressions.Regex.Match(lower, @"(\d+)\s*(đêm|dem|ngày|ngay)");
            if (matchDem.Success) thoiGian = matchDem.Groups[1].Value + " " + matchDem.Groups[2].Value;

            // Bóc tách ngân sách
            string nganSach = "Chưa xác định";
            var matchTien = System.Text.RegularExpressions.Regex.Match(lower, @"(\d[\d.,]*)\s*(triệu|trieu|tr|đồng|dong|đ)");
            if (matchTien.Success) nganSach = "Khoảng " + matchTien.Groups[1].Value + " " + matchTien.Groups[2].Value;

            // Bóc tách yêu cầu thêm
            var extras = new List<string>();
            if (lower.Contains("view") || lower.Contains("hướng biển") || lower.Contains("hướng thành phố"))
                extras.Add("View đẹp");
            if (lower.Contains("hồ bơi") || lower.Contains("bể bơi") || lower.Contains("ho boi"))
                extras.Add("Có hồ bơi");
            if (lower.Contains("spa")) extras.Add("Dịch vụ Spa");
            if (lower.Contains("buffet") || lower.Contains("ăn sáng") || lower.Contains("an sang"))
                extras.Add("Bao gồm ăn sáng");
            if (lower.Contains("gym") || lower.Contains("phòng tập"))
                extras.Add("Phòng tập gym");
            string yeuCauThem = extras.Count > 0 ? string.Join(", ", extras) : "Không có yêu cầu đặc biệt";

            // Cập nhật bảng tiêu chí
            txtTcSoKhach.Text = soKhach;
            txtTcSoPhong.Text = soPhong;
            txtTcThoiGian.Text = thoiGian;
            txtTcNganSach.Text = nganSach;
            txtTcYeuCauThem.Text = yeuCauThem;
            borderKetQuaPhanTich.Visibility = Visibility.Visible;

            // Tạo đề xuất phương án (demo)
            string phuongAn = "✨ Phương án 1 — Tối ưu nhất (Độ phù hợp: 95%)\n"
                + "• Đối tượng: " + soKhach + "\n"
                + "• Phòng đề xuất: 3 × Deluxe Family (Mã: 401, 402, 403) — Tầng 4, liền kề\n"
                + "• View: Hướng biển panorama\n"
                + "• Tiện ích đi kèm: " + yeuCauThem + "\n"
                + "• Thời gian lưu trú: " + thoiGian + "\n"
                + "👉 Tổng chi phí dự kiến: 7.500.000 đ\n"
                + "\n"
                + "💡 Phương án 2 — Tiết kiệm (Độ phù hợp: 80%)\n"
                + "• Phòng đề xuất: 2 × Premier Suite (Mã: 501, 502) — Sức chứa rộng rãi\n"
                + "• View: Hướng thành phố\n"
                + "• Tiện ích: Hồ bơi tầng thượng miễn phí\n"
                + "👉 Tổng chi phí dự kiến: 6.200.000 đ\n"
                + "⚠ Hạn chế: Chỉ 2 phòng thay vì 3, không view biển.\n"
                + "\n"
                + "🏷 Phương án 3 — Cao cấp (Độ phù hợp: 70%)\n"
                + "• Phòng đề xuất: 1 × Presidential Suite (Mã: 601) — Sức chứa tối đa 10 người\n"
                + "• View: Hướng biển VIP, ban công riêng\n"
                + "• Tiện ích: Butler service, Minibar miễn phí, Spa voucher\n"
                + "👉 Tổng chi phí dự kiến: 12.000.000 đ\n"
                + "⚠ Vượt ngân sách nhưng trải nghiệm cao cấp nhất.";

            txtPhanHoiAI.Text = phuongAn;
            prgAI.Visibility = Visibility.Collapsed;
            btnPhanTichAI.Content = "✨ Phân tích yêu cầu";
            btnPhanTichAI.IsEnabled = true;
        }
    }
}
