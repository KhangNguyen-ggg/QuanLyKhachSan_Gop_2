using BUS_QuanlyKhachSan.LeTanValuuTru;
using ET_QuanLyKhachSan;

Environment.SetEnvironmentVariable("QLKS_CONNECTION", "Server=localhost;Database=ValidationOnly;Integrated Security=True");
void Bang(decimal actual, decimal expected)
{
    if (actual != expected) throw new Exception($"Mong đợi {expected}, thực tế {actual}");
}
void TuChoi(SuDungDichVu sd)
{
    try { BUS_HoaDon.KiemTraDichVu(sd); }
    catch (ArgumentException) { return; }
    throw new Exception("Dữ liệu không hợp lệ chưa bị từ chối.");
}
var daCo = new List<ChiTietHoaDon> {
    new() { LoaiChiPhi = "TienPhong", SoLuong = 2, DonGia = 500000 },
    new() { LoaiChiPhi = "DichVu", SoLuong = 1, DonGia = 50000 },
    new() { LoaiChiPhi = "GoiDichVu", SoLuong = 1, DonGia = 200000 }
};
var them = new List<SuDungDichVu> {
    new() { MaDichVu = 1, SoLuong = 2, DonGia = 30000 },
    new() { MaDichVu = 2, SoLuong = 1.5m, DonGia = 40000 },
    new() { MaGoiDichVu = 1, SoLuong = 2, DonGia = 150000 },
    new() { MaGoiDichVu = 2, SoLuong = 1, DonGia = 250000 }
};
foreach (var sd in them) BUS_HoaDon.KiemTraDichVu(sd);
var tatCa = daCo.Concat(BUS_HoaDon.KhoanThuDichVu(them)).ToList();
Bang(tatCa.Count, 7);
Bang(BUS_HoaDon.TinhTong(tatCa.Where(x => x.LoaiChiPhi == "TienPhong")), 1000000);
Bang(BUS_HoaDon.TinhTong(tatCa.Where(x => x.LoaiChiPhi == "DichVu")), 170000);
Bang(BUS_HoaDon.TinhTong(tatCa.Where(x => x.LoaiChiPhi == "GoiDichVu")), 750000);
Bang(BUS_HoaDon.TinhTong(tatCa), 1920000);
Bang(BUS_HoaDon.TinhGiam(1920000, new UuDai { LoaiGiam = "PhanTram", GiaTriGiam = 10 }), 192000);
// Bỏ một dòng gói mới, các khoản đã có giữ nguyên.
them.RemoveAt(3);
Bang(BUS_HoaDon.TinhTong(daCo.Concat(BUS_HoaDon.KhoanThuDichVu(them))), 1670000);
them.Clear();
Bang(BUS_HoaDon.TinhTong(daCo.Concat(BUS_HoaDon.KhoanThuDichVu(them))), 1250000);
Bang(BUS_HoaDon.TinhTong(BUS_HoaDon.KhoanThuDichVu(them)), 0);
Bang(BUS_HoaDon.TinhTong(BUS_HoaDon.KhoanThuDichVu(new[] {
    new SuDungDichVu { MaDichVu = 1, SoLuong = 1.5m, DonGia = 10.01m },
    new SuDungDichVu { MaGoiDichVu = 1, SoLuong = 1.5m, DonGia = 10.01m }
})), 30.04m); // Làm tròn từng dòng như cột tính toán SQL.
BUS_HoaDon.KiemTraDichVu(new() { MaDichVu = 1, SoLuong = 1, DonGia = 0 });
TuChoi(new() { SoLuong = 1 });
TuChoi(new() { MaDichVu = 1, MaGoiDichVu = 1, SoLuong = 1 });
TuChoi(new() { MaDichVu = 0, SoLuong = 1 });
foreach (var so in new[] { 0m, -1m, 1.001m, 10000000000m })
    TuChoi(new() { MaDichVu = 1, SoLuong = so, DonGia = 1000 });
TuChoi(new() { MaGoiDichVu = 1, SoLuong = 1, DonGia = -1 });
TuChoi(new() { MaGoiDichVu = 1, SoLuong = 1, DonGia = 0.001m });
TuChoi(new() { MaGoiDichVu = 1, SoLuong = 2, DonGia = 9999999999999999.99m });
// Kiểm tra đầu vào thô ở BUS và lọc bảng có NULL, không cần SQL Server.
void Loi(Action action)
{
    try { action(); }
    catch (ArgumentException) { return; }
    throw new Exception("BUS chưa chặn dữ liệu không hợp lệ.");
}
Bang(BUS_KiemTra.MaDuocChon(12L), 12);
Loi(() => BUS_KiemTra.MaDuocChon(null));
Loi(() => BUS_KiemTra.MaDuocChon("abc"));
Loi(() => BUS_KiemTra.KetQua(false));
var ds = new List<int> { 1, 2 };
BUS_KiemTra.BoDong(ds, 0); Bang(ds[0], 2);
Loi(() => BUS_KiemTra.BoDong(ds, -1));
var dat = new BUS_DatPhong();
var ct = dat.TaoChiTiet(1, DateTime.Today, DateTime.Today.AddDays(1), "2", "3", "0", "500000");
Bang(ct.SoLuongPhong, 2); Bang(ct.DonGiaDuKien, 500000);
Loi(() => dat.TaoChiTiet(null, DateTime.Today, DateTime.Today.AddDays(1), "1", "1", "0", "1"));
Loi(() => dat.TaoChiTiet(1, null, DateTime.Today.AddDays(1), "1", "1", "0", "1"));
Loi(() => dat.TaoChiTiet(1, DateTime.Today, DateTime.Today, "1", "1", "0", "1"));
Loi(() => dat.TaoChiTiet(1, DateTime.Today, DateTime.Today.AddDays(1), "abc", "1", "0", "1"));
var bang = new System.Data.DataTable();
bang.Columns.Add("MaDatPhong", typeof(long)); bang.Columns.Add("HoTen"); bang.Columns.Add("TrangThai"); bang.Columns.Add("NgayTra", typeof(DateTime));
bang.Rows.Add(1L, "An", "DaXacNhan", DBNull.Value);
bang.Rows.Add(2L, "Bình", "DaHuy", DateTime.Today);
Bang(BUS_KiemTra.LocBang(bang, "an", "DaXacNhan").Count, 1);
Bang(BUS_KiemTra.LocBang(bang, "", "DaHuy").Count, 1);
Bang(BUS_KiemTra.LocBang(bang, "không có", "").Count, 0);
var hoaDon = new BUS_HoaDon();
var chon = new List<SuDungDichVu>();
hoaDon.ThemDichVu(chon, 1L, new DichVu { MaDichVu = 1, DonGia = 20000 }, "2");
hoaDon.ThemGoiDichVu(chon, 1L, new GoiDichVu { MaGoiDichVu = 1, DonGia = 100000 }, "1");
Bang(BUS_HoaDon.TinhTong(BUS_HoaDon.KhoanThuDichVu(chon)), 140000);
Loi(() => hoaDon.ThemDichVu(chon, null, new DichVu { MaDichVu = 1 }, "1"));
Loi(() => hoaDon.ThemDichVu(chon, 1L, null, "1"));
Loi(() => hoaDon.ThemGoiDichVu(chon, 1L, new GoiDichVu { MaGoiDichVu = 1 }, "abc"));
Console.WriteLine("PASS: tổng phòng + nhiều dịch vụ + nhiều gói, bỏ dòng, làm tròn và kiểm tra đầu vào. Không kết nối SQL Server.");
