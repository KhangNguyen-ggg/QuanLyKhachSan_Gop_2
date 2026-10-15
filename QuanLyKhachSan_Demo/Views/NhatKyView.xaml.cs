using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyKhachSan.Views.Dialogs;

namespace QuanLyKhachSan.Views;

public partial class NhatKyView : UserControl
{
    public NhatKyView()
    {
        InitializeComponent();
    }
    private void btnNhatKyThaoTacBoLocNhatKy_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(NhatKyView), "btnNhatKyThaoTacBoLocNhatKy")) return;
        var form = new NhatKyThaoTacForm { Owner = Window.GetWindow(this) };
        form.ShowDialog();
    }
    private void btnTimNhatKyThaoTac_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton(nameof(NhatKyView), "btnTimNhatKyThaoTac")) return;
        MessageBox.Show("Chức năng tìm kiếm nhật ký đang được hoàn thiện.", "Thông báo");
    }
    private void btnMoiNhatKyThaoTac_Click(object sender, RoutedEventArgs e)
    {
        txtTimNhatKyThaoTac.Clear();
        cboLocNhatKyThaoTac.SelectedIndex = 0;
    }
    private void NhatKyThaoTac_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!DemoSession.CanOpen(nameof(NhatKyView)) || e.OriginalSource is not DependencyObject source ||
            ItemsControl.ContainerFromElement(dgvNhatKyThaoTac, source) is not DataGridRow row) return;
        var form = new NhatKyThaoTacChiTietWindow { DataContext = row.Item, Owner = Window.GetWindow(this) };
        form.ShowDialog();
    }
}
