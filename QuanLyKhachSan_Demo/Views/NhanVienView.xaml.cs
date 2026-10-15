using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class NhanVienView : UserControl
    {
        public NhanVienView()
        {
            InitializeComponent();
        }

        private void btnNhanVienThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanVienView", "btnNhanVienThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            NhanVienForm form = new NhanVienForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void NhanVien_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanVienView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvNhanVien, source) is not DataGridRow row ||
                row.Item is not Data.NhanVienMau item) return;
            var form = new NhanVienChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvNhanVien.Items.Remove(item);
            dgvNhanVien.Items.Refresh();
        }

        private void btnTimNhanVien_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiNhanVien_Click(object sender, RoutedEventArgs e)
        {
            txtTimNhanVien.Clear();
            cboLocNhanVien.SelectedIndex = 0;
        }

        private void btnPhongBanThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanVienView", "btnPhongBanThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            PhongBanForm form = new PhongBanForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void PhongBan_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanVienView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvPhongBan, source) is not DataGridRow row ||
                row.Item is not Data.PhongBanMau item) return;
            var form = new PhongBanChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvPhongBan.Items.Remove(item);
            dgvPhongBan.Items.Refresh();
        }

        private void btnTimPhongBan_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiPhongBan_Click(object sender, RoutedEventArgs e)
        {
            txtTimPhongBan.Clear();
            cboLocPhongBan.SelectedIndex = 0;
        }

        private void btnChucVuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanVienView", "btnChucVuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            ChucVuForm form = new ChucVuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void ChucVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanVienView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvChucVu, source) is not DataGridRow row ||
                row.Item is not Data.ChucVuMau item) return;
            var form = new ChucVuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvChucVu.Items.Remove(item);
            dgvChucVu.Items.Refresh();
        }

        private void btnTimChucVu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiChucVu_Click(object sender, RoutedEventArgs e)
        {
            txtTimChucVu.Clear();
            cboLocChucVu.SelectedIndex = 0;
        }
    }
}
