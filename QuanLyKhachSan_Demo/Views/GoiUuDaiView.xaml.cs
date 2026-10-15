using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class GoiUuDaiView : UserControl
    {
        public GoiUuDaiView()
        {
            InitializeComponent();
            btnGoiDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuThemMoi");
            btnUuDaiThemMoi.IsEnabled = DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiThemMoi");
        }

        private void btnGoiDichVuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("GoiUuDaiView", "btnGoiDichVuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            GoiDichVuForm form = new GoiDichVuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void GoiDichVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("GoiUuDaiView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvGoiDichVu, source) is not DataGridRow row ||
                row.Item is not Data.GoiDichVuMau item) return;
            var form = new GoiDichVuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvGoiDichVu.Items.Remove(item);
            dgvGoiDichVu.Items.Refresh();
        }

        private void btnTimGoiDichVu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiGoiDichVu_Click(object sender, RoutedEventArgs e)
        {
            txtTimGoiDichVu.Clear();
            cboLocGoiDichVu.SelectedIndex = 0;
        }

        private void btnUuDaiThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("GoiUuDaiView", "btnUuDaiThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            UuDaiForm form = new UuDaiForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void UuDai_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("GoiUuDaiView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvUuDai, source) is not DataGridRow row ||
                row.Item is not Data.UuDaiMau item) return;
            var form = new UuDaiChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvUuDai.Items.Remove(item);
            dgvUuDai.Items.Refresh();
        }

        private void btnTimUuDai_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiUuDai_Click(object sender, RoutedEventArgs e)
        {
            txtTimUuDai.Clear();
            cboLocUuDai.SelectedIndex = 0;
        }
    }
}
