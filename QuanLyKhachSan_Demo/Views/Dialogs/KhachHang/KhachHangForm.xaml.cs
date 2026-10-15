using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachHangForm : Window
{
    private readonly string actionId;
    private readonly BUS_KhachHang bus = new BUS_KhachHang();
    private readonly KhachHang khach;
    public KhachHangForm(int ma = 0)
    {
        InitializeComponent();
        actionId = ma == 0 ? "btnKhachHangThemMoi" : "btnKhachHangSuaThongTin";
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhachHangView", actionId);
        khach = bus.DuLieuForm(ma);
        cboLoaiGiayTo.ItemsSource = BUS_KhachHang.LoaiGiayToChoPhep;
        DataContext = khach;
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void LoaiGiayTo_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            txtHuongDanGiayTo.Text = BUS_KhachHang.HuongDanSoGiayTo(cboLoaiGiayTo.SelectedItem as string);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhachHangView", actionId)) return;
        try
        {
            bus.LuuKhachHang(khach);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
