using System.Windows;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs
{
    public partial class PhongBanForm : Window
    {
        private readonly BUS_PhongBan bus = new();
        private readonly PhongBan phongBan = new() { TrangThai = true };

        public PhongBanForm()
        {
            InitializeComponent();
            DataContext = phongBan;
            btnLuu.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi");
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi")) return;
            try
            {
                bus.Luu(phongBan);
                DialogResult = true;
            }
            catch (Exception ex) { txtThongBao.Text = ex.Message; }
        }
    }
}
