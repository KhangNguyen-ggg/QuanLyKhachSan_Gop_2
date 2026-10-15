using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhanQuyenWindow : Window
{
    public PhanQuyenWindow(VaiTro? vaiTro = null)
    {
        InitializeComponent();
        DataContext = vaiTro;
        cboVaiTro.ItemsSource = vaiTro == null ? Array.Empty<VaiTro>() : new[] { vaiTro };
        cboVaiTro.SelectedItem = vaiTro;
        cboVaiTro.IsEnabled = false;
        btnLuu.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenPhanQuyen");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", "btnVaiTroQuyenPhanQuyen")) return;
        // shortcut: giữ giao diện phân quyền, nối BUS/DAL khi triển khai lưu quyền.
        txtThongBao.Text = "Chức năng lưu phân quyền chưa được triển khai. Thay đổi chưa được lưu.";
    }

    private void btnChonTatCa_Click(object sender, RoutedEventArgs e)
    {
        foreach (var group in pnlNoiDung.Children.OfType<WrapPanel>())
            foreach (var box in group.Children.OfType<CheckBox>()) box.IsChecked = true;
    }

    private void btnBoChon_Click(object sender, RoutedEventArgs e)
    {
        foreach (var group in pnlNoiDung.Children.OfType<WrapPanel>())
            foreach (var box in group.Children.OfType<CheckBox>()) box.IsChecked = false;
    }
}