using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;

namespace DAL_QuanlyKhachSan.NhanSu;

public class DAL_NhanSu
{
    private readonly string chuoi = TienIch.CauHinhKetNoi.Chuoi;

    public List<T> TatCa<T>() where T : class
    {
        using var db = new SqlConnection(chuoi);
        return db.GetAll<T>().ToList();
    }

    public T TheoId<T>(long ma) where T : class
    {
        using var db = new SqlConnection(chuoi);
        return db.Get<T>(ma);
    }

    public List<T> TruyVan<T>(string sql, object thamSo = null)
    {
        using var db = new SqlConnection(chuoi);
        return db.Query<T>(sql, thamSo).ToList();
    }

    // Thêm: dùng lại Dapper.Contrib cho các ET có IDENTITY; kiểm tra số dòng và giữ lỗi SQL.
    public void Luu<T>(T duLieu, long ma) where T : class => GiaoDich((db, tx) =>
    {
        if (ma == 0) db.Insert(duLieu, tx);
        else if (!db.Update(duLieu, tx)) throw new InvalidOperationException("Dữ liệu không còn tồn tại. Vui lòng tải lại.");
    });

    public void Xoa<T>(long ma) where T : class => GiaoDich((db, tx) =>
    {
        var duLieu = db.Get<T>(ma, tx) ?? throw new InvalidOperationException("Dữ liệu đã bị xóa. Vui lòng tải lại.");
        if (!db.Delete(duLieu, tx)) throw new InvalidOperationException("Không xóa được dữ liệu. Vui lòng tải lại.");
    });

    private void GiaoDich(Action<SqlConnection, SqlTransaction> thucHien)
    {
        try
        {
            using var db = new SqlConnection(chuoi);
            db.Open();
            using var tx = db.BeginTransaction(IsolationLevel.Serializable);
            thucHien(db, tx);
            tx.Commit();
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        { throw new InvalidOperationException("Dữ liệu bị trùng (tên, lịch hoặc kỳ lương). Vui lòng kiểm tra lại.", ex); }
        catch (SqlException ex) when (ex.Number == 547)
        { throw new InvalidOperationException("Dữ liệu đang được sử dụng hoặc không thỏa ràng buộc. Không thể thực hiện thao tác này.", ex); }
    }

    private const string NhanVienSql = "SELECT n.*, p.TenPhongBan AS PhongBan, c.TenChucVu AS ChucVu " +
        "FROM dbo.NhanVien n JOIN dbo.PhongBan p ON p.MaPhongBan=n.MaPhongBan JOIN dbo.ChucVu c ON c.MaChucVu=n.MaChucVu";
    private const string LichSql = "SELECT l.*, n.HoTen AS NhanVien, c.TenCa AS Ca, c.GioBatDau, c.GioKetThuc " +
        "FROM dbo.LichLamViec l JOIN dbo.NhanVien n ON n.MaNhanVien=l.MaNhanVien JOIN dbo.CaLamViec c ON c.MaCa=l.MaCa";

    public List<NhanVienHienThi> NhanVien() => TruyVan<NhanVienHienThi>(NhanVienSql + " ORDER BY n.MaNhanVien");
    public List<LichLamViecHienThi> Lich() => TruyVan<LichLamViecHienThi>(LichSql + " ORDER BY l.NgayLam DESC,l.MaLich DESC");
    public List<ChamCongHienThi> ChamCong() => TruyVan<ChamCongHienThi>(
        "SELECT c.*, n.HoTen AS NhanVien FROM dbo.ChamCong c JOIN dbo.NhanVien n ON n.MaNhanVien=c.MaNhanVien ORDER BY c.NgayChamCong DESC,c.MaChamCong DESC");
    public List<BangLuongHienThi> BangLuong() => TruyVan<BangLuongHienThi>(
        "SELECT b.*, n.HoTen AS NhanVien FROM dbo.BangLuong b JOIN dbo.NhanVien n ON n.MaNhanVien=b.MaNhanVien ORDER BY b.Nam DESC,b.Thang DESC,b.MaBangLuong DESC");
    public List<KhenThuongHienThi> KhenThuong() => TruyVan<KhenThuongHienThi>(
        "SELECT k.*, n.HoTen AS NhanVien FROM dbo.KhenThuong k JOIN dbo.NhanVien n ON n.MaNhanVien=k.MaNhanVien ORDER BY k.NgayKhenThuong DESC,k.MaKhenThuong DESC");

    private static void YeuCau(bool hopLe, string thongBao)
    {
        if (!hopLe) throw new InvalidOperationException(thongBao);
    }

    public void LuuNhanVien(NhanVien duLieu) => GiaoDich((db, tx) =>
    {
        var cu = duLieu.MaNhanVien == 0 ? null : db.Get<NhanVien>(duLieu.MaNhanVien, tx);
        var phongBan = db.Get<PhongBan>(duLieu.MaPhongBan, tx);
        var chucVu = db.Get<ChucVu>(duLieu.MaChucVu, tx);
        YeuCau(phongBan != null && (phongBan.TrangThai || cu?.MaPhongBan == duLieu.MaPhongBan), "Phòng ban không tồn tại hoặc đã ngừng hoạt động.");
        YeuCau(chucVu != null && (chucVu.TrangThai || cu?.MaChucVu == duLieu.MaChucVu), "Chức vụ không tồn tại hoặc đã ngừng hoạt động.");
        if (duLieu.MaNhanVien == 0) db.Insert(duLieu, tx);
        else YeuCau(db.Update(duLieu, tx), "Nhân viên không còn tồn tại.");
    });

    public void LuuCa(CaLamViec duLieu) => GiaoDich((db, tx) =>
    {
        var cu = duLieu.MaCa == 0 ? null : db.Get<CaLamViec>(duLieu.MaCa, tx);
        // Thêm: đổi giờ ca đã được xếp sẽ làm lệch lịch/chấm công; tạo ca mới để giữ lịch sử.
        if (cu != null && (cu.GioBatDau != duLieu.GioBatDau || cu.GioKetThuc != duLieu.GioKetThuc))
            YeuCau(db.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.LichLamViec WHERE MaCa=@MaCa", duLieu, tx) == 0,
                "Ca đã có lịch làm việc. Hãy tạo ca mới để thay đổi giờ, giữ nguyên lịch sử.");
        if (duLieu.MaCa == 0) db.Insert(duLieu, tx);
        else YeuCau(db.Update(duLieu, tx), "Ca làm việc không còn tồn tại.");
    });

    public void LuuLich(LichLamViec duLieu) => GiaoDich((db, tx) =>
    {
        var cu = duLieu.MaLich == 0 ? null : db.Get<LichLamViec>(duLieu.MaLich, tx);
        var nhanVien = db.Get<NhanVien>(duLieu.MaNhanVien, tx);
        var ca = db.Get<CaLamViec>(duLieu.MaCa, tx);
        YeuCau(nhanVien != null && (nhanVien.TrangThai == "DangLam" || cu?.MaNhanVien == duLieu.MaNhanVien), "Nhân viên không tồn tại hoặc không đang làm việc.");
        YeuCau(ca != null && (ca.TrangThai || cu?.MaCa == duLieu.MaCa), "Ca không tồn tại hoặc đã ngừng hoạt động.");
        bool coChamCong = db.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.ChamCong WHERE MaLich=@MaLich", duLieu, tx) > 0;
        if (coChamCong && cu != null)
            YeuCau(cu.MaNhanVien == duLieu.MaNhanVien && cu.MaCa == duLieu.MaCa && cu.NgayLam == duLieu.NgayLam && cu.TrangThai == duLieu.TrangThai,
                "Lịch đã chấm công: chỉ sửa được ghi chú.");
        if (duLieu.TrangThai == "DaXep")
        {
            DateTime batDau = duLieu.NgayLam.Date + ca.GioBatDau;
            DateTime ketThuc = duLieu.NgayLam.Date + ca.GioKetThuc;
            if (ketThuc <= batDau) ketThuc = ketThuc.AddDays(1);
            var lichKhac = db.Query<LichLamViecHienThi>(LichSql +
                " WHERE l.MaNhanVien=@MaNhanVien AND l.MaLich<>@MaLich AND l.TrangThai='DaXep' AND l.NgayLam BETWEEN @TuNgay AND @DenNgay",
                new { duLieu.MaNhanVien, duLieu.MaLich, TuNgay = duLieu.NgayLam.AddDays(-1), DenNgay = duLieu.NgayLam.AddDays(1) }, tx);
            foreach (var lich in lichKhac)
            {
                var tu = lich.NgayLam.Date + lich.GioBatDau;
                var den = lich.NgayLam.Date + lich.GioKetThuc;
                if (den <= tu) den = den.AddDays(1);
                YeuCau(batDau >= den || ketThuc <= tu, "Nhân viên đã có lịch trùng giờ, kể cả ca qua đêm.");
            }
        }
        // Khác các danh mục: MaLich dùng SEQUENCE, lấy mã bằng OUTPUT thay cho IDENTITY.
        if (duLieu.MaLich == 0)
            duLieu.MaLich = db.QuerySingle<long>("INSERT dbo.LichLamViec(MaNhanVien,MaCa,NgayLam,TrangThai,GhiChu) OUTPUT INSERTED.MaLich VALUES(@MaNhanVien,@MaCa,@NgayLam,@TrangThai,@GhiChu)", duLieu, tx);
        else YeuCau(db.Update(duLieu, tx), "Lịch làm việc không còn tồn tại.");
    });

    public void LuuChamCong(ChamCong duLieu) => GiaoDich((db, tx) =>
    {
        var lich = db.Get<LichLamViec>(duLieu.MaLich, tx);
        YeuCau(lich != null, "Lịch làm việc không còn tồn tại.");
        var cu = duLieu.MaChamCong == 0 ? null : db.Get<ChamCong>(duLieu.MaChamCong, tx);
        YeuCau(lich.TrangThai == "DaXep" || cu?.MaLich == lich.MaLich, "Chỉ chấm công cho lịch đã xếp.");
        // Khác Phòng ban: lấy nhân viên và ngày từ lịch, không nhận hai giá trị nhập rời dễ sai khóa ngoại.
        duLieu.MaNhanVien = lich.MaNhanVien;
        duLieu.NgayChamCong = lich.NgayLam;
        YeuCau(!duLieu.GioVao.HasValue || duLieu.GioVao.Value.Date == lich.NgayLam.Date, "Ngày vào phải đúng ngày của lịch.");
        if (duLieu.MaChamCong == 0) db.Insert(duLieu, tx);
        else YeuCau(db.Update(duLieu, tx), "Chấm công không còn tồn tại.");
    });

    public void LuuBangLuong(BangLuong duLieu) => GiaoDich((db, tx) =>
    {
        var cu = duLieu.MaBangLuong == 0 ? null : db.Get<BangLuong>(duLieu.MaBangLuong, tx);
        YeuCau(cu?.TrangThai != "DaThanhToan", "Bảng lương đã thanh toán, không thể sửa.");
        YeuCau(db.Get<NhanVien>(duLieu.MaNhanVien, tx) != null, "Nhân viên không còn tồn tại.");
        if (duLieu.MaBangLuong == 0) db.Insert(duLieu, tx);
        else YeuCau(db.Update(duLieu, tx), "Bảng lương không còn tồn tại.");
    });

    public void XoaBangLuong(long ma) => GiaoDich((db, tx) =>
    {
        YeuCau(db.Execute("DELETE dbo.BangLuong WHERE MaBangLuong=@ma AND TrangThai<>'DaThanhToan'", new { ma }, tx) == 1,
            "Bảng lương đã thanh toán hoặc không còn tồn tại, không thể xóa.");
    });
}
