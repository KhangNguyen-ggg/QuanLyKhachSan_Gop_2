using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuNhaCungCapChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private NhaCungCapVatTuHienThi duLieu;
    private bool dangSua;
    private const string View = "VatTuNhaCungCapView";
    private const string QuyenSua = "btnVatTuNhaCungCapSuaThongTin";

    public VatTuNhaCungCapChiTietWindow(NhaCungCapVatTuHienThi x)
    {
        InitializeComponent();
        duLieu = x;
        btnVatTuNhaCungCapSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnVatTuNhaCungCapXoa.IsEnabled = DemoSession.CanUseButton(View, "btnVatTuNhaCungCapXoa");
        HienThi();
        if (!DemoSession.CanOpen(View)) return;
        try
        {
            TaiDanhMuc(); HienThi();

        }
        catch (Exception ex) { txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }
    private void TaiDanhMuc()
    {
            DanhMuc(cboTrangThai, BUS_KhoVatTu.TrangThai, duLieu.TrangThai.ToString());
    }
    private void HienThi()
    {
        // Thêm: bản nháp riêng như Phòng ban; Hủy sửa/Đóng không đổi dòng gốc hoặc SQL.
        DataContext = BanNhap(duLieu);
        cboTrangThai.SelectedValue = duLieu.TrangThai.ToString();
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
        if (dangSua || !DemoSession.CanUseButton(View, QuyenSua)) return;
        try
        {
            duLieu = bus.NguonCung(duLieu.MaNhaCungCap).Single(x => x.MaVatTu == duLieu.MaVatTu);
            TaiDanhMuc(); HienThi();
            // Bớt: không mở Form khác; mở khóa các ô trong chính cửa sổ này.
            SetEditMode(true);
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua || !DemoSession.CanUseButton(View, QuyenSua)) return;
        try
        {
            var duLieuMoi = (VatTu_NhaCungCap)DataContext;
            duLieuMoi.DonGiaNhap = string.IsNullOrWhiteSpace(txtGiaNhapD.Text) ? null : BUS_KiemTra.SoTien(txtGiaNhapD.Text, "Giá nhập");
            duLieuMoi.ThoiGianCungCap = KhoVatTuGiaoDien.Ngay(dtpNgay, "ngày cung cấp");
            duLieuMoi.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieuMoi, false);
            duLieu = (NhaCungCapVatTuHienThi)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu nguồn cung vật tư.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnVatTuNhaCungCapXoa") || !XacNhanXoa(this, "Xóa nguồn cung vật tư này")) return;
        try { bus.Xoa((VatTu_NhaCungCap)duLieu); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

}
