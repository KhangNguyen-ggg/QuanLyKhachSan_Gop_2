using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhanVienChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private NhanVien duLieu;
    private bool dangSua;
    private const string View = "NhanVienView";
    private const string QuyenSua = "btnNhanVienSuaThongTin";

    public NhanVienChiTietWindow(NhanVien x)
    {
        InitializeComponent();
        ma = x.MaNhanVien;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();

        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<NhanVien>(ma);
            NhanSuGiaoDien.DanhMuc(cboGioiTinh, new KeyValuePair<string, string>[] { new("M", "Nam"), new("F", "Nữ"), new("O", "Khác") }, duLieu.GioiTinh ?? "");
            NhanSuGiaoDien.LuaChon(cboPhongBan, bus.PhongBan().Where(x => x.TrangThai || x.MaPhongBan == duLieu.MaPhongBan), "TenPhongBan", "MaPhongBan", duLieu.MaPhongBan);
            NhanSuGiaoDien.LuaChon(cboChucVu, bus.ChucVu().Where(x => x.TrangThai || x.MaChucVu == duLieu.MaChucVu), "TenChucVu", "MaChucVu", duLieu.MaChucVu);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("NhanVien"), duLieu.TrangThai);
        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        dtpNgaySinh.SelectedDate = duLieu.NgaySinh;
        dtpNgayVaoLam.SelectedDate = duLieu.NgayVaoLam;
        cboGioiTinh.SelectedValue = duLieu.GioiTinh ?? "";
        cboPhongBan.SelectedValue = duLieu.MaPhongBan;
        cboChucVu.SelectedValue = duLieu.MaChucVu;
        cboTrangThai.SelectedValue = duLieu.TrangThai;
        btnNhanVienSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnNhanVienXoa.IsEnabled = DemoSession.CanUseButton(View, "btnNhanVienXoa");
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        NhanSuGiaoDien.MoKhoa(pnlThongTin, sua);
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
            TaiDuLieu();
            // Bớt: không mở Form sửa riêng; mở khóa các ô trong chính cửa sổ chi tiết.
            SetEditMode(true);
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua || !DemoSession.CanUseButton(View, QuyenSua)) return;
        try
        {
            var banNhap = (NhanVien)DataContext;
            banNhap.GioiTinh = NhanSuGiaoDien.TrangThai(cboGioiTinh);
            banNhap.NgaySinh = dtpNgaySinh.SelectedDate?.Date;
            banNhap.MaPhongBan = Convert.ToInt32(cboPhongBan.SelectedValue);
            banNhap.MaChucVu = Convert.ToInt32(cboChucVu.SelectedValue);
            banNhap.NgayVaoLam = dtpNgayVaoLam.SelectedDate?.Date;
            banNhap.LuongCoBan = BUS_KiemTra.SoTien(txtLuongCoBanD.Text, "Lương cơ bản");
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu nhân viên.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnNhanVienXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa nhân viên {ma}")) return;
        try { bus.Xoa<NhanVien>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }

}
