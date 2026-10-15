using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;

public partial class PhongView : UserControl
{
    private readonly List<PhongMau> _rooms;
    private readonly ICollectionView? _roomView;

    public PhongView()
    {
        InitializeComponent();
        _rooms = lstPhong.Items.OfType<PhongMau>().ToList();
        lstPhong.Items.Clear();
        lstPhong.ItemsSource = _rooms;
        _roomView = CollectionViewSource.GetDefaultView(_rooms);
        _roomView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PhongMau.Tang)));
        _roomView.Filter = item =>
        {
            var room = (PhongMau)item;
            var keyword = txtTimPhong.Text.Trim();
            return (keyword.Length == 0 || $"{room.Ma} {room.SoPhong} {room.LoaiPhong} {room.Tang}".Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                && (cboLocPhong.SelectedIndex == 0 || room.TinhTrang == (cboLocPhong.SelectedItem as ComboBoxItem)?.Content.ToString())
                && (cboLocVeSinh.SelectedIndex == 0 || room.VeSinh == (cboLocVeSinh.SelectedItem as ComboBoxItem)?.Content.ToString());
        };
        RefreshRooms();
    }

    private void RefreshRooms()
    {
        if (_roomView == null) return;
        var selected = lstPhong.SelectedItem;
        lstPhong.SelectedItem = null;
        _roomView.Refresh();
        lstPhong.SelectedItem = selected != null && _roomView.Contains(selected) ? selected : _roomView.Cast<PhongMau>().FirstOrDefault();
        var count = _roomView.Cast<PhongMau>().Count();
        txtKhongCoPhong.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
        txtSoPhongHienThi.Text = $"Hiển thị {count}/{_rooms.Count} phòng · Bấm để xem thông tin, nhấp đúp để mở chi tiết.";
        txtTongPhong.Text = $"Tổng: {_rooms.Count} phòng";
        txtPhongTrong.Text = $"Trống: {_rooms.Count(r => r.TinhTrang == "Trống")}";
        txtPhongDangO.Text = $"Đang ở: {_rooms.Count(r => r.TinhTrang == "Đang ở")}";
        txtPhongCanDon.Text = $"Cần dọn: {_rooms.Count(r => r.VeSinh == "Cần dọn")}";
        txtPhongBaoTri.Text = $"Bảo trì: {_rooms.Count(r => r.TinhTrang == "Bảo trì")}";
    }

    private void btnPhongThemMoi_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(PhongView), "btnPhongThemMoi")) return;
        new PhongForm { Owner = Window.GetWindow(this) }.ShowDialog();
    }

    private void OpenRoom(PhongMau room)
    {
        if (!DemoSession.CanUseButton(nameof(PhongView), "btnPhongChiTiet")) return;
        var form = new PhongChiTietWindow(room) { Owner = Window.GetWindow(this) };
        if (form.ShowDialog() == true) _rooms.Remove(room);
        RefreshRooms();
    }

    private void btnPhongChiTiet_Click(object sender, RoutedEventArgs e)
    {
        if (lstPhong.SelectedItem is PhongMau room) OpenRoom(room);
    }

    private void Phong_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source &&
            ItemsControl.ContainerFromElement(lstPhong, source) is ListBoxItem card && card.Content is PhongMau room)
            OpenRoom(room);
    }

    private void btnTimPhong_Click(object sender, RoutedEventArgs e) => RefreshRooms();
    private void LocPhong_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshRooms();
    private void TimPhong_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        RefreshRooms();
        e.Handled = true;
    }

    private void btnMoiPhong_Click(object sender, RoutedEventArgs e)
    {
        txtTimPhong.Clear();
        cboLocPhong.SelectedIndex = cboLocVeSinh.SelectedIndex = 0;
        RefreshRooms();
    }
}
