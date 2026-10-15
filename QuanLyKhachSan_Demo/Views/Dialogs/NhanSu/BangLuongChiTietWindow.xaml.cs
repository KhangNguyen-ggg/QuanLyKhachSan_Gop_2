using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class BangLuongChiTietWindow : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly long ma;
    private BangLuong duLieu;
    private bool dangSua;
    private const string View = "NhanSuView";
    private const string QuyenSua = "btnBangLuongSuaThongTin";

    public BangLuongChiTietWindow(BangLuong x)
    {
        InitializeComponent();
        ma = x.MaBangLuong;
        duLieu = NhanSuGiaoDien.BanNhap(x);
        HienThi();

        if (!DemoSession.CanOpen(View)) return;
        try { TaiDuLieu(); }
        catch (Exception ex) { txtThongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }
    private void TaiDuLieu()
    {
        duLieu = bus.TheoId<BangLuong>(ma);
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien(), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("BangLuong"), duLieu.TrangThai);

        HienThi();
    }
    private void HienThi()
    {
        dangSua = false;
        // Thêm: dùng bản nháp độc lập như Phòng ban; Hủy sửa/Đóng không ghi dữ liệu.
        DataContext = NhanSuGiaoDien.BanNhap(duLieu);
        cboNhanVien.SelectedValue = duLieu.MaNhanVien;
        cboTrangThai.SelectedValue = duLieu.TrangThai;
        CapNhatThucLinh();
        btnBangLuongSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua) && duLieu.TrangThai != "DaThanhToan";
        btnBangLuongXoa.IsEnabled = DemoSession.CanUseButton(View, "btnBangLuongXoa") && duLieu.TrangThai != "DaThanhToan";
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        NhanSuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để mở khóa thông tin tại đây.";
        btnNapLuongThuong.IsEnabled = sua && duLieu.TrangThai != "DaThanhToan" && DemoSession.CanUseButton(View, QuyenSua);
        if (duLieu.TrangThai == "DaThanhToan") txtThongBao.Text = "Bảng lương đã thanh toán: chỉ xem.";
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnHuySua_Click(object sender, RoutedEventArgs e) => HienThi();
    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, QuyenSua)) return;
        try
        {
            TaiDuLieu();
            if (duLieu.TrangThai == "DaThanhToan") return;
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
            var banNhap = (BangLuong)DataContext;
            banNhap.MaNhanVien = Convert.ToInt32(cboNhanVien.SelectedValue);
            banNhap.Thang = BUS_KiemTra.SoNguyen(txtThang.Text, "Tháng");
            banNhap.Nam = BUS_KiemTra.SoNguyen(txtNam.Text, "Năm");
            banNhap.LuongCoBan = BUS_KiemTra.SoTien(txtLuongCoBanD.Text, "Lương cơ bản tham chiếu");
            banNhap.TienCong = BUS_KiemTra.SoTien(txtTienCongD.Text, "Tiền công");
            banNhap.TienTangCa = BUS_KiemTra.SoTien(txtTienTangCaD.Text, "Tiền tăng ca");
            banNhap.TienThuong = BUS_KiemTra.SoTien(txtThuongD.Text, "Tiền thưởng kỳ này");
            banNhap.TienPhuCap = BUS_KiemTra.SoTien(txtPhuCapD.Text, "Phụ cấp");
            banNhap.TienKhauTru = BUS_KiemTra.SoTien(txtKhauTruD.Text, "Khấu trừ");
            banNhap.Thue = BUS_KiemTra.SoTien(txtThueD.Text, "Thuế");
            banNhap.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(banNhap);
            duLieu = banNhap;
            HienThi();
            txtThongBao.Text = "Đã lưu bảng lương.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnBangLuongXoa")) return;
        if (!NhanSuGiaoDien.XacNhanXoa(this, $"Xóa bảng lương {ma}")) return;
        try { bus.Xoa<BangLuong>(ma); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được. " + ex.Message; }
    }
    // Thêm: dùng lại phép tính của BUS cho cả xem trước và lưu, tránh hai công thức khác nhau.
    private BangLuong DocCacKhoan() => new()
    {
        LuongCoBan = BUS_KiemTra.SoTien(txtLuongCoBanD.Text, "Lương cơ bản tham chiếu"),
        TienCong = BUS_KiemTra.SoTien(txtTienCongD.Text, "Tiền công"),
        TienTangCa = BUS_KiemTra.SoTien(txtTienTangCaD.Text, "Tiền tăng ca"),
        TienThuong = BUS_KiemTra.SoTien(txtThuongD.Text, "Tiền thưởng kỳ này"),
        TienPhuCap = BUS_KiemTra.SoTien(txtPhuCapD.Text, "Phụ cấp"),
        TienKhauTru = BUS_KiemTra.SoTien(txtKhauTruD.Text, "Khấu trừ"),
        Thue = BUS_KiemTra.SoTien(txtThueD.Text, "Thuế"),
    };

    private void Tien_TextChanged(object sender, TextChangedEventArgs e) => CapNhatThucLinh();

    private void CapNhatThucLinh()
    {
        if (txtTongLuong == null || txtThucLinhMinhHoaD == null) return;
        try
        {
            var cacKhoan = DocCacKhoan();
            BUS_NhanSu.TinhLuong(cacKhoan);
            txtTongLuong.Text = cacKhoan.TongLuong.ToString("0.00", CultureInfo.InvariantCulture);
            txtThucLinhMinhHoaD.Text = cacKhoan.ThucLinh.ToString("0.00", CultureInfo.InvariantCulture);
        }
        catch (ArgumentException) { txtTongLuong.Text = txtThucLinhMinhHoaD.Text = ""; }
    }

    private void btnNapLuongThuong_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua || !DemoSession.CanUseButton(View, QuyenSua) || duLieu.TrangThai == "DaThanhToan") return;
        try
        {
            int ma = Convert.ToInt32(cboNhanVien.SelectedValue);
            var nhanVien = bus.TheoId<NhanVien>(ma);
            int thang = BUS_KiemTra.SoNguyen(txtThang.Text, "Tháng");
            int nam = BUS_KiemTra.SoNguyen(txtNam.Text, "Năm");
            decimal thuong = bus.TongThuong(ma, thang, nam);
            txtLuongCoBanD.Text = nhanVien.LuongCoBan.ToString("0.00", CultureInfo.InvariantCulture);
            // Thêm: gán tổng thưởng đã duyệt của kỳ; không cộng dồn khi bấm lại.
            txtThuongD.Text = thuong.ToString("0.00", CultureInfo.InvariantCulture);
            txtThongBao.Text = "Đã nạp lương tham chiếu và thưởng đã duyệt. Tiền công, tăng ca, phụ cấp, thuế nhập theo kỳ.";
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }


}
