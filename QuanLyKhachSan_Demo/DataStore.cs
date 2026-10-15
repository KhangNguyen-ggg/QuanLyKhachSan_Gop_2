using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan
{
    public static partial class DataStore
    {
        // 1. Khu vực, Tiện nghi, Phòng
        public static List<KhuVuc> KhuVucs { get; private set; } = new();
        public static List<Tang> Tangs { get; private set; } = new();
        public static List<TienNghi> TienNghis { get; private set; } = new();
        public static List<LoaiPhong> LoaiPhongs { get; private set; } = new();
        public static List<LoaiPhong_TienNghi> LoaiPhongTienNghis { get; private set; } = new();
        public static List<Phong> Phongs { get; private set; } = new();

        // 2. Khách hàng & Đặt phòng
        public static List<KhachHang> KhachHangs { get; private set; } = new();
        public static List<DatPhong> DatPhongs { get; private set; } = new();
        public static List<ChiTietDatPhong> ChiTietDatPhongs { get; private set; } = new();
        public static List<XepPhong> XepPhongs { get; private set; } = new();

        // 3. Dịch vụ
        public static List<LoaiDichVu> LoaiDichVus { get; private set; } = new();
        public static List<DichVu> DichVus { get; private set; } = new();
        public static List<GoiDichVu> GoiDichVus { get; private set; } = new();
        public static List<ChiTietGoiDichVu> ChiTietGoiDichVus { get; private set; } = new();
        public static List<SuDungDichVu> SuDungDichVus { get; private set; } = new();

        // 4. Ưu đãi & Hóa đơn & Thanh toán
        public static List<UuDai> UuDais { get; private set; } = new();
        public static List<HoaDon> HoaDons { get; private set; } = new();
        public static List<ChiTietHoaDon> ChiTietHoaDons { get; private set; } = new();
        public static List<PhuongThucThanhToan> PhuongThucThanhToans { get; private set; } = new();
        public static List<ThanhToan> ThanhToans { get; private set; } = new();

        // 5. Nhân sự
        public static List<PhongBan> PhongBans { get; private set; } = new();
        public static List<ChucVu> ChucVus { get; private set; } = new();
        public static List<NhanVien> NhanViens { get; private set; } = new();
        public static List<CaLamViec> CaLamViecs { get; private set; } = new();
        public static List<LichLamViec> LichLamViecs { get; private set; } = new();
        public static List<ChamCong> ChamCongs { get; private set; } = new();
        public static List<BangLuong> BangLuongs { get; private set; } = new();
        public static List<CongViecDonPhong> CongViecDonPhongs { get; private set; } = new();

        // 6. Bảo trì & Thiết bị
        public static List<ThietBi> ThietBis { get; private set; } = new();
        public static List<BaoTriPhong> BaoTriPhongs { get; private set; } = new();

        // 7. Kho & Vật tư
        public static List<Kho> Khos { get; private set; } = new();
        public static List<NhomVatTu> NhomVatTus { get; private set; } = new();
        public static List<VatTu> VatTus { get; private set; } = new();
        public static List<TieuHaoVatTu> TieuHaoVatTus { get; private set; } = new();
        public static List<GiaoDichKho> GiaoDichKhos { get; private set; } = new();
        public static List<ChiTietGiaoDichKho> ChiTietGiaoDichKhos { get; private set; } = new();

        // 8. Nhà cung cấp
        public static List<NhaCungCap> NhaCungCaps { get; private set; } = new();
        public static List<NhanVienNhaCungCap> NhanVienNhaCungCaps { get; private set; } = new();
        public static List<VatTu_NhaCungCap> VatTuNhaCungCaps { get; private set; } = new();

        // 9. Phân quyền
        public static List<TaiKhoan> TaiKhoans { get; private set; } = new();
        public static List<VaiTro> VaiTros { get; private set; } = new();
        public static List<Quyen> Quyens { get; private set; } = new();
        public static List<VaiTro_Quyen> VaiTroQuyens { get; private set; } = new();
        public static List<NhatKyThaoTac> NhatKyThaoTacs { get; private set; } = new();

        // Helpers for ID Generation
        private static int _nextPhongId = 50;
        private static int _nextKhachHangId = 20;
        private static int _nextNhanVienId = 20;
        private static long _nextDatPhongId = 20;
        private static long _nextHoaDonId = 20;
        public static int NextPhongId() => ++_nextPhongId;
        public static int NextKhachHangId() => ++_nextKhachHangId;
        public static int NextNhanVienId() => ++_nextNhanVienId;
        public static long NextDatPhongId() => ++_nextDatPhongId;
        public static long NextHoaDonId() => ++_nextHoaDonId;

        static DataStore() { Seed(); }

        private static void Seed()
        {
            var today = DateTime.Today;

            // 1. Khu vực, Tiện nghi, Phòng
            KhuVucs = new List<KhuVuc>
            {
                new() { MaKhuVuc = 1, TenKhuVuc = "Tòa A - Trung tâm", MoTa = "Khu vực chính" },
                new() { MaKhuVuc = 2, TenKhuVuc = "Tòa B - Dịch vụ", MoTa = "Khu vực nhà hàng và Spa" }
            };

            Tangs = new List<Tang>
            {
                new() { MaTang = 1, MaKhuVuc = 1, SoTang = 1, TenTang = "Tầng 1", MoTa = "Sảnh tiếp đón" },
                new() { MaTang = 2, MaKhuVuc = 1, SoTang = 2, TenTang = "Tầng 2", MoTa = "Phòng Standard" },
                new() { MaTang = 3, MaKhuVuc = 1, SoTang = 3, TenTang = "Tầng 3", MoTa = "Phòng Deluxe" },
                new() { MaTang = 4, MaKhuVuc = 1, SoTang = 4, TenTang = "Tầng 4", MoTa = "Phòng Suite" },
                new() { MaTang = 5, MaKhuVuc = 1, SoTang = 5, TenTang = "Tầng 5", MoTa = "Presidential Suite" },
            };

            LoaiPhongs = new List<LoaiPhong>
            {
                new() { MaLoaiPhong = 1, TenLoaiPhong = "Standard", GiaCoBan = 2_500_000, SucChuaNguoiLon = 2, SucChuaTreEm = 1, DienTich = 28, MoTa = "Phòng Standard tiêu chuẩn" },
                new() { MaLoaiPhong = 2, TenLoaiPhong = "Deluxe", GiaCoBan = 4_500_000, SucChuaNguoiLon = 2, SucChuaTreEm = 2, DienTich = 38, MoTa = "Phòng Deluxe view thành phố" },
                new() { MaLoaiPhong = 3, TenLoaiPhong = "Junior Suite", GiaCoBan = 7_000_000, SucChuaNguoiLon = 3, SucChuaTreEm = 2, DienTich = 55, MoTa = "Suite nhỏ với phòng khách riêng" },
                new() { MaLoaiPhong = 4, TenLoaiPhong = "Suite", GiaCoBan = 12_000_000, SucChuaNguoiLon = 4, SucChuaTreEm = 2, DienTich = 75, MoTa = "Suite cao cấp đầy đủ tiện nghi" },
                new() { MaLoaiPhong = 5, TenLoaiPhong = "Presidential Suite", GiaCoBan = 25_000_000, SucChuaNguoiLon = 6, SucChuaTreEm = 4, DienTich = 150, MoTa = "Presidential Suite đẳng cấp" },
            };

            Phongs = new List<Phong>();
            string[] vesinhs = { "SACH", "SACH", "BAN", "SACH", "DANG_DON", "SACH" };
            int id = 1;
            for (int i = 1; i <= 12; i++)
                Phongs.Add(new Phong { MaPhong = id++, MaTang = 2, MaLoaiPhong = 1, SoPhong = $"2{i:D2}", LoaiView = "Thành phố", TrangThaiVeSinh = vesinhs[i % 6], TrangThaiKhaiThac = i == 12 ? "BAO_TRI" : "DANG_HOAT_DONG" });
            for (int i = 1; i <= 12; i++)
                Phongs.Add(new Phong { MaPhong = id++, MaTang = 3, MaLoaiPhong = 2, SoPhong = $"3{i:D2}", LoaiView = "Hồ bơi", TrangThaiVeSinh = vesinhs[(i + 1) % 6], TrangThaiKhaiThac = "DANG_HOAT_DONG" });
            for (int i = 1; i <= 10; i++)
                Phongs.Add(new Phong { MaPhong = id++, MaTang = 4, MaLoaiPhong = i <= 7 ? 3 : 4, SoPhong = $"4{i:D2}", LoaiView = "Biển", TrangThaiVeSinh = vesinhs[(i + 2) % 6], TrangThaiKhaiThac = "DANG_HOAT_DONG" });
            for (int i = 1; i <= 4; i++)
                Phongs.Add(new Phong { MaPhong = id++, MaTang = 5, MaLoaiPhong = 5, SoPhong = $"5{i:D2}", LoaiView = "Toàn cảnh", TrangThaiVeSinh = i == 1 ? "BAN" : "SACH", TrangThaiKhaiThac = "DANG_HOAT_DONG" });

            // 2. Khách hàng
            KhachHangs = new List<KhachHang>
            {
                new() { MaKhachHang = 1, HoTen = "Nguyễn Văn An", GioiTinh = "M", NgaySinh = new DateTime(1985,3,15), LoaiGiayTo = "CCCD", SoGiayTo = "079085012345", QuocTich = "Việt Nam", DienThoai = "0901234567", Email = "nguyenvanan@gmail.com", HangKhachHang = "VANG", NgayTao = today.AddYears(-2) },
                new() { MaKhachHang = 2, HoTen = "Trần Thị Bình", GioiTinh = "F", NgaySinh = new DateTime(1990,7,20), LoaiGiayTo = "CCCD", SoGiayTo = "079090067890", QuocTich = "Việt Nam", DienThoai = "0912345678", Email = "tranthib@gmail.com", HangKhachHang = "BAC", NgayTao = today.AddYears(-1) },
                new() { MaKhachHang = 3, HoTen = "Lê Hoàng Cường", GioiTinh = "M", NgaySinh = new DateTime(1978,11,5), LoaiGiayTo = "CCCD", SoGiayTo = "079078034567", QuocTich = "Việt Nam", DienThoai = "0923456789", Email = "lehcuong@yahoo.com", HangKhachHang = "KIM_CUONG", NgayTao = today.AddYears(-3) },
                new() { MaKhachHang = 4, HoTen = "Phạm Thị Dung", GioiTinh = "F", NgaySinh = new DateTime(1995,2,28), LoaiGiayTo = "CCCD", SoGiayTo = "079095056789", QuocTich = "Việt Nam", DienThoai = "0934567890", Email = "phamtdung@gmail.com", HangKhachHang = "THUONG", NgayTao = today.AddMonths(-6) },
                new() { MaKhachHang = 5, HoTen = "Hoàng Văn Em", GioiTinh = "M", NgaySinh = new DateTime(1982,8,12), LoaiGiayTo = "CCCD", SoGiayTo = "079082078901", QuocTich = "Việt Nam", DienThoai = "0945678901", Email = "hvanhem@gmail.com", HangKhachHang = "VANG", NgayTao = today.AddYears(-1) },
                new() { MaKhachHang = 6, HoTen = "John Smith", GioiTinh = "M", NgaySinh = new DateTime(1975,6,18), LoaiGiayTo = "PASSPORT", SoGiayTo = "USA123456789", QuocTich = "Hoa Kỳ", DienThoai = "+14155552671", Email = "john.smith@gmail.com", HangKhachHang = "VANG", NgayTao = today.AddYears(-1) },
                new() { MaKhachHang = 7, HoTen = "Tanaka Hiroshi", GioiTinh = "M", NgaySinh = new DateTime(1988,4,25), LoaiGiayTo = "PASSPORT", SoGiayTo = "JPN987654321", QuocTich = "Nhật Bản", DienThoai = "+81901234567", Email = "tanaka.h@jp.co", HangKhachHang = "BAC", NgayTao = today.AddMonths(-8) },
                new() { MaKhachHang = 8, HoTen = "Marie Dupont", GioiTinh = "F", NgaySinh = new DateTime(1992,9,10), LoaiGiayTo = "PASSPORT", SoGiayTo = "FRA456789012", QuocTich = "Pháp", DienThoai = "+33612345678", Email = "marie.dupont@fr.com", HangKhachHang = "THUONG", NgayTao = today.AddMonths(-3) },
                new() { MaKhachHang = 9, HoTen = "Vũ Thị Hoa", GioiTinh = "F", NgaySinh = new DateTime(1998,1,3), LoaiGiayTo = "CCCD", SoGiayTo = "079098089012", QuocTich = "Việt Nam", DienThoai = "0956789012", Email = "vuthoa98@gmail.com", HangKhachHang = "THUONG", NgayTao = today.AddMonths(-4) },
                new() { MaKhachHang = 10, HoTen = "Đỗ Minh Khôi", GioiTinh = "M", NgaySinh = new DateTime(1987,12,20), LoaiGiayTo = "CCCD", SoGiayTo = "079087001234", QuocTich = "Việt Nam", DienThoai = "0967890123", Email = "domkhoi@company.vn", HangKhachHang = "KIM_CUONG", NgayTao = today.AddYears(-2) },
                new() { MaKhachHang = 11, HoTen = "Bùi Thị Lan", GioiTinh = "F", NgaySinh = new DateTime(1993,5,7), LoaiGiayTo = "CCCD", SoGiayTo = "079093023456", QuocTich = "Việt Nam", DienThoai = "0978901234", Email = "builan93@gmail.com", HangKhachHang = "BAC", NgayTao = today.AddMonths(-9) },
                new() { MaKhachHang = 12, HoTen = "Kim Min Jun", GioiTinh = "M", NgaySinh = new DateTime(1991,8,15), LoaiGiayTo = "PASSPORT", SoGiayTo = "KOR234567890", QuocTich = "Hàn Quốc", DienThoai = "+821012345678", Email = "minjun.kim@kor.net", HangKhachHang = "VANG", NgayTao = today.AddMonths(-5) },
                new() { MaKhachHang = 13, HoTen = "Nguyễn Thị Mai", GioiTinh = "F", NgaySinh = new DateTime(1986,3,22), LoaiGiayTo = "CCCD", SoGiayTo = "079086045678", QuocTich = "Việt Nam", DienThoai = "0989012345", Email = "ntmai86@hotmail.com", HangKhachHang = "VANG", NgayTao = today.AddYears(-1) },
                new() { MaKhachHang = 14, HoTen = "Trịnh Công Nam", GioiTinh = "M", NgaySinh = new DateTime(1980,10,30), LoaiGiayTo = "CCCD", SoGiayTo = "079080067890", QuocTich = "Việt Nam", DienThoai = "0990123456", Email = "tcnam80@gmail.com", HangKhachHang = "BAC", NgayTao = today.AddMonths(-7) },
                new() { MaKhachHang = 15, HoTen = "Sarah Johnson", GioiTinh = "F", NgaySinh = new DateTime(1989,12,5), LoaiGiayTo = "PASSPORT", SoGiayTo = "GBR567890123", QuocTich = "Anh", DienThoai = "+447712345678", Email = "sarah.j@uk.co", HangKhachHang = "THUONG", NgayTao = today.AddMonths(-2) },
            };

            // 5. Nhân sự
            PhongBans = new List<PhongBan>
            {
                new() { MaPhongBan = 1, TenPhongBan = "Lễ tân", MoTa = "Tiếp đón và phục vụ khách" },
                new() { MaPhongBan = 2, TenPhongBan = "Buồng phòng", MoTa = "Vệ sinh và quản lý phòng" },
                new() { MaPhongBan = 3, TenPhongBan = "Nhà hàng & Bar", MoTa = "Dịch vụ ẩm thực" },
                new() { MaPhongBan = 4, TenPhongBan = "Kỹ thuật", MoTa = "Bảo trì và sửa chữa" },
                new() { MaPhongBan = 5, TenPhongBan = "An ninh", MoTa = "Bảo vệ và an toàn" },
                new() { MaPhongBan = 6, TenPhongBan = "Ban giám đốc", MoTa = "Quản lý khách sạn" },
            };

            ChucVus = new List<ChucVu>
            {
                new() { MaChucVu = 1, TenChucVu = "Giám đốc" },
                new() { MaChucVu = 2, TenChucVu = "Trưởng phòng" },
                new() { MaChucVu = 3, TenChucVu = "Lễ tân viên" },
                new() { MaChucVu = 4, TenChucVu = "Nhân viên buồng phòng" },
                new() { MaChucVu = 5, TenChucVu = "Bếp trưởng" },
                new() { MaChucVu = 6, TenChucVu = "Phục vụ" },
                new() { MaChucVu = 7, TenChucVu = "Kỹ thuật viên" },
                new() { MaChucVu = 8, TenChucVu = "Bảo vệ" },
            };

            NhanViens = new List<NhanVien>
            {
                new() { MaNhanVien=1, MaPhongBan=6, MaChucVu=1, HoTen="Nguyễn Đức Thắng", GioiTinh="M", NgaySinh=new DateTime(1975,5,10), SoDienThoai="0901000001", Email="giamdoc@hotel.vn", NgayVaoLam=new DateTime(2020,1,1), LuongCoBan=50_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=2, MaPhongBan=1, MaChucVu=2, HoTen="Trần Thị Thu", GioiTinh="F", NgaySinh=new DateTime(1988,8,25), SoDienThoai="0901000002", Email="thu.le@hotel.vn", NgayVaoLam=new DateTime(2020,3,1), LuongCoBan=25_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=3, MaPhongBan=1, MaChucVu=3, HoTen="Lê Văn Bảo", GioiTinh="M", NgaySinh=new DateTime(1995,2,14), SoDienThoai="0901000003", Email="bao.le@hotel.vn", NgayVaoLam=new DateTime(2021,6,1), LuongCoBan=12_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=4, MaPhongBan=1, MaChucVu=3, HoTen="Phạm Thị Cẩm", GioiTinh="F", NgaySinh=new DateTime(1997,11,30), SoDienThoai="0901000004", Email="cam.pham@hotel.vn", NgayVaoLam=new DateTime(2021,9,1), LuongCoBan=11_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=5, MaPhongBan=2, MaChucVu=2, HoTen="Hoàng Thị Duyên", GioiTinh="F", NgaySinh=new DateTime(1985,7,7), SoDienThoai="0901000005", Email="duyen.hoang@hotel.vn", NgayVaoLam=new DateTime(2020,2,1), LuongCoBan=22_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=6, MaPhongBan=2, MaChucVu=4, HoTen="Vũ Thị Hằng", GioiTinh="F", NgaySinh=new DateTime(1993,4,18), SoDienThoai="0901000006", Email="hang.vu@hotel.vn", NgayVaoLam=new DateTime(2022,1,1), LuongCoBan=9_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=7, MaPhongBan=2, MaChucVu=4, HoTen="Đặng Văn Giàu", GioiTinh="M", NgaySinh=new DateTime(1990,9,22), SoDienThoai="0901000007", Email="giau.dang@hotel.vn", NgayVaoLam=new DateTime(2021,3,1), LuongCoBan=9_500_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=8, MaPhongBan=3, MaChucVu=5, HoTen="Bùi Văn Hải", GioiTinh="M", NgaySinh=new DateTime(1980,12,3), SoDienThoai="0901000008", Email="hai.bui@hotel.vn", NgayVaoLam=new DateTime(2020,1,15), LuongCoBan=30_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=9, MaPhongBan=3, MaChucVu=6, HoTen="Đinh Thị Ích", GioiTinh="F", NgaySinh=new DateTime(1998,6,11), SoDienThoai="0901000009", Email="ich.dinh@hotel.vn", NgayVaoLam=new DateTime(2022,5,1), LuongCoBan=8_500_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=10, MaPhongBan=3, MaChucVu=6, HoTen="Lý Văn Khánh", GioiTinh="M", NgaySinh=new DateTime(1996,3,27), SoDienThoai="0901000010", Email="khanh.ly@hotel.vn", NgayVaoLam=new DateTime(2022,7,1), LuongCoBan=8_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=11, MaPhongBan=4, MaChucVu=7, HoTen="Mai Văn Lợi", GioiTinh="M", NgaySinh=new DateTime(1982,1,15), SoDienThoai="0901000011", Email="loi.mai@hotel.vn", NgayVaoLam=new DateTime(2020,4,1), LuongCoBan=15_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=12, MaPhongBan=4, MaChucVu=7, HoTen="Ngô Thị Mẫn", GioiTinh="F", NgaySinh=new DateTime(1989,10,8), SoDienThoai="0901000012", Email="man.ngo@hotel.vn", NgayVaoLam=new DateTime(2021,1,1), LuongCoBan=13_000_000, TrangThai="TAM_NGHI" },
                new() { MaNhanVien=13, MaPhongBan=5, MaChucVu=2, HoTen="Phan Văn Nam", GioiTinh="M", NgaySinh=new DateTime(1978,5,20), SoDienThoai="0901000013", Email="nam.phan@hotel.vn", NgayVaoLam=new DateTime(2020,1,1), LuongCoBan=20_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=14, MaPhongBan=5, MaChucVu=8, HoTen="Quách Thị Oanh", GioiTinh="F", NgaySinh=new DateTime(1994,8,16), SoDienThoai="0901000014", Email="oanh.quach@hotel.vn", NgayVaoLam=new DateTime(2022,3,1), LuongCoBan=10_000_000, TrangThai="DANG_LAM" },
                new() { MaNhanVien=15, MaPhongBan=5, MaChucVu=8, HoTen="Rạng Văn Phú", GioiTinh="M", NgaySinh=new DateTime(1991,2,28), SoDienThoai="0901000015", Email="phu.rang@hotel.vn", NgayVaoLam=new DateTime(2021,11,1), LuongCoBan=10_500_000, TrangThai="DANG_LAM" },
            };

            DatPhongs = new List<DatPhong>
            {
                new() { MaDatPhong=1, MaKhachHangDat=1, MaNhanVienTao=3, NgayDat=today.AddDays(-10), TrangThai="DA_TRA_PHONG", GhiChu="Khách VIP" },
                new() { MaDatPhong=2, MaKhachHangDat=3, MaNhanVienTao=3, NgayDat=today.AddDays(-5), TrangThai="DA_NHAN_PHONG", GhiChu="Đặt qua website" },
                new() { MaDatPhong=3, MaKhachHangDat=6, MaNhanVienTao=4, NgayDat=today.AddDays(-3), TrangThai="DA_NHAN_PHONG", GhiChu="Business trip" },
                new() { MaDatPhong=4, MaKhachHangDat=2, MaNhanVienTao=3, NgayDat=today.AddDays(-1), TrangThai="DA_XAC_NHAN", GhiChu="" },
                new() { MaDatPhong=5, MaKhachHangDat=7, MaNhanVienTao=4, NgayDat=today, TrangThai="DA_XAC_NHAN", GhiChu="Honeymoon" },
                new() { MaDatPhong=6, MaKhachHangDat=10, MaNhanVienTao=3, NgayDat=today, TrangThai="CHO_XAC_NHAN", GhiChu="Đặt trực tiếp" },
                new() { MaDatPhong=7, MaKhachHangDat=5, MaNhanVienTao=4, NgayDat=today.AddDays(-20), TrangThai="DA_TRA_PHONG", GhiChu="" },
                new() { MaDatPhong=8, MaKhachHangDat=12, MaNhanVienTao=3, NgayDat=today.AddDays(-7), TrangThai="DA_NHAN_PHONG", GhiChu="Korean delegation" },
                new() { MaDatPhong=9, MaKhachHangDat=8, MaNhanVienTao=4, NgayDat=today.AddDays(2), TrangThai="DA_XAC_NHAN", GhiChu="Conference" },
                new() { MaDatPhong=10, MaKhachHangDat=15, MaNhanVienTao=3, NgayDat=today.AddDays(1), TrangThai="CHO_XAC_NHAN", GhiChu="" },
                new() { MaDatPhong=11, MaKhachHangDat=4, MaNhanVienTao=4, NgayDat=today.AddDays(-15), TrangThai="HUY", GhiChu="Khách huỷ không lý do" },
                new() { MaDatPhong=12, MaKhachHangDat=9, MaNhanVienTao=3, NgayDat=today.AddDays(-2), TrangThai="DA_NHAN_PHONG", GhiChu="" },
                new() { MaDatPhong=13, MaKhachHangDat=11, MaNhanVienTao=4, NgayDat=today.AddDays(3), TrangThai="DA_XAC_NHAN", GhiChu="Anniversary" },
                new() { MaDatPhong=14, MaKhachHangDat=13, MaNhanVienTao=3, NgayDat=today.AddDays(-30), TrangThai="DA_TRA_PHONG", GhiChu="" },
                new() { MaDatPhong=15, MaKhachHangDat=14, MaNhanVienTao=4, NgayDat=today.AddDays(5), TrangThai="CHO_XAC_NHAN", GhiChu="Corporate rate" },
            };

            ChiTietDatPhongs = new List<ChiTietDatPhong>
            {
                new() { MaChiTietDatPhong=1, MaDatPhong=1, MaLoaiPhong=2, NgayNhanDuKien=today.AddDays(-10), NgayTraDuKien=today.AddDays(-7), SoLuongPhong=1, NguoiLon=2, TreEm=0, DonGiaPhong=4_500_000 },
                new() { MaChiTietDatPhong=2, MaDatPhong=2, MaLoaiPhong=4, NgayNhanDuKien=today.AddDays(-5), NgayTraDuKien=today.AddDays(2), SoLuongPhong=1, NguoiLon=2, TreEm=1, DonGiaPhong=12_000_000 },
                new() { MaChiTietDatPhong=3, MaDatPhong=3, MaLoaiPhong=2, NgayNhanDuKien=today.AddDays(-3), NgayTraDuKien=today.AddDays(1), SoLuongPhong=2, NguoiLon=4, TreEm=0, DonGiaPhong=4_500_000 },
                new() { MaChiTietDatPhong=4, MaDatPhong=4, MaLoaiPhong=1, NgayNhanDuKien=today.AddDays(1), NgayTraDuKien=today.AddDays(3), SoLuongPhong=1, NguoiLon=2, TreEm=0, DonGiaPhong=2_500_000 },
                new() { MaChiTietDatPhong=5, MaDatPhong=5, MaLoaiPhong=3, NgayNhanDuKien=today.AddDays(2), NgayTraDuKien=today.AddDays(7), SoLuongPhong=1, NguoiLon=2, TreEm=0, DonGiaPhong=7_000_000 },
                new() { MaChiTietDatPhong=6, MaDatPhong=6, MaLoaiPhong=1, NgayNhanDuKien=today.AddDays(3), NgayTraDuKien=today.AddDays(5), SoLuongPhong=1, NguoiLon=1, TreEm=0, DonGiaPhong=2_500_000 },
                new() { MaChiTietDatPhong=7, MaDatPhong=7, MaLoaiPhong=2, NgayNhanDuKien=today.AddDays(-20), NgayTraDuKien=today.AddDays(-15), SoLuongPhong=1, NguoiLon=2, TreEm=2, DonGiaPhong=4_500_000 },
                new() { MaChiTietDatPhong=8, MaDatPhong=8, MaLoaiPhong=3, NgayNhanDuKien=today.AddDays(-7), NgayTraDuKien=today.AddDays(1), SoLuongPhong=1, NguoiLon=3, TreEm=0, DonGiaPhong=7_000_000 },
                new() { MaChiTietDatPhong=9, MaDatPhong=12, MaLoaiPhong=2, NgayNhanDuKien=today.AddDays(-2), NgayTraDuKien=today.AddDays(3), SoLuongPhong=1, NguoiLon=2, TreEm=0, DonGiaPhong=4_500_000 },
                new() { MaChiTietDatPhong=10, MaDatPhong=14, MaLoaiPhong=5, NgayNhanDuKien=today.AddDays(-30), NgayTraDuKien=today.AddDays(-25), SoLuongPhong=1, NguoiLon=4, TreEm=2, DonGiaPhong=25_000_000 },
            };

            XepPhongs = new List<XepPhong>
            {
                new() { MaXepPhong=1, MaChiTietDatPhong=2, MaPhong=13, NgayNhanThucTe=today.AddDays(-5), TrangThai="DA_NHAN_PHONG" },
                new() { MaXepPhong=2, MaChiTietDatPhong=3, MaPhong=14, NgayNhanThucTe=today.AddDays(-3), TrangThai="DA_NHAN_PHONG" },
                new() { MaXepPhong=3, MaChiTietDatPhong=3, MaPhong=15, NgayNhanThucTe=today.AddDays(-3), TrangThai="DA_NHAN_PHONG" },
                new() { MaXepPhong=4, MaChiTietDatPhong=8, MaPhong=25, NgayNhanThucTe=today.AddDays(-7), TrangThai="DA_NHAN_PHONG" },
                new() { MaXepPhong=5, MaChiTietDatPhong=9, MaPhong=16, NgayNhanThucTe=today.AddDays(-2), TrangThai="DA_NHAN_PHONG" },
            };

            LoaiDichVus = new List<LoaiDichVu>
            {
                new() { MaLoaiDichVu=1, TenLoaiDichVu="Ẩm thực", MoTa="Nhà hàng, Bar, Room Service" },
                new() { MaLoaiDichVu=2, TenLoaiDichVu="Spa & Sức khoẻ", MoTa="Spa, gym, hồ bơi" },
                new() { MaLoaiDichVu=3, TenLoaiDichVu="Vận chuyển", MoTa="Đưa đón sân bay, thuê xe" },
                new() { MaLoaiDichVu=4, TenLoaiDichVu="Giải trí", MoTa="Tour, tham quan" },
                new() { MaLoaiDichVu=5, TenLoaiDichVu="Tiện ích phòng", MoTa="Giặt là, minibar" },
            };

            DichVus = new List<DichVu>
            {
                new() { MaDichVu=1, MaLoaiDichVu=1, TenDichVu="Bữa sáng buffet", Gia=350_000, DonViTinh="Người", MoTa="Buffet sáng tại nhà hàng chính" },
                new() { MaDichVu=2, MaLoaiDichVu=1, TenDichVu="Bữa tối đặc biệt", Gia=850_000, DonViTinh="Người", MoTa="Set menu cao cấp" },
                new() { MaDichVu=3, MaLoaiDichVu=1, TenDichVu="Room Service", Gia=150_000, DonViTinh="Lần", MoTa="Phí giao đồ ăn tận phòng" },
                new() { MaDichVu=4, MaLoaiDichVu=2, TenDichVu="Massage 60 phút", Gia=1_200_000, DonViTinh="Lần", MoTa="Massage full-body thư giãn" },
                new() { MaDichVu=5, MaLoaiDichVu=2, TenDichVu="Hồ bơi vô cực", Gia=200_000, DonViTinh="Người/ngày", MoTa="Hồ bơi rooftop view thành phố" },
                new() { MaDichVu=6, MaLoaiDichVu=2, TenDichVu="Phòng gym", Gia=150_000, DonViTinh="Người/ngày", MoTa="Phòng tập với máy hiện đại" },
                new() { MaDichVu=7, MaLoaiDichVu=3, TenDichVu="Đón/Tiễn sân bay", Gia=800_000, DonViTinh="Chuyến", MoTa="Xe Mercedes đón sân bay Tân Sơn Nhất" },
                new() { MaDichVu=8, MaLoaiDichVu=3, TenDichVu="Thuê xe có tài xế", Gia=1_500_000, DonViTinh="Ngày", MoTa="Xe sang có tài xế riêng" },
                new() { MaDichVu=9, MaLoaiDichVu=4, TenDichVu="Tour thành phố half-day", Gia=1_200_000, DonViTinh="Người", MoTa="Tham quan điểm du lịch nổi tiếng" },
                new() { MaDichVu=10, MaLoaiDichVu=5, TenDichVu="Giặt là Express", Gia=200_000, DonViTinh="Bộ", MoTa="Giặt là trong 3 giờ" },
                new() { MaDichVu=11, MaLoaiDichVu=5, TenDichVu="Minibar", Gia=100_000, DonViTinh="Lần", MoTa="Bổ sung đồ uống minibar" },
                new() { MaDichVu=12, MaLoaiDichVu=2, TenDichVu="Facial Treatment", Gia=900_000, DonViTinh="Lần", MoTa="Chăm sóc da mặt cao cấp" },
            };

            GoiDichVus = new List<GoiDichVu>
            {
                new() { MaGoiDichVu=1, TenGoiDichVu="Gói Tuần trăng mật 💍", Gia=5_000_000, MoTa="Bữa tối lãng mạn, Massage đôi, Hoa tươi, Rượu vang Pháp" },
                new() { MaGoiDichVu=2, TenGoiDichVu="Gói Gia đình vui vẻ 👨‍👩‍👧‍👦", Gia=3_500_000, MoTa="Sáng buffet gia đình, Vé hồ bơi, Tour thành phố" },
                new() { MaGoiDichVu=3, TenGoiDichVu="Gói Doanh nhân 💼", Gia=2_500_000, MoTa="Sáng sớm, Phòng họp 2h, Đón/tiễn sân bay" },
                new() { MaGoiDichVu=4, TenGoiDichVu="Gói Sức khoẻ & Thư giãn 🧘", Gia=4_000_000, MoTa="Spa toàn thân, Gym, Sauna, Healthy breakfast" },
            };

            UuDais = new List<UuDai>
            {
                new() { MaUuDai=1, TenUuDai="Khuyến mãi Cuối tuần", MaCode="WEEKEND20", LoaiGiam="PHAN_TRAM", GiaTriGiam=20, GiaTriGiamToiDa=2_000_000, GiaTriDonToiThieu=5_000_000, NgayBatDau=today.AddDays(-30), NgayKetThuc=today.AddDays(30), TrangThai=true },
                new() { MaUuDai=2, TenUuDai="Chào mừng khách mới", MaCode="WELCOME10", LoaiGiam="PHAN_TRAM", GiaTriGiam=10, GiaTriGiamToiDa=1_000_000, NgayBatDau=today.AddDays(-60), NgayKetThuc=today.AddDays(60), TrangThai=true },
                new() { MaUuDai=3, TenUuDai="Giảm tiền mặt 500K", MaCode="SAVE500", LoaiGiam="SO_TIEN", GiaTriGiam=500_000, GiaTriDonToiThieu=3_000_000, NgayBatDau=today.AddDays(-10), NgayKetThuc=today.AddDays(20), TrangThai=true },
                new() { MaUuDai=4, TenUuDai="VIP Member 15%", MaCode="VIP15", LoaiGiam="PHAN_TRAM", GiaTriGiam=15, GiaTriGiamToiDa=5_000_000, GiaTriDonToiThieu=10_000_000, NgayBatDau=today.AddDays(-90), NgayKetThuc=today.AddDays(90), TrangThai=true },
                new() { MaUuDai=5, TenUuDai="Flash Sale 30%", MaCode="FLASH30", LoaiGiam="PHAN_TRAM", GiaTriGiam=30, GiaTriGiamToiDa=3_000_000, NgayBatDau=today.AddDays(-5), NgayKetThuc=today.AddDays(-1), TrangThai=false },
            };

            PhuongThucThanhToans = new List<PhuongThucThanhToan>
            {
                new() { MaPhuongThucThanhToan = 1, TenPhuongThuc = "Tiền mặt" },
                new() { MaPhuongThucThanhToan = 2, TenPhuongThuc = "Thẻ tín dụng" },
                new() { MaPhuongThucThanhToan = 3, TenPhuongThuc = "Chuyển khoản" },
                new() { MaPhuongThucThanhToan = 4, TenPhuongThuc = "Ví điện tử" }
            };

            HoaDons = new List<HoaDon>
            {
                new() { MaHoaDon=1, MaDatPhong=1, MaKhachHang=1, MaNhanVienLap=3, NgayLap=today.AddDays(-7), TongTienHang=13_500_000, TienGiam=0, TongThanhToan=13_500_000, TrangThai="DA_THANH_TOAN" },
                new() { MaHoaDon=2, MaDatPhong=7, MaKhachHang=5, MaNhanVienLap=4, NgayLap=today.AddDays(-15), TongTienHang=22_500_000, TienGiam=0, TongThanhToan=22_500_000, TrangThai="DA_THANH_TOAN" },
                new() { MaHoaDon=3, MaDatPhong=14, MaKhachHang=13, MaUuDai=4, MaNhanVienLap=3, NgayLap=today.AddDays(-25), TongTienHang=125_000_000, TienGiam=5_000_000, TongThanhToan=120_000_000, TrangThai="DA_THANH_TOAN", GhiChu="VIP" },
                new() { MaHoaDon=4, MaDatPhong=2, MaKhachHang=3, MaUuDai=1, MaNhanVienLap=4, NgayLap=today, TongTienHang=84_000_000, TienGiam=2_000_000, TongThanhToan=82_000_000, TrangThai="CHUA_THANH_TOAN", GhiChu="Đang ở" },
                new() { MaHoaDon=5, MaDatPhong=3, MaKhachHang=6, MaNhanVienLap=3, NgayLap=today, TongTienHang=36_000_000, TienGiam=0, TongThanhToan=36_000_000, TrangThai="THANH_TOAN_MOT_PHAN", GhiChu="Đặt cọc 50%" },
                new() { MaHoaDon=6, MaDatPhong=8, MaKhachHang=12, MaNhanVienLap=4, NgayLap=today.AddDays(-1), TongTienHang=56_000_000, TienGiam=0, TongThanhToan=56_000_000, TrangThai="CHUA_THANH_TOAN" },
                new() { MaHoaDon=7, MaDatPhong=12, MaKhachHang=9, MaUuDai=3, MaNhanVienLap=3, NgayLap=today, TongTienHang=22_500_000, TienGiam=500_000, TongThanhToan=22_000_000, TrangThai="CHUA_THANH_TOAN" },
                new() { MaHoaDon=8, MaDatPhong=11, MaKhachHang=4, MaNhanVienLap=4, NgayLap=today.AddDays(-14), TongTienHang=5_000_000, TienGiam=0, TongThanhToan=5_000_000, TrangThai="HUY", GhiChu="Khách huỷ" },
            };

            ThanhToans = new List<ThanhToan>
            {
                new() { MaThanhToan=1, MaHoaDon=1, MaNhanVienThu=3, MaPhuongThucThanhToan=1, NgayThanhToan=today.AddDays(-7), SoTien=13_500_000, PhuongThuc="TIEN_MAT", TrangThai="DA_GHI_NHAN" },
                new() { MaThanhToan=2, MaHoaDon=2, MaNhanVienThu=4, MaPhuongThucThanhToan=2, NgayThanhToan=today.AddDays(-15), SoTien=22_500_000, PhuongThuc="THE_TIN_DUNG", TrangThai="DA_GHI_NHAN" },
                new() { MaThanhToan=3, MaHoaDon=3, MaNhanVienThu=3, MaPhuongThucThanhToan=3, NgayThanhToan=today.AddDays(-25), SoTien=120_000_000, PhuongThuc="CHUYEN_KHOAN", TrangThai="DA_GHI_NHAN" },
                new() { MaThanhToan=4, MaHoaDon=5, MaNhanVienThu=4, MaPhuongThucThanhToan=2, NgayThanhToan=today.AddDays(-3), SoTien=18_000_000, PhuongThuc="THE_TIN_DUNG", TrangThai="DA_GHI_NHAN" },
            };
        }

        // ── Lookup helpers ──────────────────────────────────────────────────
        public static string TenTang(int id) => Tangs.FirstOrDefault(x => x.MaTang == id)?.TenTang ?? "–";
        public static string TenLoaiPhong(int id) => LoaiPhongs.FirstOrDefault(x => x.MaLoaiPhong == id)?.TenLoaiPhong ?? "–";
        public static string TenKhachHang(int id) => KhachHangs.FirstOrDefault(x => x.MaKhachHang == id)?.HoTen ?? "–";
        public static string TenNhanVien(int? id) => id == null ? "–" : NhanViens.FirstOrDefault(x => x.MaNhanVien == id)?.HoTen ?? "–";
        public static string TenPhongBan(int id) => PhongBans.FirstOrDefault(x => x.MaPhongBan == id)?.TenPhongBan ?? "–";
        public static string TenChucVu(int id) => ChucVus.FirstOrDefault(x => x.MaChucVu == id)?.TenChucVu ?? "–";

        // ── Dashboard stats ─────────────────────────────────────────────────
        public static int PhongDangO() => XepPhongs.Count(x => x.TrangThai == "DA_NHAN_PHONG");
        public static int PhongTrong()
        {
            var occupied = XepPhongs.Where(x => x.TrangThai == "DA_NHAN_PHONG").Select(x => x.MaPhong).ToHashSet();
            return Phongs.Count(p => p.TrangThaiKhaiThac == "DANG_HOAT_DONG" && !occupied.Contains(p.MaPhong));
        }
        public static int PhongBaoTri() => Phongs.Count(p => p.TrangThaiKhaiThac == "BAO_TRI");
        public static int DatPhongHomNay() => DatPhongs.Count(d => d.NgayDat.Date == DateTime.Today);
        public static decimal DoanhThuHomNay() =>
            HoaDons.Where(h => h.NgayLap.Date == DateTime.Today && h.TrangThai != "HUY").Sum(h => h.TongThanhToan);

        public static List<(string Label, decimal Value)> DoanhThu7Ngay()
        {
            var result = new List<(string, decimal)>();
            var seedValues = new[] { 85_000_000m, 62_000_000m, 120_000_000m, 95_000_000m, 143_000_000m, 78_000_000m, 56_000_000m };
            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                var real = HoaDons.Where(h => h.NgayLap.Date == day && h.TrangThai != "HUY").Sum(h => h.TongThanhToan);
                var seed = seedValues[6 - i];
                result.Add((day.ToString("dd/MM"), real + seed));
            }
            return result;
        }
    }
}
