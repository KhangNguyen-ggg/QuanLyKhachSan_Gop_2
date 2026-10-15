using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChamCongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private ChamCong duLieu;
    private bool dangSua;
    private const string View = "NhanSuView";
    private const string QuyenSua = "btnChamCongSuaThongTin";

    public ChamCongChiTietWindow(ChamCong x)
    {
        InitializeComponent();
        ma = x.MaChamCong;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();
        cboLich.SelectionChanged += cboLich_SelectionChanged;
        cboTrangThai.SelectionChanged += (_, _) => ApDungTrangThai();
        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<ChamCong>(ma);
            var daCham = bus.ChamCong().Where(x => x.MaChamCong != duLieu.MaChamCong).Select(x => x.MaLich).ToHashSet();
            NhanSuGiaoDien.LuaChon(cboLich, bus.LichLamViec().Where(x => !daCham.Contains(x.MaLich) && (x.TrangThai == "DaXep" || x.MaLich == duLieu.MaLich)), "LuaChon", "MaLich", duLieu.MaLich);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("ChamCong"), duLieu.TrangThai);

        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        cboLich.SelectedValue = duLieu.MaLich;
        cboTrangThai.SelectedValue = duLieu.TrangThai;
        dtpNgayVao.SelectedDate = duLieu.GioVao?.Date;
        dtpNgayRa.SelectedDate = duLieu.GioRa?.Date;
        txtGioVao.Text = duLieu.GioVao?.ToString("HH:mm") ?? "";
        txtGioRa.Text = duLieu.GioRa?.ToString("HH:mm") ?? "";
        btnChamCongSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnChamCongXoa.IsEnabled = DemoSession.CanUseButton(View, "btnChamCongXoa");
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        NhanSuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
        ApDungTrangThai();
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
            var banNhap = (ChamCong)DataContext;
            banNhap.MaLich = Convert.ToInt64(cboLich.SelectedValue);
            banNhap.SoGioTangCa = BUS_KiemTra.SoTien(txtTangCa.Text, "Số giờ tăng ca");
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            // Bớt: không nhập nhân viên/ngày chấm riêng; BUS lấy đúng các giá trị này từ lịch.
            bool coGio = banNhap.TrangThai is "DuCong" or "ThieuCong";
            banNhap.GioVao = coGio ? BUS_KiemTra.Ngay(dtpNgayVao.SelectedDate, "ngày vào").Date + NhanSuQuyUoc.Gio(txtGioVao.Text, "Giờ vào") : null;
            banNhap.GioRa = coGio ? BUS_KiemTra.Ngay(dtpNgayRa.SelectedDate, "ngày ra").Date + NhanSuQuyUoc.Gio(txtGioRa.Text, "Giờ ra") : null;
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu chấm công.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnChamCongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa chấm công {ma}")) return;
        try { bus.Xoa<ChamCong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }
    private void cboLich_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!dangSua || cboLich.SelectedItem is not LichLamViecHienThi lich) return;
        dtpNgayVao.SelectedDate = lich.NgayLam;
        dtpNgayRa.SelectedDate = lich.NgayLam.AddDays(lich.GioKetThuc <= lich.GioBatDau ? 1 : 0);
        txtGioVao.Text = lich.GioBatDau.ToString(@"hh\:mm");
        txtGioRa.Text = lich.GioKetThuc.ToString(@"hh\:mm");
        ApDungTrangThai();
    }

    private void ApDungTrangThai()
    {
        bool coGio = NhanSuGiaoDien.TrangThai(cboTrangThai) is "DuCong" or "ThieuCong";
        txtGioVao.IsEnabled = txtGioRa.IsEnabled = dtpNgayVao.IsEnabled = dtpNgayRa.IsEnabled = txtTangCa.IsEnabled = dangSua && coGio;
        if (dangSua && !coGio) { txtGioVao.Clear(); txtGioRa.Clear(); txtTangCa.Text = "0"; }
    }


}
