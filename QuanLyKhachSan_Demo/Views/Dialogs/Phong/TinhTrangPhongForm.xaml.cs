using System.Windows;
using System.Windows.Controls;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class TinhTrangPhongForm : Window
{
    private readonly PhongMau? _room;
    public TinhTrangPhongForm(PhongMau? room = null)
    {
        InitializeComponent();
        DataContext = _room = room;
        btnLuu.IsEnabled = DemoSession.CanUseButton("PhongView", "btnTinhTrangPhongCapNhatTinhTrang");
        if (room == null) return;
        Title = txtTieuDe.Text = $"Cập nhật tình trạng phòng {room.SoPhong}";
        cboPhong.Items.Clear();
        cboPhong.Items.Add(new ComboBoxItem { Content = $"{room.SoPhong} — {room.LoaiPhong}", Style = (Style)FindResource("ReadableComboBoxItem") });
        cboPhong.SelectedIndex = 0;
        cboPhong.IsEnabled = false;
        cboTinhTrang.SelectedItem = cboTinhTrang.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, room.TinhTrang));
        cboVeSinh.SelectedItem = cboVeSinh.Items.OfType<ComboBoxItem>().FirstOrDefault(i => Equals(i.Content, room.VeSinh));
        txtGhiChu.Text = room.GhiChu;
    }
    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();
    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("PhongView", "btnTinhTrangPhongCapNhatTinhTrang")) return;
        if (_room == null)
        {
            // shortcut: form không có phòng đích chỉ là demo, truyền phòng khi nối dữ liệu thật.
            Close();
            return;
        }
        if (cboTinhTrang.SelectedItem is not ComboBoxItem state || cboVeSinh.SelectedItem is not ComboBoxItem cleaning)
        {
            txtThongBao.Text = "Vui lòng chọn tình trạng phòng và tình trạng vệ sinh.";
            return;
        }
        // shortcut: cập nhật phòng demo trong bộ nhớ, nối tầng lưu trữ khi triển khai nghiệp vụ thật.
        _room.TinhTrang = state.Content.ToString()!;
        _room.VeSinh = cleaning.Content.ToString()!;
        _room.GhiChu = txtGhiChu.Text.Trim();
        _room.KhaiThac = _room.TinhTrang switch
        {
            "Bảo trì" => "Bảo trì",
            "Ngừng khai thác" => "Ngừng khai thác",
            _ => "Đang hoạt động"
        };
        DialogResult = true;
    }
}
