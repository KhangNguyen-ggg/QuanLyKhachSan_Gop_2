using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class DatPhongChiTietWindow : Window
{
    private readonly BUS_DatPhong bus = new BUS_DatPhong();
    public DatPhongChiTietWindow(long ma = 0)
    {
        InitializeComponent();
        btnDatPhongSuaDatPhong.IsEnabled = false;
        btnDatPhongXacNhanDatPhong.IsEnabled = false;
        btnDatPhongHuyDatPhong.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext is System.Data.DataRowView row && row.Row.Table.Columns.Contains("TrangThai")
                ? Convert.ToString(row["TrangThai"]) ?? "" : "";
            btnDatPhongSuaDatPhong.IsEnabled = DataContext != null && DemoSession.CanUseButton("DatPhongView", "btnDatPhongSuaDatPhong");
            btnDatPhongXacNhanDatPhong.IsEnabled = DataContext != null && (status == "ChoXacNhan") && DemoSession.CanUseButton("DatPhongView", "btnDatPhongXacNhanDatPhong");
            btnDatPhongHuyDatPhong.IsEnabled = DataContext != null && (status is "ChoXacNhan" or "DaXacNhan") && DemoSession.CanUseButton("DatPhongView", "btnDatPhongHuyDatPhong");
        };
        lstChiTiet.ItemsSource = bus.ChiTietHienThi(ma);
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private long Ma => Convert.ToInt64(((System.Data.DataRowView)DataContext)["MaDatPhong"]);

    private void btnDatPhongSuaDatPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnDatPhongSuaDatPhong.IsEnabled || !DemoSession.CanUseButton("DatPhongView", "btnDatPhongSuaDatPhong")) return;
        try
        {
            var form = new DatPhongForm(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnDatPhongXacNhanDatPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnDatPhongXacNhanDatPhong.IsEnabled || !DemoSession.CanUseButton("DatPhongView", "btnDatPhongXacNhanDatPhong")) return;
        try
        {
            new BUS_DatPhong().XacNhan(Ma);
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnDatPhongHuyDatPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnDatPhongHuyDatPhong.IsEnabled || !DemoSession.CanUseButton("DatPhongView", "btnDatPhongHuyDatPhong")) return;
        try
        {
            if (MessageBox.Show("Hủy đơn bản ghi này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            new BUS_DatPhong().HuyDatPhong(Ma);
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
