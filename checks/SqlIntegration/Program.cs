using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BUS_QuanlyKhachSan.LeTanValuuTru;
using BUS_QuanlyKhachSan.HeThong;
using Dapper;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;
using QuanLyKhachSan;
using QuanLyKhachSan.Views;
using QuanLyKhachSan.Views.Dialogs;

class Check
{
    static readonly string PreviewFolder = Path.Combine(Path.GetTempPath(), "QLKS_UiRestore_" + Guid.NewGuid().ToString("N"));
    static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    static void Reject(Action action)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new Exception("Expected validation to reject the operation.");
    }
    static T Field<T>(FrameworkElement window, string name) => (T)window.FindName(name);
    static void Click(FrameworkElement window, string name) => Field<Button>(window, name).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    static void Input(FrameworkElement window, string name, string value)
    {
        var box = Field<TextBox>(window, name);
        box.SetCurrentValue(TextBox.TextProperty, value);
        box.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }
    static void Select(FrameworkElement window, string name, object value)
    {
        var box = Field<ComboBox>(window, name);
        box.SetCurrentValue(Selector.SelectedValueProperty, value);
        box.GetBindingExpression(Selector.SelectedValueProperty)?.UpdateSource();
    }
    static void Submit(Window form, Action action)
    {
        Exception? failure = null;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        timer.Tick += (_, _) => {
            timer.Stop();
            try { action(); } catch (Exception ex) { failure = ex; form.Close(); }
        };
        timer.Start();
        bool? saved = form.ShowDialog();
        timer.Stop();
        if (failure != null) throw failure;
        Assert(saved == true, "Save did not finish successfully: " + form.GetType().Name);
    }
    static void Columns(DataGrid grid)
    {
        Assert(grid.Items.Count > 0, "SQL grid is empty: " + grid.Name);
        foreach (var column in grid.Columns.OfType<DataGridBoundColumn>())
        {
            if (column.Binding is not Binding binding) continue;
            string name = binding.Path.Path;
            var row = grid.Items[0];
            Assert(row is DataRowView tableRow ? tableRow.Row.Table.Columns.Contains(name) : row.GetType().GetProperty(name) != null,
                "Missing binding column: " + grid.Name + "." + name);
        }
    }
    static void Snapshot(Window window, string name)
    {
        window.UpdateLayout();
        Directory.CreateDirectory(PreviewFolder);
        var bitmap = new RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(window);
        var png = new PngBitmapEncoder();
        png.Frames.Add(BitmapFrame.Create(bitmap));
        using var file = File.Create(Path.Combine(PreviewFolder, name + ".png"));
        png.Save(file);
    }
    static void OpenChild(Window parent, string button, Type childType, Action<Window> inspect)
    {
        Exception? failure = null;
        bool opened = false;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        timer.Tick += (_, _) => {
            var child = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.Owner == parent && x.GetType() == childType);
            if (child == null) return;
            timer.Stop(); opened = true;
            try { inspect(child); } catch (Exception ex) { failure = ex; }
            child.Close();
        };
        parent.Dispatcher.BeginInvoke(() => {
            try { Click(parent, button); } catch (Exception ex) { failure = ex; }
            parent.Close();
        }, DispatcherPriority.ApplicationIdle);
        timer.Start(); parent.ShowDialog(); timer.Stop();
        if (failure != null) throw failure;
        Assert(opened, "Restored action did not open its form: " + button);
    }
    static void Script(SqlConnection connection, string file)
    {
        foreach (var batch in Regex.Split(File.ReadAllText(file), @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            if (!string.IsNullOrWhiteSpace(batch))
            {
                // Database thử trống: khởi tạo proc trước script cập nhật, cùng cách bootstrap đã có ở SQL của 5Sao.
                var procedure = Regex.Match(batch, @"CREATE\s+OR\s+ALTER\s+PROCEDURE\s+(dbo\.\w+)", RegexOptions.IgnoreCase);
                if (procedure.Success)
                    connection.Execute("IF OBJECT_ID(@name, N'P') IS NULL EXEC(@sql)",
                        new { name = procedure.Groups[1].Value, sql = $"CREATE PROCEDURE {procedure.Groups[1].Value} AS RETURN;" });
                try { connection.Execute(batch, commandTimeout: 60, commandType: System.Data.CommandType.Text); }
                catch (Exception ex) { throw new Exception("SQL script batch failed: " + batch[..Math.Min(180, batch.Length)], ex); }
            }
    }

    [STAThread]
    static void Main(string[] args)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var server = args.FirstOrDefault() ?? @"lpc:.\SQLEXPRESS";
        var name = "QLKS_Gop2Check_" + Guid.NewGuid().ToString("N");
        var masterBuilder = new SqlConnectionStringBuilder { DataSource = server, InitialCatalog = "master",
            IntegratedSecurity = true, TrustServerCertificate = true, ConnectTimeout = 5 };
        using var master = new SqlConnection(masterBuilder.ConnectionString);
        master.Open();
        bool created = false;
        var previous = Environment.GetEnvironmentVariable("QLKS_CONNECTION");
        try
        {
            // Chỉ tạo tên ngẫu nhiên; không dùng database đang có của người dùng.
            Console.WriteLine("Test database: " + name);
            master.Execute($"CREATE DATABASE [{name}]"); created = true;
            var builder = new SqlConnectionStringBuilder(masterBuilder.ConnectionString) { InitialCatalog = name };
            Environment.SetEnvironmentVariable("QLKS_CONNECTION", builder.ConnectionString);
            using (var db = new SqlConnection(builder.ConnectionString))
            {
                db.Open();
                Script(db, Path.Combine(root, "checks/SqlIntegration/Schema.sql"));
                Script(db, Path.Combine(root, "CapNhat_LeTan_HoaDon.sql"));
                Script(db, Path.Combine(root, "letanvaluutru.sql"));
                Script(db, Path.Combine(root, "HeThong.sql"));
                db.Execute("""
                    INSERT KhuVuc(TenKhuVuc,TrangThai) VALUES(N'Test',1);
                    INSERT Tang(MaKhuVuc,SoTang,TenTang,TrangThai) VALUES(1,1,N'Test',1);
                    INSERT LoaiPhong(TenLoaiPhong,SucChuaNguoiLon,SucChuaTreEm,GiaCoBan,TrangThai) VALUES(N'Test',2,1,100000,1);
                    INSERT Phong(SoPhong,MaTang,MaLoaiPhong,KhongHutThuoc,TrangThaiKhaiThac,TrangThaiVeSinh) VALUES('101',1,1,1,'SanSang','Sach'),('102',1,1,1,'SanSang','Sach');
                    INSERT PhongBan(TenPhongBan,TrangThai) VALUES(N'Test',1);
                    INSERT ChucVu(TenChucVu,Chucvu,TrangThai) VALUES(N'Test',0,1);
                    INSERT NhanVien(HoTen,MaPhongBan,MaChucVu,LuongCoBan,TrangThai) VALUES(N'Test',1,1,100000,'DangLam');
                    INSERT NhanVien(HoTen,MaPhongBan,MaChucVu,LuongCoBan,TrangThai) VALUES(N'Account test',1,1,100000,'DangLam');
                    INSERT VaiTro(TenVaiTro,TrangThai) VALUES(N'Test Admin',1);
                    INSERT TaiKhoan(MaNhanVien,TenDangNhap,MatKhauHash,MaVaiTro,TrangThai) VALUES(1,'account.test','unchanged-test-hash',1,'HoatDong');
                    INSERT LoaiDichVu(TenLoaiDichVu,TrangThai) VALUES(N'Test services',1);
                    INSERT DichVu(MaLoaiDichVu,TenDichVu,DonGia,TrangThai) VALUES(1,N'Test service',30000,1);
                    INSERT GoiDichVu(TenGoiDichVu,DonGia,TrangThai) VALUES(N'Test package',150000,1);
                    """);
            }

            var app = new App(); app.InitializeComponent(); app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            // Các cửa sổ thử chạy ngoài màn hình; không chen vào phiên ứng dụng đang mở của người dùng.
            app.Resources.Add(typeof(Window), new Style(typeof(Window)) { Setters = {
                new Setter(UIElement.OpacityProperty, 0d), new Setter(Window.ShowInTaskbarProperty, false),
                new Setter(Window.LeftProperty, -30000d), new Setter(Window.TopProperty, -30000d) } });
            Assert(DemoSession.SignIn("admin", "admin123"), "Admin login failed.");
            var customer = new KhachHang { HoTen = "Khách kiểm tra", GioiTinh = "O", LoaiGiayTo = "CCCD",
                SoGiayTo = "012345678901", SoDienThoai = "0912345678", Email = "test@gmail.com", TrangThai = true };
            Assert(new BUS_KhachHang().LuuKhachHang(customer), "Customer insert failed.");
            var bookings = new BUS_DatPhong();
            var order = new DatPhong { MaKhachHangDat = customer.MaKhachHang };
            var line = new ChiTietDatPhong { MaLoaiPhong = 1, SoLuongPhong = 1, NguoiLon = 2,
                DonGiaDuKien = 100000, NgayNhanDuKien = DateTime.Today.AddHours(14), NgayTraDuKien = DateTime.Today.AddDays(2).AddHours(12) };
            Assert(bookings.LuuDatPhong(order, new() { line }), "Booking insert failed.");
            long originalLine = line.MaChiTietDatPhong;
            order.GhiChu = "Updated";
            Assert(bookings.LuuDatPhong(order, new() { line }) && line.MaChiTietDatPhong == originalLine,
                "Booking edit lost the detail key.");
            Assert(bookings.XacNhan(order.MaDatPhong), "Confirm failed.");
            var allocation = new XepPhong { MaChiTietDatPhong = line.MaChiTietDatPhong, MaPhong = 1 };
            Assert(new Bus_PhanBoPhong().LuuPhanBo(allocation), "Allocation failed.");
            var bookingRow = bookings.hienThiDPAll()[0];
            Assert(Convert.ToInt64(bookingRow["MaDatPhong"]) == order.MaDatPhong, "Booking native key missing.");
            var detail = new DatPhongChiTietWindow(order.MaDatPhong) { DataContext = bookingRow };
            Assert(!Field<Button>(detail, "btnDatPhongXacNhanDatPhong").IsEnabled, "Confirmed booking still allows Confirm.");
            Assert(Field<ListBox>(detail, "lstChiTiet").Items.Count == 1, "Booking detail is empty.");
            detail.Close();

            var group = new LeTanGroupView();
            var host = new Window { Content = group, Width = 1120, Height = 720, ShowInTaskbar = false, Opacity = 0, Left = -30000, Top = -30000 };
            host.Show(); host.UpdateLayout();
            host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            var bookingView = Field<DatPhongView>(group, "viewDatPhong");
            var bookingGrid = Field<DataGrid>(bookingView, "dgvDatPhong");
            Columns(bookingGrid);
            var allocations = Field<PhanBoPhongView>(group, "viewPhanBoPhong");
            Columns(Field<DataGrid>(allocations, "dgvPhanBoPhong"));
            var allocationForm = new PhanBoPhongForm(0, order.MaDatPhong);
            Assert(Convert.ToInt64(Field<ComboBox>(allocationForm, "cboDatPhong").SelectedValue) == order.MaDatPhong &&
                !Field<ComboBox>(allocationForm, "cboDatPhong").IsEnabled, "Allocation lost the selected booking.");
            allocationForm.Close();

            var stay = new BUS_KhachTheoPhong();
            Assert(stay.NhanPhong(allocation.MaXepPhong), "Check-in failed.");
            Assert(stay.hienThiKTPAll().Count == 1, "Current stay is not displayed.");
            Reject(() => bookings.HuyDatPhong(order.MaDatPhong));
            Assert(stay.SuaGhiChuLuuTru(allocation.MaXepPhong, "Test notes"), "Stay notes failed.");
            Assert(stay.ChuyenPhong(allocation.MaXepPhong, 2, "Move test"), "Room move failed.");
            var active = stay.HienThiXepPhong().Single(x => x.TrangThai == "DaCheckIn");
            Assert(active.MaPhong == 2 && stay.TraPhong(active.MaXepPhong), "Check-out failed.");
            Assert(new BUS_LichSuXepPhong().hienThiLSXPAll().Count == 2, "Move lost history.");
            var historyView = new LichSuLuuTruView();
            host.Content = historyView; host.UpdateLayout(); host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            Columns(Field<DataGrid>(historyView, "dgvLichSuLuuTru"));

            // Đóng form hóa đơn phải bỏ các lượt dịch vụ/gói đang chọn.
            var draft = new HoaDonForm();
            Field<ComboBox>(draft, "cboDatPhong").SelectedValue = order.MaDatPhong;
            Field<ComboBox>(draft, "cboDichVu").SelectedIndex = 0;
            Field<TextBox>(draft, "txtSoLuongDichVu").Text = "2";
            Click(draft, "btnThemDichVu");
            draft.Close();
            using (var db = new SqlConnection(builder.ConnectionString))
                Assert(db.QuerySingle<int>("SELECT COUNT(*) FROM SuDungDichVu") == 0, "Closing invoice saved draft services.");

            var invoiceForm = new HoaDonForm();
            Field<ComboBox>(invoiceForm, "cboDatPhong").SelectedValue = order.MaDatPhong;
            Field<ComboBox>(invoiceForm, "cboNhanVien").SelectedIndex = 0;
            Field<ComboBox>(invoiceForm, "cboDichVu").SelectedIndex = 0;
            Field<TextBox>(invoiceForm, "txtSoLuongDichVu").Text = "2";
            Click(invoiceForm, "btnThemDichVu");
            Field<ComboBox>(invoiceForm, "cboGoiDichVu").SelectedIndex = 0;
            Field<TextBox>(invoiceForm, "txtSoLuongGoi").Text = "1";
            Click(invoiceForm, "btnThemGoi");
            Submit(invoiceForm, () => Click(invoiceForm, "btnLuu"));
            var invoice = new BUS_HoaDon();
            var invoiceRow = invoice.hienThiHDAll()[0];
            long invoiceId = Convert.ToInt64(invoiceRow["MaHoaDon"]);
            var entity = invoice.HienThiHoaDon(new HoaDon { MaHoaDon = invoiceId });
            Assert(entity.TongTien == 410000 && invoice.ChiTietHoaDon(invoiceId).Count == 3,
                "Invoice total should include 200000 room + 60000 service + 150000 package.");
            Assert(invoice.ThuTien(invoiceId, 10000, 1, "Test payment"), "Payment failed.");
            Assert(invoice.DaThu(invoiceId) == 10000 && new BUS_LichSuThuTien().hienThiLSTTAll().Count == 1, "Payment history failed.");
            Reject(() => invoice.ThuTien(invoiceId, entity.ThanhTien, 1, "Too much"));
            Reject(() => invoice.TaoHoaDon(order.MaDatPhong, 1, null, "Duplicate"));
            var invoiceView = new HoaDonView();
            host.Content = invoiceView; host.UpdateLayout(); host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            Columns(Field<DataGrid>(invoiceView, "dgvHoaDon"));
            var receiptView = new LichSuThuTienView();
            host.Content = receiptView; host.UpdateLayout(); host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            Columns(Field<DataGrid>(receiptView, "dgvLichSuThuTien"));

            var roles = new BUS_VaiTro();
            var role = new VaiTro { TenVaiTro = "Merged role", MoTa = "Test", TrangThai = true };
            roles.LuuVaiTro(role);
            int roleId = role.MaVaiTro;
            role.TenVaiTro = "Updated role"; roles.LuuVaiTro(role);
            Assert(roles.hienThiVTAll().Count == 2 && roles.HienThiVT(new VaiTro { MaVaiTro = roleId }).TenVaiTro == "Updated role",
                "Role edit inserted a second record.");
            var roleForm = new VaiTroQuyenForm(new VaiTro { TenVaiTro = "UI role", TrangThai = true });
            Submit(roleForm, () => Click(roleForm, "btnLuu"));
            var accounts = new BUS_Taikhoang();
            var accountForm = new TaiKhoanForm();
            Submit(accountForm, () => {
                Select(accountForm, "cboNhanVien", 2);
                Select(accountForm, "cboVaiTro", roleId);
                Input(accountForm, "txtTenDangNhap", "merged.account");
                Field<PasswordBox>(accountForm, "pwdMatKhau").Password = "test password";
                Field<PasswordBox>(accountForm, "pwdNhapLaiMatKhau").Password = "test password";
                Assert(((TaiKhoan)accountForm.DataContext).TenDangNhap == "merged.account", "Account input binding did not update its draft.");
                Click(accountForm, "btnLuu");
            });
            dynamic safeRow = accounts.hienThiTKAll().Single(x => (string)x.TenDangNhap == "merged.account");
            Assert(safeRow is IDictionary<string, object> safe && !safe.ContainsKey("MatKhauHash"), "Account list exposes password hash.");
            int accountId = Convert.ToInt32(safeRow.MaTaiKhoan);
            var account = accounts.HienThiTK(accountId);
            Assert(account.MaVaiTro == roleId && account.MatKhauHash.StartsWith("PBKDF2-SHA256$"), "Account save lost role or password hash.");
            string originalHash = account.MatKhauHash;
            var editAccountForm = new TaiKhoanForm(account);
            Submit(editAccountForm, () => {
                Input(editAccountForm, "txtTenDangNhap", "updated.account");
                Click(editAccountForm, "btnLuu");
            });
            Assert(accounts.HienThiTK(accountId).MatKhauHash == originalHash, "Editing without a password changed its hash.");
            Assert(accounts.Khoa(accountId) && accounts.HienThiTK(accountId).TrangThai == "Khoa", "Lock failed.");
            Assert(accounts.mo(accountId) == "damo" && accounts.HienThiTK(accountId).TrangThai == "HoatDong", "Unlock failed.");
            Assert(!accounts.Khoa(0), "Missing account was reported as locked.");
            Assert(accounts.mo(int.MaxValue) != "damo", "Missing account was reported as unlocked.");
            var accountView = new TaiKhoanView();
            host.Content = accountView; host.UpdateLayout(); host.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            Assert(Field<DataGrid>(accountView, "dgvTaiKhoan").Items.Count == 2, "Account view did not load SQL.");
            Columns(Field<DataGrid>(accountView, "dgvVaiTroQuyen"));
            Assert(accountView.FindName("btnTaiKhoanSuaTaiKhoan") == null && accountView.FindName("btnTaiKhoanKhoaTaiKhoan") == null,
                "Detail actions leaked into the compact account list.");
            Input(accountView, "txtTimTaiKhoan", "updated.account"); Click(accountView, "btnTimTaiKhoan");
            Assert(Field<DataGrid>(accountView, "dgvTaiKhoan").Items.Count == 1, "Account search was not bound to the original BUS results.");
            Click(accountView, "btnMoiTaiKhoan");
            Assert(Field<DataGrid>(accountView, "dgvTaiKhoan").Items.Count == 2, "Account refresh did not clear the search.");
            var accountGrid = Field<DataGrid>(accountView, "dgvTaiKhoan");
            host.UpdateLayout();
            var clickedRow = accountGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow
                ?? throw new Exception("Account grid did not create its row.");
            bool detailOpened = false;
            Exception? detailFailure = null;
            var doubleClickTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            doubleClickTimer.Tick += (_, _) => {
                var window = app.Windows.OfType<TaiKhoanChiTietWindow>().FirstOrDefault(x => x.Owner == host);
                if (window == null) return;
                doubleClickTimer.Stop(); detailOpened = true;
                try
                {
                    Assert(ReferenceEquals(window.DataContext, clickedRow.Item), "Double click opened a different account.");
                    foreach (string button in new[] { "btnTaiKhoanThemTaiKhoan", "btnTaiKhoanSuaTaiKhoan", "btnTaiKhoanGanVaiTro",
                        "btnTaiKhoanKhoaTaiKhoan", "btnTaiKhoanMoKhoa", "btnTaiKhoanDatLaiMatKhau" })
                        Assert(Field<Button>(window, button).Visibility == Visibility.Visible, "Account detail lost action: " + button);
                }
                catch (Exception ex) { detailFailure = ex; }
                window.Close();
            };
            doubleClickTimer.Start();
            accountGrid.RaiseEvent(new System.Windows.Input.MouseButtonEventArgs(System.Windows.Input.Mouse.PrimaryDevice, 0,
                System.Windows.Input.MouseButton.Left) { RoutedEvent = Control.MouseDoubleClickEvent, Source = clickedRow });
            doubleClickTimer.Stop();
            if (detailFailure != null) throw detailFailure;
            Assert(detailOpened, "Account double click did not open the detail actions.");

            var roleDetail = new VaiTroQuyenChiTietWindow { DataContext = role, Width = 540 };
            foreach (string buttonName in new[] { "btnVaiTroQuyenThemVaiTro", "btnVaiTroQuyenSuaVaiTro", "btnVaiTroQuyenPhanQuyen", "btnVaiTroQuyenXoaVaiTro", "btnDong" })
                Assert(Field<Button>(roleDetail, buttonName).Visibility == Visibility.Visible, "Missing role action: " + buttonName);
            OpenChild(roleDetail, "btnVaiTroQuyenPhanQuyen", typeof(PhanQuyenWindow), window => {
                var panel = Field<StackPanel>(window, "pnlNoiDung");
                var boxes = panel.Children.OfType<WrapPanel>().SelectMany(x => x.Children.OfType<CheckBox>()).ToList();
                Assert(boxes.Count == 65, "Permission UI lost a group or checkbox.");
                Assert(ReferenceEquals(Field<ComboBox>(window, "cboVaiTro").SelectedItem, role), "Permission form lost the selected role.");
                Click(window, "btnChonTatCa"); Assert(boxes.All(x => x.IsChecked == true), "Select all missed permissions.");
                Click(window, "btnBoChon"); Assert(boxes.All(x => x.IsChecked == false), "Clear missed permissions.");
                Click(window, "btnLuu");
                Assert(window.IsVisible && Field<TextBlock>(window, "txtThongBao").Text.Contains("chưa được lưu"), "Unimplemented permission Save reported success.");
                Snapshot(roleDetail, "vai-tro-540"); Snapshot(window, "phan-quyen");
            });
            var accountDetail = new TaiKhoanChiTietWindow { DataContext = accounts.hienThiTKAll().Single(x => (string)x.TenDangNhap == "updated.account"), Width = 540 };
            OpenChild(accountDetail, "btnTaiKhoanGanVaiTro", typeof(TaiKhoanGanVaiTroWindow), window => {
                Assert(Convert.ToInt32(Field<ComboBox>(window, "cboVaiTro").SelectedValue) == roleId, "Assign-role UI loaded the wrong role.");
                Click(window, "btnLuu"); Assert(window.IsVisible, "Pending assign-role Save closed as if saved.");
                Snapshot(accountDetail, "tai-khoan-540");
            });
            accountDetail = new TaiKhoanChiTietWindow { DataContext = accounts.hienThiTKAll().Single(x => (string)x.TenDangNhap == "updated.account") };
            OpenChild(accountDetail, "btnTaiKhoanDatLaiMatKhau", typeof(TaiKhoanDatLaiMatKhauWindow), window => {
                Field<PasswordBox>(window, "pwdMatKhauMoi").Password = "new draft";
                Click(window, "btnLuu"); Assert(window.IsVisible, "Pending reset-password Save closed as if saved.");
            });
            using (var db = new SqlConnection(builder.ConnectionString))
                Assert(db.QuerySingle<int>("SELECT COUNT(*) FROM VaiTro_Quyen") == 0 && accounts.HienThiTK(accountId).MatKhauHash == originalHash &&
                    accounts.HienThiTK(accountId).MaVaiTro == roleId, "Restored pending UI wrote permissions, role or password.");
            var logView = new NhatKyView();
            host.Content = logView; host.UpdateLayout();
            bool logOpened = false;
            Exception? logFailure = null;
            var logTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            logTimer.Tick += (_, _) => {
                var window = app.Windows.OfType<NhatKyThaoTacForm>().FirstOrDefault(x => x.Owner == host);
                if (window == null) return;
                logTimer.Stop(); logOpened = true;
                try
                {
                    Assert(Field<DatePicker>(window, "dtpTuNgay").SelectedDate.HasValue && Field<DatePicker>(window, "dtpDenNgay").SelectedDate.HasValue,
                        "Restored log filter lost its date fields.");
                    Click(window, "btnLuu"); Assert(window.IsVisible, "Pending log filter reported applied.");
                }
                catch (Exception ex) { logFailure = ex; }
                window.Close();
            };
            logTimer.Start(); Click(logView, "btnNhatKyThaoTacBoLocNhatKy"); logTimer.Stop();
            if (logFailure != null) throw logFailure;
            Assert(logOpened, "Restored log filter did not open.");
            var customerDetail = new KhachHangChiTietWindow { DataContext = customer };
            OpenChild(customerDetail, "btnKhachHangThemMoi", typeof(KhachHangForm), window =>
                Assert(((KhachHang)window.DataContext).MaKhachHang == 0, "Add customer reused the selected record."));
            var allocationDetail = new PhanBoPhongChiTietWindow { DataContext = new Bus_PhanBoPhong().HienThiPBP()[0] };
            OpenChild(allocationDetail, "btnPhanBoPhongThemPhong", typeof(PhanBoPhongForm), window =>
                Assert(!Field<ComboBox>(window, "cboDatPhong").IsEnabled, "Restored Add room lost booking context."));
            var invoiceDetail = new HoaDonChiTietWindow(invoiceId) { DataContext = invoiceRow, Width = 540 };
            OpenChild(invoiceDetail, "btnHoaDonTaoHoaDon", typeof(HoaDonForm), window => {
                Assert(Field<Button>(invoiceDetail, "btnHoaDonXemMauIn").Visibility == Visibility.Visible, "Print preview button is missing.");
                Snapshot(invoiceDetail, "hoa-don-540");
            });

            Assert(DemoSession.SignIn("buongphong", "demo123"), "Role login failed.");
            var denied = new KhachHangForm(customer.MaKhachHang);
            Assert(!Field<Button>(denied, "btnLuu").IsEnabled, "Editor bypasses permissions.");
            Input(denied, "txtHoTen", "Forbidden"); Click(denied, "btnLuu"); denied.Close();
            Assert(new BUS_KhachHang().HienThiKhachHang(new KhachHang { MaKhachHang = customer.MaKhachHang }).HoTen == customer.HoTen,
                "Denied Save wrote SQL.");
            var deniedAccount = new TaiKhoanForm(accounts.HienThiTK(accountId));
            Assert(!Field<Button>(deniedAccount, "btnLuu").IsEnabled, "Account editor bypasses permissions.");
            Click(deniedAccount, "btnLuu"); deniedAccount.Close();
            Assert(accounts.HienThiTK(accountId).TenDangNhap == "updated.account", "Denied account Save wrote SQL.");
            var deniedRole = new VaiTroQuyenChiTietWindow { DataContext = role };
            Assert(!Field<Button>(deniedRole, "btnVaiTroQuyenPhanQuyen").IsEnabled, "Restored permission button bypasses access.");
            var count = app.Windows.Count;
            Click(deniedRole, "btnVaiTroQuyenPhanQuyen");
            Assert(app.Windows.Count == count, "Denied role opened the restored permission form.");
            deniedRole.Close();
            host.Close(); app.Shutdown();
            Console.WriteLine("PASS: reception/account SQL workflows, restored role/account actions, all 65 permission checkboxes, pending UI without writes, and access checks. Previews: " + PreviewFolder);
        }
        finally
        {
            Environment.SetEnvironmentVariable("QLKS_CONNECTION", previous);
            SqlConnection.ClearAllPools();
            if (created && Regex.IsMatch(name, @"^QLKS_Gop2Check_[0-9a-f]{32}$"))
                master.Execute($"ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]");
            Console.WriteLine("Test database removed: " + name);
        }
    }
}
