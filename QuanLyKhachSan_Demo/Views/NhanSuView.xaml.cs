using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class NhanSuView : UserControl
    {
        public NhanSuView()
        {
            InitializeComponent();
        }

        private void btnCaLamViecThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanSuView", "btnCaLamViecThemMoi")) return;
            CaLamViecForm form = new CaLamViecForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void CaLamViec_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanSuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvCaLamViec, source) is not DataGridRow row ||
                row.Item is not Data.CaLamViecMau item) return;
            var form = new CaLamViecChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvCaLamViec.Items.Remove(item);
            dgvCaLamViec.Items.Refresh();
        }

        private void btnTimCaLamViec_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiCaLamViec_Click(object sender, RoutedEventArgs e)
        {
            txtTimCaLamViec.Clear();
            cboLocCaLamViec.SelectedIndex = 0;
        }

        private void btnLichLamViecThemLich_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanSuView", "btnLichLamViecThemLich")) return;
            LichLamViecForm form = new LichLamViecForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void LichLamViec_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanSuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvLichLamViec, source) is not DataGridRow row ||
                row.Item is not Data.LichLamViecMau item) return;
            var form = new LichLamViecChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvLichLamViec.Items.Remove(item);
            dgvLichLamViec.Items.Refresh();
        }

        private void btnTimLichLamViec_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiLichLamViec_Click(object sender, RoutedEventArgs e)
        {
            txtTimLichLamViec.Clear();
            cboLocLichLamViec.SelectedIndex = 0;
        }

        private void btnChamCongThemChamCong_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanSuView", "btnChamCongThemChamCong")) return;
            ChamCongForm form = new ChamCongForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void ChamCong_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanSuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvChamCong, source) is not DataGridRow row ||
                row.Item is not Data.ChamCongMau item) return;
            var form = new ChamCongChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvChamCong.Items.Remove(item);
            dgvChamCong.Items.Refresh();
        }

        private void btnTimChamCong_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiChamCong_Click(object sender, RoutedEventArgs e)
        {
            txtTimChamCong.Clear();
            cboLocChamCong.SelectedIndex = 0;
        }

        private void btnBangLuongLapBangLuong_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanSuView", "btnBangLuongLapBangLuong")) return;
            BangLuongForm form = new BangLuongForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void BangLuong_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanSuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvBangLuong, source) is not DataGridRow row ||
                row.Item is not Data.BangLuongMau item) return;
            var form = new BangLuongChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvBangLuong.Items.Remove(item);
            dgvBangLuong.Items.Refresh();
        }

        private void btnTimBangLuong_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiBangLuong_Click(object sender, RoutedEventArgs e)
        {
            txtTimBangLuong.Clear();
            cboLocBangLuong.SelectedIndex = 0;
        }

        private void btnKhenThuongThemKhenThuong_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("NhanSuView", "btnKhenThuongThemKhenThuong")) return;
            KhenThuongForm form = new KhenThuongForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void KhenThuong_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("NhanSuView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvKhenThuong, source) is not DataGridRow row ||
                row.Item is not Data.KhenThuongMau item) return;
            var form = new KhenThuongChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvKhenThuong.Items.Remove(item);
            dgvKhenThuong.Items.Refresh();
        }

        private void btnTimKhenThuong_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiKhenThuong_Click(object sender, RoutedEventArgs e)
        {
            txtTimKhenThuong.Clear();
            cboLocKhenThuong.SelectedIndex = 0;
        }
    }
}
