using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachLuuTruForm : Window
{
    private readonly string actionId;
    private readonly BUS_KhachTheoPhong bus = new BUS_KhachTheoPhong();
    private readonly long ma;
    public KhachLuuTruForm(long maXepPhong = 0)
    {
        InitializeComponent();
        actionId = "btnKhachLuuTruSuaThongTin";
        btnLuu.IsEnabled = DemoSession.CanUseButton("LuuTruView", actionId);
        ma = maXepPhong;
        var xp = bus.HienThiXepPhong(new XepPhong { MaXepPhong = ma });
        DataContext = xp;
        txtPhong.DataContext = bus.PhongCuaLuot(xp);
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
            bus.SuaGhiChuLuuTru(ma, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
