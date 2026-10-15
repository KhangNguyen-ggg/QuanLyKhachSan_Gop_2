using System.Windows;

namespace QuanLyKhachSan.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            cboDemoAccount.ItemsSource = DemoSession.Accounts;
            cboDemoAccount.SelectedIndex = 0;
        }

        private void DemoAccount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cboDemoAccount.SelectedItem is not DemoAccount account) return;
            txtUser.Text = account.Username;
            txtPass.Password = account.Password;
            lblErr.Text = "";
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!DemoSession.SignIn(txtUser.Text, txtPass.Password))
            {
                lblErr.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return;
            }
            MainWindow form = new MainWindow();
            Application.Current.MainWindow = form;
            form.Show();
            Close();
        }
    }
}
