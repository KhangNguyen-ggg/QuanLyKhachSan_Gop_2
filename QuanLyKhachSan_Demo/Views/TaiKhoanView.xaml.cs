using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.HeThong;
using DAL;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;

public partial class TaiKhoanView : UserControl
{
    private readonly BUS_Taikhoang? taiKhoan;
    private readonly BUS_VaiTro? vaiTro;

    public TaiKhoanView()
    {
        InitializeComponent();
        if (CauHinhKetNoi.DaCauHinh)
        {
            taiKhoan = new BUS_Taikhoang();
            vaiTro = new BUS_VaiTro();
        }
        txtDataMode.Text = CauHinhKetNoi.DaCauHinh ? "Nhấp đúp để xem và thao tác trên tài khoản." :
            "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng chức năng Hệ thống.";
        Loaded += (_, _) => TaiLai();
    }

    private void TaiLai()
    {
        if (taiKhoan == null || vaiTro == null || !DemoSession.CanOpen(nameof(TaiKhoanView))) return;
        dgvTaiKhoan.ItemsSource = null;
        dgvVaiTroQuyen.ItemsSource = null;
        try
        {
            // Ghép Hệ thống: BUS Gop trả toàn bộ; giữ nguyên cách lọc đang có trong giao diện 5Sao.
            string trangThai = Convert.ToString(cboLocTaiKhoan.SelectedValue) ?? "";
            dgvTaiKhoan.ItemsSource = taiKhoan.hienThiTKAll().Where(x =>
                $"{x.MaTaiKhoan} {x.HoTen} {x.TenDangNhap} {x.TenVaiTro}".Contains(txtTimTaiKhoan.Text.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                (string.IsNullOrEmpty(trangThai) || Convert.ToString(x.TrangThai) == trangThai)).ToList();
            dgvVaiTroQuyen.ItemsSource = vaiTro.hienThiVTAll().Where(x =>
                $"{x.MaVaiTro} {x.TenVaiTro} {x.MoTa}".Contains(txtTimVaiTroQuyen.Text.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                (cboLocVaiTroQuyen.SelectedIndex == 0 || x.TrangThai));
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTaiKhoanThemTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(TaiKhoanView), "btnTaiKhoanThemTaiKhoan")) return;
        try
        {
            var form = new TaiKhoanForm { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void TaiKhoan_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(TaiKhoanView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvTaiKhoan, source) is not DataGridRow row) return;
        try
        {
            var form = new TaiKhoanChiTietWindow { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnVaiTroQuyenThemVaiTro_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(TaiKhoanView), "btnVaiTroQuyenThemVaiTro")) return;
        try
        {
            var form = new VaiTroQuyenForm { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void VaiTroQuyen_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(TaiKhoanView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvVaiTroQuyen, source) is not DataGridRow row) return;
        try
        {
            var form = new VaiTroQuyenChiTietWindow { DataContext = row.Item, Owner = Window.GetWindow(this) };
            form.ShowDialog();
            TaiLai();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnTimTaiKhoan_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnTimVaiTroQuyen_Click(object sender, RoutedEventArgs e) => TaiLai();
    private void btnMoiTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        txtTimTaiKhoan.Clear();
        cboLocTaiKhoan.SelectedIndex = 0;
        TaiLai();
    }
    private void btnMoiVaiTroQuyen_Click(object sender, RoutedEventArgs e)
    {
        txtTimVaiTroQuyen.Clear();
        cboLocVaiTroQuyen.SelectedIndex = 0;
        TaiLai();
    }
}
