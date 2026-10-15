using System.Windows;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CapVatTuNoiBoForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly PhieuCapVatTuNoiBo duLieu = new();
    public long MaPhieuCap => duLieu.MaPhieuCap;
    public CapVatTuNoiBoForm()
    {
        InitializeComponent(); DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap");
        txtThongBao.Text = "Lưu phiếu Mới rồi thêm các vật tư khác hoặc xác nhận ở cửa sổ chi tiết. Chưa trừ tồn khi lưu.";
        if (!DemoSession.CanOpen("TieuHaoView")) return;
        try
        {
            DanhMuc(cboMucDich, BUS_KhoVatTu.MucDichCap, duLieu.MucDich);
            KhoVatTuGiaoDien.Kho(cboKho, bus);
            KhoVatTuGiaoDien.VatTu(cboVatTu, bus);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus);
            TaiThamChieu();
            cboMucDich.SelectionChanged += (_, _) => TaiThamChieu();
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void TaiThamChieu()
    {
        try
        {
            string mucDich = TrangThai(cboMucDich);
            var nguon = new[] { new ThamChieuKho { Ten = "— Không tham chiếu —" } }.AsEnumerable();
            if (mucDich != "KHAC") nguon = nguon.Concat(bus.ThamChieu(mucDich == "DICH_VU"));
            LuaChon(cboThamChieu, nguon, "Ten", "Ma", 0L);
            cboThamChieu.IsEnabled = mucDich != "KHAC";
        }
        catch (Exception ex) { cboThamChieu.ItemsSource = null; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap")) return;
        try
        {
            duLieu.MucDich = TrangThai(cboMucDich);
            duLieu.MaKho = KhoVatTuGiaoDien.Ma(cboKho);
            duLieu.MaNhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            long? ma = cboThamChieu.SelectedValue is long so && so > 0 ? so : null;
            // Khác Phòng ban: tham chiếu phải khớp mục đích, không dùng mã hiển thị DPH001/SD002.
            duLieu.MaDatPhong = duLieu.MucDich == "DON_PHONG" ? ma : null;
            duLieu.MaSuDungDichVu = duLieu.MucDich == "DICH_VU" ? ma : null;
            bus.TaoCap(duLieu, KhoVatTuGiaoDien.Ma(cboVatTu), BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng"));
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
