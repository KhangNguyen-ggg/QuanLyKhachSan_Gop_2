using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class VatTuNhaCungCapView : UserControl
    {
        public VatTuNhaCungCapView()
        {
            InitializeComponent();
        }

        private void btnVatTuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            VatTuForm form = new VatTuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void VatTu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvVatTu, source) is not DataGridRow row ||
                row.Item is not Data.VatTuMau item) return;
            var form = new VatTuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvVatTu.Items.Remove(item);
            dgvVatTu.Items.Refresh();
        }

        private void btnTimVatTu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiVatTu_Click(object sender, RoutedEventArgs e)
        {
            txtTimVatTu.Clear();
            cboLocVatTu.SelectedIndex = 0;
        }

        private void btnNhomVatTuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhomVatTuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            NhomVatTuForm form = new NhomVatTuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void NhomVatTu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvNhomVatTu, source) is not DataGridRow row ||
                row.Item is not Data.NhomVatTuMau item) return;
            var form = new NhomVatTuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvNhomVatTu.Items.Remove(item);
            dgvNhomVatTu.Items.Refresh();
        }

        private void btnTimNhomVatTu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiNhomVatTu_Click(object sender, RoutedEventArgs e)
        {
            txtTimNhomVatTu.Clear();
            cboLocNhomVatTu.SelectedIndex = 0;
        }

        private void btnNhaCungCapThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("VatTuNhaCungCapView", "btnNhaCungCapThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            NhaCungCapForm form = new NhaCungCapForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void NhaCungCap_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvNhaCungCap, source) is not DataGridRow row ||
                row.Item is not Data.NhaCungCapMau item) return;
            var form = new NhaCungCapChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvNhaCungCap.Items.Remove(item);
            dgvNhaCungCap.Items.Refresh();
        }

        private void btnTimNhaCungCap_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiNhaCungCap_Click(object sender, RoutedEventArgs e)
        {
            txtTimNhaCungCap.Clear();
            cboLocNhaCungCap.SelectedIndex = 0;
        }

    }
}
