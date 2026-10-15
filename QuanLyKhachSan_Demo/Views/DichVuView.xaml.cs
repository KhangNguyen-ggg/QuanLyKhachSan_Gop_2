using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class DichVuView : UserControl
    {
        public DichVuView()
        {
            InitializeComponent();
            btnDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnDichVuThemMoi");
            btnSuDungDichVuThemLuotSuDung.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuThemLuotSuDung");
            btnLoaiDichVuThemMoi.IsEnabled = DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuThemMoi");
        }

        private void btnDichVuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DichVuView", "btnDichVuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            DichVuForm form = new DichVuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void DichVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DichVuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvDichVu, source) is not DataGridRow row ||
                row.Item is not Data.DichVuMau item) return;
            var form = new DichVuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvDichVu.Items.Remove(item);
            dgvDichVu.Items.Refresh();
        }

        private void btnTimDichVu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiDichVu_Click(object sender, RoutedEventArgs e)
        {
            txtTimDichVu.Clear();
            cboLocDichVu.SelectedIndex = 0;
        }

        private void btnSuDungDichVuThemLuotSuDung_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DichVuView", "btnSuDungDichVuThemLuotSuDung")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            SuDungDichVuForm form = new SuDungDichVuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void SuDungDichVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DichVuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvSuDungDichVu, source) is not DataGridRow row ||
                row.Item is not Data.SuDungDichVuMau item) return;
            var form = new SuDungDichVuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvSuDungDichVu.Items.Remove(item);
            dgvSuDungDichVu.Items.Refresh();
        }

        private void btnTimSuDungDichVu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiSuDungDichVu_Click(object sender, RoutedEventArgs e)
        {
            txtTimSuDungDichVu.Clear();
            cboLocSuDungDichVu.SelectedIndex = 0;
        }

        private void btnLoaiDichVuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DichVuView", "btnLoaiDichVuThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            LoaiDichVuForm form = new LoaiDichVuForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void LoaiDichVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DichVuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvLoaiDichVu, source) is not DataGridRow row ||
                row.Item is not Data.LoaiDichVuMau item) return;
            var form = new LoaiDichVuChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvLoaiDichVu.Items.Remove(item);
            dgvLoaiDichVu.Items.Refresh();
        }

        private void btnTimLoaiDichVu_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiLoaiDichVu_Click(object sender, RoutedEventArgs e)
        {
            txtTimLoaiDichVu.Clear();
            cboLocLoaiDichVu.SelectedIndex = 0;
        }
    }
}
