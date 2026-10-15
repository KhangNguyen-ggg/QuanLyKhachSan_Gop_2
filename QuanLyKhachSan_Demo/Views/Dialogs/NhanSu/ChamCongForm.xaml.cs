using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChamCongForm : Window
{
    private readonly BUS_NhanSu bus = new();
    private readonly ChamCong duLieu;
    private readonly string quyen;

    public ChamCongForm() : this(null) { }

    // Thêm: một form dùng lại cho Thêm và Sửa; bản nháp giúp đóng/hủy không đổi dữ liệu gốc.
    public ChamCongForm(ChamCong? banGoc)
    {
        InitializeComponent();
        duLieu = banGoc == null ? new ChamCong { TrangThai = "DuCong" } : NhanSuGiaoDien.BanNhap(banGoc);
        quyen = duLieu.MaChamCong == 0 ? "btnChamCongThemChamCong" : "btnChamCongSuaThongTin";
        DataContext = duLieu;
        txtTieuDe.Text = (duLieu.MaChamCong == 0 ? "Thêm — " : "Sửa — ") + "Chấm công";
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhanSuView", quyen);
        if (!DemoSession.CanOpen("NhanSuView")) return;
        try
        {
            var daCham = bus.ChamCong().Where(x => x.MaChamCong != duLieu.MaChamCong).Select(x => x.MaLich).ToHashSet();
            NhanSuGiaoDien.LuaChon(cboLich, bus.LichLamViec().Where(x => !daCham.Contains(x.MaLich) && (x.TrangThai == "DaXep" || x.MaLich == duLieu.MaLich)), "LuaChon", "MaLich", duLieu.MaLich);
            NhanSuGiaoDien.DanhMuc(cboTrangThai, NhanSuQuyUoc.TrangThai("ChamCong"), duLieu.TrangThai);
            // Thêm: đổi lịch tự điền ngày/giờ ca; ca đêm có ngày ra là hôm sau.
            cboLich.SelectionChanged += cboLich_SelectionChanged;
            dtpNgayVao.SelectedDate = duLieu.GioVao?.Date;
            dtpNgayRa.SelectedDate = duLieu.GioRa?.Date;
            txtGioVao.Text = duLieu.GioVao?.ToString("HH:mm") ?? "";
            txtGioRa.Text = duLieu.GioRa?.ToString("HH:mm") ?? "";
            cboTrangThai.SelectionChanged += (_, _) => ApDungTrangThai();
            ApDungTrangThai();
        }
        catch (Exception ex) { btnLuu.IsEnabled = false; txtThongBao.Text = "Không tải được danh mục. " + ex.Message; }
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanSuView", quyen)) return;
        try
        {
            duLieu.MaLich = Convert.ToInt64(cboLich.SelectedValue);
            duLieu.SoGioTangCa = BUS_KiemTra.SoTien(txtTangCa.Text, "Số giờ tăng ca");
            duLieu.TrangThai = NhanSuGiaoDien.TrangThai(cboTrangThai);
            // Bớt: không nhập nhân viên/ngày chấm riêng; BUS lấy đúng các giá trị này từ lịch.
            bool coGio = duLieu.TrangThai is "DuCong" or "ThieuCong";
            duLieu.GioVao = coGio ? BUS_KiemTra.Ngay(dtpNgayVao.SelectedDate, "ngày vào").Date + NhanSuQuyUoc.Gio(txtGioVao.Text, "Giờ vào") : null;
            duLieu.GioRa = coGio ? BUS_KiemTra.Ngay(dtpNgayRa.SelectedDate, "ngày ra").Date + NhanSuQuyUoc.Gio(txtGioRa.Text, "Giờ ra") : null;
            bus.Luu(duLieu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được chấm công. " + ex.Message; }
    }

    private void cboLich_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cboLich.SelectedItem is not LichLamViecHienThi lich) return;
        dtpNgayVao.SelectedDate = lich.NgayLam;
        dtpNgayRa.SelectedDate = lich.NgayLam.AddDays(lich.GioKetThuc <= lich.GioBatDau ? 1 : 0);
        txtGioVao.Text = lich.GioBatDau.ToString(@"hh\:mm");
        txtGioRa.Text = lich.GioKetThuc.ToString(@"hh\:mm");
        ApDungTrangThai();
    }

    private void ApDungTrangThai()
    {
        bool coGio = NhanSuGiaoDien.TrangThai(cboTrangThai) is "DuCong" or "ThieuCong";
        txtGioVao.IsEnabled = txtGioRa.IsEnabled = dtpNgayVao.IsEnabled = dtpNgayRa.IsEnabled = txtTangCa.IsEnabled = coGio;
        if (!coGio) { txtGioVao.Clear(); txtGioRa.Clear(); txtTangCa.Text = "0"; }
    }

}
