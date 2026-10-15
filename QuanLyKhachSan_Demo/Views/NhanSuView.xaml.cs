using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using QuanLyKhachSan.Helpers;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;

public partial class NhanSuView : UserControl
{
    private readonly BUS_NhanSu? bus;

    public NhanSuView()
    {
        InitializeComponent();
        if (DAL.CauHinhKetNoi.DaCauHinh) bus = new();
        // Thêm: dùng chung tìm kiếm, lọc và mở form; chỉ chỉ định BUS/ET riêng cho mỗi tab.
        NhanSuGiaoDien.GanTimKiem(txtTimCaLamViec, txtLoiTimCaLamViec);
        NhanSuGiaoDien.DanhMuc(cboLocCaLamViec, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("CaLamViec")), "");
        cboLocCaLamViec.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiCaLamViec(); };
        btnCaLamViecThemMoi.IsEnabled = bus != null && DemoSession.CanUseButton(nameof(NhanSuView), "btnCaLamViecThemMoi");
        NhanSuGiaoDien.GanTimKiem(txtTimLichLamViec, txtLoiTimLichLamViec);
        NhanSuGiaoDien.DanhMuc(cboLocLichLamViec, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("LichLamViec")), "");
        cboLocLichLamViec.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiLichLamViec(); };
        btnLichLamViecThemLich.IsEnabled = bus != null && DemoSession.CanUseButton(nameof(NhanSuView), "btnLichLamViecThemLich");
        NhanSuGiaoDien.GanTimKiem(txtTimChamCong, txtLoiTimChamCong);
        NhanSuGiaoDien.DanhMuc(cboLocChamCong, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("ChamCong")), "");
        cboLocChamCong.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiChamCong(); };
        btnChamCongThemChamCong.IsEnabled = bus != null && DemoSession.CanUseButton(nameof(NhanSuView), "btnChamCongThemChamCong");
        NhanSuGiaoDien.GanTimKiem(txtTimBangLuong, txtLoiTimBangLuong);
        NhanSuGiaoDien.DanhMuc(cboLocBangLuong, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("BangLuong")), "");
        cboLocBangLuong.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiBangLuong(); };
        btnBangLuongLapBangLuong.IsEnabled = bus != null && DemoSession.CanUseButton(nameof(NhanSuView), "btnBangLuongLapBangLuong");
        NhanSuGiaoDien.GanTimKiem(txtTimKhenThuong, txtLoiTimKhenThuong);
        NhanSuGiaoDien.DanhMuc(cboLocKhenThuong, new[] { new KeyValuePair<string, string>("", "Tất cả trạng thái") }.Concat(NhanSuQuyUoc.TrangThai("KhenThuong")), "");
        cboLocKhenThuong.SelectionChanged += (_, _) => { if (IsLoaded) TaiLaiKhenThuong(); };
        btnKhenThuongThemKhenThuong.IsEnabled = bus != null && DemoSession.CanUseButton(nameof(NhanSuView), "btnKhenThuongThemKhenThuong");
        Loaded += (_, _) => TaiLaiTatCa();
        tabChinh.SelectionChanged += (_, e) => { if (e.Source == tabChinh && IsLoaded) TaiLaiTabDangChon(); };
    }

    private void TaiLaiTatCa()
    {
        TaiLaiCaLamViec();
        TaiLaiLichLamViec();
        TaiLaiChamCong();
        TaiLaiBangLuong();
        TaiLaiKhenThuong();
    }

    private void TaiLaiTabDangChon()
    {
        switch (tabChinh.SelectedIndex)
        {
            case 0: TaiLaiCaLamViec(); break;
            case 1: TaiLaiLichLamViec(); break;
            case 2: TaiLaiChamCong(); break;
            case 3: TaiLaiBangLuong(); break;
            case 4: TaiLaiKhenThuong(); break;
        }
    }

    private void TaiLai<T>(DataGrid bang, TextBox tim, ComboBox loc, TextBlock thongBao, Func<string, string, List<T>> layDuLieu)
    {
        if (!DemoSession.CanOpen(nameof(NhanSuView))) return;
        var maDaChon = bang.SelectedValue;
        bang.ItemsSource = null;
        if (bus == null) { thongBao.Text = "Cấu hình connection.txt hoặc QLKS_CONNECTION để dùng Nhân sự."; return; }
        try
        {
            bang.ItemsSource = layDuLieu(tim.Text, NhanSuGiaoDien.TrangThai(loc));
            bang.SelectedValue = maDaChon;
            if (bang.SelectedIndex < 0 && bang.Items.Count > 0) bang.SelectedIndex = 0;
            thongBao.Text = $"{bang.Items.Count} dòng · Nhấp đúp để xem chi tiết";
        }
        catch (Exception ex) { thongBao.Text = "Không tải được dữ liệu. " + ex.Message; }
    }

    private void MoForm(string quyen, Func<Window> taoForm, Action taiLai, TextBlock thongBao)
    {
        if (bus == null || !DemoSession.CanUseButton(nameof(NhanSuView), quyen)) return;
        try { var form = taoForm(); form.Owner = Window.GetWindow(this); form.ShowDialog(); taiLai(); }
        catch (Exception ex) { thongBao.Text = ex.Message; }
    }

    private void MoChiTiet<T>(MouseButtonEventArgs e, DataGrid bang, Func<T, Window> taoForm, Action taiLai, TextBlock thongBao)
    {
        if (bus == null || !DemoSession.CanOpen(nameof(NhanSuView)) || e.OriginalSource is not DependencyObject nguon ||
            ItemsControl.ContainerFromElement(bang, nguon) is not DataGridRow dong || dong.Item is not T duLieu) return;
        try { var form = taoForm(duLieu); form.Owner = Window.GetWindow(this); form.ShowDialog(); taiLai(); }
        catch (Exception ex) { thongBao.Text = ex.Message; }
    }

    private void TaiLaiCaLamViec() => TaiLai(dgvCaLamViec, txtTimCaLamViec, cboLocCaLamViec, txtThongBaoCaLamViec, (tuKhoa, trangThai) => bus!.CaLamViec(tuKhoa, trangThai));
    private void btnCaLamViecThemMoi_Click(object sender, RoutedEventArgs e) => MoForm("btnCaLamViecThemMoi", () => new CaLamViecForm(), TaiLaiCaLamViec, txtThongBaoCaLamViec);
    private void CaLamViec_DoubleClick(object sender, MouseButtonEventArgs e) => MoChiTiet<CaLamViec>(e, dgvCaLamViec, x => new CaLamViecChiTietWindow(x), TaiLaiCaLamViec, txtThongBaoCaLamViec);
    private void btnTimCaLamViec_Click(object sender, RoutedEventArgs e) => TaiLaiCaLamViec();
    private void btnMoiCaLamViec_Click(object sender, RoutedEventArgs e)
    {
        NhanSuGiaoDien.MoiTimKiem(txtTimCaLamViec, txtLoiTimCaLamViec, cboLocCaLamViec);
        TaiLaiCaLamViec();
    }

    private void TaiLaiLichLamViec() => TaiLai(dgvLichLamViec, txtTimLichLamViec, cboLocLichLamViec, txtThongBaoLichLamViec, (tuKhoa, trangThai) => bus!.LichLamViec(tuKhoa, trangThai));
    private void btnLichLamViecThemLich_Click(object sender, RoutedEventArgs e) => MoForm("btnLichLamViecThemLich", () => new LichLamViecForm(), TaiLaiLichLamViec, txtThongBaoLichLamViec);
    private void LichLamViec_DoubleClick(object sender, MouseButtonEventArgs e) => MoChiTiet<LichLamViec>(e, dgvLichLamViec, x => new LichLamViecChiTietWindow(x), TaiLaiLichLamViec, txtThongBaoLichLamViec);
    private void btnTimLichLamViec_Click(object sender, RoutedEventArgs e) => TaiLaiLichLamViec();
    private void btnMoiLichLamViec_Click(object sender, RoutedEventArgs e)
    {
        NhanSuGiaoDien.MoiTimKiem(txtTimLichLamViec, txtLoiTimLichLamViec, cboLocLichLamViec);
        TaiLaiLichLamViec();
    }

    private void TaiLaiChamCong() => TaiLai(dgvChamCong, txtTimChamCong, cboLocChamCong, txtThongBaoChamCong, (tuKhoa, trangThai) => bus!.ChamCong(tuKhoa, trangThai));
    private void btnChamCongThemChamCong_Click(object sender, RoutedEventArgs e) => MoForm("btnChamCongThemChamCong", () => new ChamCongForm(), TaiLaiChamCong, txtThongBaoChamCong);
    private void ChamCong_DoubleClick(object sender, MouseButtonEventArgs e) => MoChiTiet<ChamCong>(e, dgvChamCong, x => new ChamCongChiTietWindow(x), TaiLaiChamCong, txtThongBaoChamCong);
    private void btnTimChamCong_Click(object sender, RoutedEventArgs e) => TaiLaiChamCong();
    private void btnMoiChamCong_Click(object sender, RoutedEventArgs e)
    {
        NhanSuGiaoDien.MoiTimKiem(txtTimChamCong, txtLoiTimChamCong, cboLocChamCong);
        TaiLaiChamCong();
    }

    private void TaiLaiBangLuong() => TaiLai(dgvBangLuong, txtTimBangLuong, cboLocBangLuong, txtThongBaoBangLuong, (tuKhoa, trangThai) => bus!.BangLuong(tuKhoa, trangThai));
    private void btnBangLuongLapBangLuong_Click(object sender, RoutedEventArgs e) => MoForm("btnBangLuongLapBangLuong", () => new BangLuongForm(), TaiLaiBangLuong, txtThongBaoBangLuong);
    private void BangLuong_DoubleClick(object sender, MouseButtonEventArgs e) => MoChiTiet<BangLuong>(e, dgvBangLuong, x => new BangLuongChiTietWindow(x), TaiLaiBangLuong, txtThongBaoBangLuong);
    private void btnTimBangLuong_Click(object sender, RoutedEventArgs e) => TaiLaiBangLuong();
    private void btnMoiBangLuong_Click(object sender, RoutedEventArgs e)
    {
        NhanSuGiaoDien.MoiTimKiem(txtTimBangLuong, txtLoiTimBangLuong, cboLocBangLuong);
        TaiLaiBangLuong();
    }

    private void TaiLaiKhenThuong() => TaiLai(dgvKhenThuong, txtTimKhenThuong, cboLocKhenThuong, txtThongBaoKhenThuong, (tuKhoa, trangThai) => bus!.KhenThuong(tuKhoa, trangThai));
    private void btnKhenThuongThemKhenThuong_Click(object sender, RoutedEventArgs e) => MoForm("btnKhenThuongThemKhenThuong", () => new KhenThuongForm(), TaiLaiKhenThuong, txtThongBaoKhenThuong);
    private void KhenThuong_DoubleClick(object sender, MouseButtonEventArgs e) => MoChiTiet<KhenThuong>(e, dgvKhenThuong, x => new KhenThuongChiTietWindow(x), TaiLaiKhenThuong, txtThongBaoKhenThuong);
    private void btnTimKhenThuong_Click(object sender, RoutedEventArgs e) => TaiLaiKhenThuong();
    private void btnMoiKhenThuong_Click(object sender, RoutedEventArgs e)
    {
        NhanSuGiaoDien.MoiTimKiem(txtTimKhenThuong, txtLoiTimKhenThuong, cboLocKhenThuong);
        TaiLaiKhenThuong();
    }
}
