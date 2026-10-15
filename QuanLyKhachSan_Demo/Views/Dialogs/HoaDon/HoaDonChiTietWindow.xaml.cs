using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class HoaDonChiTietWindow : Window
{
    private readonly BUS_HoaDon bus = new BUS_HoaDon();
    private readonly long ma;
    public HoaDonChiTietWindow(long maHoaDon = 0)
    {
        InitializeComponent();
        btnHoaDonTaoHoaDon.IsEnabled = DemoSession.CanUseButton("HoaDonView", "btnHoaDonTaoHoaDon");
        btnHoaDonXemMauIn.IsEnabled = DemoSession.CanUseButton("HoaDonView", "btnHoaDonXemMauIn");
        btnHoaDonThuTienMat.IsEnabled = false;
        btnHoaDonApDungUuDai.IsEnabled = false;
        btnHoaDonHuyHoaDon.IsEnabled = false;
        DataContextChanged += (_, _) =>
        {
            string status = DataContext is System.Data.DataRowView row && row.Row.Table.Columns.Contains("TrangThai")
                ? Convert.ToString(row["TrangThai"]) ?? "" : "";
            btnHoaDonThuTienMat.IsEnabled = DataContext != null && (status is "ChuaThanhToan" or "ThanhToanMotPhan") && DemoSession.CanUseButton("HoaDonView", "btnHoaDonThuTienMat");
            btnHoaDonApDungUuDai.IsEnabled = DataContext != null && (status != "Huy") && DemoSession.CanUseButton("HoaDonView", "btnHoaDonApDungUuDai");
            btnHoaDonHuyHoaDon.IsEnabled = DataContext != null && (status == "ChuaThanhToan") && DemoSession.CanUseButton("HoaDonView", "btnHoaDonHuyHoaDon");
        };
        ma = maHoaDon;
        dgvChiTiet.ItemsSource = bus.ChiTietHoaDon(ma);
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void In_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("HoaDonView", "btnHoaDonXemMauIn")) return;
        try
        {
            var doc = new FlowDocument { FontFamily = new FontFamily("Segoe UI"), FontSize = 12, PagePadding = new Thickness(40) };
            foreach (string dong in bus.NoiDungIn(ma))
                doc.Blocks.Add(new Paragraph(new Run(dong)));
            var dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
                dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, $"Hóa đơn {ma}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }

    private long Ma => ma;

    private void btnHoaDonThuTienMat_Click(object sender, RoutedEventArgs e)
    {
        if (!btnHoaDonThuTienMat.IsEnabled || !DemoSession.CanUseButton("HoaDonView", "btnHoaDonThuTienMat")) return;
        try
        {
            var form = new HoaDonThuTienMatWindow(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnHoaDonApDungUuDai_Click(object sender, RoutedEventArgs e)
    {
        if (!btnHoaDonApDungUuDai.IsEnabled || !DemoSession.CanUseButton("HoaDonView", "btnHoaDonApDungUuDai")) return;
        try
        {
            var form = new HoaDonApDungUuDaiWindow(Ma);
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
    private void btnHoaDonHuyHoaDon_Click(object sender, RoutedEventArgs e)
    {
        if (!btnHoaDonHuyHoaDon.IsEnabled || !DemoSession.CanUseButton("HoaDonView", "btnHoaDonHuyHoaDon")) return;
        try
        {
            if (MessageBox.Show("Hủy hóa đơn bản ghi này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            new BUS_HoaDon().HuyHoaDon(Ma);
            Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }

    private void btnHoaDonTaoHoaDon_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("HoaDonView", "btnHoaDonTaoHoaDon")) return;
        try
        {
            var form = new HoaDonForm();
            form.Owner = this;
            if (form.ShowDialog() == true) Close();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Thông báo"); }
    }
}
