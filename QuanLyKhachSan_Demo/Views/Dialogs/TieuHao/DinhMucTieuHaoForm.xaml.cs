using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class DinhMucTieuHaoForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly TieuHaoVatTu duLieu;
    private readonly bool them;
    private readonly string quyen;
    public DinhMucTieuHaoForm() : this(null) { }
    public DinhMucTieuHaoForm(TieuHaoVatTu? banGoc)
    {
        InitializeComponent();
        them = banGoc == null;
        // Khác Phòng ban: khóa ghép không có IDENTITY; sửa giữ nguyên hai mã.
        duLieu = them ? new TieuHaoVatTu { SoLuongTieuHao = 1 } : BanNhap(banGoc!);
        quyen = them ? "btnDinhMucTieuHaoThemMoi" : "btnDinhMucTieuHaoSuaThongTin";
        DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("TieuHaoView", quyen);
        if (!DemoSession.CanOpen("TieuHaoView")) return;
        try
        {
            LuaChon(cboDichVu, bus.DanhMuc<DichVu>().Where(x => x.TrangThai || x.MaDichVu == duLieu.MaDichVu), "TenDichVu", "MaDichVu", duLieu.MaDichVu);
            LuaChon(cboVatTu, bus.DanhMuc<VatTu>().Where(x => x.TrangThai || x.MaLoaiVatTu == duLieu.MaVatTu), "TenVatTu", "MaLoaiVatTu", duLieu.MaVatTu);
            cboDichVu.IsEnabled = them;
            cboVatTu.IsEnabled = them;

        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("TieuHaoView", quyen)) return;
        try
        {
            duLieu.MaDichVu = cboDichVu.SelectedValue is int ma ? ma : 0;
            duLieu.MaVatTu = cboVatTu.SelectedValue is int vatTu ? vatTu : 0;
            duLieu.SoLuongTieuHao = BUS_KhoVatTu.SoLuong(txtSoLuongLan.Text, "Số lượng mỗi lần", false, 8);
            bus.Luu(duLieu, them);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
