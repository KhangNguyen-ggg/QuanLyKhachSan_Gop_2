using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Views.Dialogs;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views
{
    public partial class NhanVienView : UserControl
    {
        private readonly BUS_PhongBan? phongBan;
        private readonly BUS_ChucVu? chucVu;
        private readonly BUS_NhanSu? nhanSu;

        public NhanVienView()
        {
            InitializeComponent();
            // Bớt: chặn gõ/dán và báo lỗi tìm kiếm dùng chung cho cả Phòng ban, Chức vụ, Nhân viên.
            NhanSuGiaoDien.GanTimKiem(txtTimPhongBan, txtLoiTimPhongBan);
            NhanSuGiaoDien.GanTimKiem(txtTimChucVu, txtLoiTimChucVu);
            if (DAL.CauHinhKetNoi.DaCauHinh) phongBan = new BUS_PhongBan();
            btnPhongBanThemMoi.IsEnabled = phongBan != null && DemoSession.CanUseButton(nameof(NhanVienView), "btnPhongBanThemMoi");
            Loaded += (_, _) => TaiLaiPhongBan();
            if (DAL.CauHinhKetNoi.DaCauHinh) chucVu = new BUS_ChucVu();
            btnChucVuThemMoi.IsEnabled = chucVu != null && DemoSession.CanUseButton(nameof(NhanVienView), "btnChucVuThemMoi");
            Loaded += (_, _) => TaiLaiChucVu();
            if (DAL.CauHinhKetNoi.DaCauHinh) nhanSu = new();
            // Thêm: hồ sơ Nhân viên dùng lại SQL, tìm kiếm và danh mục trạng thái chung.
            NhanSuGiaoDien.GanTimKiem(txtTimNhanVien, txtLoiTimNhanVien);
            NhanSuGiaoDien.DanhMuc(cboLocNhanVien, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("NhanVien")), "");
            cboLocNhanVien.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiNhanVien(); };
            btnNhanVienThemMoi.IsEnabled = nhanSu != null && DemoSession.CanUseButton(nameof(NhanVienView), "btnNhanVienThemMoi");
            Loaded += (_, _) => TaiLaiNhanVien();
            tabChinh.SelectionChanged += (_, e) =>
            {
                if (e.Source != tabChinh || !IsLoaded) return;
                if (tabChinh.SelectedIndex == 0) TaiLaiNhanVien();
                else if (tabChinh.SelectedIndex == 1) TaiLaiPhongBan();
                else if (tabChinh.SelectedIndex == 2) TaiLaiChucVu();
            };
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

        private void TaiLaiChucVu()
        {
            if (!DemoSession.CanOpen(nameof(NhanVienView))) return;
            var maDaChon = dgvChucVu.SelectedValue;
            dgvChucVu.ItemsSource = null;
            if (chucVu == null)
            {
                txtThongBaoChucVu.Text = "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng chức năng Chức vụ.";
                return;
            }
            try
            {
                dgvChucVu.ItemsSource = chucVu.HienThiTatCa(txtTimChucVu.Text, cboLocChucVu.SelectedValue as bool?);
                dgvChucVu.SelectedValue = maDaChon;
                if (dgvChucVu.SelectedIndex < 0 && dgvChucVu.Items.Count > 0) dgvChucVu.SelectedIndex = 0;
                txtThongBaoChucVu.Text = $"{dgvChucVu.Items.Count} dòng · Nhấp đúp để xem chi tiết / sửa";
            }
            catch (Exception ex) { txtThongBaoChucVu.Text = "Không tải được chức vụ: " + ex.Message; }
        }

        private void TaiLaiNhanVien()
        {
            if (!DemoSession.CanOpen(nameof(NhanVienView))) return;
            var maDaChon = dgvNhanVien.SelectedValue;
            dgvNhanVien.ItemsSource = null;
            if (nhanSu == null) { txtThongBaoNhanVien.Text = "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng Nhân viên."; return; }
            try
            {
                dgvNhanVien.ItemsSource = nhanSu.NhanVien(txtTimNhanVien.Text, NhanSuGiaoDien.TrangThai(cboLocNhanVien));
                dgvNhanVien.SelectedValue = maDaChon;
                if (dgvNhanVien.SelectedIndex < 0 && dgvNhanVien.Items.Count > 0) dgvNhanVien.SelectedIndex = 0;
                txtThongBaoNhanVien.Text = $"{dgvNhanVien.Items.Count} dòng · Nhấp đúp để xem chi tiết";
            }
            catch (Exception ex) { txtThongBaoNhanVien.Text = "Không tải được nhân viên. " + ex.Message; }
        }

        private void btnNhanVienThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (nhanSu == null || !DemoSession.CanUseButton(nameof(NhanVienView), "btnNhanVienThemMoi")) return;
            try
            {
                var form = new NhanVienForm { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiNhanVien();
            }
            catch (Exception ex) { txtThongBaoNhanVien.Text = ex.Message; }
        }

        private void NhanVien_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (nhanSu == null || !DemoSession.CanOpen(nameof(NhanVienView)) || e.OriginalSource is not DependencyObject nguon ||
                ItemsControl.ContainerFromElement(dgvNhanVien, nguon) is not DataGridRow dong ||
                dong.Item is not NhanVien duLieu) return;
            try
            {
                var form = new NhanVienChiTietWindow(duLieu) { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiNhanVien();
            }
            catch (Exception ex) { txtThongBaoNhanVien.Text = ex.Message; }
        }

        private void btnTimNhanVien_Click(object sender, RoutedEventArgs e) => TaiLaiNhanVien();

        private void btnMoiNhanVien_Click(object sender, RoutedEventArgs e)
        {
            NhanSuGiaoDien.MoiTimKiem(txtTimNhanVien, txtLoiTimNhanVien, cboLocNhanVien);
            TaiLaiNhanVien();
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
            NhanSuGiaoDien.MoiTimKiem(txtTimPhongBan, txtLoiTimPhongBan, cboLocPhongBan);
            TaiLaiPhongBan();
        }

        private void btnChucVuThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (chucVu == null || !DemoSession.CanUseButton(nameof(NhanVienView), "btnChucVuThemMoi")) return;
            try
            {
                var form = new ChucVuForm { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiChucVu();
            }
            catch (Exception ex) { txtThongBaoChucVu.Text = ex.Message; }
        }

        private void ChucVu_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (chucVu == null || !DemoSession.CanOpen("NhanVienView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvChucVu, source) is not DataGridRow row ||
                row.Item is not ChucVu item) return;
            try
            {
                var form = new ChucVuChiTietWindow(chucVu.HienThiTheoId(item.MaChucVu)) { Owner = Window.GetWindow(this) };
                form.ShowDialog();
                TaiLaiChucVu();
            }
            catch (Exception ex) { txtThongBaoChucVu.Text = "Không mở được chi tiết chức vụ: " + ex.Message; }
        }

        private void btnTimChucVu_Click(object sender, RoutedEventArgs e)
        {
            TaiLaiChucVu();
        }

        private void btnMoiChucVu_Click(object sender, RoutedEventArgs e)
        {
            NhanSuGiaoDien.MoiTimKiem(txtTimChucVu, txtLoiTimChucVu, cboLocChucVu);
            TaiLaiChucVu();
        }

        private void cboLocChucVu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SelectedIndex có thể phát sự kiện khi các control phía sau chưa được tạo.
            if (!IsLoaded) return;
            TaiLaiChucVu();
        }

        private void cboLocPhongBan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectionChanged có thể chạy ngay trong lúc giao diện đang được tạo, trước khi chọn trạng thái.
            if (!IsLoaded) return;//chặn lệnh chạy khi giao diện chưa load xong.
            TaiLaiPhongBan();
        }
    }
}
