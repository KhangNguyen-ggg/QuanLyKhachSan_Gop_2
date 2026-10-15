using System;
using System.Windows;
using System.Windows.Controls;
using ET_QuanLyKhachSan;
// Ghép Gop: dùng đúng namespace nghiệp vụ gốc, giữ nguyên luồng giao diện.
using BUS_QuanlyKhachSan.LeTanValuuTru;
using BUS_QuanlyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;
public partial class HoaDonForm : Window
{
    private readonly string actionId;
    private readonly BUS_HoaDon bus = new BUS_HoaDon();
    private readonly List<SuDungDichVu> dichVuThem = new List<SuDungDichVu>();
    private List<ChiTietHoaDon> khoanThu = new List<ChiTietHoaDon>();
    private List<DichVu> dichVu = new List<DichVu>();
    private List<GoiDichVu> goiDichVu = new List<GoiDichVu>();
    public HoaDonForm()
    {
        InitializeComponent();
        actionId = "btnHoaDonTaoHoaDon";
        btnLuu.IsEnabled = DemoSession.CanUseButton("HoaDonView", actionId);
        cboDatPhong.ItemsSource = bus.HienThiDatPhong();
        cboNhanVien.ItemsSource = bus.HienThiNhanVien();
        cboUuDai.ItemsSource = bus.HienThiUuDai();
        dichVu = bus.HienThiDichVu();
        goiDichVu = bus.HienThiGoiDichVu();
        cboDichVu.ItemsSource = dichVu;
        cboGoiDichVu.ItemsSource = goiDichVu;
        TinhTien();
    }
    private void btnDong_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void TinhTien()
    {
        dgvKhoanThu.ItemsSource = khoanThu;
        dgvDichVuThem.ItemsSource = bus.DichVuDaChon(dichVuThem, dichVu, goiDichVu);
        DataContext = bus.XemTruocHoaDon(khoanThu, dichVuThem, cboUuDai.SelectedItem as UuDai);
    }
    private void Don_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            dichVuThem.Clear();
            khoanThu.Clear();
            txtKhachHang.DataContext = null;
            TinhTien();
            var don = cboDatPhong.SelectedItem as DatPhong;
            khoanThu = bus.KhoanThuDuocChon(don);
            txtKhachHang.DataContext = bus.KhachCuaDon(don);
            TinhTien();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void ThemDichVu_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bus.ThemDichVu(dichVuThem, cboDatPhong.SelectedValue, cboDichVu.SelectedItem as DichVu, txtSoLuongDichVu.Text);
            TinhTien();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void ThemGoi_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bus.ThemGoiDichVu(dichVuThem, cboDatPhong.SelectedValue, cboGoiDichVu.SelectedItem as GoiDichVu, txtSoLuongGoi.Text);
            TinhTien();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void BoDichVu_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BUS_KiemTra.BoDong(dichVuThem, dgvDichVuThem.SelectedIndex);
            TinhTien();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
    private void UuDai_Changed(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            TinhTien();
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
            bus.LuuHoaDon(cboDatPhong.SelectedValue, cboNhanVien.SelectedValue,
                cboUuDai.SelectedItem as UuDai, txtGhiChu.Text, dichVuThem);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Thông báo");
        }
    }
}
