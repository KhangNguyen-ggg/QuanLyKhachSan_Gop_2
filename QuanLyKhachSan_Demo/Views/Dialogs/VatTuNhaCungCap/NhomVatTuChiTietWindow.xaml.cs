using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhomVatTuChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private NhomVatTu duLieu;
    private bool dangSua;
    private const string View = "VatTuNhaCungCapView";
    private const string QuyenSua = "btnNhomVatTuSuaThongTin";

    public NhomVatTuChiTietWindow(NhomVatTu x)
    {
        InitializeComponent();
        duLieu = x;
        btnNhomVatTuSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnNhomVatTuXoa.IsEnabled = DemoSession.CanUseButton(View, "btnNhomVatTuXoa");
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
            duLieu = bus.TheoId<NhomVatTu>(duLieu.MaNhomVatTu);
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
            var duLieuMoi = (NhomVatTu)DataContext;
            duLieuMoi.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieuMoi);
            duLieu = (NhomVatTu)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu nhóm vật tư.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnNhomVatTuXoa") || !XacNhanXoa(this, "Xóa nhóm vật tư này")) return;
        try { bus.Xoa<NhomVatTu>(duLieu.MaNhomVatTu); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

}
