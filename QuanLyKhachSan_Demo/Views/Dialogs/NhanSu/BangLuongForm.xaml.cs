using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class BangLuongForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly BangLuong duLieu;
    private readonly string quyen;

    public BangLuongForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public BangLuongForm(BangLuong? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new BangLuong { Thang = DateTime.Today.Month, Nam = DateTime.Today.Year, NgayLap = DateTime.Now, TrangThai = "TamTinh" } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaBangLuong == 0 ? "btnBangLuongLapBangLuong" : "btnBangLuongSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaBangLuong == 0 ? "Thêm — " : "Sửa — ") + "Bảng lương";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanSuView", quyen);
        if (!DemoSession.CanOpen("NhanSuView")) return;
        try
        {
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien(), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("BangLuong"), duLieu.TrangThai);
            CapNhatThucLinh();
            if (duLieu.TrangThai == "DaThanhToan") { btnLuu.IsEnabled = false; btnNapLuongThuong.IsEnabled = false; txtThongBao.Text = "Bảng lương đã thanh toán, chỉ xem."; }
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", quyen)) return;
        try
        {
            duLieu.MaNhanVien = Convert.ToInt32(cboNhanVien.SelectedValue);
            duLieu.Thang = BUS_KiemTra.SoNguyen(txtThang.Text, "Tháng");
            duLieu.Nam = BUS_KiemTra.SoNguyen(txtNam.Text, "Năm");
            duLieu.LuongCoBan = BUS_KiemTra.SoTien(txtLuongCoBanD.Text, "Lương cơ bản tham chiếu");
            duLieu.TienCong = BUS_KiemTra.SoTien(txtTienCongD.Text, "Tiền công");
            duLieu.TienTangCa = BUS_KiemTra.SoTien(txtTienTangCaD.Text, "Tiền tăng ca");
            duLieu.TienThuong = BUS_KiemTra.SoTien(txtThuongD.Text, "Tiền thưởng kỳ này");
            duLieu.TienPhuCap = BUS_KiemTra.SoTien(txtPhuCapD.Text, "Phụ cấp");
            duLieu.TienKhauTru = BUS_KiemTra.SoTien(txtKhauTruD.Text, "Khấu trừ");
            duLieu.Thue = BUS_KiemTra.SoTien(txtThueD.Text, "Thuế");
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được bảng lương. " + ex.Message; }
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
        if (!DemoSession.CanUseButton("NhanSuView", quyen) || duLieu.TrangThai == "DaThanhToan") return;
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
