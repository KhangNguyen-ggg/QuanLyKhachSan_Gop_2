using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachLuuTruChuyenPhongWindow : Window
{
    private readonly string actionId;
    private readonly BUS_KhachTheoPhong bus = new BUS_KhachTheoPhong();
    private readonly long ma;
    public KhachLuuTruChuyenPhongWindow(long maXepPhong = 0)
    {
        InitializeComponent();
        actionId = "btnKhachLuuTruChuyenPhong";
        btnLuu.IsEnabled = DemoSession.CanUseButton("LuuTruView", actionId);
        ma = maXepPhong;
        var xp = bus.HienThiXepPhong(new XepPhong { MaXepPhong = ma });
        txtPhongCu.DataContext = bus.PhongCuaLuot(xp);
        cboPhong.ItemsSource = bus.PhongChuyenDen(ma);
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("LuuTruView", actionId)) return;
        try
        {
            bus.ChuyenPhongDuocChon(ma, cboPhong.SelectedValue, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
