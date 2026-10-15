using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TonKhoXuatKhoWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    public TonKhoXuatKhoWindow() : this(null) { }
    public TonKhoXuatKhoWindow(TonKhoHienThi? ton)
    {
        InitializeComponent();
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho");
        txtThongBao.Text = "Xác nhận sẽ tạo phiếu đã xác nhận và cập nhật tồn ngay.";
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            KhoVatTuGiaoDien.Kho(cboKho, bus, ton?.MaKho ?? 0);
            KhoVatTuGiaoDien.VatTu(cboVatTu, bus, ton?.MaVatTu ?? 0);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus);


        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoXuatKho")) return;
        try
        {
            int kho = KhoVatTuGiaoDien.Ma(cboKho), vatTu = KhoVatTuGiaoDien.Ma(cboVatTu), nhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            decimal soLuong = BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng");
            bus.NhapXuat("XUAT", kho, vatTu, soLuong, nhanVien, null, null, txtMucDich.Text, txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
