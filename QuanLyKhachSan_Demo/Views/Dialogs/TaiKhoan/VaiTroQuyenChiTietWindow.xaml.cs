using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.HeThong;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VaiTroQuyenChiTietWindow : Window
{
    private readonly BUS_VaiTro bus = new();

    public VaiTroQuyenChiTietWindow()
    {
        InitializeComponent();
        btnVaiTroQuyenThemVaiTro.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenThemVaiTro");
        btnVaiTroQuyenSuaVaiTro.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenSuaVaiTro");
        btnVaiTroQuyenPhanQuyen.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenPhanQuyen");
        btnVaiTroQuyenXoaVaiTro.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenXoaVaiTro");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnVaiTroQuyenThemVaiTro_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenThemVaiTro")) return;
        try
        {
            var form = new VaiTroQuyenForm { Owner = this };
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenSuaVaiTro")) return;
        try
        {
            var selected = (VaiTro)DataContext;
            var form = new VaiTroQuyenForm(new VaiTro { MaVaiTro = selected.MaVaiTro, TenVaiTro = selected.TenVaiTro,
                MoTa = selected.MoTa, TrangThai = selected.TrangThai }) { Owner = this };
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenXoaVaiTro")) return;
        try
        {
            if (MessageBox.Show("Xóa vai trò này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            // Ghép Gop: kiểm tra kết quả xóa do BUS gốc trả về trước khi đóng chi tiết.
            BUS_KiemTra.KetQua(bus.xoaVT((VaiTro)DataContext));
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnVaiTroQuyenPhanQuyen_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenPhanQuyen")) return;
        var form = new PhanQuyenWindow(DataContext as VaiTro) { Owner = this };
        form.ShowDialog();
    }
}
