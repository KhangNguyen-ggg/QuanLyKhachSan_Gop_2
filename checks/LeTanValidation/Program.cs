using BUS_QuanlyKhachSan.LeTanValuuTru;
using ET_QuanLyKhachSan;
using System.Globalization;

// Chuỗi thử chỉ để khởi tạo BUS; các kiểm tra này không mở kết nối.
Environment.SetEnvironmentVariable("QLKS_CONNECTION", "Server=localhost;Database=QLKS_ValidationOnly;Integrated Security=True");
int checks = 0;
void Bang(decimal actual, decimal expected)
{
    if (actual != expected) throw new Exception($"Expected {expected}, got {actual}");
    checks++;
}
void Dung(bool value) { if (!value) throw new Exception("Expected true"); checks++; }
void TuChoi(Action action)
{
    try { action(); }
    catch (ArgumentException ex) { Dung(!string.IsNullOrWhiteSpace(ex.Message)); return; }
    throw new Exception("Expected input validation to reject this value.");
}
KhachHang Khach() => new KhachHang { HoTen = "Dương Hoàng Trọng Phúc", GioiTinh = "M", LoaiGiayTo = "CCCD",
    SoGiayTo = "012345678901", SoDienThoai = "0912345678", Email = "trong.phuc@gmail.com", NgaySinh = DateTime.Today.AddYears(-20) };
ChiTietDatPhong ChiTiet() => new ChiTietDatPhong { MaLoaiPhong = 1, SoLuongPhong = 1, NguoiLon = 2,
    NgayNhanDuKien = DateTime.Today.AddHours(14), NgayTraDuKien = DateTime.Today.AddDays(1).AddHours(12), DonGiaDuKien = 500000 };

Bang(BUS_KiemTra.SoNguyen(" 12 ", "Số phòng"), 12);
Bang(BUS_KiemTra.SoNguyen("0", "Trẻ em"), 0);
foreach (var s in new[] { "", "abc", "1.5", "-1", "1e3", "2147483648", "１２" }) TuChoi(() => BUS_KiemTra.SoNguyen(s, "Số phòng"));
foreach (var culture in new[] { "vi-VN", "en-US" })
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
    Bang(BUS_KiemTra.SoTien("500000"), 500000);
    Bang(BUS_KiemTra.SoTien("10,25"), 10.25m);
    Bang(BUS_KiemTra.SoTien("10.25"), 10.25m);
}
Bang(BUS_KiemTra.SoTien("9999999999999999.99"), 9999999999999999.99m);
foreach (var s in new[] { "", "abc", "-1", "1e3", "10.255", "1,000,000", "1.000.000", "10000000000000000" }) TuChoi(() => BUS_KiemTra.SoTien(s));
TuChoi(() => BUS_KiemTra.Tien(0, "Tiền thu", true));
TuChoi(() => BUS_KiemTra.Tien(0.001m, "Tiền thu", true));
Dung(BUS_KiemTra.DienThoai(" 0912345678 ") == "0912345678");
Dung(BUS_KiemTra.DienThoai("123456789012345") == "123456789012345");
foreach (var s in new[] { "", "0912abc678", "091234567", "1234567890123456", "0912 345678", "+84912345678" }) TuChoi(() => BUS_KiemTra.DienThoai(s));
Dung(BUS_KiemTra.Gmail(" Trong.Phuc@GMAIL.COM ") == "trong.phuc@gmail.com");
foreach (var s in new[] { "", "phuc.gmail.com", "phuc@yahoo.com", "phuc@gmail.com.fake", "phuc @gmail.com", "a..b@gmail.com", "<phuc@gmail.com>", "@gmail.com" }) TuChoi(() => BUS_KiemTra.Gmail(s));
var kh = Khach(); BUS_KhachHang.KiemTraKhachHang(kh); checks++;
kh.HoTen = "Phúc123"; TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh = Khach(); kh.HoTen = " "; TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh = Khach(); kh.NgaySinh = DateTime.Today.AddDays(1); TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh = Khach(); kh.LoaiGiayTo = ""; TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
foreach (var (loai, so) in new[] {
    ("CCCD", "012345678901"), ("CMND", "001234567"), ("CMND", "012345678901"),
    ("Hộ chiếu", "B0123456"), ("Hộ chiếu", "ab1234567"), ("Hộ chiếu", "123456"),
    ("Khác", "AB-001/2026"), ("Khác", new string('A', 50)) })
{
    kh = Khach(); kh.LoaiGiayTo = loai; kh.SoGiayTo = " " + so + " ";
    BUS_KhachHang.KiemTraKhachHang(kh); Dung(kh.SoGiayTo == so);
}
foreach (var (loai, so) in new[] {
    ("CCCD", ""), ("CCCD", "1"), ("CCCD", "01234567890"), ("CCCD", "0123456789012"),
    ("CCCD", "A12345678901"), ("CCCD", "0123 5678901"), ("CCCD", "０１２３４５６７８９０１"),
    ("CCCD", "01234\n5678901"), ("CMND", "12345678"), ("CMND", "1234567890"),
    ("CMND", "12345678901"), ("CMND", "1234567890123"), ("CMND", "12A456789"),
    ("Hộ chiếu", "A1234"), ("Hộ chiếu", "AB12345678"), ("Hộ chiếu", "B 123456"),
    ("Hộ chiếu", "B-123456"), ("Hộ chiếu", "Á1234567"),
    ("Khác", ""), ("Khác", "/---"), ("Khác", "AB 123"), ("Khác", "AB@123"),
    ("Khác", "Á123"), ("Khác", new string('A', 51)) })
{
    kh = Khach(); kh.LoaiGiayTo = loai; kh.SoGiayTo = so;
    TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
}
kh = Khach(); kh.LoaiGiayTo = "Passport"; kh.SoGiayTo = "B0123456";
BUS_KhachHang.KiemTraKhachHang(kh); Dung(kh.LoaiGiayTo == "Hộ chiếu");
kh = Khach(); kh.LoaiGiayTo = "khac"; BUS_KhachHang.KiemTraKhachHang(kh); Dung(kh.LoaiGiayTo == "Khác");
// Đổi loại phải kiểm tra lại mã đang có theo loại vừa chọn.
kh = Khach(); kh.LoaiGiayTo = "Hộ chiếu"; kh.SoGiayTo = "B0123456";
BUS_KhachHang.KiemTraKhachHang(kh); kh.LoaiGiayTo = "CCCD";
TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh = Khach(); kh.LoaiGiayTo = "012345678901"; TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh = Khach(); kh.LoaiGiayTo = "Giấy không có trong danh sách"; TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
kh.DiaChi = new string('a', 301); TuChoi(() => BUS_KhachHang.KiemTraKhachHang(kh));
var ct = ChiTiet(); BUS_DatPhong.KiemTraChiTiet(ct); checks++;
ct.SoLuongPhong = 0; TuChoi(() => BUS_DatPhong.KiemTraChiTiet(ct));
ct = ChiTiet(); ct.NguoiLon = 0; TuChoi(() => BUS_DatPhong.KiemTraChiTiet(ct));
ct = ChiTiet(); ct.TreEm = -1; TuChoi(() => BUS_DatPhong.KiemTraChiTiet(ct));
ct = ChiTiet(); ct.NgayTraDuKien = ct.NgayNhanDuKien; TuChoi(() => BUS_DatPhong.KiemTraChiTiet(ct));
ct = ChiTiet(); ct.DonGiaDuKien = -1; TuChoi(() => BUS_DatPhong.KiemTraChiTiet(ct));
var dp = new DatPhong { MaKhachHangDat = 1 };
TuChoi(() => BUS_DatPhong.KiemTraDatPhong(dp, new()));
ct = ChiTiet(); ct.NgayNhanDuKien = DateTime.Today.AddDays(-1); TuChoi(() => BUS_DatPhong.KiemTraDatPhong(dp, new() { ct }));
BUS_DatPhong.KiemTraDatPhong(dp, new() { ChiTiet() }); checks++;
TuChoi(() => BUS_KiemTra.Ngay(null, "ngày nhận"));
TuChoi(() => BUS_KiemTra.Chon<DatPhong>(null, "đơn đặt phòng"));
TuChoi(() => Bus_PhanBoPhong.KiemTraPhanBo(new XepPhong { MaChiTietDatPhong = 1 }));
Bus_PhanBoPhong.KiemTraPhanBo(new XepPhong { MaChiTietDatPhong = 1, MaPhong = 1 }); checks++;
// Các hàm lưu BUS phải chặn đầu vào trước khi gọi SQL Server.
TuChoi(() => new BUS_DatPhong().LuuDatPhong(new DatPhong(), new()));
TuChoi(() => new BUS_KhachHang().LuuKhachHang(new KhachHang()));
TuChoi(() => new Bus_PhanBoPhong().LuuPhanBo(new XepPhong()));
TuChoi(() => new BUS_HoaDon().TaoHoaDon(0, 1, null, ""));
TuChoi(() => new BUS_HoaDon().ThuTien(1, -1, 1, ""));
TuChoi(() => new BUS_KhachTheoPhong().ChuyenPhong(1, 0, ""));
var uu = new UuDai { LoaiGiam = "PhanTram", GiaTriGiam = 10 };
Bang(BUS_HoaDon.TinhGiam(500000, uu), 50000);
Bang(BUS_HoaDon.TinhGiam(100.05m, uu), 10.01m);
Bang(BUS_HoaDon.TinhGiam(500000, null), 0);
uu.LoaiGiam = "SoTien"; uu.GiaTriGiam = 80000;
Bang(BUS_HoaDon.TinhGiam(500000, uu), 80000);
Console.WriteLine($"PASS: {checks} input-validation and calculation checks. No SQL Server connection used.");
