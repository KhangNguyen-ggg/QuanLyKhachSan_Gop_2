using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class KhachLuuTruChiTietWindow : Window
{
    public KhachLuuTruChiTietWindow()
    {
        InitializeComponent();
        btnKhachLuuTruThemKhachLuuTru.IsEnabled = DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruThemKhachLuuTru");
        btnKhachLuuTruSuaThongTin.IsEnabled = false;
        btnKhachLuuTruChuyenPhong.IsEnabled = false;
        btnKhachLuuTruBoKhachKhoiPhong.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext is System.Data.DataRowView row && row.Row.Table.Columns.Contains("TrangThai")
                ? Convert.ToString(row["TrangThai"]) ?? "" : "";
            btnKhachLuuTruSuaThongTin.IsEnabled = DataContext != null && DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruSuaThongTin");
            btnKhachLuuTruChuyenPhong.IsEnabled = DataContext != null && DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruChuyenPhong");
            btnKhachLuuTruBoKhachKhoiPhong.IsEnabled = DataContext != null && DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruBoKhachKhoiPhong");
        };
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private long Ma => Convert.ToInt64(((System.Data.DataRowView)DataContext)["MaXepPhong"]);

    private void btnKhachLuuTruSuaThongTin_Click(object sender, RoutedEventArgs e)
    {
        if (!btnKhachLuuTruSuaThongTin.IsEnabled || !DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruSuaThongTin")) return;
        try
        {
            var form = new KhachLuuTruForm(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnKhachLuuTruChuyenPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnKhachLuuTruChuyenPhong.IsEnabled || !DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruChuyenPhong")) return;
        try
        {
            var form = new KhachLuuTruChuyenPhongWindow(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnKhachLuuTruBoKhachKhoiPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnKhachLuuTruBoKhachKhoiPhong.IsEnabled || !DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruBoKhachKhoiPhong")) return;
        try
        {
            var form = new KhachLuuTruTraPhongWindow(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnKhachLuuTruThemKhachLuuTru_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("LuuTruView", "btnKhachLuuTruThemKhachLuuTru")) return;
        try
        {
            var form = new KhachLuuTruNhanPhongWindow();
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
