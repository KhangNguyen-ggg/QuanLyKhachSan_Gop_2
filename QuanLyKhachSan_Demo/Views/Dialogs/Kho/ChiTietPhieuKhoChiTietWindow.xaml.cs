using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChiTietPhieuKhoChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private ChiTietPhieuKhoHienThi duLieu;
    private readonly bool cap;
    private bool dangSua;
    private readonly string view, quyenSua, quyenXoa;
    public ChiTietPhieuKhoChiTietWindow(ChiTietCapHienThi x) : this(DongCap(x), true) { }
    private static ChiTietPhieuKhoHienThi DongCap(ChiTietCapHienThi x) => new() {
        MaChiTietPhieuKho = x.MaChiTietCap, MaPhieu = x.MaPhieuCap, MaVatTu = x.MaVatTu,
        TenVatTu = x.TenVatTu, DonViTinh = x.DonViTinh, SoLuong = x.SoLuong };
    public ChiTietPhieuKhoChiTietWindow(ChiTietPhieuKhoHienThi x, bool capNoiBo = false)
    {
        InitializeComponent(); duLieu = x; cap = capNoiBo;
        pnlGia.Visibility = cap ? Visibility.Collapsed : Visibility.Visible;
        view = cap ? "TieuHaoView" : "KhoView";
        quyenSua = cap ? "btnCapVatTuNoiBoTaoPhieuCap" : "btnChiTietPhieuKhoSuaThongTin";
        quyenXoa = cap ? "btnCapVatTuNoiBoHuyPhieuCap" : "btnChiTietPhieuKhoXoa";
        cboVatTu.SelectionChanged += (_, _) => { if (cboVatTu.SelectedItem is VatTu v) txtDonVi.Text = v.DonViTinh; };
        HienThi(); Tai();
    }
    private bool Tai()
    {
        btnChiTietPhieuKhoSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = btnChiTietPhieuKhoXoa.IsEnabled = false;
        if (!DemoSession.CanOpen(view)) return false;
        try
        {
            bool moi = (cap ? bus.TheoId<PhieuCapVatTuNoiBo>(duLieu.MaPhieu).TrangThai : bus.TheoId<PhieuNhapXuat>(duLieu.MaPhieu).TrangThai) == "MOI";
            duLieu = cap ? DongCap(bus.ChiTietCap(duLieu.MaPhieu).Single(x => x.MaChiTietCap == duLieu.MaChiTietPhieuKho)) :
                bus.ChiTiet(duLieu.MaPhieu).Single(x => x.MaChiTietPhieuKho == duLieu.MaChiTietPhieuKho);
            LuaChon(cboVatTu, bus.DanhMuc<VatTu>().Where(x => x.TrangThai || x.MaLoaiVatTu == duLieu.MaVatTu), "TenVatTu", "MaLoaiVatTu", duLieu.MaVatTu);
            btnChiTietPhieuKhoSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = moi && DemoSession.CanUseButton(view, quyenSua);
            btnChiTietPhieuKhoXoa.IsEnabled = moi && DemoSession.CanUseButton(view, quyenXoa);
            HienThi();
            if (!moi) txtThongBao.Text = "Phiếu đã xử lý: chỉ xem.";
            return moi;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; return false; }
    }
    private void HienThi()
    {
        DataContext = BanNhap(duLieu);
        cboVatTu.SelectedValue = duLieu.MaVatTu;
        txtDonVi.Text = duLieu.DonViTinh;
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        KhoVatTuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnHuySua_Click(object sender, RoutedEventArgs e) => HienThi();
    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua && DemoSession.CanUseButton(view, quyenSua) && Tai()) SetEditMode(true);
    }
    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua || !DemoSession.CanUseButton(view, quyenSua)) return;
        try
        {
            var x = (ChiTietPhieuKhoHienThi)DataContext;
            x.MaVatTu = KhoVatTuGiaoDien.Ma(cboVatTu);
            x.SoLuong = BUS_KhoVatTu.SoLuong(txtSoLuong.Text, "Số lượng");
            // Thêm: cấp nội bộ dùng chung phần mở khóa nhưng không có giá hoặc ghi chú dòng.
            if (cap) bus.Luu(new ChiTietCapVatTuNoiBo { MaChiTietCap = x.MaChiTietPhieuKho, MaPhieuCap = x.MaPhieu, MaVatTu = x.MaVatTu, SoLuong = x.SoLuong });
            else
            {
                x.DonGia = string.IsNullOrWhiteSpace(txtDonGiaD.Text) ? null : BUS_KiemTra.SoTien(txtDonGiaD.Text, "Đơn giá");
                bus.Luu((ChiTietPhieuKho)x);
                x.ThanhTien = decimal.Round(x.SoLuong * (x.DonGia ?? 0), 2, MidpointRounding.AwayFromZero);
            }
            if (cboVatTu.SelectedItem is VatTu v) { x.TenVatTu = v.TenVatTu; x.DonViTinh = v.DonViTinh; }
            duLieu = x; HienThi();
            txtThongBao.Text = "Đã lưu vật tư trong phiếu.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(view, quyenXoa) || !XacNhanXoa(this, "Xóa vật tư này khỏi phiếu")) return;
        try { bus.XoaChiTiet(duLieu.MaChiTietPhieuKho, duLieu.MaPhieu, cap); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
