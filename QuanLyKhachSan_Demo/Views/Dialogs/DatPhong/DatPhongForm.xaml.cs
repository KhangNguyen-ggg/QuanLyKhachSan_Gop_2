using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class DatPhongForm : Window
{
    private readonly string actionId;
    private readonly BUS_DatPhong bus = new BUS_DatPhong();
    private readonly long maDatPhong;
    private List<ChiTietDatPhong> chiTiet = new List<ChiTietDatPhong>();
    private List<LoaiPhong> loaiPhong = new List<LoaiPhong>();
    public DatPhongForm(long ma = 0)
    {
        InitializeComponent();
        actionId = ma == 0 ? "btnDatPhongDatPhongMoi" : "btnDatPhongSuaDatPhong";
        btnLuu.IsEnabled = DemoSession.CanUseButton("DatPhongView", actionId);
        maDatPhong = ma;
        cboKhachHang.ItemsSource = bus.HienThiKhachHang();
        loaiPhong = bus.HienThiLoaiPhong();
        cboLoaiPhong.ItemsSource = loaiPhong;
        dtpNgayNhan.SelectedDate = DateTime.Today;
        dtpNgayTra.SelectedDate = DateTime.Today.AddDays(1);
        if (ma != 0)
        {
            DataContext = bus.HienThiDatPhong(new DatPhong { MaDatPhong = ma });
            chiTiet = bus.ChiTiet(ma);
        }
        TaiChiTiet();
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void TaiChiTiet()
    {
        lstChiTiet.ItemsSource = bus.HienThiChiTietTam(chiTiet, loaiPhong);
    }
    private void ThemChiTiet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var ct = bus.TaoChiTiet(cboLoaiPhong.SelectedValue, dtpNgayNhan.SelectedDate,
                dtpNgayTra.SelectedDate, txtSoPhong.Text, txtNguoiLon.Text, txtTreEm.Text, txtDonGia.Text);
            chiTiet.Add(ct);
            TaiChiTiet();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void BoChiTiet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BUS_KiemTra.BoDong(chiTiet, lstChiTiet.SelectedIndex);
            TaiChiTiet();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("DatPhongView", actionId)) return;
        try
        {
            bus.LuuDatPhong(maDatPhong, cboKhachHang.SelectedValue, txtGhiChu.Text, chiTiet);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
