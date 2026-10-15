using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class DanhMucPhongView : UserControl
    {
        private readonly ObservableCollection<KhuVucMau> _areas;
        private readonly ICollectionView _areaView;

        public DanhMucPhongView()
        {
            InitializeComponent();
            _areas = new ObservableCollection<KhuVucMau>(dgvKhuVuc.Items.Cast<KhuVucMau>());
            dgvKhuVuc.Items.Clear();
            dgvKhuVuc.ItemsSource = _areas;
            _areaView = CollectionViewSource.GetDefaultView(_areas);
            _areaView.Filter = value => value is KhuVucMau area &&
                (area.Ma.Contains(txtTimKhuVuc.Text.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
                 area.TenKhuVuc.Contains(txtTimKhuVuc.Text.Trim(), StringComparison.CurrentCultureIgnoreCase));
            dgvKhuVuc.SelectedItem = _areas.FirstOrDefault();
            UpdateFloors();
            btnKhuVucThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucThemMoi");
            btnTangThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTangThemMoi");
            btnLoaiPhongThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongThemMoi");
            btnTienNghiThemMoi.IsEnabled = DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiThemMoi");
        }

        private void btnKhuVucThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DanhMucPhongView", "btnKhuVucThemMoi")) return;
            var item = new KhuVucMau { TrangThai = "Đang hoạt động" };
            var form = new KhuVucChiTietWindow(item, true, false, _areas) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() != true) return;
            _areas.Add(item);
            txtTimKhuVuc.Clear();
            _areaView.Refresh();
            dgvKhuVuc.SelectedItem = item;
        }

        private void KhuVuc_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DanhMucPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvKhuVuc, source) is not DataGridRow row ||
                row.Item is not Data.KhuVucMau item) return;
            var form = new KhuVucChiTietWindow(item, false, false, _areas) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) _areas.Remove(item);
            dgvKhuVuc.Items.Refresh();
            UpdateFloors();
        }

        private void btnTimKhuVuc_Click(object sender, RoutedEventArgs e)
        {
            _areaView.Refresh();
            if (dgvKhuVuc.SelectedItem == null) dgvKhuVuc.SelectedItem = _areaView.Cast<KhuVucMau>().FirstOrDefault();
            UpdateFloors();
        }

        private void btnMoiKhuVuc_Click(object sender, RoutedEventArgs e)
        {
            txtTimKhuVuc.Clear();
            btnTimKhuVuc_Click(sender, e);
        }

        private void btnTangThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTangThemMoi") || dgvKhuVuc.SelectedItem is not KhuVucMau area) return;
            var item = new TangMau { KhuVuc = area.TenKhuVuc, TrangThai = "Đang hoạt động" };
            var form = new TangChiTietWindow(item, true, true, _areas.SelectMany(a => a.Tangs)) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) area.Tangs.Add(item);
        }

        private void Tang_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DanhMucPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvTang, source) is not DataGridRow row ||
                row.Item is not Data.TangMau item) return;
            var form = new TangChiTietWindow(item, false, true, _areas.SelectMany(a => a.Tangs)) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true && dgvKhuVuc.SelectedItem is KhuVucMau area) area.Tangs.Remove(item);
            dgvTang.Items.Refresh();
        }





        private void btnLoaiPhongThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DanhMucPhongView", "btnLoaiPhongThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            LoaiPhongForm form = new LoaiPhongForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void LoaiPhong_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DanhMucPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvLoaiPhong, source) is not DataGridRow row ||
                row.Item is not Data.LoaiPhongMau item) return;
            var form = new LoaiPhongChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvLoaiPhong.Items.Remove(item);
            dgvLoaiPhong.Items.Refresh();
        }

        private void btnTimLoaiPhong_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiLoaiPhong_Click(object sender, RoutedEventArgs e)
        {
            txtTimLoaiPhong.Clear();
            cboLocLoaiPhong.SelectedIndex = 0;
        }

        private void btnTienNghiThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("DanhMucPhongView", "btnTienNghiThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            TienNghiForm form = new TienNghiForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void TienNghi_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("DanhMucPhongView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvTienNghi, source) is not DataGridRow row ||
                row.Item is not Data.TienNghiMau item) return;
            var form = new TienNghiChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvTienNghi.Items.Remove(item);
            dgvTienNghi.Items.Refresh();
        }

        private void btnTimTienNghi_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiTienNghi_Click(object sender, RoutedEventArgs e)
        {
            txtTimTienNghi.Clear();
            cboLocTienNghi.SelectedIndex = 0;
        }

        private void KhuVuc_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateFloors();

        private void UpdateFloors()
        {
            if (dgvTang == null || txtKhuVucDangChon == null || btnTangThemMoi == null) return;
            var area = dgvKhuVuc.SelectedItem as KhuVucMau;
            dgvTang.ItemsSource = area?.Tangs;
            txtKhuVucDangChon.Text = area == null ? "Chọn một khu vực" : $"Tầng thuộc {area.TenKhuVuc}";
            btnTangThemMoi.IsEnabled = area != null && DemoSession.CanUseButton("DanhMucPhongView", "btnTangThemMoi");
        }
    }
}
