using System.Windows;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhaCungCapForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly NhaCungCap duLieu;
    private readonly string quyen;
    public NhaCungCapForm() : this(null) { }

    // Thêm: dùng lại form cho Thêm/Sửa và bản nháp như Phòng ban; hủy không đổi dòng gốc.
    public NhaCungCapForm(NhaCungCap? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new NhaCungCap { TrangThai = true } : BanNhap(banGoc);
        quyen = duLieu.MaNhaCungCap == 0 ? "btnNhaCungCapThemMoi" : "btnNhaCungCapSuaThongTin";
        DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("VatTuNhaCungCapView", quyen);
        if (!DemoSession.CanOpen("VatTuNhaCungCapView")) return;
        try
        {
            DanhMuc(cboTrangThai, BUS_KhoVatTu.TrangThai, duLieu.TrangThai.ToString());

        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("VatTuNhaCungCapView", quyen)) return;
        try
        {

            duLieu.TrangThai = TrangThai(cboTrangThai) == "True";
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được. " + ex.Message; }
    }
}
