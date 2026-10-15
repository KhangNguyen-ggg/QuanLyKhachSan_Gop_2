using System.Windows;

namespace QuanLyKhachSan.Views;

public partial class LoginThuCongWindow : Window
{
    public LoginThuCongWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUser.Text.Trim();
        string password = txtPass.Password;
        // Giới hạn khớp form tài khoản; giữ nguyên mật khẩu người dùng nhập.
        if (username.Length == 0 || username.Length > 50)
        {
            lblErr.Text = username.Length == 0
                ? "Vui lòng nhập tên đăng nhập."
                : "Tên đăng nhập không được vượt quá 50 ký tự.";
            txtUser.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(password) || password.Length > 1000)
        {
            lblErr.Text = string.IsNullOrWhiteSpace(password)
                ? "Vui lòng nhập mật khẩu."
                : "Mật khẩu không được vượt quá 1000 ký tự.";
            txtPass.Focus();
            return;
        }
        // Đăng nhập nhập tay dùng chung xác thực và vai trò với form test.
        if (!DemoSession.SignIn(username, password))
        {
            lblErr.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
            txtPass.Focus();
            return;
        }
        var form = new MainWindow();
        Application.Current.MainWindow = form;
        form.Show();
        Close();
    }
}
