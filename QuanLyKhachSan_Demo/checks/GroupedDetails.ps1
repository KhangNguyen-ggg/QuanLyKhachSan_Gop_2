# Run: powershell -ExecutionPolicy Bypass -File checks/GroupedDetails.ps1
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$checkRoot = Join-Path ([IO.Path]::GetTempPath()) ('HotelGrouped-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkRoot | Out-Null
$appOutput = Join-Path $checkRoot 'app'
dotnet build (Join-Path $projectRoot 'QuanLyKhachSan.csproj') --no-restore -o $appOutput
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0-windows</TargetFramework><UseWPF>true</UseWPF><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
  <ItemGroup><ProjectReference Include="$projectRoot/QuanLyKhachSan.csproj" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $checkRoot 'Check.csproj') -Encoding UTF8
@'
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using QuanLyKhachSan;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Views;
using QuanLyKhachSan.Views.Dialogs;

record Relation(Type View, string ParentGrid, Type ParentDialog, string Collection, string ParentName,
    string Child, Type ChildDialog, string ParentKey, string Key, string NewKey, string RenameButton);
class Check
{
    static Application App = null!;
    static T Field<T>(FrameworkElement view, string name) => (T)view.FindName(name);
    static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    static void Input(Window window, string name, string value) => Field<TextBox>(window, name).SetCurrentValue(TextBox.TextProperty, value);
    static void Click(FrameworkElement view, string name) => Field<Button>(view, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    static string Value(object item, string property) => (string)item.GetType().GetProperty(property)!.GetValue(item)!;
    static IList Children(object item, string property) => (IList)item.GetType().GetProperty(property)!.GetValue(item)!;
    static Window Host(UserControl view) => new() { Content = view, Width = 1120, Height = 720, Left = -10000, Top = -10000, ShowInTaskbar = false };
    static void Later(Action action) => App.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
    static Window Nested(Window owner, Type type) => App.Windows.Cast<Window>().Single(w => w.Owner == owner && w.GetType() == type);
    static void Confirm(Window owner, string action, bool accept)
    {
        Later(() => {
            var dialog = App.Windows.Cast<Window>().SingleOrDefault(w => w.Owner == owner && w is XacNhanDemoWindow);
            if (dialog == null) throw new Exception($"No confirmation for {owner.GetType().Name}/{action}: {(owner.FindName("txtThongBao") as TextBlock)?.Text}");
            Assert(!string.IsNullOrWhiteSpace(Field<TextBlock>(dialog, "txtNoiDung").Text), "Empty confirmation.");
            if (accept) Click(dialog, "btnLuu"); else dialog.Close();
        });
        Click(owner, action);
    }
    static void DoubleClick(DataGrid grid, int index)
    {
        grid.ScrollIntoView(grid.Items[index]); grid.UpdateLayout();
        var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
        grid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = row });
    }
    static void Snapshot(Window host, string file)
    {
        host.UpdateLayout();
        host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
        host.UpdateLayout();
        var image = new RenderTargetBitmap((int)host.ActualWidth, (int)host.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        image.Render(host);
        var png = new PngBitmapEncoder(); png.Frames.Add(BitmapFrame.Create(image));
        using var output = System.IO.File.Create(file); png.Save(output);
    }
    [STAThread]
    static void Main(string[] args)
    {
        App = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        App.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/QuanLyKhachSan;component/Resources/Styles.xaml", UriKind.Relative) });
        DemoSession.SignIn("admin", "admin123");
        Relation[] cases = {
            new(typeof(DanhMucPhongView), "dgvLoaiPhong", typeof(LoaiPhongChiTietWindow), "TienNghis", "TenLoai", "TienNghiTheoLoai", typeof(TienNghiTheoLoaiChiTietWindow), "LoaiPhong", "TienNghi", "Máy sấy tóc", "btnLoaiPhongSuaThongTin"),
            new(typeof(GoiUuDaiView), "dgvGoiDichVu", typeof(GoiDichVuChiTietWindow), "ThanhPhans", "TenGoi", "ThanhPhanGoi", typeof(ThanhPhanGoiChiTietWindow), "Goi", "DichVu", "Giặt ủi", "btnGoiDichVuSuaThongTin"),
            new(typeof(KhoView), "dgvPhieuNhapXuat", typeof(PhieuNhapXuatChiTietWindow), "ChiTiets", "MaPhieu", "ChiTietPhieuKho", typeof(ChiTietPhieuKhoChiTietWindow), "Phieu", "VatTu", "Khăn mặt", "btnPhieuNhapXuatSuaPhieu"),
            new(typeof(VatTuNhaCungCapView), "dgvNhaCungCap", typeof(NhaCungCapChiTietWindow), "VatTus", "TenNhaCungCap", "VatTuNhaCungCap", typeof(VatTuNhaCungCapChiTietWindow), "NhaCungCap", "VatTu", "Dầu gội 30 ml", "btnNhaCungCapSuaThongTin")
        };
        foreach (var test in cases)
        {
            var view = (UserControl)Activator.CreateInstance(test.View)!;
            var table = Field<DataGrid>(view, test.ParentGrid);
            var parent = table.Items[0]; var other = table.Items[1];
            var children = Children(parent, test.Collection); var otherChildren = Children(other, test.Collection);
            var otherKeys = otherChildren.Cast<object>().Select(c => Value(c, test.Key)).ToArray();
            Assert(children.Count > 0, "Existing child fixtures lost.");
            var first = children[0]!; var originalKey = Value(first, test.Key); var before = children.Count;
            var detail = (Window)Activator.CreateInstance(test.ParentDialog, parent)!;
            detail.WindowStartupLocation = WindowStartupLocation.Manual; detail.Left = detail.Top = -10000; detail.Show();
            Field<TabControl>(detail, "tabChiTiet").SelectedItem = Field<TabItem>(detail, "tabLienKet"); detail.UpdateLayout();
            var grid = Field<DataGrid>(detail, "dgv" + test.Child);
            Assert(ReferenceEquals(grid.ItemsSource, children) && grid.Items.Count == before, "Detail uses copied or unrelated child rows.");
            foreach (var width in new[] { 760d, 540d }) { detail.Width = width; detail.UpdateLayout(); Snapshot(detail, System.IO.Path.Combine(args[0], test.Child + "-" + width + ".png")); }
            detail.Width = 760;
            Later(() => {
                var draft = Nested(detail, test.ChildDialog);
                Assert(Field<TextBox>(draft, "txt" + test.ParentKey).IsReadOnly && Value(draft.DataContext, test.ParentKey) == Value(parent, test.ParentName), "New row lost parent context.");
                Input(draft, "txt" + test.Key, test.NewKey); draft.Close();
            });
            Click(detail, "btn" + test.Child + "ThemMoi");
            Assert(children.Count == before, "Canceled addition persisted.");
            Later(() => {
                var draft = Nested(detail, test.ChildDialog);
                Click(draft, "btnLuuSua"); Assert(draft.IsVisible && children.Count == before, "Empty new row persisted.");
                Input(draft, "txt" + test.Key, originalKey); Click(draft, "btnLuuSua");
                Assert(draft.IsVisible && children.Count == before, "Duplicate new row persisted.");
                Input(draft, "txt" + test.Key, test.NewKey);
                Confirm(draft, "btnLuuSua", true);
            });
            Click(detail, "btn" + test.Child + "ThemMoi");
            Assert(children.Count == before + 1 && grid.Items.Count == before + 1, "Added row did not reach parent's collection.");
            var target = children[before]!; grid.SelectedIndex = 0; detail.UpdateLayout();
            Later(() => {
                var editor = Nested(detail, test.ChildDialog);
                Assert(ReferenceEquals(editor.DataContext, target), "Nested double-click followed selected row instead of clicked row.");
                Click(editor, "btn" + test.Child + "SuaThongTin");
                Assert(Field<TextBox>(editor, "txt" + test.ParentKey).IsReadOnly, "Parent key is editable.");
                Input(editor, "txt" + test.ParentKey, "Sai mục cha"); Click(editor, "btnLuuSua");
                Assert(Value(target, test.ParentKey) == Value(parent, test.ParentName), "Locked parent changed.");
                Input(editor, "txt" + test.ParentKey, Value(parent, test.ParentName));
                Input(editor, "txt" + test.Key, originalKey); Click(editor, "btnLuuSua");
                Assert(Value(target, test.Key) == test.NewKey, "Duplicate edit changed source.");
                Click(editor, "btnHuySua"); Assert(Field<TextBox>(editor, "txt" + test.Key).Text == test.NewKey, "Canceled edit did not restore fields.");
                Click(editor, "btn" + test.Child + "SuaThongTin"); Input(editor, "txt" + test.Key, test.NewKey + " sửa"); Confirm(editor, "btnLuuSua", true);
                Assert(Value(target, test.Key) == test.NewKey + " sửa", "Nested save did not persist.");
                Confirm(editor, "btn" + test.Child + "Xoa", false); Assert(children.Contains(target), "Canceled child delete persisted.");
                Confirm(editor, "btn" + test.Child + "Xoa", true);
            });
            DoubleClick(grid, before);
            Assert(children.Count == before && !children.Contains(target), "Child deletion missed target.");
            Assert(Value(first, test.Key) == originalKey && otherChildren.Cast<object>().Select(c => Value(c, test.Key)).SequenceEqual(otherKeys), "Child action changed sibling or another parent.");
            grid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = grid });
            Assert(!App.Windows.Cast<Window>().Any(w => w.Owner == detail), "Blank related grid opened editor.");
            if (test.ParentName != "MaPhieu")
            {
                Click(detail, test.RenameButton); Input(detail, "txt" + test.ParentName, Value(parent, test.ParentName) + " mới"); Confirm(detail, "btnLuuSua", true);
                Assert(children.Cast<object>().All(child => Value(child, test.ParentKey) == Value(parent, test.ParentName)), "Parent rename lost relationships.");
            }
            detail.Close();
            var reopened = (Window)Activator.CreateInstance(test.ParentDialog, parent)!;
            Assert(ReferenceEquals(Field<DataGrid>(reopened, "dgv" + test.Child).ItemsSource, children), "Reopening recreated child data."); reopened.Close();
            if (otherChildren.Count == 0)
            {
                var empty = (Window)Activator.CreateInstance(test.ParentDialog, other)!;
                empty.WindowStartupLocation = WindowStartupLocation.Manual; empty.Left = empty.Top = -10000; empty.Show();
                Field<TabControl>(empty, "tabChiTiet").SelectedItem = Field<TabItem>(empty, "tabLienKet"); empty.UpdateLayout();
                Later(() => {
                    var draft = Nested(empty, test.ChildDialog); Input(draft, "txt" + test.Key, test.NewKey); Confirm(draft, "btnLuuSua", true);
                });
                Click(empty, "btn" + test.Child + "ThemMoi"); Assert(otherChildren.Count == 1, "Empty related list cannot add its first row.");
                Later(() => Confirm(Nested(empty, test.ChildDialog), "btn" + test.Child + "Xoa", true));
                DoubleClick(Field<DataGrid>(empty, "dgv" + test.Child), 0); Assert(otherChildren.Count == 0, "Empty-list check left test data."); empty.Close();
            }
        }
        var structure = new DanhMucPhongView(); var structureHost = Host(structure); structureHost.Show();
        var areas = Field<DataGrid>(structure, "dgvKhuVuc"); var floors = Field<DataGrid>(structure, "dgvTang");
        var a = (KhuVucMau)areas.Items[0]; var b = (KhuVucMau)areas.Items[1];
        Assert(ReferenceEquals(floors.ItemsSource, a.Tangs) && floors.Items.Count == 2, "Default area lost floors.");
        areas.SelectedItem = b; Assert(floors.Items.Count == 0, "Changing area retained previous floors.");
        Later(() => {
            var editor = Nested(structureHost, typeof(TangChiTietWindow));
            Assert(Field<TextBox>(editor, "txtKhuVuc").IsReadOnly, "New floor allows wrong area.");
            Input(editor, "txtMa", "T02"); Input(editor, "txtTenTang", "Tầng 1"); Input(editor, "txtSoTang", "1"); Click(editor, "btnLuuSua");
            Assert(editor.IsVisible && b.Tangs.Count == 0, "Global floor identifier duplicate accepted.");
            Input(editor, "txtMa", "TB01"); Confirm(editor, "btnLuuSua", true);
        });
        Click(structure, "btnTangThemMoi"); Assert(b.Tangs.Count == 1 && a.Tangs.Count == 2, "Floor added to wrong area.");
        areas.SelectedItem = a; Assert(floors.Items.Count == 2, "Returning to area lost floors.");
        foreach (var width in new[] { 1120d, 800d }) { structureHost.Width = width; structureHost.UpdateLayout(); Snapshot(structureHost, System.IO.Path.Combine(args[0], "structure-" + width + ".png")); }
        Field<TextBox>(structure, "txtTimKhuVuc").Text = "không có"; Click(structure, "btnTimKhuVuc");
        Assert(areas.Items.Count == 0 && floors.Items.Count == 0 && !Field<Button>(structure, "btnTangThemMoi").IsEnabled, "Empty area search keeps another area's floors.");
        Click(structure, "btnMoiKhuVuc"); Assert(areas.Items.Count == 2, "Refresh did not restore areas."); structureHost.Close();

        // Lễ tân/phân bổ SQL được kiểm tra trong checks/SqlIntegration.
        foreach (var account in DemoSession.Accounts)
        {
            DemoSession.SignIn(account.Username, account.Password);
            foreach (var test in cases)
            {
                var parentView = (UserControl)Activator.CreateInstance(test.View)!;
                var parent = Field<DataGrid>(parentView, test.ParentGrid).Items[0];
                var detail = (Window)Activator.CreateInstance(test.ParentDialog, parent)!;
                var add = Field<Button>(detail, "btn" + test.Child + "ThemMoi");
                Assert(add.IsEnabled == DemoSession.CanUseButton(test.View.Name, add.Name), "Nested create permission changed.");
                if (!add.IsEnabled) { var count = App.Windows.Count; Click(detail, add.Name); Assert(App.Windows.Count == count, "Denied role opened child creation."); }
                detail.Close();
            }
        }
        App.Shutdown(); Console.WriteLine("PASS: four parent/child workflows, preserved fixtures, add/edit/cancel/delete/duplicates, area/floor context and permissions.");
    }
}
'@ | Set-Content (Join-Path $checkRoot 'Program.cs') -Encoding UTF8
dotnet run --project (Join-Path $checkRoot 'Check.csproj') -- $checkRoot
if ($LASTEXITCODE -ne 0) { throw 'Grouped detail checks failed.' }
Write-Output "Screenshots: $checkRoot"
