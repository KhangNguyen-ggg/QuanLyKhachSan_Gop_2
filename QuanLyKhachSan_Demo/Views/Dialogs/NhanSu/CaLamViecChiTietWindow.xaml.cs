using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class CaLamViecChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private CaLamViec duLieu;
    private bool dangSua;
    private bool daXepLich;
    private const string View = "NhanSuView";
    private const string QuyenSua = "btnCaLamViecSuaThongTin";

    public CaLamViecChiTietWindow(CaLamViec x)
    {
        InitializeComponent();
        ma = x.MaCa;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();

        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<CaLamViec>(ma);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("CaLamViec"), duLieu.TrangThai.ToString());
            daXepLich = bus.LichLamViec().Any(x => x.MaCa == ma);
        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        cboTrangThai.SelectedValue = duLieu.TrangThai.ToString();
        btnCaLamViecSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnCaLamViecXoa.IsEnabled = DemoSession.CanUseButton(View, "btnCaLamViecXoa");
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        NhanSuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
        // Khác Phòng ban: ca đã có lịch chỉ giữ nguyên giờ để không làm lệch lịch/chấm công.
        if (daXepLich)
        {
            txtGioBatDau.IsReadOnly = txtGioKetThuc.IsReadOnly = true;
            if (sua) txtThongBao.Text = "Ca đã có lịch: giữ nguyên giờ, có thể sửa tên, ghi chú và trạng thái.";
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
            var banNhap = (CaLamViec)DataContext;
            banNhap.GioBatDau = NhanSuQuyUoc.Gio(txtGioBatDau.Text, "Giờ bắt đầu");
            banNhap.GioKetThuc = NhanSuQuyUoc.Gio(txtGioKetThuc.Text, "Giờ kết thúc");
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai) == "True";
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu ca làm việc.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnCaLamViecXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa ca làm việc {ma}")) return;
        try { bus.Xoa<CaLamViec>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }

}
