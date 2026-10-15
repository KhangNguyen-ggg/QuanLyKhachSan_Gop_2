using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class BuongPhongView : UserControl
    {
        public BuongPhongView()
        {
            InitializeComponent();
            btnDonPhongThemCongViec.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnDonPhongThemCongViec");
            btnBaoTriTaoYeuCau.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnBaoTriTaoYeuCau");
            btnThietBiThemMoi.IsEnabled = DemoSession.CanUseButton("BuongPhongView", "btnThietBiThemMoi");
        }

        private void btnDonPhongThemCongViec_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("BuongPhongView", "btnDonPhongThemCongViec")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            DonPhongForm form = new DonPhongForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void DonPhong_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("BuongPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvDonPhong, source) is not DataGridRow row ||
                row.Item is not Data.DonPhongMau item) return;
            var form = new DonPhongChiTietWindow(item) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            dgvDonPhong.Items.Refresh();
        }

        private void btnTimDonPhong_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiDonPhong_Click(object sender, RoutedEventArgs e)
        {
            txtTimDonPhong.Clear();
            cboLocDonPhong.SelectedIndex = 0;
        }

        private void btnBaoTriTaoYeuCau_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("BuongPhongView", "btnBaoTriTaoYeuCau")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            BaoTriForm form = new BaoTriForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void BaoTri_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("BuongPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvBaoTri, source) is not DataGridRow row ||
                row.Item is not Data.BaoTriMau item) return;
            var form = new BaoTriChiTietWindow(item) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            dgvBaoTri.Items.Refresh();
        }

        private void btnTimBaoTri_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiBaoTri_Click(object sender, RoutedEventArgs e)
        {
            txtTimBaoTri.Clear();
            cboLocBaoTri.SelectedIndex = 0;
        }

        private void btnThietBiThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("BuongPhongView", "btnThietBiThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            ThietBiForm form = new ThietBiForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void ThietBi_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("BuongPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvThietBi, source) is not DataGridRow row ||
                row.Item is not Data.ThietBiMau item) return;
            var form = new ThietBiChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvThietBi.Items.Remove(item);
            dgvThietBi.Items.Refresh();
        }

        private void btnTimThietBi_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiThietBi_Click(object sender, RoutedEventArgs e)
        {
            txtTimThietBi.Clear();
            cboLocThietBi.SelectedIndex = 0;
        }
    }
}
