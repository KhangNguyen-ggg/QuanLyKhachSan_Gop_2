using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class KhoView : UserControl
    {
        public KhoView()
        {
            InitializeComponent();
        }

        private void TonKho_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvTonKho, source) is not DataGridRow row ||
                row.Item is not Data.TonKhoMau item) return;
            var form = new TonKhoChiTietWindow(item) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            dgvTonKho.Items.Refresh();
        }

        private void btnTimTonKho_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiTonKho_Click(object sender, RoutedEventArgs e)
        {
            txtTimTonKho.Clear();
            cboLocTonKho.SelectedIndex = 0;
        }

        private void btnPhieuNhapXuatTaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("KhoView", "btnPhieuNhapXuatTaoPhieu")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            PhieuNhapXuatForm form = new PhieuNhapXuatForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void PhieuNhapXuat_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvPhieuNhapXuat, source) is not DataGridRow row ||
                row.Item is not Data.PhieuNhapXuatMau item) return;
            var form = new PhieuNhapXuatChiTietWindow(item) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            dgvPhieuNhapXuat.Items.Refresh();
        }

        private void btnTimPhieuNhapXuat_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiPhieuNhapXuat_Click(object sender, RoutedEventArgs e)
        {
            txtTimPhieuNhapXuat.Clear();
            cboLocPhieuNhapXuat.SelectedIndex = 0;
        }

        private void btnKhoThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("KhoView", "btnKhoThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            KhoForm form = new KhoForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void Kho_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("KhoView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvKho, source) is not DataGridRow row ||
                row.Item is not Data.KhoMau item) return;
            var form = new KhoChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvKho.Items.Remove(item);
            dgvKho.Items.Refresh();
        }

        private void btnTimKho_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiKho_Click(object sender, RoutedEventArgs e)
        {
            txtTimKho.Clear();
            cboLocKho.SelectedIndex = 0;
        }
    }
}
