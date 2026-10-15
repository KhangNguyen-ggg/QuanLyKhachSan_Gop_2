using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class PhanBoPhongForm : Window
{
    private readonly string actionId;
    private readonly Bus_PhanBoPhong bus = new Bus_PhanBoPhong();
    private readonly long maXepPhong;
    public PhanBoPhongForm(long ma = 0, long maDatPhong = 0)
    {
        InitializeComponent();
        actionId = ma == 0 ? "btnPhanBoPhongThemPhong" : "btnPhanBoPhongDoiPhong";
        btnLuu.IsEnabled = DemoSession.CanUseButton("PhanBoPhongView", actionId);
        maXepPhong = ma;
        cboDatPhong.ItemsSource = bus.HienThiDatPhong();
        if (ma != 0)
        {
            var xp = bus.HienThiXepPhong(new XepPhong { MaXepPhong = ma });
            var ct = bus.HienThiChiTietDatPhong(new ChiTietDatPhong { MaChiTietDatPhong = xp.MaChiTietDatPhong });
            DataContext = xp;
            cboDatPhong.SelectedValue = ct.MaDatPhong;
            cboChiTiet.SelectedValue = ct.MaChiTietDatPhong;
            cboPhong.SelectedValue = xp.MaPhong;
        }
        else if (maDatPhong != 0) cboDatPhong.SelectedValue = maDatPhong;
        cboDatPhong.IsEnabled = ma == 0 && maDatPhong == 0;
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void Don_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            cboChiTiet.ItemsSource = bus.ChiTietDuocChon(cboDatPhong.SelectedValue);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void ChiTiet_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var ct = cboChiTiet.SelectedItem as ChiTietDatPhong;
            txtChiTiet.DataContext = bus.ThongTinChiTiet(ct);
            cboPhong.ItemsSource = bus.PhongTheoChiTiet(ct);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhanBoPhongView", actionId)) return;
        try
        {
            bus.LuuPhanBo(maXepPhong, cboDatPhong.SelectedValue, cboChiTiet.SelectedValue,
                cboPhong.SelectedValue, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
