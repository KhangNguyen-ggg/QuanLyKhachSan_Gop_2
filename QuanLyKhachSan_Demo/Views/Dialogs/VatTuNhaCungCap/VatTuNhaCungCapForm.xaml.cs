using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuNhaCungCapForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly VatTu_NhaCungCap duLieu;
    private readonly bool them;
    private readonly string quyen;
    public VatTuNhaCungCapForm() : this(null, 0) { }
    public VatTuNhaCungCapForm(VatTu_NhaCungCap? banGoc, int nhaCungCap = 0)
    {
        InitializeComponent();
        them = banGoc == null;
        // Khác Phòng ban: khóa ghép không có IDENTITY; sửa giữ nguyên hai mã.
        duLieu = them ? new VatTu_NhaCungCap { TrangThai = true, MaNhaCungCap = nhaCungCap } : BanNhap(banGoc!);
        quyen = them ? "btnVatTuNhaCungCapThemMoi" : "btnVatTuNhaCungCapSuaThongTin";
        DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", quyen);
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            LuaChon(cboNhaCungCap, bus.DanhMuc<NhaCungCap>().Where(x => x.TrangThai || x.MaNhaCungCap == duLieu.MaNhaCungCap), "TenNhaCungCap", "MaNhaCungCap", duLieu.MaNhaCungCap);
            LuaChon(cboVatTu, bus.DanhMuc<VatTu>().Where(x => x.TrangThai || x.MaLoaiVatTu == duLieu.MaVatTu), "TenVatTu", "MaLoaiVatTu", duLieu.MaVatTu);
            cboNhaCungCap.IsEnabled = them && nhaCungCap == 0;
            cboVatTu.IsEnabled = them;
            DanhMuc(cboTrangThai, BUS_KhoVatTu.TrangThai, duLieu.TrangThai.ToString());
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", quyen)) return;
        try
        {
            duLieu.MaNhaCungCap = cboNhaCungCap.SelectedValue is int ma ? ma : 0;
            duLieu.MaVatTu = cboVatTu.SelectedValue is int vatTu ? vatTu : 0;
            duLieu.DonGiaNhap = string.IsNullOrWhiteSpace(txtGiaNhapD.Text) ? null : BUS_KiemTra.SoTien(txtGiaNhapD.Text, "Giá nhập");
            duLieu.TrangThai = TrangThai(cboTrangThai) == "True";
            duLieu.ThoiGianCungCap = KhoVatTuGiaoDien.Ngay(dtpNgay, "ngày cung cấp");
            bus.Luu(duLieu, them);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
