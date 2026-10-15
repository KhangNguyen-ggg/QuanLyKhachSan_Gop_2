using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class HoaDonApDungUuDaiWindow : Window
{
    private readonly string actionId;
    private readonly BUS_HoaDon bus = new BUS_HoaDon();
    private readonly HoaDon hoaDon;
    public HoaDonApDungUuDaiWindow(long ma = 0)
    {
        InitializeComponent();
        actionId = "btnHoaDonApDungUuDai";
        btnLuu.IsEnabled = DemoSession.CanUseButton("HoaDonView", actionId);
        hoaDon = bus.HienThiHoaDon(new HoaDon { MaHoaDon = ma });
        DataContext = hoaDon;
        cboUuDai.ItemsSource = bus.HienThiUuDai();
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void UuDai_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            txtGiam.DataContext = BUS_HoaDon.TinhGiam(hoaDon.TongTien, cboUuDai.SelectedItem as UuDai);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void BoUuDai_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            cboUuDai.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("HoaDonView", actionId)) return;
        try
        {
            bus.ApDungUuDai(hoaDon.MaHoaDon, (cboUuDai.SelectedItem as UuDai)?.MaUuDai, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
