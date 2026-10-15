using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TonKhoNhapKhoWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    public TonKhoNhapKhoWindow() : this(null) { }
    public TonKhoNhapKhoWindow(TonKhoHienThi? ton)
    {
        InitializeComponent();
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho");
        txtThongBao.Text = "Xác nhận sẽ tạo phiếu đã xác nhận và cập nhật tồn ngay.";
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            KhoVatTuGiaoDien.Kho(cboKho, bus, ton?.MaKho ?? 0);
            KhoVatTuGiaoDien.VatTu(cboVatTu, bus, ton?.MaVatTu ?? 0);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus);
            KhoVatTuGiaoDien.NhaCungCap(cboNhaCungCap, bus);

        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoNhapKho")) return;
        try
        {
            int kho = KhoVatTuGiaoDien.Ma(cboKho), vatTu = KhoVatTuGiaoDien.Ma(cboVatTu), nhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            decimal soLuong = BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng");
            int ncc = KhoVatTuGiaoDien.Ma(cboNhaCungCap);
            decimal? gia = string.IsNullOrWhiteSpace(txtDonGiaNhapD.Text) ? null : BUS_KiemTra.SoTien(txtDonGiaNhapD.Text, "Đơn giá");
            bus.NhapXuat("NHAP", kho, vatTu, soLuong, nhanVien, ncc == 0 ? null : ncc, gia, "", txtGhiChu.Text);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
