using System.Globalization;
using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuChiTietWindow : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private VatTuHienThi duLieu;
    private bool dangSua;
    private const string View = "VatTuNhaCungCapView";
    private const string QuyenSua = "btnVatTuSuaThongTin";

    public VatTuChiTietWindow(VatTuHienThi x)
    {
        InitializeComponent();
        duLieu = x;
        btnVatTuSuaThongTin.IsEnabled = btnLuuSua.IsEnabled = DemoSession.CanUseButton(View, QuyenSua);
        btnVatTuXoa.IsEnabled = DemoSession.CanUseButton(View, "btnVatTuXoa");
        HienThi();
        if (!DemoSession.CanOpen(View)) return;
        try
        {
            TaiDanhMuc(); HienThi();

        }
        catch (Exception ex) { txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }
    private void TaiDanhMuc()
    {
            DanhMuc(cboTrangThai, BUS_KhoVatTu.TrangThai, duLieu.TrangThai.ToString());
            LuaChon(cboNhom, bus.DanhMuc<NhomVatTu>().Where(x => x.TrangThai || x.MaNhomVatTu == duLieu.MaNhomVatTu), "TenNhomVatTu", "MaNhomVatTu", duLieu.MaNhomVatTu);
            LuaChon(cboKho, new[] { new Kho { TenKho = "— Không chọn —" } }.Concat(bus.DanhMuc<Kho>().Where(x => x.TrangThai || x.MaKho == duLieu.MaKhoMacDinh)), "TenKho", "MaKho", duLieu.MaKhoMacDinh ?? 0);
    }
    private void HienThi()
    {
        // Thêm: bản nháp riêng như Phòng ban; Hủy sửa/Đóng không đổi dòng gốc hoặc SQL.
        DataContext = BanNhap(duLieu);
        cboTrangThai.SelectedValue = duLieu.TrangThai.ToString();
        cboNhom.SelectedValue = duLieu.MaNhomVatTu;
        cboKho.SelectedValue = duLieu.MaKhoMacDinh ?? 0;
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
            duLieu = bus.VatTu().Single(x => x.MaLoaiVatTu == duLieu.MaLoaiVatTu);
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
            var duLieuMoi = (VatTu)DataContext;
            duLieuMoi.TrangThai = TrangThai(cboTrangThai) == "True";
            duLieuMoi.MaNhomVatTu = KhoVatTuGiaoDien.Ma(cboNhom);
            int kho = KhoVatTuGiaoDien.Ma(cboKho);
            duLieuMoi.MaKhoMacDinh = kho == 0 ? null : kho;
            // Thêm: số lượng có 4 số lẻ, đọc trực tiếp để không lưu lại số cũ khi gõ sai.
            duLieuMoi.SoLuongToiThieu = BUS_KhoVatTu.SoLuong(txtTonToiThieu.Text, "Tồn tối thiểu", true);
            bus.Luu(duLieuMoi);
            duLieu = (VatTuHienThi)DataContext;
            HienThi();
            txtThongBao.Text = "Đã lưu vật tư.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (dangSua || !DemoSession.CanUseButton(View, "btnVatTuXoa") || !XacNhanXoa(this, "Xóa vật tư này")) return;
        try { bus.Xoa<VatTu>(duLieu.MaLoaiVatTu); DialogResult = true; }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }

}
