using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class DinhMucTieuHaoChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private DinhMucHienThi duLieu;
    private bool dangSua;
    private const string View = "TieuHaoView";
    private const string QuyenSua = "btnDinhMucTieuHaoSuaThongTin";

    public DinhMucTieuHaoChiTietWindow(DinhMucHienThi x)
    {
        InitializeComponent();
        duLieu = x;
        btnDinhMucTieuHaoSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnDinhMucTieuHaoXoa.IsEnabled = DemoSession.CanUseButton(View, "btnDinhMucTieuHaoXoa");
        HienThi();
    }
    private void HienThi()
    {
        // Thêm: bản nháp riêng như Phòng ban; Hủy sửa/Đóng không đổi dòng gốc hoặc SQL.
        DataContext = BanNhap(duLieu);

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
            duLieu = bus.DinhMuc().Single(x => x.MaDichVu == duLieu.MaDichVu && x.MaVatTu == duLieu.MaVatTu);
            HienThi();
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
            var duLieuMoi = (TieuHaoVatTu)DataContext;
            // Giữ nguyên khóa ghép và đơn vị; chỉ sửa lượng tiêu hao và ghi chú.
            duLieuMoi.SoLuongTieuHao = BUS_KhoVatTu.SoLuong(txtSoLuongLan.Text, "Số lượng mỗi lần", false, 8);
            bus.Luu(duLieuMoi, false);
            duLieu = (DinhMucHienThi)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu định mức tiêu hao.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnDinhMucTieuHaoXoa") || !XacNhanXoa(this, "Xóa định mức tiêu hao này")) return;
        try { bus.Xoa((TieuHaoVatTu)duLieu); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

}
