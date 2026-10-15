using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BUS_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;
using QuanLyKhachSan;
using QuanLyKhachSan.Views;
using QuanLyKhachSan.Views.Dialogs;

internal static class Program
{
    private static int checks;

    private static void Assert(bool valid, string message)
    {
        if (!valid) throw new Exception(message);
        checks++;
    }

    private static void Reject(Action action)
    {
        try { action(); }
        catch (ArgumentException) { checks++; return; }
        throw new Exception("Invalid department was accepted.");
    }

    private static T Field<T>(FrameworkElement element, string name) => (T)element.FindName(name);
    private static void Click(FrameworkElement element, string name) =>
        Field<Button>(element, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

    private static void SetText(FrameworkElement element, string name, string value)
    {
        var text = Field<TextBox>(element, name);
        text.SetCurrentValue(TextBox.TextProperty, value);
        text.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }

    private static void SetStatus(FrameworkElement element, string name, bool value)
    {
        var combo = Field<ComboBox>(element, name);
        combo.SetCurrentValue(Selector.SelectedValueProperty, value);
        combo.GetBindingExpression(Selector.SelectedValueProperty)?.UpdateSource();
    }

    private static void ShowHidden(Window window)
    {
        window.Opacity = 0;
        window.ShowInTaskbar = false;
        window.Show();
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        window.UpdateLayout();
    }

    private static bool? CheckForm(PhongBanForm form, Action action)
    {
        Exception? failure = null;
        form.Opacity = 0;
        form.ShowInTaskbar = false;
        form.Loaded += (_, _) => form.Dispatcher.BeginInvoke(new Action(() =>
        {
            try { action(); }
            catch (Exception ex) { failure = ex; }
            finally { if (form.IsVisible) form.Close(); }
        }));
        bool? result = form.ShowDialog();
        if (failure != null) throw failure;
        return result;
    }

    private static void Execute(SqlConnection connection, string sql)
    {
        using var command = new SqlCommand(sql, connection) { CommandTimeout = 15 };
        command.ExecuteNonQuery();
    }

    [STAThread]
    private static int Main()
    {
        var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.InitializeComponent();
        string? previousConnection = Environment.GetEnvironmentVariable("QLKS_CONNECTION");
        var database = "QLKS_PhongBanCheck_" + Guid.NewGuid().ToString("N");
        SqlConnection? admin = null;
        bool created = false;
        try
        {
            Reject(() => BUS_PhongBan.KiemTra(null!));
            foreach (string name in new[] { "", " \t\u00a0", new string('a', 101) })
                Reject(() => BUS_PhongBan.KiemTra(new PhongBan { TenPhongBan = name }));
            Reject(() => BUS_PhongBan.KiemTra(new PhongBan { MaPhongBan = -1, TenPhongBan = "Test" }));
            Reject(() => BUS_PhongBan.KiemTra(new PhongBan { TenPhongBan = "Test", MoTa = new string('a', 501) }));
            var boundary = new PhongBan { TenPhongBan = new string('a', 100), MoTa = new string('b', 500) };
            BUS_PhongBan.KiemTra(boundary);
            Assert(boundary.TenPhongBan.Length == 100 && boundary.MoTa.Length == 500, "Exact length limits rejected.");
            var trimmed = new PhongBan { TenPhongBan = "  Nhân sự  ", MoTa = "  Ghi chú  " };
            BUS_PhongBan.KiemTra(trimmed);
            Assert(trimmed.TenPhongBan == "Nhân sự" && trimmed.MoTa == "Ghi chú", "Whitespace not trimmed.");

            string source = string.IsNullOrWhiteSpace(previousConnection)
                ? File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "connection.txt")) : previousConnection;
            var connection = new SqlConnectionStringBuilder(source) { InitialCatalog = "master", ConnectTimeout = 5 };
            admin = new SqlConnection(connection.ConnectionString);
            admin.Open();
            Assert(Guid.TryParseExact(database["QLKS_PhongBanCheck_".Length..], "N", out _), "Unsafe test database name.");
            Execute(admin, $"CREATE DATABASE [{database}]");
            created = true;
            connection.InitialCatalog = database;
            Environment.SetEnvironmentVariable("QLKS_CONNECTION", connection.ConnectionString);
            using var testConnection = new SqlConnection(connection.ConnectionString);
            testConnection.Open();
            Execute(testConnection, "CREATE TABLE dbo.PhongBan (MaPhongBan int IDENTITY(1,1) PRIMARY KEY, " +
                "TenPhongBan nvarchar(100) NOT NULL UNIQUE, MoTa nvarchar(500) NULL, TrangThai bit NOT NULL)");

            var bus = new BUS_PhongBan();
            var department = new PhongBan { TenPhongBan = "  Phòng kiểm tra  ", MoTa = "Ghi chú", TrangThai = true };
            bus.Luu(department);
            Assert(department.MaPhongBan > 0 && bus.HienThiTatCa().Count == 1, "Insert/identity failed.");
            Assert(bus.HienThiTheoId(department.MaPhongBan).TenPhongBan == "Phòng kiểm tra", "SQL read failed.");
            Reject(() => bus.Luu(new PhongBan { TenPhongBan = "Phòng kiểm tra" }));
            Reject(() => bus.Luu(new PhongBan { MaPhongBan = int.MaxValue, TenPhongBan = "Missing" }));

            department.TenPhongBan = "Phòng đã sửa";
            department.TrangThai = false;
            bus.Luu(department);
            Assert(bus.HienThiTheoId(department.MaPhongBan).TenPhongBan == "Phòng đã sửa", "SQL update failed.");
            Assert(bus.HienThiTatCa("ĐÃ SỬA", false).Count == 1 && bus.HienThiTatCa("", true).Count == 0,
                "Keyword/status filter failed.");
            bus.Luu(boundary);
            Assert(bus.HienThiTheoId(boundary.MaPhongBan).MoTa.Length == 500, "Boundary round trip failed.");

            DemoSession.SignIn("admin", "admin123");
            var invalid = new PhongBanForm();
            CheckForm(invalid, () =>
            {
                Assert(Field<TextBox>(invalid, "txtTenPhongBan").Text == "", "Add form has sample data.");
                Assert(Field<ComboBox>(invalid, "cboTrangThai").SelectedValue is true, "New status must be active.");
                SetText(invalid, "txtTenPhongBan", " ");
                Click(invalid, "btnLuu");
                Assert(invalid.IsVisible && Field<TextBlock>(invalid, "txtThongBao").Text.Contains("tên phòng ban"),
                    "Invalid form closed or lost error.");
                SetText(invalid, "txtTenPhongBan", new string('x', 101));
                Click(invalid, "btnLuu");
                Assert(invalid.IsVisible && Field<TextBlock>(invalid, "txtThongBao").Text.Contains("100"),
                    "Overlong name was not rejected in form.");
            });

            var form = new PhongBanForm();
            Assert(CheckForm(form, () =>
            {
                SetText(form, "txtTenPhongBan", "  Tạo từ form  ");
                SetText(form, "txtGhiChu", "Binding SQL");
                SetStatus(form, "cboTrangThai", true);
                Click(form, "btnLuu");
            }) == true, "Add form did not report a successful SQL save.");
            var added = bus.HienThiTatCa("Tạo từ form").Single();
            Assert(added.MoTa == "Binding SQL" && added.TrangThai, "Add binding did not reach SQL.");

            var detail = new PhongBanChiTietWindow(added);
            ShowHidden(detail);
            Assert(!Field<Button>(detail, "btnPhongBanXoa").IsEnabled, "Unimplemented delete is enabled.");
            Assert(Field<TextBox>(detail, "txtMa").Text == added.MaPhongBan.ToString(), "Detail ID binding failed.");
            Click(detail, "btnPhongBanSuaThongTin");
            SetText(detail, "txtTenPhongBan", "Chưa lưu");
            SetStatus(detail, "cboTrangThai", false);
            Assert(added.TenPhongBan == "Tạo từ form" && added.TrangThai, "Edit mutated the original before saving.");
            Click(detail, "btnHuySua");
            Assert(Field<TextBox>(detail, "txtTenPhongBan").Text == "Tạo từ form" &&
                Field<ComboBox>(detail, "cboTrangThai").SelectedValue is true, "Cancel did not restore the draft.");
            Click(detail, "btnPhongBanSuaThongTin");
            SetText(detail, "txtTenPhongBan", "");
            Click(detail, "btnLuuSua");
            Assert(!Field<TextBox>(detail, "txtTenPhongBan").IsReadOnly &&
                bus.HienThiTheoId(added.MaPhongBan).TenPhongBan == "Tạo từ form", "Invalid edit reached SQL.");
            SetText(detail, "txtTenPhongBan", "Đã sửa từ chi tiết");
            SetText(detail, "txtMoTa", "Đã cập nhật");
            SetStatus(detail, "cboTrangThai", false);
            Click(detail, "btnLuuSua");
            var updated = bus.HienThiTheoId(added.MaPhongBan);
            Assert(updated.TenPhongBan == "Đã sửa từ chi tiết" && updated.MoTa == "Đã cập nhật" && !updated.TrangThai,
                "Detail changes were not persisted.");
            Assert(Field<TextBox>(detail, "txtTenPhongBan").IsReadOnly, "Save did not exit edit mode.");
            detail.Close();

            var view = new NhanVienView();
            Field<TabControl>(view, "tabChinh").SelectedIndex = 1;
            var host = new Window { Content = view, Width = 1200, Height = 800 };
            ShowHidden(host);
            var grid = Field<DataGrid>(view, "dgvPhongBan");
            Assert(grid.ItemsSource is List<PhongBan> && grid.Items.Count == 3, "List is not loaded from SQL.");
            Assert(grid.SelectedValuePath == "MaPhongBan" && grid.SelectedValue != null, "Selected ID binding failed.");
            SetText(view, "txtTimPhongBan", "Đã sửa từ chi tiết");
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 1, "Search did not reload SQL data.");
            Field<ComboBox>(view, "cboLocPhongBan").SelectedIndex = 1;
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 0, "Active filter failed.");
            Field<ComboBox>(view, "cboLocPhongBan").SelectedIndex = 2;
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 1, "Inactive filter failed.");
            Click(view, "btnMoiPhongBan");
            Assert(grid.Items.Count == 3 && Field<TextBox>(view, "txtTimPhongBan").Text == "", "Refresh failed.");
            host.Close();

            DemoSession.SignIn("letan", "demo123");
            var denied = new PhongBanForm();
            CheckForm(denied, () =>
            {
                Assert(!Field<Button>(denied, "btnLuu").IsEnabled, "Role restriction missing.");
                SetText(denied, "txtTenPhongBan", "Forbidden");
                Click(denied, "btnLuu");
                Assert(denied.IsVisible, "Unauthorized save closed the form.");
            });
            Assert(bus.HienThiTatCa().Count == 3, "Unauthorized save reached SQL.");
            DemoSession.SignIn("admin", "admin123");
            Execute(testConnection, "DROP TABLE dbo.PhongBan");
            var failed = new PhongBanForm();
            CheckForm(failed, () =>
            {
                SetText(failed, "txtTenPhongBan", "Không lưu được");
                Click(failed, "btnLuu");
                Assert(failed.IsVisible && Field<TextBlock>(failed, "txtThongBao").Text.Contains("Không lưu được"),
                    "DAL failure was reported as success.");
            });

            Console.WriteLine($"PASS: {checks} department validation/SQL/WPF checks. Application database unchanged.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally
        {
            foreach (Window window in app.Windows.Cast<Window>().ToArray()) window.Close();
            DemoSession.SignOut();
            Environment.SetEnvironmentVariable("QLKS_CONNECTION", previousConnection);
            if (created && admin != null)
            {
                Execute(admin, $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]");
                Console.WriteLine("Removed isolated test database: " + database);
            }
            admin?.Dispose();
            app.Shutdown();
        }
    }
}
