using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhenThuongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private KhenThuong duLieu;
    private bool dangSua;
    private const string View = "NhanSuView";
    private const string QuyenSua = "btnKhenThuongSuaThongTin";

    public KhenThuongChiTietWindow(KhenThuong x)
    {
        InitializeComponent();
        ma = x.MaKhenThuong;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();

        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<KhenThuong>(ma);
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien(), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.DanhMuc(cboHinhThuc, NhanSuQuyUoc.HinhThuc, duLieu.HinhThuc);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("KhenThuong"), duLieu.TrangThai);
        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        dtpNgay.SelectedDate = duLieu.NgayKhenThuong;
        cboNhanVien.SelectedValue = duLieu.MaNhanVien;
        cboHinhThuc.SelectedValue = duLieu.HinhThuc;
        cboTrangThai.SelectedValue = duLieu.TrangThai;
        btnKhenThuongSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnKhenThuongXoa.IsEnabled = DemoSession.CanUseButton(View, "btnKhenThuongXoa");
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
            var banNhap = (KhenThuong)DataContext;
            banNhap.MaNhanVien = Convert.ToInt32(cboNhanVien.SelectedValue);
            banNhap.NgayKhenThuong = NhanSuQuyUoc.Ngay(dtpNgay.SelectedDate, "Ngày khen thưởng");
            banNhap.HinhThuc = NhanSuGiaoDien.TrangThai(cboHinhThuc);
            banNhap.SoTien = BUS_KiemTra.SoTien(txtSoTienD.Text, "Số tiền (đ; biểu dương/giấy khen nhập 0)");
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu khen thưởng.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnKhenThuongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa khen thưởng {ma}")) return;
        try { bus.Xoa<KhenThuong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }

}
