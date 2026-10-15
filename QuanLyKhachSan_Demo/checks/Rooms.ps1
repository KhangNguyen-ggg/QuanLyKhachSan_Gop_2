# Run: powershell -ExecutionPolicy Bypass -File checks/Rooms.ps1
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$checkRoot = Join-Path ([IO.Path]::GetTempPath()) ('HotelRooms-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkRoot | Out-Null
$appOutput = Join-Path $checkRoot 'app'
dotnet build (Join-Path $projectRoot 'QuanLyKhachSan.csproj') --no-restore -o $appOutput
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0-windows</TargetFramework><UseWPF>true</UseWPF><ImplicitUsings>enable</ImplicitUsings></PropertyGroup>
  <ItemGroup><ProjectReference Include="$projectRoot/QuanLyKhachSan.csproj" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $checkRoot 'Check.csproj') -Encoding UTF8
@'
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using QuanLyKhachSan;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Views;
using QuanLyKhachSan.Views.Dialogs;

class Check
{
    static T Field<T>(FrameworkElement form, string name) => (T)form.FindName(name);
    static void Assert(bool ok, string message) { if (!ok) throw new Exception(message); }
    static void Click(FrameworkElement form, string name) => Field<Button>(form, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    static IEnumerable<DependencyObject> Visuals(DependencyObject root)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i); yield return child;
            foreach (var nested in Visuals(child)) yield return nested;
        }
    }
    static void Snapshot(Window window, string file)
    {
        window.UpdateLayout();
        var bmp = new RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, 96, 96, PixelFormats.Pbgra32); bmp.Render(window);
        var png = new PngBitmapEncoder(); png.Frames.Add(BitmapFrame.Create(bmp));
        using var output = System.IO.File.Create(file); png.Save(output);
    }
    static void Select(FrameworkElement window, string name, string value)
    {
        var combo = Field<ComboBox>(window, name);
        combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().Single(i => Equals(i.Content, value));
    }
    [STAThread]
    static void Main(string[] args)
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/QuanLyKhachSan;component/Resources/Styles.xaml", UriKind.Relative) });
        DemoSession.SignIn("admin", "admin123");
        var screen = new PhongView();
        var host = new Window { Content = screen, Width = 1120, Height = 720, ShowInTaskbar = false, Left = -10000, Top = -10000 };
        host.Show(); host.UpdateLayout();
        var list = Field<ListBox>(screen, "lstPhong");
        var rooms = ((IEnumerable<PhongMau>)list.ItemsSource).ToArray();
        Assert(list.Items.Count == 3 && rooms.Select(r => r.SoPhong).SequenceEqual(new[] { "201", "202", "301" }), "Room inventory changed.");
        Assert(!Visuals(screen).OfType<TabControl>().Any(), "Merged view still has separate room/status tabs.");
        Assert(CollectionViewSource.GetDefaultView(list.ItemsSource).Groups.Count == 2, "Floor grouping failed.");
        Assert(Field<TextBlock>(screen, "txtPhongDangO").Text.EndsWith("2") && Field<TextBlock>(screen, "txtPhongBaoTri").Text.EndsWith("1"), "Incorrect room counts.");
        foreach (var size in new[] { (1120d,720d), (800d,720d), (800d,560d) })
        {
            host.Width = size.Item1; host.Height = size.Item2; host.UpdateLayout();
            Assert(Field<Border>(screen, "pnlThongTinPhong").ActualWidth == 280, "Room info panel lost its width.");
            Assert(Visuals(list).OfType<ListBoxItem>().Count() == 3, "Room cards disappeared.");
            var reset = Field<Button>(screen, "btnMoiPhong");
            Assert(reset.TranslatePoint(new Point(reset.ActualWidth, 0), screen).X <= screen.ActualWidth, "Refresh button is clipped.");
            if (size.Item1 == 1120)
            {
                var cards = Visuals(list).OfType<ListBoxItem>().ToArray();
                var first = cards.Single(c => c.Content == rooms[0]).TranslatePoint(new Point(), list);
                var second = cards.Single(c => c.Content == rooms[1]).TranslatePoint(new Point(), list);
                var nextFloor = cards.Single(c => c.Content == rooms[2]).TranslatePoint(new Point(), list);
                Assert(Math.Abs(first.Y - second.Y) < 1 && nextFloor.Y > first.Y, "Floor groups did not arrange rooms in rows.");
            }
            Snapshot(host, System.IO.Path.Combine(args[0], $"rooms-{size.Item1}-{size.Item2}.png"));
        }
        host.Width = 1120; host.Height = 720;
        list.SelectedItem = rooms[1]; host.UpdateLayout();
        Assert(ReferenceEquals(Field<Border>(screen, "pnlThongTinPhong").DataContext, rooms[1]) && Field<TextBlock>(screen, "txtSoPhongChon").Text == "202", "Single-click did not select room details.");

        Field<TextBox>(screen, "txtTimPhong").Text = " suite "; Click(screen, "btnTimPhong"); host.UpdateLayout();
        Assert(list.Items.Count == 1 && list.Items[0] == rooms[2], "Search by room type failed.");
        Click(screen, "btnMoiPhong");
        Select(screen, "cboLocPhong", "Đang ở"); Select(screen, "cboLocVeSinh", "Cần dọn"); host.UpdateLayout();
        Assert(list.Items.Count == 1 && list.Items[0] == rooms[1], "Combined room and cleaning filter failed.");
        Select(screen, "cboLocPhong", "Trống"); host.UpdateLayout();
        Assert(list.Items.Count == 0 && list.SelectedItem == null && Field<TextBlock>(screen, "txtKhongCoPhong").Visibility == Visibility.Visible && !Field<Button>(screen, "btnPhongChiTiet").IsEnabled, "Empty result kept stale details.");
        Snapshot(host, System.IO.Path.Combine(args[0], "rooms-empty.png"));
        void Later(Action action) => app.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        Later(() => { var create = app.Windows.OfType<PhongForm>().Single(); Assert(create.Owner == host, "Create form lost owner."); create.Close(); });
        Click(screen, "btnPhongThemMoi");
        Click(screen, "btnMoiPhong"); host.UpdateLayout();
        Assert(list.Items.Count == 3 && Field<ComboBox>(screen, "cboLocVeSinh").SelectedIndex == 0, "Reset filters failed.");

        void OpenRoom(PhongMau target, Action<PhongChiTietWindow> check)
        {
            list.SelectedItem = rooms[0]; host.UpdateLayout();
            var card = Visuals(list).OfType<ListBoxItem>().Single(i => ReferenceEquals(i.Content, target));
            Later(() => {
                var detail = app.Windows.OfType<PhongChiTietWindow>().Single();
                Assert(ReferenceEquals(detail.DataContext, target) && detail.Owner == host, "Double-click opened selected room instead of clicked room.");
                check(detail);
                if (detail.IsVisible) detail.Close();
            });
            list.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = card });
            host.UpdateLayout();
        }
        OpenRoom(rooms[1], detail => {
            Snapshot(detail, System.IO.Path.Combine(args[0], "room-detail.png"));
            Later(() => {
                var status = app.Windows.OfType<TinhTrangPhongForm>().Single();
                Assert(status.Owner == detail && status.DataContext == rooms[1] && !Field<ComboBox>(status, "cboPhong").IsEnabled, "Status form lost target.");
                Field<ComboBox>(status, "cboTinhTrang").SelectedIndex = -1; Click(status, "btnLuu");
                Assert(status.IsVisible && rooms[1].TinhTrang == "Đang ở", "Invalid status mutated room.");
                Select(status, "cboTinhTrang", "Trống"); Select(status, "cboVeSinh", "Sạch");
                Snapshot(status, System.IO.Path.Combine(args[0], "room-status.png"));
                Click(status, "btnLuu");
            });
            Click(detail, "btnTinhTrangPhongCapNhatTinhTrang");
            Assert(Field<TextBlock>(detail, "txtTinhTrang").Text == "Trống", "Detail status did not refresh.");
        });
        Assert(rooms[1].TinhTrang == "Trống" && rooms[1].VeSinh == "Sạch" && rooms[0].TinhTrang == "Đang ở"
            && Field<TextBlock>(screen, "txtPhongTrong").Text.EndsWith("1") && Field<TextBlock>(screen, "txtPhongCanDon").Text.EndsWith("0"), "Board did not refresh edited status.");
        OpenRoom(rooms[2], detail => {
            Later(() => {
                var edit = app.Windows.OfType<PhongForm>().Single();
                Assert(edit.Owner == detail && Field<TextBox>(edit, "txtSoPhong").Text == "301" && Field<TextBox>(edit, "txtSoPhong").IsReadOnly, "Edit form loaded wrong room.");
                Field<ComboBox>(edit, "cboLoaiPhong").SelectedIndex = -1; Click(edit, "btnLuu");
                Assert(edit.IsVisible && rooms[2].LoaiPhong == "Suite", "Invalid metadata mutated room.");
                Select(edit, "cboLoaiPhong", "Standard");
                Select(edit, "cboKhaiThac", "Đang hoạt động");
                Click(edit, "btnLuu");
            });
            Click(detail, "btnPhongSuaThongTin");
        });
        Assert(rooms[2].LoaiPhong == "Standard" && rooms[2].TinhTrang == "Chưa cập nhật" && rooms[2].KhaiThac == "Đang hoạt động", "Leaving maintenance assumed room was available.");
        OpenRoom(rooms[2], detail => {
            Later(() => {
                var edit = app.Windows.OfType<PhongForm>().Single();
                Select(edit, "cboKhaiThac", "Ngừng khai thác");
                Click(edit, "btnLuu");
            });
            Click(detail, "btnPhongSuaThongTin");
        });
        Assert(rooms[2].TinhTrang == "Ngừng khai thác" && rooms[2].KhaiThac == "Ngừng khai thác", "Inactive room retained an available state.");
        OpenRoom(rooms[1], detail => {
            Later(() => { var edit = app.Windows.OfType<TinhTrangPhongForm>().Single(); Select(edit, "cboTinhTrang", "Bảo trì"); edit.Close(); });
            Click(detail, "btnTinhTrangPhongCapNhatTinhTrang");
            Assert(rooms[1].TinhTrang == "Trống", "Canceled status changed source.");
            Later(() => { var confirm = app.Windows.OfType<XacNhanDemoWindow>().Single(); Assert(Field<TextBlock>(confirm, "txtNoiDung").Text.Contains("202"), "Delete lost room identity."); confirm.Close(); });
            Click(detail, "btnPhongXoa");
            Assert(detail.IsVisible, "Canceled delete removed room.");
            Later(() => Click(app.Windows.OfType<XacNhanDemoWindow>().Single(), "btnLuu"));
            Click(detail, "btnPhongXoa");
        });
        Assert(list.Items.Count == 2 && list.Items.Contains(rooms[0]) && !list.Items.Contains(rooms[1]), "Delete removed wrong room.");
        list.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = list });
        Assert(!app.Windows.OfType<PhongChiTietWindow>().Any(), "Blank board opened a detail.");
        host.Close();
        foreach (var account in DemoSession.Accounts)
        {
            DemoSession.SignIn(account.Username, account.Password);
            var detail = new PhongChiTietWindow(rooms[0]);
            foreach (var name in new[] { "btnPhongSuaThongTin", "btnPhongXoa", "btnTinhTrangPhongCapNhatTinhTrang" })
            {
                var allowed = DemoSession.CanUseButton("PhongView", name);
                Assert(Field<Button>(detail, name).IsEnabled == allowed, "Room permission changed.");
                if (!allowed)
                {
                    var count = app.Windows.Count; Click(detail, name); Assert(app.Windows.Count == count, "Denied room action opened dialog.");
                }
            }
            detail.Close();
            foreach (var edit in new Window[] { new PhongForm(rooms[0]), new TinhTrangPhongForm(rooms[0]) })
            {
                var name = edit is PhongForm ? "btnPhongSuaThongTin" : "btnTinhTrangPhongCapNhatTinhTrang";
                Assert(Field<Button>(edit, "btnLuu").IsEnabled == DemoSession.CanUseButton("PhongView", name), "Room save permission changed.");
                edit.Close();
            }
        }
        app.Shutdown();
        Console.WriteLine("PASS: unified room board, layouts, filtering, selection, status/edit refresh, validation, cancellation, deletion and permissions.");
    }
}
'@ | Set-Content (Join-Path $checkRoot 'Program.cs') -Encoding UTF8
dotnet run --project (Join-Path $checkRoot 'Check.csproj') -- $checkRoot
if ($LASTEXITCODE -ne 0) { throw 'Room view checks failed.' }
Write-Output "Screenshots: $checkRoot"
