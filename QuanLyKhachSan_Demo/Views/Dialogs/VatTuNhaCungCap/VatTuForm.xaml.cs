using System.Windows;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class VatTuForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly VatTu duLieu;
    private readonly string quyen;
    public VatTuForm() : this(null) { }

    // Thêm: dùng lại form cho Thêm/Sửa và bản nháp như Phòng ban; hủy không đổi dòng gốc.
    public VatTuForm(VatTu? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new VatTu { TrangThai = true } : BanNhap(banGoc);
        quyen = duLieu.MaLoaiVatTu == 0 ? "btnVatTuThemMoi" : "btnVatTuSuaThongTin";
        DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", quyen);
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            DanhMuc(cboTrangThai, BUS_KhoVatTu.TrangThai, duLieu.TrangThai.ToString());
            LuaChon(cboNhom, bus.DanhMuc<NhomVatTu>().Where(x => x.TrangThai || x.MaNhomVatTu == duLieu.MaNhomVatTu), "TenNhomVatTu", "MaNhomVatTu", duLieu.MaNhomVatTu);
            // Thêm: có lựa chọn bỏ trống vì MaKhoMacDinh trong SQL cho phép NULL.
            LuaChon(cboKho, new[] { new Kho { TenKho = "— Không chọn —" } }.Concat(bus.DanhMuc<Kho>().Where(x => x.TrangThai || x.MaKho == duLieu.MaKhoMacDinh)), "TenKho", "MaKho", duLieu.MaKhoMacDinh ?? 0);
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", quyen)) return;
        try
        {
            duLieu.MaNhomVatTu = cboNhom.SelectedValue is int nhom ? nhom : 0;
            duLieu.MaKhoMacDinh = cboKho.SelectedValue is int kho && kho > 0 ? kho : null;
            duLieu.SoLuongToiThieu = BUS_KhoVatTu.SoLuong(txtTonToiThieu.Text, "Tồn tối thiểu", true);
            duLieu.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
}
