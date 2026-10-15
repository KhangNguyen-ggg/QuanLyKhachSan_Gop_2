using System.Windows;

namespace QuanLyKhachSan.Views.Dialogs;

public partial class NhatKyThaoTacForm : Window
{
    public NhatKyThaoTacForm()
    {
        InitializeComponent();
        dtpTuNgay.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpDenNgay.SelectedDate = DateTime.Today;
        btnLuu.IsEnabled = DemoSession.CanUseButton("NhatKyView", "btnNhatKyThaoTacBoLocNhatKy");
    }

    private void btnDong_Click(object sender, RoutedEventArgs e) => Close();

    private void btnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!DemoSession.CanUseButton("NhatKyView", "btnNhatKyThaoTacBoLocNhatKy")) return;
        // shortcut: giữ bộ lọc nhật ký, nối BUS/DAL khi triển khai đọc nhật ký.
        txtThongBao.Text = "Chức năng lọc nhật ký chưa được triển khai. Lựa chọn chưa được áp dụng.";
    }
}