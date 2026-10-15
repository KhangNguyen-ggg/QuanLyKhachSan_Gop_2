using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views
{
    public partial class TieuHaoView : UserControl
    {
        public TieuHaoView()
        {
            InitializeComponent();
        }

        private void btnDinhMucTieuHaoThemMoi_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("TieuHaoView", "btnDinhMucTieuHaoThemMoi")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            DinhMucTieuHaoForm form = new DinhMucTieuHaoForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void DinhMucTieuHao_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("TieuHaoView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvDinhMucTieuHao, source) is not DataGridRow row ||
                row.Item is not Data.DinhMucTieuHaoMau item) return;
            var form = new DinhMucTieuHaoChiTietWindow(item) { Owner = Window.GetWindow(this) };
            if (form.ShowDialog() == true) dgvDinhMucTieuHao.Items.Remove(item);
            dgvDinhMucTieuHao.Items.Refresh();
        }

        private void btnTimDinhMucTieuHao_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiDinhMucTieuHao_Click(object sender, RoutedEventArgs e)
        {
            txtTimDinhMucTieuHao.Clear();
            cboLocDinhMucTieuHao.SelectedIndex = 0;
        }

        private void btnCapVatTuNoiBoTaoPhieuCap_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.CanUseButton("TieuHaoView", "btnCapVatTuNoiBoTaoPhieuCap")) return;
            // Chỉ mở cửa sổ XAML có sẵn; không tạo control bằng code.
            CapVatTuNoiBoForm form = new CapVatTuNoiBoForm();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }

        private void CapVatTuNoiBo_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!DemoSession.CanOpen("TieuHaoView") || e.OriginalSource is not DependencyObject source ||
                ItemsControl.ContainerFromElement(dgvCapVatTuNoiBo, source) is not DataGridRow row ||
                row.Item is not Data.CapVatTuNoiBoMau item) return;
            var form = new CapVatTuNoiBoChiTietWindow(item) { Owner = Window.GetWindow(this) };
            form.ShowDialog();
            dgvCapVatTuNoiBo.Items.Refresh();
        }

        private void btnTimCapVatTuNoiBo_Click(object sender, RoutedEventArgs e)
        {
            // Chưa triển khai tìm kiếm.
        }

        private void btnMoiCapVatTuNoiBo_Click(object sender, RoutedEventArgs e)
        {
            txtTimCapVatTuNoiBo.Clear();
            cboLocCapVatTuNoiBo.SelectedIndex = 0;
        }
    }
}
