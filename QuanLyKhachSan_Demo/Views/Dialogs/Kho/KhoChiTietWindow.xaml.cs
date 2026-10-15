using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhoChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private Kho duLieu;
    private bool dangSua;
    private const string View = "KhoView";
    private const string QuyenSua = "btnKhoSuaThongTin";

    public KhoChiTietWindow(Kho x)
    {
        InitializeComponent();
        duLieu = x;
        btnKhoSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnKhoXoa.IsEnabled = DemoSession.CanUseButton(View, "btnKhoXoa");
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
            duLieu = bus.TheoId<Kho>(duLieu.MaKho);
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
            var duLieuMoi = (Kho)DataContext;
            duLieuMoi.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieuMoi);
            duLieu = (Kho)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu kho.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnKhoXoa") || !XacNhanXoa(this, "Xóa kho này")) return;
        try { bus.Xoa<Kho>(duLieu.MaKho); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

}
