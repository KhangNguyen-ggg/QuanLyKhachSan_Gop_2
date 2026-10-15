using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhaCungCapChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private NhaCungCap duLieu;
    private bool dangSua;
    private const string View = "VatTuNhaCungCapView";
    private const string QuyenSua = "btnNhaCungCapSuaThongTin";

    public NhaCungCapChiTietWindow(NhaCungCap x)
    {
        InitializeComponent();
        duLieu = x;
        btnNhaCungCapSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnNhaCungCapXoa.IsEnabled = DemoSession.CanUseButton(View, "btnNhaCungCapXoa");
        HienThi();
        if (!DemoSession.CanOpen(View)) return;
        try
        {
            TaiDanhMuc(); HienThi();
            btnVatTuNhaCungCapThemMoi.IsEnabled = DemoSession.CanUseButton(View, "btnVatTuNhaCungCapThemMoi");
            TaiNguonCung();
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
        pnlLienKet.IsEnabled = !sua;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnHuySua_Click(object sender, RoutedEventArgs e) => HienThi();
    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, QuyenSua)) return;
        try
        {
            duLieu = bus.TheoId<NhaCungCap>(duLieu.MaNhaCungCap);
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
            var duLieuMoi = (NhaCungCap)DataContext;
            duLieuMoi.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieuMoi);
            duLieu = (NhaCungCap)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu nhà cung cấp.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnNhaCungCapXoa") || !XacNhanXoa(this, "Xóa nhà cung cấp này")) return;
        try { bus.Xoa<NhaCungCap>(duLieu.MaNhaCungCap); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void TaiNguonCung() => dgvVatTuNhaCungCap.ItemsSource = bus.NguonCung(duLieu.MaNhaCungCap);
    private void btnNguonCung_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton("VatTuNhaCungCapView", "btnVatTuNhaCungCapThemMoi")) return;
        try
        {
            var form = new VatTuNhaCungCapForm(null, duLieu.MaNhaCungCap) { Owner = this };
            if (form.ShowDialog() == true) TaiNguonCung();
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void NguonCung_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (dangSua || !DemoSession.CanOpen("VatTuNhaCungCapView") || e.OriginalSource is not DependencyObject source ||
            System.Windows.Controls.ItemsControl.ContainerFromElement(dgvVatTuNhaCungCap, source) is not System.Windows.Controls.DataGridRow row ||
            row.Item is not NhaCungCapVatTuHienThi x) return;
        new VatTuNhaCungCapChiTietWindow(x) { Owner = this }.ShowDialog();
        try { TaiNguonCung(); } catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }


}
