using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachHangChiTietWindow : Window
{
    public KhachHangChiTietWindow()
    {
        InitializeComponent();
        btnKhachHangThemMoi.IsEnabled = DemoSession.CanUseButton("KhachHangView", "btnKhachHangThemMoi");
        btnKhachHangSuaThongTin.IsEnabled = false;
        btnKhachHangXoa.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext is System.Data.DataRowView row && row.Row.Table.Columns.Contains("TrangThai")
                ? Convert.ToString(row["TrangThai"]) ?? "" : "";
            btnKhachHangSuaThongTin.IsEnabled = DataContext != null && DemoSession.CanUseButton("KhachHangView", "btnKhachHangSuaThongTin");
            btnKhachHangXoa.IsEnabled = DataContext != null && DemoSession.CanUseButton("KhachHangView", "btnKhachHangXoa");
        };
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private long Ma => ((ET_QuanLyKhachSan.KhachHang)DataContext).MaKhachHang;

    private void btnKhachHangSuaThongTin_Click(object sender, RoutedEventArgs e)
    {
        if (!btnKhachHangSuaThongTin.IsEnabled || !DemoSession.CanUseButton("KhachHangView", "btnKhachHangSuaThongTin")) return;
        try
        {
            var form = new KhachHangForm(checked((int)Ma));
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnKhachHangXoa_Click(object sender, RoutedEventArgs e)
    {
        if (!btnKhachHangXoa.IsEnabled || !DemoSession.CanUseButton("KhachHangView", "btnKhachHangXoa")) return;
        try
        {
            if (MessageBox.Show("Xóa bản ghi này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            new BUS_KhachHang().XoaKhachHang(checked((int)Ma));
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnKhachHangThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhachHangView", "btnKhachHangThemMoi")) return;
        try
        {
            var form = new KhachHangForm();
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
