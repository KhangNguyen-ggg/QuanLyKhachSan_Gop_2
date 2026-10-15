using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TonKhoForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    public TonKhoForm() : this(null) { }
    public TonKhoForm(TonKhoHienThi? ton)
    {
        InitializeComponent();
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon");
        txtThongBao.Text = "Kiểm kê ghi chứng từ và chênh lệch vào nhật ký; nhập đúng số đang có thực tế.";
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            KhoVatTuGiaoDien.Kho(cboKho, bus, ton?.MaKho ?? 0);
            KhoVatTuGiaoDien.VatTu(cboVatTu, bus, ton?.MaVatTu ?? 0);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus);

            txtTonKiemKe.Text = (ton?.Ton ?? 0).ToString("0.####");
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", "btnTonKhoDieuChinhTon")) return;
        try
        {
            int kho = KhoVatTuGiaoDien.Ma(cboKho), vatTu = KhoVatTuGiaoDien.Ma(cboVatTu), nhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            // Khác danh mục: không gán thẳng số tồn; procedure ghi phiếu kiểm kê và nhật ký.
            bus.KiemKe(kho, vatTu, BUS_KhoVatTu.SoLuong(txtTonKiemKe.Text, "Tồn thực tế", true), nhanVien, txtLyDoDieuChinh.Text);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
