using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using QuanLyKhachSan;
using QuanLyKhachSan.Views;

internal static class Program
{
    private static void Assert(bool valid, string message)
    {
        if (!valid) throw new Exception(message);
    }

    private static T Field<T>(FrameworkElement element, string name) => (T)element.FindName(name);
    private static void Click(Button button) => button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

    private static IEnumerable<DependencyObject> Children(DependencyObject parent)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
        {
            yield return child;
            foreach (var descendant in Children(child)) yield return descendant;
        }
    }

    [STAThread]
    private static int Main()
    {
        // Chỉ thử đăng nhập demo; không dùng kết nối SQL hoặc mở cửa sổ trên màn hình người dùng.
        Environment.SetEnvironmentVariable("QLKS_CONNECTION", " ");
        var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.InitializeComponent();
        app.Resources.Add(typeof(Window), new Style(typeof(Window)) { Setters = {
            new Setter(UIElement.OpacityProperty, 0d), new Setter(Window.ShowInTaskbarProperty, false) } });
        int result = 0;
        app.Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                Assert(!App.DungDangNhapDemo, "Default login must be manual.");
                var login = app.MainWindow as LoginThuCongWindow ?? throw new Exception("Startup opened the wrong login.");
                Assert(!Children(login).OfType<ComboBox>().Any(), "Manual login has a user selector.");
                Assert(Field<TextBox>(login, "txtUser").Text == "" && Field<PasswordBox>(login, "txtPass").Password == "",
                    "Manual login prefilled credentials.");
                Assert(Field<Button>(login, "btnDangNhap").IsDefault, "Enter does not submit login.");
                login.UpdateLayout();
                var bitmap = new RenderTargetBitmap((int)login.ActualWidth, (int)login.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(login);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                string root = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../../../../"));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(root, "KiemTraGop2"));
                using (var file = System.IO.File.Create(System.IO.Path.Combine(root, "KiemTraGop2/LoginThuCong.png"))) encoder.Save(file);

                foreach (var sample in new[] {
                    (Username: "", Password: "admin123", Error: "Vui lòng nhập tên đăng nhập."),
                    (Username: " \t\u00A0", Password: "admin123", Error: "Vui lòng nhập tên đăng nhập."),
                    (Username: "admin", Password: "", Error: "Vui lòng nhập mật khẩu."),
                    (Username: "admin", Password: " \t", Error: "Vui lòng nhập mật khẩu."),
                    (Username: new string('a', 51), Password: "admin123", Error: "Tên đăng nhập không được vượt quá 50 ký tự."),
                    (Username: "admin", Password: new string('p', 1001), Error: "Mật khẩu không được vượt quá 1000 ký tự."),
                    (Username: new string('a', 50), Password: new string('p', 1000), Error: "Tên đăng nhập hoặc mật khẩu không đúng."),
                    (Username: "wrong", Password: "wrong", Error: "Tên đăng nhập hoặc mật khẩu không đúng."),
                    (Username: "admin", Password: " admin123 ", Error: "Tên đăng nhập hoặc mật khẩu không đúng.")
                })
                {
                    Field<TextBox>(login, "txtUser").Text = sample.Username;
                    Field<PasswordBox>(login, "txtPass").Password = sample.Password;
                    Click(Field<Button>(login, "btnDangNhap"));
                    Assert(ReferenceEquals(app.MainWindow, login) && DemoSession.Current == null &&
                        Field<TextBlock>(login, "lblErr").Text == sample.Error, "Login validation failed: " + sample.Error);
                }
                Field<TextBox>(login, "txtUser").Text = " admin ";
                Field<PasswordBox>(login, "txtPass").Password = "admin123";
                Click(Field<Button>(login, "btnDangNhap"));
                Assert(app.MainWindow is MainWindow && DemoSession.Current?.Role == DemoRole.Admin && !login.IsVisible,
                    "Manual login did not preserve authentication/role.");
                Click(Field<Button>(app.MainWindow, "btnDangXuat"));
                Assert(app.MainWindow is LoginThuCongWindow && DemoSession.Current == null &&
                    Field<TextBox>(app.MainWindow, "txtUser").Text == "" && Field<PasswordBox>(app.MainWindow, "txtPass").Password == "",
                    "Logout did not return to a blank manual login.");

                App.DungDangNhapDemo = true;
                var previous = app.MainWindow;
                var demo = App.TaoFormDangNhap() as LoginWindow ?? throw new Exception("Demo selection opened the wrong form.");
                app.MainWindow = demo;
                demo.Show();
                previous.Close();
                Assert(Field<ComboBox>(demo, "cboDemoAccount").Items.Count == DemoSession.Accounts.Count,
                    "Original demo account selector was changed.");
                Click(Children(demo).OfType<Button>().Single(x => Convert.ToString(x.Content)?.Contains("ĐĂNG NHẬP") == true));
                Assert(app.MainWindow is MainWindow && DemoSession.Current?.Role == DemoRole.Admin, "Original demo login failed.");
                Click(Field<Button>(app.MainWindow, "btnDangXuat"));
                Assert(app.MainWindow is LoginWindow && DemoSession.Current == null, "Demo logout opened the wrong form.");
                Console.WriteLine("PASS: empty/whitespace fields, length limits/boundaries, password spaces preserved, invalid/valid login, unchanged role, and logout in both login modes. No SQL used.");
            }
            catch (Exception ex) { Console.Error.WriteLine(ex); result = 1; }
            finally { app.Shutdown(); }
        }), DispatcherPriority.ApplicationIdle);
        app.Run();
        return result;
    }
}
