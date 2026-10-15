using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachLuuTruNhanPhongWindow : Window
{
    private readonly string actionId;
    private readonly BUS_KhachTheoPhong bus = new BUS_KhachTheoPhong();
    public KhachLuuTruNhanPhongWindow(long maXepPhong = 0)
    {
        InitializeComponent();
        actionId = "btnKhachLuuTruThemKhachLuuTru";
        btnLuu.IsEnabled = DemoSession.CanUseButton("LuuTruView", actionId);
        cboXepPhong.ItemsSource = bus.XepPhongTheoTrangThai("DaXep");
        cboXepPhong.SelectedValue = maXepPhong;
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void Xep_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            txtPhong.DataContext = bus.PhongCuaLuot(cboXepPhong.SelectedItem as XepPhong);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("LuuTruView", actionId)) return;
        try
        {
            bus.NhanPhongDuocChon(cboXepPhong.SelectedValue);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
