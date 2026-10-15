using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhieuNhapXuatChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly long ma;
    private PhieuKhoHienThi duLieu;
    private bool dangSua;
    private const string View = "KhoView";
    public PhieuNhapXuatChiTietWindow(PhieuKhoHienThi x)
    {
        InitializeComponent(); duLieu = x; ma = x.MaPhieu;
        HienThi(); Tai();
    }
    private bool Tai()
    {
        btnPhieuNhapXuatSuaPhieu.IsEnabled = btnPhieuNhapXuatXacNhanPhieu.IsEnabled =
            btnPhieuNhapXuatHuyPhieu.IsEnabled = btnChiTietPhieuKhoThemMoi.IsEnabled = btnLuuSua.IsEnabled = false;
        if (!DemoSession.CanOpen(View)) return false;
        try
        {
            duLieu = bus.Phieu().Single(x => x.MaPhieu == ma);
            DanhMuc(cboLoaiPhieu, BUS_KhoVatTu.LoaiPhieu, duLieu.LoaiPhieu);
            // Thêm: giữ tên danh mục cũ để đọc lịch sử, kể cả khi đã ngừng hoạt động.
            LuaChon(cboKho, bus.DanhMuc<Kho>().Where(x => x.TrangThai || x.MaKho == duLieu.MaKho), "TenKho", "MaKho", duLieu.MaKho);
            LuaChon(cboNhaCungCap, new[] { new NhaCungCap { TenNhaCungCap = "— Không chọn —" } }.Concat(bus.DanhMuc<NhaCungCap>().Where(x => x.TrangThai || x.MaNhaCungCap == duLieu.MaNhaCungCap)), "TenNhaCungCap", "MaNhaCungCap", duLieu.MaNhaCungCap ?? 0);
            LuaChon(cboNhanVien, bus.DanhMuc<NhanVien>().Where(x => x.TrangThai == "DangLam" || x.MaNhanVien == duLieu.MaNhanVien), "HoTen", "MaNhanVien", duLieu.MaNhanVien);
            dgvChiTiet.ItemsSource = bus.ChiTiet(ma);
            bool moi = duLieu.TrangThai == "MOI";
            int maNguoi = duLieu.MaNguoiXacNhan ?? duLieu.MaNhanVien;
            LuaChon(cboNguoiXacNhan, bus.DanhMuc<NhanVien>().Where(x => x.TrangThai == "DangLam" || !moi && x.MaNhanVien == maNguoi), "HoTen", "MaNhanVien", maNguoi);
            cboNguoiXacNhan.IsEnabled = moi && DemoSession.CanUseButton(View, "btnPhieuNhapXuatXacNhanPhieu");
            btnChiTietPhieuKhoThemMoi.IsEnabled = moi && DemoSession.CanUseButton(View, "btnChiTietPhieuKhoThemMoi");
            btnPhieuNhapXuatXacNhanPhieu.IsEnabled = moi && DemoSession.CanUseButton(View, "btnPhieuNhapXuatXacNhanPhieu");
            btnPhieuNhapXuatHuyPhieu.IsEnabled = moi && DemoSession.CanUseButton(View, "btnPhieuNhapXuatHuyPhieu");
            btnPhieuNhapXuatSuaPhieu.IsEnabled = btnLuuSua.IsEnabled = moi && DemoSession.CanUseButton(View, "btnPhieuNhapXuatSuaPhieu");
            HienThi();
            return true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; return false; }
    }
    private void HienThi()
    {
        DataContext = BanNhap(duLieu);
        cboLoaiPhieu.SelectedValue = duLieu.LoaiPhieu;
        cboKho.SelectedValue = duLieu.MaKho;
        cboNhanVien.SelectedValue = duLieu.MaNhanVien;
        cboNhaCungCap.SelectedValue = duLieu.MaNhaCungCap ?? 0;
        SetEditMode(false);
    }
    private void SetEditMode(bool sua)
    {
        dangSua = sua;
        KhoVatTuGiaoDien.MoKhoa(pnlThongTin, sua);
        pnlLienKet.IsEnabled = !sua;
        pnlThaoTac.Visibility = sua ? Visibility.Collapsed : Visibility.Visible;
        pnlLuu.Visibility = sua ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = sua ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." :
            duLieu.TrangThai == "MOI" ? "Nhấn Sửa để mở khóa. Phiếu Mới chưa thay đổi tồn." : "Phiếu đã xử lý: chỉ xem.";
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnHuySua_Click(object sender, RoutedEventArgs e) => HienThi();
    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnPhieuNhapXuatSuaPhieu")) return;
        // Bớt form sửa riêng; đọc lại trạng thái SQL trước khi mở khóa phiếu Mới.
        if (Tai() && duLieu.TrangThai == "MOI") SetEditMode(true);
    }
    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!dangSua || !DemoSession.CanUseButton(View, "btnPhieuNhapXuatSuaPhieu")) return;
        try
        {
            var x = (PhieuNhapXuat)DataContext;
            x.LoaiPhieu = TrangThai(cboLoaiPhieu);
            x.MaKho = KhoVatTuGiaoDien.Ma(cboKho);
            x.MaNhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            int ncc = KhoVatTuGiaoDien.Ma(cboNhaCungCap);
            x.MaNhaCungCap = ncc == 0 ? null : ncc;
            x.NgayPhieu = KhoVatTuGiaoDien.Ngay(dtpNgay, "ngày phiếu", true)!.Value;
            bus.Luu(x);
            duLieu = (PhieuKhoHienThi)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu phiếu. Tồn kho chưa thay đổi.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnThemDong_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnChiTietPhieuKhoThemMoi") || duLieu.TrangThai != "MOI") return;
        if (new ChiTietPhieuKhoForm(ma, null, false) { Owner = this }.ShowDialog() == true) Tai();
    }
    private void ChiTiet_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dangSua || !DemoSession.CanOpen(View) || e.OriginalSource is not DependencyObject source || ItemsControl.ContainerFromElement(dgvChiTiet, source) is not DataGridRow row || row.Item is not ChiTietPhieuKhoHienThi x) return;
        new ChiTietPhieuKhoChiTietWindow(x) { Owner = this }.ShowDialog(); Tai();
    }
    private void XuLy(bool huy)
    {
        string quyen = huy ? "btnPhieuNhapXuatHuyPhieu" : "btnPhieuNhapXuatXacNhanPhieu";
        if (dangSua || !DemoSession.CanUseButton(View, quyen) || duLieu.TrangThai != "MOI") return;
        var hoi = new XacNhanDemoWindow { Owner = this };
        hoi.txtNoiDung.Text = huy ? "Hủy phiếu Mới này?" : "Xác nhận phiếu và cập nhật tồn? Phiếu đã xác nhận sẽ không sửa hoặc hủy được.";
        if (hoi.ShowDialog() != true) return;
        try
        {
            int nguoi = KhoVatTuGiaoDien.Ma(cboNguoiXacNhan);
            if (!huy) BUS_QuanlyKhachSan.BUS_KiemTra.Ma(nguoi, "nhân viên xác nhận");
            bus.XuLyPhieu(ma, false, huy, huy ? null : nguoi); Tai();
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
    private void btnXacNhan_Click(object sender, RoutedEventArgs e) => XuLy(false);
    private void btnHuy_Click(object sender, RoutedEventArgs e) => XuLy(true);

}
