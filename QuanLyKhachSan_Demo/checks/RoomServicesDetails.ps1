# Run: powershell -ExecutionPolicy Bypass -File checks/RoomServicesDetails.ps1
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$checkRoot = Join-Path ([IO.Path]::GetTempPath()) ('HotelRoomServices-' + [Guid]::NewGuid().ToString('N'))
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

record RowCase(Type View, string Tab, string Grid, Type Detail, string[] Fields, string[] Actions, string? Create, bool Edit, bool Delete);
class Check
{
    static T Field<T>(FrameworkElement form, string name) => (T)form.FindName(name);
    static void Assert(bool ok, string message) { if (!ok) throw new Exception(message); }
    static void Click(Window form, string name) => Field<Button>(form, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    static void Input(Window form, string name, string value) => Field<TextBox>(form, name).SetCurrentValue(TextBox.TextProperty, value);
    static string Value(object item, string prop) => (string)item.GetType().GetProperty(prop)!.GetValue(item)!;
    static Window Host(UserControl view) => new() { Content = view, Width = 1120, Height = 720, ShowInTaskbar = false, Left = -10000, Top = -10000 };
    static void Snapshot(Window host, string file)
    {
        host.UpdateLayout();
        var bitmap = new RenderTargetBitmap((int)host.ActualWidth, (int)host.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(host);
        var png = new PngBitmapEncoder();
        png.Frames.Add(BitmapFrame.Create(bitmap));
        using var output = System.IO.File.Create(file);
        png.Save(output);
    }
    [STAThread]
    static void Main(string[] args)
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/QuanLyKhachSan;component/Resources/Styles.xaml", UriKind.Relative) });
        DemoSession.SignIn("admin", "admin123");
        RowCase[] rows = {
            new(typeof(BuongPhongView), "tabDonPhong", "dgvDonPhong", typeof(DonPhongChiTietWindow), new[] { "Ma", "Phong", "NhanVien", "NgayGiao", "LoaiViec", "TrangThai" }, new[] { "btnDonPhongThemCongViec", "btnDonPhongPhanCong", "btnDonPhongBatDauDon", "btnDonPhongHoanThanhDon" }, "btnDonPhongThemCongViec", false, false),
            new(typeof(BuongPhongView), "tabBaoTri", "dgvBaoTri", typeof(BaoTriChiTietWindow), new[] { "Ma", "Phong", "ThietBi", "NoiDung", "MucDo", "NguoiPhuTrach", "TrangThai" }, new[] { "btnBaoTriTaoYeuCau", "btnBaoTriPhanCongXuLy", "btnBaoTriBatDauXuLy", "btnBaoTriHoanThanhBaoTri" }, "btnBaoTriTaoYeuCau", false, false),
            new(typeof(BuongPhongView), "tabThietBi", "dgvThietBi", typeof(ThietBiChiTietWindow), new[] { "Ma", "TenThietBi", "Phong", "NhaCungCap", "NgayMua", "HanBaoHanh", "TrangThai" }, new[] { "btnThietBiThemMoi", "btnThietBiSuaThongTin", "btnThietBiXoa" }, "btnThietBiThemMoi", true, true),
            new(typeof(DanhMucPhongView), "tabCauTrucKhachSan", "dgvKhuVuc", typeof(KhuVucChiTietWindow), new[] { "Ma", "TenKhuVuc", "MoTa", "TrangThai" }, new[] { "btnKhuVucSuaThongTin", "btnKhuVucXoa" }, "btnKhuVucThemMoi", true, true),
            new(typeof(DanhMucPhongView), "tabTang", "dgvTang", typeof(TangChiTietWindow), new[] { "Ma", "TenTang", "SoTang", "KhuVuc", "TrangThai" }, new[] { "btnTangSuaThongTin", "btnTangXoa" }, "btnTangThemMoi", true, true),
            new(typeof(DanhMucPhongView), "tabLoaiPhong", "dgvLoaiPhong", typeof(LoaiPhongChiTietWindow), new[] { "Ma", "TenLoai", "GiaDem", "NguoiLon", "TreEm", "DienTich", "TrangThai" }, new[] { "btnLoaiPhongThemMoi", "btnLoaiPhongSuaThongTin", "btnLoaiPhongXoa" }, "btnLoaiPhongThemMoi", true, true),
            new(typeof(DanhMucPhongView), "tabTienNghi", "dgvTienNghi", typeof(TienNghiChiTietWindow), new[] { "Ma", "TenTienNghi", "MoTa", "TrangThai" }, new[] { "btnTienNghiThemMoi", "btnTienNghiSuaThongTin", "btnTienNghiXoa" }, "btnTienNghiThemMoi", true, true),
            new(typeof(DanhMucPhongView), "tabTienNghiTheoLoai", "dgvTienNghiTheoLoai", typeof(TienNghiTheoLoaiChiTietWindow), new[] { "LoaiPhong", "TienNghi", "SoLuong", "GhiChu" }, new[] { "btnTienNghiTheoLoaiSuaThongTin", "btnTienNghiTheoLoaiXoa" }, "btnTienNghiTheoLoaiThemMoi", true, true),
            new(typeof(DichVuView), "tabDichVu", "dgvDichVu", typeof(DichVuChiTietWindow), new[] { "Ma", "TenDichVu", "LoaiDichVu", "DonVi", "Gia", "TrangThai" }, new[] { "btnDichVuThemMoi", "btnDichVuSuaThongTin", "btnDichVuXoa" }, "btnDichVuThemMoi", true, true),
            new(typeof(DichVuView), "tabSuDungDichVu", "dgvSuDungDichVu", typeof(SuDungDichVuChiTietWindow), new[] { "Ma", "DatPhong", "KhachHang", "DichVuGoi", "SoLuong", "DonGia", "ThanhTien", "ThoiDiem" }, new[] { "btnSuDungDichVuThemLuotSuDung", "btnSuDungDichVuSuaLuotSuDung", "btnSuDungDichVuHuyLuotSuDung" }, "btnSuDungDichVuThemLuotSuDung", true, true),
            new(typeof(DichVuView), "tabLoaiDichVu", "dgvLoaiDichVu", typeof(LoaiDichVuChiTietWindow), new[] { "Ma", "TenLoai", "MoTa", "TrangThai" }, new[] { "btnLoaiDichVuThemMoi", "btnLoaiDichVuSuaThongTin", "btnLoaiDichVuXoa" }, "btnLoaiDichVuThemMoi", true, true),
            new(typeof(GoiUuDaiView), "tabGoiDichVu", "dgvGoiDichVu", typeof(GoiDichVuChiTietWindow), new[] { "Ma", "TenGoi", "GiaGoi", "MoTa", "TrangThai" }, new[] { "btnGoiDichVuThemMoi", "btnGoiDichVuSuaThongTin", "btnGoiDichVuXoa" }, "btnGoiDichVuThemMoi", true, true),
            new(typeof(GoiUuDaiView), "tabThanhPhanGoi", "dgvThanhPhanGoi", typeof(ThanhPhanGoiChiTietWindow), new[] { "Goi", "DichVu", "SoLuong", "DonGiaThanhPhan", "GhiChu" }, new[] { "btnThanhPhanGoiSuaThongTin", "btnThanhPhanGoiXoa" }, "btnThanhPhanGoiThemMoi", true, true),
            new(typeof(GoiUuDaiView), "tabUuDai", "dgvUuDai", typeof(UuDaiChiTietWindow), new[] { "Ma", "TenUuDai", "MaApDung", "LoaiGiam", "GiaTri", "TuNgay", "DenNgay", "TrangThai" }, new[] { "btnUuDaiThemMoi", "btnUuDaiSuaThongTin", "btnUuDaiXoa" }, "btnUuDaiThemMoi", true, true),
        };
        void Later(Action check) => app.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, check);
        void Confirm(Window detail, string action, bool accept)
        {
            Later(() => {
                var confirm = app.Windows.OfType<XacNhanDemoWindow>().Single();
                Assert(confirm.Owner == detail, "Confirmation owner changed.");
                Assert(Field<TextBlock>(confirm, "txtNoiDung").Text.Contains(Value(detail.DataContext, rows.Single(r => r.Detail == detail.GetType()).Fields[0])), "Confirmation lost target identity.");
                if (accept) Click(confirm, "btnLuu"); else confirm.Close();
            });
            Click(detail, action);
        }
        void CheckGroups()
        {
            foreach (var group in new UserControl[] { new PhongBuongGroupView(), new DichVuKhachSanGroupView() })
            {
                DemoSession.ApplyAccess(group);
                var host = Host(group); host.Show();
                var tabs = Field<TabControl>(group, "tabNhom");
                var screens = rows.Select(r => r.View).Append(typeof(PhongView)).Distinct().Select(t => group.FindName("view" + t.Name)).OfType<UserControl>().OrderBy(v => tabs.Items.IndexOf(Field<TabItem>(group, "tab" + v.GetType().Name))).ToArray();
                var defaultScreen = screens.FirstOrDefault(v => DemoSession.CanOpen(v.GetType().Name));
                if (defaultScreen == null)
                {
                    Assert(group.Visibility == Visibility.Collapsed, "Denied group is visible.");
                    host.Close();
                    continue;
                }
                Assert(defaultScreen.Visibility == Visibility.Visible && (defaultScreen is PhongView || Field<TabControl>(defaultScreen, "tabChinh").SelectedIndex == 0), "Wrong default group screen.");
                foreach (var screen in screens)
                {
                    var name = screen.GetType().Name;
                    var primary = Field<TabItem>(group, "tab" + name);
                    if (!DemoSession.CanOpen(name))
                    {
                        Assert(primary.Visibility == Visibility.Collapsed && screen.Visibility == Visibility.Collapsed, "Denied system view is visible.");
                        continue;
                    }
                    tabs.SelectedItem = primary;
                    if (screen is PhongView rooms)
                    {
                        host.UpdateLayout();
                        Assert(rooms.Visibility == Visibility.Visible && Field<ListBox>(rooms, "lstPhong").Items.Count == 3, "Unified room board was lost.");
                        foreach (var context in rows.Where(r => group.FindName(r.Tab) is TabItem)) Assert(Field<TabItem>(group, context.Tab).Visibility == Visibility.Collapsed, "Room board leaks contextual family.");
                        Snapshot(host, System.IO.Path.Combine(args[0], "room-board.png"));
                        continue;
                    }
                    var childCases = rows.Where(r => r.View == screen.GetType() && screen.FindName(r.Tab) != null).ToArray();
                    var defaultHeader = group.FindName(childCases[0].Tab) as TabItem;
                    if (defaultHeader != null)
                    {
                        Assert(tabs.SelectedItem == defaultHeader && defaultHeader.Visibility == Visibility.Visible, "Primary did not select its named default context.");
                        Assert(Equals(defaultHeader.Header, Field<TabItem>(screen, childCases[0].Tab).Header), "Default context label differs from its form.");
                        Assert(tabs.Items.IndexOf(defaultHeader) > screens.Max(s => tabs.Items.IndexOf(Field<TabItem>(group, "tab" + s.GetType().Name))), "Default context precedes primary tabs.");
                    }
                    for (int i = 0; i < childCases.Length; i++)
                    {
                        if (i > 0 || defaultHeader != null) tabs.SelectedItem = Field<TabItem>(group, childCases[i].Tab);
                        host.UpdateLayout();
                        Assert(screen.Visibility == Visibility.Visible && Field<TabControl>(screen, "tabChinh").SelectedIndex == i, "Context tab opened wrong form.");
                        foreach (var sibling in screens.Where(s => s != screen)) Assert(sibling.Visibility == Visibility.Collapsed, "Sibling group content leaked.");
                        foreach (var other in rows.Where(r => screens.Any(s => s.GetType() == r.View)).GroupBy(r => r.View))
                        {
                            foreach (var context in other.Where(r => group.FindName(r.Tab) is TabItem))
                            {
                                var header = Field<TabItem>(group, context.Tab);
                                Assert(header.Visibility == (other.Key == screen.GetType() ? Visibility.Visible : Visibility.Collapsed), "Wrong contextual header family.");
                            }
                        }
                        foreach (var width in new[] { 1120d, 800d })
                        {
                            host.Width = width; host.UpdateLayout();
                            if (i > 0 || defaultHeader != null)
                            {
                                var header = Field<TabItem>(group, childCases[i].Tab);
                                Assert(Math.Abs(primary.TranslatePoint(new Point(), group).Y - header.TranslatePoint(new Point(), group).Y) < 1, "Group headers wrapped.");
                                Assert(header.Template.FindName("toolBorder", header) is Border border && border.BorderThickness.Left >= 1, "Context header lost its frame.");
                            }
                        }
                        if (DemoSession.Current!.Role == DemoRole.Admin)
                        {
                            Snapshot(host, System.IO.Path.Combine(args[0], "group-" + childCases[i].Grid + ".png"));
                            host.Width = 1120; host.UpdateLayout();
                            Snapshot(host, System.IO.Path.Combine(args[0], "group-wide-" + childCases[i].Grid + ".png"));
                        }
                    }
                    tabs.SelectedItem = primary;
                    Assert(Field<TabControl>(screen, "tabChinh").SelectedIndex == 0, "Primary did not reset to its default form.");
                }
                host.Close();
            }
        }
        CheckGroups();
        foreach (var rowCase in rows)
        {
            var screen = (UserControl)Activator.CreateInstance(rowCase.View)!;
            if (screen.FindName(rowCase.Tab) == null) continue;
            Field<TabControl>(screen, "tabChinh").SelectedItem = Field<TabItem>(screen, rowCase.Tab);
            var host = Host(screen); host.Show(); host.UpdateLayout();
            var grid = Field<DataGrid>(screen, rowCase.Grid);
            var sibling = grid.Items[0]; var target = grid.Items[1]; var beforeCount = grid.Items.Count;
            var siblingValues = rowCase.Fields.Select(p => Value(sibling, p)).ToArray();
            grid.SelectedIndex = 0; grid.UpdateLayout();
            var clickedRow = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(1);
            foreach (var width in new[] { 1120d, 800d })
            {
                host.Width = width; host.UpdateLayout();
                if (rowCase.Create != null)
                {
                    var create = Field<Button>(screen, rowCase.Create);
                    var search = Field<Button>(screen, "btnTim" + rowCase.Grid[3..]);
                    Assert(Math.Abs(create.TranslatePoint(new Point(), screen).Y - search.TranslatePoint(new Point(), screen).Y) < 1, "Create/search toolbar row split.");
                }
            }
            foreach (var name in rowCase.Actions.Where(n => n != rowCase.Create))
                Assert(screen.FindName(name) == null, "Row action remains on main toolbar: " + name);
            Assert(screen.FindName("btn" + rowCase.Grid[3..] + "ChiTiet") == null, "Detail action remains on toolbar.");
            Later(() => {
                var detail = app.Windows.Cast<Window>().Single(w => w.GetType() == rowCase.Detail);
                Assert(detail.Owner == host && ReferenceEquals(detail.DataContext, target), "Double-click followed selected row instead of clicked row.");
                foreach (var prop in rowCase.Fields)
                    Assert(Field<TextBox>(detail, "txt" + prop).Text == Value(target, prop), "Detail displays sample instead of target: " + prop);
                Snapshot(detail, System.IO.Path.Combine(args[0], rowCase.Detail.Name + ".png"));
                foreach (var name in rowCase.Actions.Where(n => n == rowCase.Create || (!rowCase.Edit && n.Contains("Sua")) || n.Contains("NhapKho") || n.Contains("XuatKho") || n.Contains("DieuChinh") || n.Contains("GanVaiTro") || n.Contains("DatLai") || n.Contains("PhanQuyen") || n.Contains("PhanCong") || n.Contains("HoanThanhBaoTri")))
                {
                    Later(() => {
                        var nested = app.Windows.Cast<Window>().Single(w => w.Owner == detail);
                        Assert(nested.IsVisible, "Row action did not open its form.");
                        if (name != rowCase.Create)
                            Assert(ReferenceEquals(nested.DataContext, target) && nested.Title.Contains(Value(target, rowCase.Fields[0])), "Nested form lost row identity.");
                        Snapshot(nested, System.IO.Path.Combine(args[0], name + ".png"));
                        nested.Close();
                    });
                    Click(detail, name);
                }
                if (rowCase.Edit)
                {
                    var editName = rowCase.Actions.Single(n => n.Contains("Sua"));
                    Click(detail, editName);
                    var editable = rowCase.Fields.First(p => !Field<TextBox>(detail, "txt" + p).IsReadOnly);
                    var original = Value(target, editable);
                    Input(detail, "txt" + editable, "");
                    Click(detail, "btnLuuSua");
                    Assert(Value(target, editable) == original && Field<Button>(detail, "btnLuuSua").Visibility == Visibility.Visible, "Invalid edit mutated source.");
                    Input(detail, "txt" + editable, original + " sửa");
                    Click(detail, "btnHuySua");
                    Assert(Field<TextBox>(detail, "txt" + editable).Text == original && Value(target, editable) == original, "Cancel failed to restore data.");
                    Click(detail, editName);
                    Input(detail, "txt" + editable, original + " sửa");
                    Confirm(detail, "btnLuuSua", false);
                    Assert(Value(target, editable) == original, "Canceled save mutated source.");
                    Confirm(detail, "btnLuuSua", true);
                    Assert(Value(target, editable) == original + " sửa" && Field<TextBox>(detail, "txt" + editable).IsReadOnly, "Save failed to update target.");
                    Assert(rowCase.Fields.Select(p => Value(sibling, p)).SequenceEqual(siblingValues), "Edit changed a sibling.");
                }
                foreach (var name in rowCase.Actions.Where(n => n.Contains("KhoaTai") || n.EndsWith("MoKhoa") || n.Contains("XacNhan") || n.Contains("HuyPhieu") || n.Contains("BatDau") || n.Contains("HoanThanhDon")))
                {
                    Confirm(detail, name, false);
                    Confirm(detail, name, true);
                    Assert(detail.IsVisible, "Demo transaction unexpectedly removed record.");
                }
                if (rowCase.Delete)
                {
                    var deleteName = rowCase.Actions.Single(n => n.EndsWith("Xoa") || n.Contains("HuyLuotSuDung"));
                    Confirm(detail, deleteName, false);
                    Assert(detail.IsVisible && grid.Items.Contains(target), "Canceled deletion removed record.");
                    Confirm(detail, deleteName, true);
                }
                else detail.Close();
            });
            grid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = clickedRow });
            Assert(grid.Items.Contains(sibling), "Wrong row removed.");
            Assert(grid.Items.Count == beforeCount - (rowCase.Delete ? 1 : 0), "Delete did not update main table.");
            grid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = grid });
            Assert(!app.Windows.Cast<Window>().Any(w => w.GetType() == rowCase.Detail), "Blank grid opened modal.");
            if (rowCase.Create != null)
            {
                if (grid.ItemsSource is System.Collections.IList list) list.Clear(); else grid.Items.Clear();
                Later(() => { var create = app.Windows.Cast<Window>().Single(w => w.Owner == host); create.Close(); });
                ClickScreen(screen, rowCase.Create);
            }
            host.Close();
        }
        // Check computations, invalid dates/hours, negative numbers and overflow before any source changes.
        void SaveCheck(Window detail, string edit, string field, string bad)
        {
            detail.Show(); Click(detail, edit);
            var item = detail.DataContext;
            var values = item.GetType().GetProperties().Select(p => p.GetValue(item)).ToArray();
            Input(detail, field, bad); Click(detail, "btnLuuSua");
            Assert(item.GetType().GetProperties().Select(p => p.GetValue(item)).SequenceEqual(values), "Invalid value partly modified source.");
            Assert(!app.Windows.OfType<XacNhanDemoWindow>().Any(), "Invalid value opened save confirmation.");
            detail.Close();
        }
        var service = (SuDungDichVuMau)Field<DataGrid>(new DichVuView(), "dgvSuDungDichVu").Items[0];
        SaveCheck(new SuDungDichVuChiTietWindow(service), "btnSuDungDichVuSuaLuotSuDung", "txtSoLuong", "0");
        SaveCheck(new SuDungDichVuChiTietWindow(service), "btnSuDungDichVuSuaLuotSuDung", "txtSoLuong", "-1");
        SaveCheck(new SuDungDichVuChiTietWindow(service), "btnSuDungDichVuSuaLuotSuDung", "txtDonGia", decimal.MaxValue.ToString(System.Globalization.CultureInfo.GetCultureInfo("vi-VN")));
        SaveCheck(new SuDungDichVuChiTietWindow(service), "btnSuDungDichVuSuaLuotSuDung", "txtThoiDiem", "31/02/2026 25:00");
        var serviceDetail = new SuDungDichVuChiTietWindow(service); serviceDetail.Show();
        Click(serviceDetail, "btnSuDungDichVuSuaLuotSuDung");
        Assert(Field<TextBox>(serviceDetail, "txtThanhTien").IsReadOnly, "Derived service total can be overwritten.");
        Input(serviceDetail, "txtSoLuong", "4");
        Confirm(serviceDetail, "btnLuuSua", true);
        Assert(service.ThanhTien == "600.000 đ", "Service total did not follow quantity and price.");
        Click(serviceDetail, "btnSuDungDichVuSuaLuotSuDung"); Input(serviceDetail, "txtSoLuong", "1,5");
        Confirm(serviceDetail, "btnLuuSua", true);
        Assert(service.ThanhTien == "225.000 đ", "Fractional quantities such as laundry kg failed.");
        serviceDetail.Close();
        var promotion = (UuDaiMau)Field<DataGrid>(new GoiUuDaiView(), "dgvUuDai").Items[0];
        SaveCheck(new UuDaiChiTietWindow(promotion), "btnUuDaiSuaThongTin", "txtGiaTri", "101%");
        SaveCheck(new UuDaiChiTietWindow(promotion), "btnUuDaiSuaThongTin", "txtGiaTri", "-1%");
        SaveCheck(new UuDaiChiTietWindow(promotion), "btnUuDaiSuaThongTin", "txtLoaiGiam", "Không hợp lệ");
        SaveCheck(new UuDaiChiTietWindow(promotion), "btnUuDaiSuaThongTin", "txtDenNgay", "31/02/2026");
        SaveCheck(new UuDaiChiTietWindow(promotion), "btnUuDaiSuaThongTin", "txtDenNgay", "01/01/2026");
        var promotionDetail = new UuDaiChiTietWindow(promotion); promotionDetail.Show();
        Click(promotionDetail, "btnUuDaiSuaThongTin"); Input(promotionDetail, "txtGiaTri", "25");
        Confirm(promotionDetail, "btnLuuSua", true);
        Assert(promotion.GiaTri == "25%", "Percentage display did not follow discount type."); promotionDetail.Close();
        var device = (ThietBiMau)Field<DataGrid>(new BuongPhongView(), "dgvThietBi").Items[0];
        SaveCheck(new ThietBiChiTietWindow(device), "btnThietBiSuaThongTin", "txtHanBaoHanh", "01/01/2025");
        var roomType = (LoaiPhongMau)Field<DataGrid>(new DanhMucPhongView(), "dgvLoaiPhong").Items[0];
        SaveCheck(new LoaiPhongChiTietWindow(roomType), "btnLoaiPhongSuaThongTin", "txtGiaDem", "-1");
        SaveCheck(new LoaiPhongChiTietWindow(roomType), "btnLoaiPhongSuaThongTin", "txtNguoiLon", "0");
        SaveCheck(new LoaiPhongChiTietWindow(roomType), "btnLoaiPhongSuaThongTin", "txtDienTich", "0");

        foreach (var account in DemoSession.Accounts)
        {
            DemoSession.SignIn(account.Username, account.Password);
            CheckGroups();
            foreach (var row in rows)
            {
                var detail = (Window)Activator.CreateInstance(row.Detail, new object?[] { null })!;
                foreach (var action in row.Actions)
                {
                    var button = Field<Button>(detail, action);
                    var allowed = DemoSession.CanUseButton(row.View.Name, action);
                    Assert(button.IsEnabled == allowed, "Detail permission mismatch: " + action);
                    if (!allowed)
                    {
                        var count = app.Windows.Count;
                        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        Assert(app.Windows.Count == count, "Denied action opened a modal: " + action);
                        Assert(row.Fields.All(p => Field<TextBox>(detail, "txt" + p).IsReadOnly), "Denied role entered edit mode.");
                    }
                }
                detail.Close();
            }
        }
        Assert(DemoSession.Accounts.Count == 5 && DemoSession.SignIn("admin", "admin123"), "UI changed fixed demo authentication.");
        app.Shutdown();
        Console.WriteLine("PASS: all 14 row details, 2 contextual groups, toolbars, editing/cancellation, confirmations, computations, nested actions and role permissions.");
    }
    static void ClickScreen(UserControl screen, string name) => Field<Button>(screen, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
}
'@ | Set-Content (Join-Path $checkRoot 'Program.cs') -Encoding UTF8
dotnet run --project (Join-Path $checkRoot 'Check.csproj') -- $checkRoot
if ($LASTEXITCODE -ne 0) { throw 'Room and service detail checks failed.' }
Write-Output "Screenshots: $checkRoot"
