using System.Windows;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChuyenKhoForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    public ChuyenKhoForm(TonKhoHienThi? ton = null)
    {
        InitializeComponent();
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoChuyenKho");
        txtThongBao.Text = "Trừ kho xuất và cộng kho nhập trong cùng giao dịch; thiếu tồn sẽ không chuyển.";
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            KhoVatTuGiaoDien.Kho(cboKhoXuat, bus, ton?.MaKho ?? 0);
            KhoVatTuGiaoDien.Kho(cboKhoNhap, bus);
            KhoVatTuGiaoDien.VatTu(cboVatTu, bus, ton?.MaVatTu ?? 0);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus);
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoChuyenKho")) return;
        try
        {
            bus.ChuyenKho(KhoVatTuGiaoDien.Ma(cboKhoXuat), KhoVatTuGiaoDien.Ma(cboKhoNhap), KhoVatTuGiaoDien.Ma(cboVatTu), BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng"), KhoVatTuGiaoDien.Ma(cboNhanVien), txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}

