using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using QuanLyKhachSan.Helpers;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Views
{
    public partial class BaoCaoView : UserControl
    {
        public BaoCaoView()
        {
            InitializeComponent();
            BuildKpi(); BuildRoomStats(); BuildTopCustomers(); BuildDeptCost();
        }

        private void BuildKpi()
        {
            decimal tongDT = DataStore.HoaDons.Where(h => h.TrangThai == "DA_THANH_TOAN").Sum(h => h.TongThanhToan);
            int tongDP = DataStore.DatPhongs.Count;
            int tongHD = DataStore.HoaDons.Count;
            int tongKH = DataStore.KhachHangs.Count;
            decimal dtTB = tongHD > 0 ? tongDT / tongHD : 0;

            kpiGrid.Children.Add(MakeKpi("💰", "Tổng doanh thu", ThemeHelper.Vnd(tongDT), Color.FromRgb(52, 199, 89)));
            kpiGrid.Children.Add(MakeKpi("📋", "Tổng đặt phòng", tongDP.ToString(), Color.FromRgb(75, 155, 255)));
            kpiGrid.Children.Add(MakeKpi("📄", "Tổng hoá đơn", tongHD.ToString(), Color.FromRgb(201, 168, 76)));
            kpiGrid.Children.Add(MakeKpi("👥", "Tổng khách hàng", tongKH.ToString(), Color.FromRgb(255, 165, 55)));
            kpiGrid.Children.Add(MakeKpi("📊", "DT TB / HĐ", ThemeHelper.Vnd(dtTB), Color.FromRgb(240, 70, 70)));
        }

        private static Border MakeKpi(string icon, string title, string value, Color c)
        {
            var card = new Border { Background = new SolidColorBrush(Color.FromRgb(22, 30, 52)), Margin = new Thickness(0, 0, 14, 0) };
            var g = new Grid();
            g.Children.Add(new Border { Width = 4, HorizontalAlignment = HorizontalAlignment.Left, Background = new SolidColorBrush(c) });
            var stk = new StackPanel { Margin = new Thickness(18, 12, 12, 12) };
            stk.Children.Add(new TextBlock { Text = icon, FontSize = 22, Foreground = new SolidColorBrush(c) });
            stk.Children.Add(new TextBlock { Text = title, FontSize = 10.5, Foreground = new SolidColorBrush(Color.FromRgb(130, 148, 178)), Margin = new Thickness(0, 4, 0, 0) });
            stk.Children.Add(new TextBlock { Text = value, FontSize = 22, FontWeight = FontWeights.Black, Foreground = new SolidColorBrush(Color.FromRgb(220, 228, 242)), Margin = new Thickness(0, 2, 0, 0) });
            g.Children.Add(stk); card.Child = g;
            return card;
        }

        private void BuildRoomStats()
        {
            var occupied = DataStore.XepPhongs.Where(x => x.TrangThai == "DA_NHAN_PHONG").Select(x => x.MaPhong).ToHashSet();
            dgvPhong.ItemsSource = DataStore.LoaiPhongs.Select(lp =>
            {
                var phongs = DataStore.Phongs.Where(p => p.MaLoaiPhong == lp.MaLoaiPhong).ToList();
                return new
                {
                    LoaiPhong = lp.TenLoaiPhong, Tong = phongs.Count,
                    DangO = phongs.Count(p => occupied.Contains(p.MaPhong)),
                    Trong = phongs.Count(p => p.TrangThaiKhaiThac == "DANG_HOAT_DONG" && !occupied.Contains(p.MaPhong)),
                    BaoTri = phongs.Count(p => p.TrangThaiKhaiThac == "BAO_TRI"),
                    GiaTB = ThemeHelper.Vnd(lp.GiaCoBan)
                };
            }).ToList();
        }

        private void BuildTopCustomers()
        {
            var topKH = DataStore.KhachHangs.Select(kh => new
            {
                kh.HoTen, kh.QuocTich,
                SoLanDat = DataStore.DatPhongs.Count(d => d.MaKhachHangDat == kh.MaKhachHang),
                TongChiTieu = DataStore.HoaDons.Where(h => h.MaKhachHang == kh.MaKhachHang && h.TrangThai == "DA_THANH_TOAN").Sum(h => h.TongThanhToan),
                kh.HangKhachHang
            }).OrderByDescending(x => x.TongChiTieu).Take(10).Select((x, i) => new
            {
                STT = i + 1, x.HoTen, x.QuocTich, x.SoLanDat,
                TongChiTieu = ThemeHelper.Vnd(x.TongChiTieu),
                Hang = ThemeHelper.StatusText(x.HangKhachHang)
            }).ToList();
            dgvTopKH.ItemsSource = topKH;
        }

        private void BuildDeptCost()
        {
            dgvDept.ItemsSource = DataStore.PhongBans.Select(pb =>
            {
                var nvs = DataStore.NhanViens.Where(n => n.MaPhongBan == pb.MaPhongBan && n.TrangThai == "DANG_LAM").ToList();
                return new
                {
                    PB = pb.TenPhongBan, SoNV = nvs.Count,
                    LuongTB = nvs.Count > 0 ? ThemeHelper.Vnd(nvs.Average(n => n.LuongCoBan)) : "–",
                    TongLuong = ThemeHelper.Vnd(nvs.Sum(n => n.LuongCoBan))
                };
            }).ToList();
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e) => DrawChart();

        private void DrawChart()
        {
            chartCanvas.Children.Clear();
            double w = chartCanvas.ActualWidth, h = chartCanvas.ActualHeight;
            if (w < 60 || h < 60) return;

            // Generate monthly revenue data (last 6 months)
            var data = new List<(string Label, decimal Value)>();
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                decimal rev = DataStore.HoaDons.Where(hd => hd.TrangThai == "DA_THANH_TOAN" && hd.NgayLap.Year == month.Year && hd.NgayLap.Month == month.Month).Sum(hd => hd.TongThanhToan);
                data.Add(($"T{month.Month}/{month.Year % 100}", rev));
            }

            decimal maxVal = data.Max(d => d.Value);
            if (maxVal < 1) maxVal = 1;

            double mL = 60, mB = 34, mT = 10;
            double cW = w - mL - 20, cH = h - mB - mT;
            if (cW < 20 || cH < 20) return;
            double barW = cW / data.Count - 14;
            if (barW < 6) barW = 6;

            // Grid lines
            for (int i = 0; i <= 4; i++)
            {
                double gy = mT + cH * (1.0 - i / 4.0);
                chartCanvas.Children.Add(new Line { X1 = mL, Y1 = gy, X2 = mL + cW, Y2 = gy, Stroke = new SolidColorBrush(Color.FromArgb(35, 255, 255, 255)), StrokeThickness = 1 });
                decimal lbl = maxVal * i / 4 / 1_000_000;
                var lt = new TextBlock { Text = $"{lbl:0.#}M", FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(130, 148, 178)) };
                Canvas.SetLeft(lt, 2); Canvas.SetTop(lt, gy - 8);
                chartCanvas.Children.Add(lt);
            }

            for (int i = 0; i < data.Count; i++)
            {
                double bH = data[i].Value > 0 ? (double)data[i].Value / (double)maxVal * cH : 0;
                double bx = mL + i * (barW + 14) + 5, by = mT + cH - bH;
                if (bH >= 3)
                {
                    chartCanvas.Children.Add(new Rectangle { Width = barW, Height = bH, Fill = new LinearGradientBrush(Color.FromRgb(235, 200, 105), Color.FromRgb(201, 168, 76), 90) });
                    Canvas.SetLeft(chartCanvas.Children[^1], bx); Canvas.SetTop(chartCanvas.Children[^1], by);
                }
                if (data[i].Value > 0)
                {
                    var vt = new TextBlock { Text = ThemeHelper.FormatM(data[i].Value), FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(220, 228, 242)) };
                    Canvas.SetLeft(vt, bx - 2); Canvas.SetTop(vt, by - 16); chartCanvas.Children.Add(vt);
                }
                var xl = new TextBlock { Text = data[i].Label, FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(130, 148, 178)) };
                Canvas.SetLeft(xl, bx + barW / 2 - 14); Canvas.SetTop(xl, mT + cH + 6);
                chartCanvas.Children.Add(xl);
            }
        }
    }
}
