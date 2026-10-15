using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChiTietPhieuKhoForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly ChiTietPhieuKho duLieu;
    private readonly bool cap;
    private readonly string view, quyen;
    // Thêm: cấp nội bộ dùng lại form chọn vật tư/số lượng; phần giá không áp dụng.
    public ChiTietPhieuKhoForm(long phieu, ChiTietPhieuKho? dong = null, bool capNoiBo = false)
    {
        InitializeComponent(); cap = capNoiBo;
        duLieu = dong == null ? new ChiTietPhieuKho { MaPhieu = phieu, SoLuong = 1 } : BanNhap(dong);
        view = cap ? "TieuHaoView" : "KhoView";
        quyen = cap ? "btnCapVatTuNoiBoTaoPhieuCap" : duLieu.MaChiTietPhieuKho == 0 ? "btnChiTietPhieuKhoThemMoi" : "btnChiTietPhieuKhoSuaThongTin";
        DataContext = duLieu;
        pnlGia.Visibility = cap ? Visibility.Collapsed : Visibility.Visible;
        btnLuu.IsEnabled = DemoSession.CanUseButton(view, quyen);
        if (!DemoSession.CanOpen(view)) return;
        try { KhoVatTuGiaoDien.VatTu(cboVatTu, bus, duLieu.MaVatTu); }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(view, quyen)) return;
        try
        {
            duLieu.MaVatTu = KhoVatTuGiaoDien.Ma(cboVatTu);
            duLieu.SoLuong = BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng");
            if (cap)
                bus.Luu(new ChiTietCapVatTuNoiBo { MaChiTietCap = duLieu.MaChiTietPhieuKho, MaPhieuCap = duLieu.MaPhieu, MaVatTu = duLieu.MaVatTu, SoLuong = duLieu.SoLuong });
            else
            {
                duLieu.DonGia = string.IsNullOrWhiteSpace(txtDonGiaD.Text) ? null : BUS_KiemTra.SoTien(txtDonGiaD.Text, "Đơn giá");
                bus.Luu(duLieu);
            }
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
