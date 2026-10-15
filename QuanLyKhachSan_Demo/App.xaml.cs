using System.Windows;
using QuanLyKhachSan.Views;

namespace QuanLyKhachSan
{
    public partial class App : Application
    {
        // true: form có ComboBox để test; false: form nhập tay để trình bày.
        public static bool DungDangNhapDemo = true;

        public static Window TaoFormDangNhap() => DungDangNhapDemo
            ? new LoginWindow()
            : new LoginThuCongWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = TaoFormDangNhap();
            MainWindow.Show();
        }
    }
}
