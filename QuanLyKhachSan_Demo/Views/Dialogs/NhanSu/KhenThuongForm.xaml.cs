using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class KhenThuongForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly KhenThuong duLieu;
    private readonly string quyen;

    public KhenThuongForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public KhenThuongForm(KhenThuong? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new KhenThuong { NgayKhenThuong = DateTime.Today } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaKhenThuong == 0 ? "btnKhenThuongThemKhenThuong" : "btnKhenThuongSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaKhenThuong == 0 ? "Thêm — " : "Sửa — ") + "Khen thưởng";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanSuView", quyen);
        if (!DemoSession.CanOpen("NhanSuView")) return;
        try
        {
            NhanSuGiaoDien.LuaChon(cboNhanVien, bus.NhanVien(), "LuaChon", "MaNhanVien", duLieu.MaNhanVien);
            NhanSuGiaoDien.DanhMuc(cboHinhThuc, NhanSuQuyUoc.HinhThuc, duLieu.HinhThuc);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("KhenThuong"), duLieu.TrangThai);
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
            duLieu.NgayKhenThuong = NhanSuQuyUoc.Ngay(dtpNgay.SelectedDate, "Ngày khen thưởng");
            duLieu.HinhThuc = NhanSuGiaoDien.TrangThai(cboHinhThuc);
            duLieu.SoTien = BUS_KiemTra.SoTien(txtSoTienD.Text, "Số tiền (đ; biểu dương/giấy khen nhập 0)");
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được khen thưởng. " + ex.Message; }
    }

}
