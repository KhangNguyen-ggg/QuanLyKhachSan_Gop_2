using System.Windows;
using BUS_QuanlyKhachSan.HeThong;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VaiTroQuyenForm : Window
{
    private readonly BUS_VaiTro bus = new();
    private readonly VaiTro vaiTro;
    private readonly string actionId;

    public VaiTroQuyenForm(VaiTro? duLieu = null)
    {
        InitializeComponent();
        vaiTro = duLieu ?? new VaiTro { TrangThai = true };
        actionId = vaiTro.MaVaiTro == 0 ? "btnVaiTroQuyenThemVaiTro" : "btnVaiTroQuyenSuaVaiTro";
        DataContext = vaiTro;
        btnLuu.IsEnabled = DemoSession.CanUseButton("TaiKhoanView", actionId);
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TaiKhoanView", actionId)) return;
        try
        {
            bus.LuuVaiTro(vaiTro);
            DialogResult = true;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}