using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

    private static void Reject(Action action, string? expectedMessage = null)
    {
        try { action(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Assert(expectedMessage == null || ex.Message.Contains(expectedMessage), "Unexpected rejection: " + ex.Message);
            return;
        }
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

    private static bool? CheckForm(Window form, Action action)
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

    private static void ConfirmDelete(PhongBanChiTietWindow detail, bool accept)
    {
        Exception? failure = null;
        bool handled = false;
        var operation = detail.Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                var confirm = Application.Current.Windows.OfType<XacNhanDemoWindow>().Single();
                confirm.Opacity = 0;
                var department = (PhongBan)detail.DataContext;
                Assert(confirm.Owner == detail && Field<TextBlock>(confirm, "txtNoiDung").Text.Contains(department.TenPhongBan),
                    "Delete confirmation lost the department identity.");
                Click(confirm, accept ? "btnLuu" : "btnDong");
            }
            catch (Exception ex)
            {
                failure = ex;
                foreach (var window in Application.Current.Windows.OfType<XacNhanDemoWindow>().ToArray()) window.Close();
            }
            finally { handled = true; }
        }));
        Click(detail, "btnPhongBanXoa");
        if (!handled) { operation.Abort(); throw new Exception("Delete did not open a confirmation."); }
        if (failure != null) throw failure;
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
            Execute(testConnection, "CREATE TABLE dbo.NhanVien (MaNhanVien int IDENTITY(1,1) PRIMARY KEY, " +
                "MaPhongBan int NOT NULL REFERENCES dbo.PhongBan(MaPhongBan))");

            var bus = new BUS_PhongBan();
            Reject(() => bus.Xoa(0));
            Reject(() => bus.Xoa(int.MaxValue), "không còn tồn tại");
            var department = new PhongBan { TenPhongBan = "  Phòng kiểm tra  ",
                MoTa = "Nhân sự và đào tạo; lễ tân, quản lý buồng phòng".Normalize(System.Text.NormalizationForm.FormD),
                TrangThai = true };
            bus.Luu(department);
            Assert(department.MaPhongBan > 0 && bus.HienThiTatCa().Count == 1, "Insert/identity failed.");
            Assert(bus.HienThiTheoId(department.MaPhongBan).TenPhongBan == "Phòng kiểm tra", "SQL read failed.");
            Reject(() => bus.Luu(new PhongBan { TenPhongBan = "  Phòng kiểm tra  " }), "đã tồn tại");
            Reject(() => bus.Luu(new PhongBan { MaPhongBan = int.MaxValue, TenPhongBan = "Missing" }));

            department.TenPhongBan = "Phòng đã sửa";
            department.TrangThai = false;
            bus.Luu(department);
            Assert(bus.HienThiTheoId(department.MaPhongBan).TenPhongBan == "Phòng đã sửa", "SQL update failed.");
            Assert(bus.HienThiTatCa("ĐÃ SỬA", false).Count == 1 && bus.HienThiTatCa("", true).Count == 0,
                "Keyword/status filter failed.");
            foreach (string keyword in new[] { "  PHONG DA SUA  ", "phong da sua",
                "Phòng đã sửa".Normalize(System.Text.NormalizationForm.FormD),
                "nhan su", "dao tao", "le tan", "quan ly", "buong phong" })
            {
                var matches = bus.HienThiTatCa(keyword, false);
                Assert(matches.Count == 1 && matches[0].MaPhongBan == department.MaPhongBan,
                    "Accent-insensitive name/description search failed: " + keyword);
            }
            Assert(bus.HienThiTatCa("dao tao", true).Count == 0,
                "Accent-insensitive search bypassed the status filter.");
            foreach (string keyword in new[] { "nhan su@", "#", "$", "%", "_", "&", "le-tan", "nhan.su",
                "'", "\"", "nhan/su", "nhan\\su", "nhan;su", "(nhan su)", "😀", "\u0301",
                "nhan\tsu", "nhan\nsu", "nhan\u200Bsu", "nhan su'; DROP TABLE dbo.PhongBan;--" })
                Reject(() => bus.HienThiTatCa(keyword), "Từ khóa tìm kiếm");
            Assert(bus.HienThiTatCa("123").Count == 0, "Numeric search was rejected.");
            Assert(bus.HienThiTatCa("   ").Count == 1 && bus.HienThiTatCa(null!).Count == 1,
                "Empty keyword no longer displays all departments.");
            bus.Luu(boundary);
            Assert(bus.HienThiTheoId(boundary.MaPhongBan).MoTa.Length == 500, "Boundary round trip failed.");
            Reject(() => bus.Luu(new PhongBan { MaPhongBan = department.MaPhongBan,
                TenPhongBan = boundary.TenPhongBan }), "đã tồn tại");
            bus.Luu(department);
            Assert(bus.HienThiTheoId(department.MaPhongBan).TenPhongBan == "Phòng đã sửa",
                "An unchanged name was rejected or a duplicate edit changed SQL data.");

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
                SetText(invalid, "txtTenPhongBan", department.TenPhongBan);
                Click(invalid, "btnLuu");
                Assert(invalid.IsVisible && Field<TextBlock>(invalid, "txtThongBao").Text.Contains("đã tồn tại"),
                    "Duplicate add did not keep the form open with an actionable error.");
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
            Assert(Field<Button>(detail, "btnPhongBanXoa").IsEnabled, "Authorized delete is disabled.");
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
            SetText(detail, "txtTenPhongBan", department.TenPhongBan);
            Click(detail, "btnLuuSua");
            Assert(Field<TextBlock>(detail, "txtThongBao").Text.Contains("đã tồn tại") &&
                bus.HienThiTheoId(added.MaPhongBan).TenPhongBan == "Tạo từ form", "Duplicate edit reached SQL.");
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
            SetText(view, "txtTimPhongBan", "  DA SUA TU CHI TIET  ");
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 1, "Accent-insensitive name search did not reload SQL data.");
            Field<ComboBox>(view, "cboLocPhongBan").SelectedIndex = 1;
            Assert(grid.Items.Count == 0, "Active filter did not apply immediately.");
            Field<ComboBox>(view, "cboLocPhongBan").SelectedIndex = 2;
            Assert(grid.Items.Count == 1, "Inactive filter did not apply immediately or lost the keyword.");
            SetText(view, "txtTimPhongBan", "  dA CAP NHAT  ");
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 1, "Description search did not trim or ignore accents/case.");
            var search = Field<TextBox>(view, "txtTimPhongBan");
            var searchError = Field<TextBlock>(view, "txtLoiTimPhongBan");
            search.Select(2, 2);
            foreach (string input in new[] { "@", "#", "%", "_", "-", ".", "'", "😀", "\t", "\n" })
            {
                var typing = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                    new TextComposition(InputManager.Current, search, input))
                    { RoutedEvent = TextCompositionManager.PreviewTextInputEvent };
                search.RaiseEvent(typing);
                Assert(typing.Handled && search.Text == "  dA CAP NHAT  " && search.SelectionStart == 2 &&
                    search.SelectionLength == 2 && searchError.Visibility == Visibility.Visible && grid.Items.Count == 1,
                    "Invalid typing was not blocked immediately without changing text/selection/results: " + input);
            }
            Assert(search.BorderBrush == search.FindResource("DangerBrush"), "Invalid input did not highlight the field.");
            view.UpdateLayout();
            var searchPosition = search.TranslatePoint(new Point(), view);
            var errorPosition = searchError.TranslatePoint(new Point(), view);
            var searchButton = Field<Button>(view, "btnTimPhongBan");
            Assert(Math.Abs(errorPosition.X - searchPosition.X) < 1 &&
                errorPosition.Y >= searchPosition.Y + search.ActualHeight && searchError.ActualHeight > 0 &&
                Math.Abs(searchButton.TranslatePoint(new Point(), view).Y - searchPosition.Y) < 1 &&
                searchButton.ActualHeight < search.ActualHeight + searchError.ActualHeight,
                "Inline error is misplaced or stretches toolbar buttons.");
            if (Environment.GetEnvironmentVariable("QLKS_CHECK_SCREENSHOT") is string screenshotPath)
            {
                var screenshot = new RenderTargetBitmap((int)Math.Ceiling(view.ActualWidth),
                    (int)Math.Ceiling(view.ActualHeight), 96, 96, PixelFormats.Pbgra32);
                screenshot.Render(view);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(screenshot));
                using var output = File.Create(screenshotPath);
                encoder.Save(output);
            }
            foreach (string input in new[] { "nhân sự@", "le-tan", "😀", "\u0301", "nhan\nsu" })
            {
                var pasting = new DataObjectPastingEventArgs(new DataObject(DataFormats.UnicodeText, input),
                    false, DataFormats.UnicodeText);
                search.RaiseEvent(pasting);
                Assert(pasting.CommandCancelled && search.Text == "  dA CAP NHAT  " &&
                    search.SelectionStart == 2 && search.SelectionLength == 2 && searchError.Visibility == Visibility.Visible,
                    "Invalid paste was not blocked without losing the selection: " + input);
            }
            search.SelectAll();
            foreach (string input in new[] { "Nhân sự", "nhan su", "Đào tạo 123", "Nhân sự".Normalize(System.Text.NormalizationForm.FormD) })
            {
                var typing = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                    new TextComposition(InputManager.Current, search, input))
                    { RoutedEvent = TextCompositionManager.PreviewTextInputEvent };
                search.RaiseEvent(typing);
                var pasting = new DataObjectPastingEventArgs(new DataObject(DataFormats.UnicodeText, input),
                    false, DataFormats.UnicodeText);
                search.RaiseEvent(pasting);
                Assert(!typing.Handled && !pasting.CommandCancelled && searchError.Visibility == Visibility.Collapsed &&
                    search.BorderBrush == search.FindResource("BorderBrush"),
                    "Valid Vietnamese input/paste was blocked or did not clear the error: " + input);
            }
            SetText(view, "txtTimPhongBan", "nhan su@");
            Assert(search.Text == "  dA CAP NHAT  " && searchError.Visibility == Visibility.Visible && grid.Items.Count == 1,
                "TextChanged fallback did not restore valid text and display the inline error.");
            SetText(view, "txtTimPhongBan", "nhan su");
            Assert(searchError.Visibility == Visibility.Collapsed, "Valid editing did not clear the inline error.");
            SetText(view, "txtTimPhongBan", "PhongBanKhongTonTai");
            Click(view, "btnTimPhongBan");
            Assert(grid.Items.Count == 0 && Field<TextBlock>(view, "txtThongBaoPhongBan").Text.Contains("0 dòng"),
                "No-match search did not show an empty result.");
            Click(view, "btnMoiPhongBan");
            Assert(grid.Items.Count == 3 && Field<TextBox>(view, "txtTimPhongBan").Text == "" &&
                Field<ComboBox>(view, "cboLocPhongBan").SelectedIndex == 0, "Refresh did not reset both filters.");
            var emptyTyping = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                new TextComposition(InputManager.Current, search, "@"))
                { RoutedEvent = TextCompositionManager.PreviewTextInputEvent };
            search.RaiseEvent(emptyTyping);
            Assert(emptyTyping.Handled && search.Text == "" && searchError.Visibility == Visibility.Visible,
                "Invalid first character was not rejected with an inline error.");
            Click(view, "btnMoiPhongBan");
            Assert(searchError.Visibility == Visibility.Collapsed && grid.Items.Count == 3,
                "Refresh did not clear the error when the input was already empty.");

            Execute(testConnection, $"INSERT INTO dbo.NhanVien(MaPhongBan) VALUES ({added.MaPhongBan})");
            Reject(() => bus.Xoa(added.MaPhongBan), "nhân viên");
            grid.SelectedIndex = 0;
            grid.UpdateLayout();
            var target = grid.Items.Cast<PhongBan>().Single(x => x.MaPhongBan == added.MaPhongBan);
            var clickedRow = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(target);
            Exception? deleteFailure = null;
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                PhongBanChiTietWindow? deleting = null;
                try
                {
                    deleting = app.Windows.OfType<PhongBanChiTietWindow>().Single();
                    deleting.Opacity = 0;
                    Assert(Field<TextBox>(deleting, "txtMa").Text == added.MaPhongBan.ToString(),
                        "Double-click opened the selected row instead of the clicked row.");
                    ConfirmDelete(deleting, false);
                    Assert(deleting.IsVisible && bus.HienThiTatCa().Count == 3, "Cancel deleted the department.");
                    ConfirmDelete(deleting, true);
                    Assert(deleting.IsVisible && Field<TextBlock>(deleting, "txtThongBao").Text.Contains("nhân viên") &&
                        bus.HienThiTatCa().Count == 3, "Referenced department was deleted or the error closed the detail.");
                    Execute(testConnection, "DELETE FROM dbo.NhanVien");
                    ConfirmDelete(deleting, true);
                    Assert(!deleting.IsVisible, "Successful deletion did not close the detail.");
                }
                catch (Exception ex) { deleteFailure = ex; }
                finally { if (deleting?.IsVisible == true) deleting.Close(); }
            }));
            grid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                { RoutedEvent = Control.MouseDoubleClickEvent, Source = clickedRow });
            if (deleteFailure != null) throw deleteFailure;
            Assert(grid.Items.Count == 2 && grid.Items.Cast<PhongBan>().All(x => x.MaPhongBan != added.MaPhongBan) &&
                bus.HienThiTatCa().Count == 2, "Delete did not persist and refresh the SQL list.");
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
            var deniedDetail = new PhongBanChiTietWindow(department);
            CheckForm(deniedDetail, () =>
            {
                Assert(!Field<Button>(deniedDetail, "btnPhongBanXoa").IsEnabled &&
                    !Field<Button>(deniedDetail, "btnPhongBanSuaThongTin").IsEnabled, "Detail role restrictions missing.");
                Click(deniedDetail, "btnPhongBanXoa");
                Click(deniedDetail, "btnPhongBanSuaThongTin");
                Assert(!app.Windows.OfType<XacNhanDemoWindow>().Any() &&
                    Field<TextBox>(deniedDetail, "txtTenPhongBan").IsReadOnly, "Denied action opened a modal or edit mode.");
            });
            Assert(bus.HienThiTatCa().Count == 2, "Unauthorized save/delete reached SQL.");
            DemoSession.SignIn("admin", "admin123");
            Execute(testConnection, "DROP TABLE dbo.NhanVien");
            Execute(testConnection, "DROP TABLE dbo.PhongBan");
            Reject(() => bus.HienThiTatCa("@"), "Từ khóa tìm kiếm");
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
