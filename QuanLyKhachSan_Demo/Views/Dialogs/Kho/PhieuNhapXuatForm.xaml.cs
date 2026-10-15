using System.Windows;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.KhoVatTu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using static QuanLyKhachSan.Helpers.NhanSuGiaoDien;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhieuNhapXuatForm : Window
{
    private readonly BUS_KhoVatTu bus = new();
    private readonly PhieuNhapXuat duLieu;
    private readonly string quyen;
    public long MaPhieu => duLieu.MaPhieu;
    public PhieuNhapXuatForm() : this(null) { }
    public PhieuNhapXuatForm(PhieuNhapXuat? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new PhieuNhapXuat() : BanNhap(banGoc);
        quyen = duLieu.MaPhieu == 0 ? "btnPhieuNhapXuatTaoPhieu" : "btnPhieuNhapXuatSuaPhieu";
        DataContext = duLieu;
        btnLuu.IsEnabled = DemoSession.CanUseButton("KhoView", quyen) && duLieu.TrangThai == "MOI";
        txtThongBao.Text = "Lưu phiếu Mới, sau đó thêm vật tư và xác nhận để cập nhật tồn.";
        if (!DemoSession.CanOpen("KhoView")) return;
        try
        {
            DanhMuc(cboLoaiPhieu, BUS_KhoVatTu.LoaiPhieu, duLieu.LoaiPhieu);
            KhoVatTuGiaoDien.Kho(cboKho, bus, duLieu.MaKho);
            KhoVatTuGiaoDien.NhaCungCap(cboNhaCungCap, bus, duLieu.MaNhaCungCap);
            KhoVatTuGiaoDien.NhanVien(cboNhanVien, bus, duLieu.MaNhanVien);
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = ex.Message; }
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("KhoView", quyen)) return;
        try
        {
            duLieu.LoaiPhieu = TrangThai(cboLoaiPhieu);
            duLieu.MaKho = KhoVatTuGiaoDien.Ma(cboKho);
            duLieu.MaNhanVien = KhoVatTuGiaoDien.Ma(cboNhanVien);
            int ncc = KhoVatTuGiaoDien.Ma(cboNhaCungCap);
            duLieu.MaNhaCungCap = ncc == 0 ? null : ncc;
            duLieu.NgayPhieu = KhoVatTuGiaoDien.Ngay(dtpNgay, "ngày phiếu", true)!.Value;
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = ex.Message; }
    }
}
