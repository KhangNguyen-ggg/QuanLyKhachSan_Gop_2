using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class HoaDonThuTienMatWindow : Window
{
    private readonly string actionId;
    private readonly BUS_HoaDon bus = new BUS_HoaDon();
    private readonly long ma;
    public HoaDonThuTienMatWindow(long maHoaDon = 0)
    {
        InitializeComponent();
        actionId = "btnHoaDonThuTienMat";
        btnLuu.IsEnabled = DemoSession.CanUseButton("HoaDonView", actionId);
        ma = maHoaDon;
        cboNhanVien.ItemsSource = bus.HienThiNhanVien();
        DataContext = bus.ThongTinThuTien(ma);
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("HoaDonView", actionId)) return;
        try
        {
            bus.ThuTienNhap(ma, txtSoTien.Text, cboNhanVien.SelectedValue, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
