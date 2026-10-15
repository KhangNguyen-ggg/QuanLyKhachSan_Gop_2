using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs
{
    public partial class ChucVuForm : Window
    {
        private readonly BUS_ChucVu bus = new();
        private readonly ChucVu chucVu = new() { TrangThai = true };

        public ChucVuForm()
        {
            InitializeComponent();
            DataContext = chucVu;
            btnLuu.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuThemMoi");

        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton(
                "NhanVienView", "btnChucVuThemMoi")) return;
            try
            {
                chucVu.Chucvu = BUS_KiemTra.SoTien(txtLuong.Text, "Mức lương");
                bus.Luu(chucVu);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                txtThongBao.Text = "Không lưu được chức vụ. " + ex.Message;
            }
        }
    }
}
