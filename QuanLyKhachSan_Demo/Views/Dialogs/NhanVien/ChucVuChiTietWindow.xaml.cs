using System.Windows;
using System.Windows.Controls;
using BUS_QuanlyKhachSan;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class ChucVuChiTietWindow : Window
{
    private readonly BUS_ChucVu bus = new();
    private ChucVu chucVu;
    private bool _editing;

    public ChucVuChiTietWindow(ChucVu duLieu)
    {
        InitializeComponent();
        chucVu = duLieu;
        btnChucVuSuaThongTin.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
        btnChucVuXoa.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuXoa");
        btnLuuSua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
        btnHuySua.IsEnabled = DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin");
        HienThiChucVu();
    }

    private void HienThiChucVu()
    {
        // Sửa trên bản nháp để Hủy sửa hoặc Đóng không đổi dữ liệu đang hiển thị.
        DataContext = new ChucVu
        {
            MaChucVu = chucVu.MaChucVu,
            TenChucVu = chucVu.TenChucVu,
            MoTa = chucVu.MoTa,
            // Khác Phòng ban: sao chép cả mức lương để Hủy sửa khôi phục đúng lương cũ.
            Chucvu = chucVu.Chucvu,
            TrangThai = chucVu.TrangThai
        };
        SetEditMode(false);
        // Thêm: buộc ô lương OneWay trở về bản nháp mới sau Hủy sửa.
        txtLuong.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnSua_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin")) return;
        SetEditMode(true);
    }

    private void btnHuySua_Click(object sender, RoutedEventArgs e)
    {
        // Thêm: hủy chỉ bỏ bản nháp, vẫn cho phép khi quyền lưu vừa bị thu hồi.
        HienThiChucVu();
    }

    private void SetEditMode(bool editing)
    {
        _editing = editing;
        txtTenChucVu.IsReadOnly = !editing;
        txtMoTa.IsReadOnly = !editing;
        // Khác Phòng ban: bật/tắt chỉnh sửa thêm ô mức lương.
        txtLuong.IsReadOnly = !editing;
        cboTrangThai.IsEnabled = editing;
        btnChucVuSuaThongTin.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnChucVuXoa.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        btnLuuSua.Visibility = btnHuySua.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        txtThongBao.Text = editing ? "Chỉnh sửa rồi nhấn Lưu thay đổi hoặc Hủy sửa." : "Nhấn Sửa để chỉnh sửa thông tin của dòng này.";
    }

    private void btnLuuSua_Click(object sender, RoutedEventArgs e)
    {
        if (!_editing || !DemoSession.CanUseButton("NhanVienView", "btnChucVuSuaThongTin")) return;
        try
        {
            var duLieu = (ChucVu)DataContext;
            // Khác Phòng ban: kiểm tra chuỗi nhập rồi đổi sang tiền, không lưu nhầm lương cũ khi nhập sai.
            duLieu.Chucvu = BUS_KiemTra.SoTien(txtLuong.Text, "Mức lương");
            bus.Luu(duLieu);
            chucVu = duLieu;
            HienThiChucVu();
            txtThongBao.Text = "Đã lưu thông tin chức vụ.";
        }
        catch (Exception ex) { txtThongBao.Text = "Không lưu được chức vụ. " + ex.Message; }
    }

    private void btnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (_editing || !DemoSession.CanUseButton("NhanVienView", "btnChucVuXoa")) return;
        var confirm = new XacNhanDemoWindow { Owner = this };
        confirm.txtNoiDung.Text = $"Xóa chức vụ {chucVu.MaChucVu} — {chucVu.TenChucVu}? Thao tác này không thể hoàn tác.";
        if (confirm.ShowDialog() != true) return;
        try
        {
            bus.Xoa(chucVu.MaChucVu);
            DialogResult = true;
        }
        catch (Exception ex) { txtThongBao.Text = "Không xóa được chức vụ. " + ex.Message; }
    }

}
