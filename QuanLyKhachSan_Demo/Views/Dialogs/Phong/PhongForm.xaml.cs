using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class PhongForm : Window
{
    private readonly PhongMau? _room;
    public PhongForm(PhongMau? room = null)
    {
        InitializeComponent();
        DataContext = _room = room;
        btnLuu.IsEnabled = DemoSession.CanUseButton("PhongView", room == null ? "btnPhongThemMoi" : "btnPhongSuaThongTin");
        if (room == null) return;
        Title = txtTieuDe.Text = $"Sửa thông tin phòng {room.SoPhong}";
        txtSoPhong.Text = room.SoPhong;
        txtSoPhong.IsReadOnly = true;
        pnlThongTinBoSung.Visibility = Visibility.Collapsed;
        txtGhiChu.Text = room.GhiChu;
        cboLoaiPhong.SelectedItem = cboLoaiPhong.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, room.LoaiPhong));
        cboVeSinh.SelectedItem = cboVeSinh.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, room.VeSinh));
        cboKhaiThac.SelectedItem = cboKhaiThac.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, room.KhaiThac));
        cboTang.SelectedItem = cboTang.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString()!.StartsWith(room.Tang + " —"));
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhongView", _room == null ? "btnPhongThemMoi" : "btnPhongSuaThongTin")) return;
        if (_room == null)
        {
            // shortcut: form tạo phòng vẫn là demo, nối tầng lưu trữ khi triển khai tạo phòng thật.
            Close();
            return;
        }
        if (cboLoaiPhong.SelectedItem is not ComboBoxItem type || cboTang.SelectedItem is not ComboBoxItem floor ||
            cboVeSinh.SelectedItem is not ComboBoxItem cleaning || cboKhaiThac.SelectedItem is not ComboBoxItem operation)
        {
            txtThongBao.Text = "Vui lòng chọn đầy đủ loại phòng, tầng, vệ sinh và khai thác.";
            return;
        }
        // shortcut: cập nhật phòng demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _room.LoaiPhong = type.Content.ToString()!;
        _room.Tang = floor.Content.ToString()!.Split(" — ")[0];
        _room.VeSinh = cleaning.Content.ToString()!;
        _room.KhaiThac = operation.Content.ToString()!;
        _room.GhiChu = txtGhiChu.Text.Trim();
        if (_room.KhaiThac is "Bảo trì" or "Ngừng khai thác") _room.TinhTrang = _room.KhaiThac;
        else if (_room.TinhTrang is "Bảo trì" or "Ngừng khai thác") _room.TinhTrang = "Chưa cập nhật";
        DialogResult = true;
    }
}
