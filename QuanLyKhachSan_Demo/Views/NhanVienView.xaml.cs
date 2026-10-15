using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class NhanVienView : UserControl
    {
        private readonly BUS_PhongBan? phongBan;

        public NhanVienView()
        {
            InitializeComponent();
            if (DAL.CauHinhKetNoi.DaCauHinh) phongBan = new BUS_PhongBan();
            btnPhongBanThemMoi.IsEnabled = phongBan != null && DemoSession.CanUseButton(nameof(NhanVienView), "btnPhongBanThemMoi");
            Loaded += (_, _) => TaiLaiPhongBan();
        }

        private void TaiLaiPhongBan()
        {
            if (!DemoSession.CanOpen(nameof(NhanVienView))) return;
            var maDaChon = dgvPhongBan.SelectedValue;
            dgvPhongBan.ItemsSource = null;
            if (phongBan == null)
            {
                txtThongBaoPhongBan.Text = "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng chức năng Phòng ban.";
                return;
            }
            try
            {
                dgvPhongBan.ItemsSource = phongBan.HienThiTatCa(txtTimPhongBan.Text, cboLocPhongBan.SelectedValue as bool?);
                dgvPhongBan.SelectedValue = maDaChon;
                if (dgvPhongBan.SelectedIndex < 0 && dgvPhongBan.Items.Count > 0) dgvPhongBan.SelectedIndex = 0;
                txtThongBaoPhongBan.Text = $"{dgvPhongBan.Items.Count} dòng · Nhấp đúp để xem chi tiết / sửa";
            }
            catch (Exception ex) { txtThongBaoPhongBan.Text = "Không tải được phòng ban: " + ex.Message; }
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
            if (phongBan == null || !DemoSession.CanUseButton(nameof(NhanVienView), "btnPhongBanThemMoi")) return;
            try
            {
                var form = new PhongBanForm { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiPhongBan();
            }
            catch (Exception ex) { txtThongBaoPhongBan.Text = ex.Message; }
        }

        private void PhongBan_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (phongBan == null || !DemoSession.CanOpen(nameof(NhanVienView)) || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvPhongBan, source) is not DataGridRow row ||
                row.Item is not PhongBan item) return;
            try
            {
                var form = new PhongBanChiTietWindow(phongBan.HienThiTheoId(item.MaPhongBan)) { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiPhongBan();
            }
            catch (Exception ex) { txtThongBaoPhongBan.Text = ex.Message; }
        }

        private void btnTimPhongBan_Click(object sender, RoutedEventArgs e)
        {
            TaiLaiPhongBan();
        }

        private void btnMoiPhongBan_Click(object sender, RoutedEventArgs e)
        {
            txtTimPhongBan.Clear();
            cboLocPhongBan.SelectedIndex = 0;
            TaiLaiPhongBan();
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
