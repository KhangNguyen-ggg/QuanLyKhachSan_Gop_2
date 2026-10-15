using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class LichLamViecChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private LichLamViec duLieu;
    private bool dangSua;
    private bool daChamCong;
    private const string View = "NhanSuView";
    private const string QuyenSua = "btnLichLamViecSuaThongTin";

    public LichLamViecChiTietWindow(LichLamViec x)
    {
        InitializeComponent();
        ma = x.MaLich;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();

        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<LichLamViec>(ma);
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien().Where(x => x.TrangThai == "DangLam" || x.MaNhanVien == duLieu.MaNhanVien), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.LuaChon(cboCa, bus.CaLamViec().Where(x => x.TrangThai || x.MaCa == duLieu.MaCa), "TenCa", "MaCa", duLieu.MaCa);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("LichLamViec"), duLieu.TrangThai);
            daChamCong = bus.ChamCong().Any(x => x.MaLich == ma);
        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        // Thêm: khôi phục cả ô ngày sau khi người dùng xóa ngày bắt buộc rồi Hủy sửa.
        dtpNgayLam.SelectedDate = duLieu.NgayLam;
        cboNhanVien.SelectedValue = duLieu.MaNhanVien;
        cboCa.SelectedValue = duLieu.MaCa;
        cboTrangThai.SelectedValue = duLieu.TrangThai;
        btnLichLamViecSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnLichLamViecXoa.IsEnabled = DemoSession.CanUseButton(View, "btnLichLamViecXoa");
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        NhanSuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
        // Khác Phòng ban: lịch đã chấm công chỉ sửa ghi chú; DAL vẫn kiểm tra lại khi lưu.
        if (daChamCong)
        {
            cboNhanVien.IsEnabled = cboCa.IsEnabled = dtpNgayLam.IsEnabled = cboTrangThai.IsEnabled = false;
            if (sua) txtThongBao.Text = "Lịch đã chấm công: chỉ sửa được ghi chú.";
        }
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
            var banNhap = (LichLamViec)DataContext;
            banNhap.MaNhanVien = Convert.ToInt32(cboNhanVien.SelectedValue);
            banNhap.NgayLam = NhanSuQuyUoc.Ngay(dtpNgayLam.SelectedDate, "Ngày làm");
            banNhap.MaCa = Convert.ToInt32(cboCa.SelectedValue);
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu lịch làm việc.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnLichLamViecXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa lịch làm việc {ma}")) return;
        try { bus.Xoa<LichLamViec>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }

}
