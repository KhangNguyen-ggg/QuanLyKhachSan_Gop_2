using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class PhanBoPhongChiTietWindow : Window
{
    public PhanBoPhongChiTietWindow()
    {
        InitializeComponent();
        btnPhanBoPhongThemPhong.IsEnabled = DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongThemPhong");
        btnPhanBoPhongDoiPhong.IsEnabled = false;
        btnPhanBoPhongBoPhanBo.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext is System.Data.DataRowView row && row.Row.Table.Columns.Contains("TrangThai")
                ? Convert.ToString(row["TrangThai"]) ?? "" : "";
            btnPhanBoPhongDoiPhong.IsEnabled = DataContext != null && (status == "DaXep") && DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongDoiPhong");
            btnPhanBoPhongBoPhanBo.IsEnabled = DataContext != null && (status == "DaXep") && DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongBoPhanBo");
        };
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private long Ma => Convert.ToInt64(((System.Data.DataRowView)DataContext)["MaXepPhong"]);

    private void btnPhanBoPhongDoiPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!btnPhanBoPhongDoiPhong.IsEnabled || !DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongDoiPhong")) return;
        try
        {
            var form = new PhanBoPhongForm(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnPhanBoPhongBoPhanBo_Click(object sender, RoutedEventArgs e)
    {
        if (!btnPhanBoPhongBoPhanBo.IsEnabled || !DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongBoPhanBo")) return;
        try
        {
            if (MessageBox.Show("Bỏ phân bổ bản ghi này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            new Bus_PhanBoPhong().BoPhanBo(Ma);
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnPhanBoPhongThemPhong_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhanBoPhongView", "btnPhanBoPhongThemPhong")) return;
        try
        {
            var form = new PhanBoPhongForm(0, Convert.ToInt64(((System.Data.DataRowView)DataContext)["MaDatPhong"]));
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
