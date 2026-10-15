using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;

namespace DAL_QuanlyKhachSan.KhoVatTu;

public class DAL_KhoVatTu
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

    // Thêm: mọi ghi dữ liệu kiểm tra số dòng; không nuốt lỗi SQL thành "false".
    private void GiaoDich(Action<SqlConnection, SqlTransaction> xuLy, bool khoaTon = false)
    {
        try
        {
            using var db = new SqlConnection(chuoi);
            db.Open();
            using var tx = db.BeginTransaction(IsolationLevel.Serializable);
            if (khoaTon) db.Execute("DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'QLKS_KHO_TON_KHO',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; IF @r<0 THROW 52600,N'Kho đang bận, vui lòng thử lại.',1;", transaction: tx);
            xuLy(db, tx);
            tx.Commit();
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        { throw new InvalidOperationException("Dữ liệu bị trùng. Vật tư chỉ xuất hiện một lần trong cùng phiếu/danh mục liên kết.", ex); }
        catch (SqlException ex) when (ex.Number == 547)
        { throw new InvalidOperationException("Dữ liệu đang được sử dụng hoặc tham chiếu không còn hợp lệ. Hãy tải lại; có thể chuyển danh mục sang ngừng hoạt động thay vì xóa.", ex); }
    }
    private static void Can(bool dung, string loi)
    {
        if (!dung) throw new InvalidOperationException(loi);
    }
    private static void LuuDong<T>(SqlConnection db, SqlTransaction tx, T x, long ma) where T : class
    {
        if (ma == 0) db.Insert(x, tx);
        else Can(db.Update(x, tx), "Dữ liệu không còn tồn tại. Vui lòng tải lại.");
    }
    public void LuuDanhMuc<T>(T x, long ma) where T : class => GiaoDich((db, tx) =>
    {
        // Thêm: chỉ những bảng cố định này được đi qua CRUD danh mục; phiếu/tồn dùng luồng riêng.
        var (bang, cotMa, cotTen, ten) = x switch
        {
            Kho k => ("Kho", "MaKho", "TenKho", k.TenKho),
            NhomVatTu n => ("NhomVatTu", "MaNhomVatTu", "TenNhomVatTu", n.TenNhomVatTu),
            VatTu vatTu => ("VatTu", "MaLoaiVatTu", "TenVatTu", vatTu.TenVatTu),
            NhaCungCap n => ("NhaCungCap", "MaNhaCungCap", "TenNhaCungCap", n.TenNhaCungCap),
            _ => throw new ArgumentException("Không phải danh mục Kho/Vật tư.")
        };
        Can(db.ExecuteScalar<int>($"SELECT COUNT(*) FROM dbo.{bang} WITH(UPDLOCK,HOLDLOCK) WHERE {cotTen}=@ten AND {cotMa}<>@ma", new { ten, ma }, tx) == 0, "Tên đã tồn tại. Vui lòng chọn tên khác.");
        if (x is VatTu v)
        {
            var cu = ma == 0 ? null : db.Get<VatTu>(ma, tx);
            var nhom = db.Get<NhomVatTu>(v.MaNhomVatTu, tx);
            Can(nhom != null && (nhom.TrangThai || cu?.MaNhomVatTu == v.MaNhomVatTu), "Nhóm vật tư không tồn tại hoặc đã ngừng hoạt động.");
            if (v.MaKhoMacDinh.HasValue)
            {
                var kho = db.Get<Kho>(v.MaKhoMacDinh.Value, tx);
                Can(kho != null && (kho.TrangThai || cu?.MaKhoMacDinh == v.MaKhoMacDinh), "Kho mặc định không tồn tại hoặc đã ngừng hoạt động.");
            }
        }
        LuuDong(db, tx, x, ma);
    });
    public void XoaDanhMuc<T>(long ma) where T : class => GiaoDich((db, tx) =>
    {
        Can(typeof(T) == typeof(Kho) || typeof(T) == typeof(VatTu) || typeof(T) == typeof(NhomVatTu) || typeof(T) == typeof(NhaCungCap), "Không phải danh mục.");
        var x = db.Get<T>(ma, tx);
        Can(x != null && db.Delete(x, tx), "Dữ liệu đã bị xóa. Vui lòng tải lại.");
    });

    public List<VatTuHienThi> VatTu() => TruyVan<VatTuHienThi>("SELECT v.*,n.TenNhomVatTu AS Nhom,k.TenKho AS KhoMacDinh FROM dbo.VatTu v JOIN dbo.NhomVatTu n ON n.MaNhomVatTu=v.MaNhomVatTu LEFT JOIN dbo.Kho k ON k.MaKho=v.MaKhoMacDinh ORDER BY v.MaLoaiVatTu");
    public List<DinhMucHienThi> DinhMuc() => TruyVan<DinhMucHienThi>("SELECT t.*,d.TenDichVu AS DichVu,v.TenVatTu AS VatTu,v.DonViTinh AS DonVi FROM dbo.TieuHaoVatTu t JOIN dbo.DichVu d ON d.MaDichVu=t.MaDichVu JOIN dbo.VatTu v ON v.MaLoaiVatTu=t.MaVatTu ORDER BY t.MaDichVu,t.MaVatTu");
    public List<NhaCungCapVatTuHienThi> NguonCung(int ma) => TruyVan<NhaCungCapVatTuHienThi>("SELECT t.*,v.TenVatTu AS VatTu,n.TenNhaCungCap AS NhaCungCap FROM dbo.VatTu_NhaCungCap t JOIN dbo.VatTu v ON v.MaLoaiVatTu=t.MaVatTu JOIN dbo.NhaCungCap n ON n.MaNhaCungCap=t.MaNhaCungCap WHERE t.MaNhaCungCap=@ma ORDER BY t.MaVatTu", new { ma });

    // Khác danh mục IDENTITY: bảng định mức/nguồn cung có khóa ghép, luôn WHERE đủ hai mã.
    public void LuuDinhMuc(TieuHaoVatTu x, bool them) => GiaoDich((db, tx) =>
    {
        var cu = db.QuerySingleOrDefault<TieuHaoVatTu>("SELECT * FROM dbo.TieuHaoVatTu WHERE MaDichVu=@MaDichVu AND MaVatTu=@MaVatTu", x, tx);
        Can(them ? cu == null : cu != null, them ? "Định mức này đã tồn tại." : "Định mức không còn tồn tại.");
        var v = db.Get<VatTu>(x.MaVatTu, tx);
        var d = db.Get<DichVu>(x.MaDichVu, tx);
        Can(v != null && (v.TrangThai || !them) && d != null && (d.TrangThai || !them), "Chọn dịch vụ và vật tư còn hoạt động.");
        string sql = them ? "INSERT dbo.TieuHaoVatTu(MaDichVu,MaVatTu,SoLuongTieuHao,GhiChu) VALUES(@MaDichVu,@MaVatTu,@SoLuongTieuHao,@GhiChu)" : "UPDATE dbo.TieuHaoVatTu SET SoLuongTieuHao=@SoLuongTieuHao,GhiChu=@GhiChu WHERE MaDichVu=@MaDichVu AND MaVatTu=@MaVatTu";
        Can(db.Execute(sql, x, tx) == 1, "Không lưu được định mức.");
    });
    public void XoaDinhMuc(int dichVu, int vatTu) => GiaoDich((db, tx) => Can(db.Execute("DELETE dbo.TieuHaoVatTu WHERE MaDichVu=@dichVu AND MaVatTu=@vatTu", new { dichVu, vatTu }, tx) == 1, "Định mức đã bị xóa."));
    public void LuuNguonCung(VatTu_NhaCungCap x, bool them) => GiaoDich((db, tx) =>
    {
        var cu = db.QuerySingleOrDefault<VatTu_NhaCungCap>("SELECT * FROM dbo.VatTu_NhaCungCap WHERE MaNhaCungCap=@MaNhaCungCap AND MaVatTu=@MaVatTu", x, tx);
        Can(them ? cu == null : cu != null, them ? "Vật tư đã được liên kết với nhà cung cấp này." : "Liên kết không còn tồn tại.");
        var v = db.Get<VatTu>(x.MaVatTu, tx);
        var n = db.Get<NhaCungCap>(x.MaNhaCungCap, tx);
        Can(v != null && (v.TrangThai || !them) && n != null && (n.TrangThai || !them), "Chọn vật tư và nhà cung cấp còn hoạt động.");
        string sql = them ? "INSERT dbo.VatTu_NhaCungCap(MaVatTu,MaNhaCungCap,MaSanPhamNCC,DonGiaNhap,ThoiGianCungCap,TrangThai,GhiChu) VALUES(@MaVatTu,@MaNhaCungCap,@MaSanPhamNCC,@DonGiaNhap,@ThoiGianCungCap,@TrangThai,@GhiChu)" : "UPDATE dbo.VatTu_NhaCungCap SET MaSanPhamNCC=@MaSanPhamNCC,DonGiaNhap=@DonGiaNhap,ThoiGianCungCap=@ThoiGianCungCap,TrangThai=@TrangThai,GhiChu=@GhiChu WHERE MaVatTu=@MaVatTu AND MaNhaCungCap=@MaNhaCungCap";
        Can(db.Execute(sql, x, tx) == 1, "Không lưu được nguồn cung.");
    });
    public void XoaNguonCung(int vatTu, int nhaCungCap) => GiaoDich((db, tx) => Can(db.Execute("DELETE dbo.VatTu_NhaCungCap WHERE MaVatTu=@vatTu AND MaNhaCungCap=@nhaCungCap", new { vatTu, nhaCungCap }, tx) == 1, "Liên kết đã bị xóa."));

    public List<TonKhoHienThi> Ton() => TruyVan<TonKhoHienThi>("SELECT * FROM dbo.vw_Kho_TonKho ORDER BY MaKho,MaVatTu");
    public List<NhatKyKhoHienThi> NhatKy(int kho, int vatTu) => TruyVan<NhatKyKhoHienThi>("SELECT ThoiGian,BienDong,TonTruoc,TonSau,LoaiChungTu,MaChungTu,GhiChu FROM dbo.NhatKyKho WHERE MaKho=@kho AND MaVatTu=@vatTu ORDER BY MaNhatKyKho DESC", new { kho, vatTu });
    public List<PhieuKhoHienThi> Phieu() => TruyVan<PhieuKhoHienThi>("SELECT p.*,v.Kho,v.NhaCungCap,v.NhanVien,v.TongTien,v.SoDong FROM dbo.PhieuNhapXuat p JOIN dbo.vw_Kho_PhieuNhapXuat v ON v.MaPhieu=p.MaPhieu ORDER BY p.NgayPhieu DESC,p.MaPhieu DESC");
    public List<ChiTietPhieuKhoHienThi> ChiTiet(long ma) => TruyVan<ChiTietPhieuKhoHienThi>("SELECT * FROM dbo.vw_Kho_ChiTietPhieuKho WHERE MaPhieu=@ma ORDER BY MaChiTietPhieuKho", new { ma });
    public List<CapNoiBoHienThi> Cap() => TruyVan<CapNoiBoHienThi>("SELECT * FROM dbo.vw_Kho_CapVatTuNoiBo ORDER BY NgayCap DESC,MaPhieuCap DESC");
    public List<ChiTietCapHienThi> ChiTietCap(long ma) => TruyVan<ChiTietCapHienThi>("SELECT c.*,v.TenVatTu,v.DonViTinh FROM dbo.ChiTietCapVatTuNoiBo c JOIN dbo.VatTu v ON v.MaLoaiVatTu=c.MaVatTu WHERE c.MaPhieuCap=@ma ORDER BY c.MaChiTietCap", new { ma });
    public List<ThamChieuKho> ThamChieu(bool dichVu) => TruyVan<ThamChieuKho>(dichVu
        ? "SELECT s.MaSuDungDichVu AS Ma,CONCAT(N'Sử dụng DV #',s.MaSuDungDichVu,N' — ',d.TenDichVu) AS Ten FROM dbo.SuDungDichVu s JOIN dbo.DichVu d ON d.MaDichVu=s.MaDichVu ORDER BY s.MaSuDungDichVu DESC"
        : "SELECT MaDatPhong AS Ma,CONCAT(N'Đặt phòng #',MaDatPhong) AS Ten FROM dbo.DatPhong ORDER BY MaDatPhong DESC");

    private static void KhoaPhieu(SqlConnection db, SqlTransaction tx, long ma, bool cap)
    {
        string bang = cap ? "PhieuCapVatTuNoiBo" : "PhieuNhapXuat", cot = cap ? "MaPhieuCap" : "MaPhieu";
        Can(db.QuerySingleOrDefault<string>($"SELECT TrangThai FROM dbo.{bang} WITH(UPDLOCK,HOLDLOCK) WHERE {cot}=@ma", new { ma }, tx) == "MOI", "Chỉ sửa được phiếu Mới còn tồn tại. Hãy tải lại.");
    }
    private static void DanhMucPhieu(SqlConnection db, SqlTransaction tx, int kho, int nhanVien, int? ncc = null)
    {
        Can(db.Get<Kho>(kho, tx)?.TrangThai == true, "Kho không tồn tại hoặc đã ngừng hoạt động.");
        Can(db.Get<NhanVien>(nhanVien, tx)?.TrangThai == "DangLam", "Nhân viên không còn đang làm việc.");
        if (ncc.HasValue) Can(db.Get<NhaCungCap>(ncc.Value, tx)?.TrangThai == true, "Nhà cung cấp không còn hoạt động.");
    }
    public void LuuPhieu(PhieuNhapXuat x) => GiaoDich((db, tx) =>
    {
        if (x.MaPhieu != 0) KhoaPhieu(db, tx, x.MaPhieu, false);
        DanhMucPhieu(db, tx, x.MaKho, x.MaNhanVien, x.MaNhaCungCap);
        if (x.MaPhieu == 0)
            x.MaPhieu = db.QuerySingle<long>("INSERT dbo.PhieuNhapXuat(LoaiPhieu,MaKho,MaNhaCungCap,MaNhanVien,NgayPhieu,MucDich,GhiChu) OUTPUT INSERTED.MaPhieu VALUES(@LoaiPhieu,@MaKho,@MaNhaCungCap,@MaNhanVien,@NgayPhieu,@MucDich,@GhiChu)", x, tx);
        else Can(db.Execute("UPDATE dbo.PhieuNhapXuat SET LoaiPhieu=@LoaiPhieu,MaKho=@MaKho,MaNhaCungCap=@MaNhaCungCap,MaNhanVien=@MaNhanVien,NgayPhieu=@NgayPhieu,MucDich=@MucDich,GhiChu=@GhiChu WHERE MaPhieu=@MaPhieu AND TrangThai='MOI'", x, tx) == 1, "Không lưu được phiếu.");
    }, true);
    public void LuuChiTiet(ChiTietPhieuKho x) => GiaoDich((db, tx) =>
    {
        KhoaPhieu(db, tx, x.MaPhieu, false);
        Can(db.Get<VatTu>(x.MaVatTu, tx)?.TrangThai == true, "Vật tư không còn hoạt động.");
        if (x.MaChiTietPhieuKho == 0) db.Insert(x, tx);
        else Can(db.Execute("UPDATE dbo.ChiTietPhieuKho SET MaVatTu=@MaVatTu,SoLuong=@SoLuong,DonGia=@DonGia,GhiChu=@GhiChu WHERE MaChiTietPhieuKho=@MaChiTietPhieuKho AND MaPhieu=@MaPhieu", x, tx) == 1, "Chi tiết không còn thuộc phiếu này.");
    }, true);
    public void XoaChiTiet(long ma, long phieu, bool cap) => GiaoDich((db, tx) =>
    {
        KhoaPhieu(db, tx, phieu, cap);
        string sql = cap ? "DELETE dbo.ChiTietCapVatTuNoiBo WHERE MaChiTietCap=@ma AND MaPhieuCap=@phieu" : "DELETE dbo.ChiTietPhieuKho WHERE MaChiTietPhieuKho=@ma AND MaPhieu=@phieu";
        Can(db.Execute(sql, new { ma, phieu }, tx) == 1, "Chi tiết không còn tồn tại.");
    }, true);
    public void TaoCap(PhieuCapVatTuNoiBo x, int vatTu, decimal soLuong) => GiaoDich((db, tx) =>
    {
        DanhMucPhieu(db, tx, x.MaKho, x.MaNhanVien);
        Can(db.Get<VatTu>(vatTu, tx)?.TrangThai == true, "Vật tư không còn hoạt động.");
        x.MaPhieuCap = db.QuerySingle<long>("INSERT dbo.PhieuCapVatTuNoiBo(MaKho,MaNhanVien,MucDich,MaDatPhong,MaSuDungDichVu,GhiChu) OUTPUT INSERTED.MaPhieuCap VALUES(@MaKho,@MaNhanVien,@MucDich,@MaDatPhong,@MaSuDungDichVu,@GhiChu)", x, tx);
        db.Execute("INSERT dbo.ChiTietCapVatTuNoiBo(MaPhieuCap,MaVatTu,SoLuong) VALUES(@ma,@vatTu,@soLuong)", new { ma = x.MaPhieuCap, vatTu, soLuong }, tx);
    }, true);
    public void LuuChiTietCap(ChiTietCapVatTuNoiBo x) => GiaoDich((db, tx) =>
    {
        KhoaPhieu(db, tx, x.MaPhieuCap, true);
        Can(db.Get<VatTu>(x.MaVatTu, tx)?.TrangThai == true, "Vật tư không còn hoạt động.");
        if (x.MaChiTietCap == 0) db.Insert(x, tx);
        else Can(db.Execute("UPDATE dbo.ChiTietCapVatTuNoiBo SET MaVatTu=@MaVatTu,SoLuong=@SoLuong WHERE MaChiTietCap=@MaChiTietCap AND MaPhieuCap=@MaPhieuCap", x, tx) == 1, "Chi tiết không còn thuộc phiếu cấp.");
    }, true);
    public void XuLyPhieu(long ma, bool cap, bool huy, int? nguoiXacNhan = null) => GiaoDich((db, tx) =>
    {
        KhoaPhieu(db, tx, ma, cap);
        if (cap && huy) Can(db.Execute("UPDATE dbo.PhieuCapVatTuNoiBo SET TrangThai='DA_HUY' WHERE MaPhieuCap=@ma AND TrangThai='MOI'", new { ma }, tx) == 1, "Không hủy được phiếu cấp.");
        else
        {
            string sql = cap ? "EXEC dbo.usp_Kho_XacNhanCapNoiBo @MaPhieuCap=@ma" :
                huy ? "EXEC dbo.usp_Kho_HuyPhieuNhapXuat @MaPhieu=@ma" :
                "EXEC dbo.usp_Kho_XacNhanPhieuNhapXuat @MaPhieu=@ma,@MaNguoiXacNhan=@nguoiXacNhan";
            db.Execute(sql, new { ma, nguoiXacNhan }, tx);
        }
    }, true);

    // Bớt: không UPDATE TonKho từ giao diện; procedure ghi phiếu + tồn + nhật ký cùng giao dịch.
    public void NhapXuat(string loai, int kho, int vatTu, decimal soLuong, int nhanVien, int? ncc, decimal? gia, string mucDich, string ghiChu) =>
        GiaoDich((db, tx) => db.Execute("EXEC dbo.usp_Kho_NhapXuatNhanh @LoaiPhieu=@loai,@MaKho=@kho,@MaVatTu=@vatTu,@SoLuong=@soLuong,@MaNhanVien=@nhanVien,@MaNhaCungCap=@ncc,@DonGia=@gia,@MucDich=@mucDich,@GhiChu=@ghiChu", new { loai, kho, vatTu, soLuong, nhanVien, ncc, gia, mucDich, ghiChu }, tx), true);
    public void KiemKe(int kho, int vatTu, decimal ton, int nhanVien, string lyDo) =>
        GiaoDich((db, tx) => db.Execute("EXEC dbo.usp_Kho_KiemKeNhanh @MaKho=@kho,@MaVatTu=@vatTu,@TonThucTe=@ton,@MaNhanVien=@nhanVien,@LyDo=@lyDo", new { kho, vatTu, ton, nhanVien, lyDo }, tx), true);
    public void ChuyenKho(int xuat, int nhap, int vatTu, decimal soLuong, int nhanVien, string ghiChu) =>
        GiaoDich((db, tx) => db.Execute("EXEC dbo.usp_Kho_ChuyenKhoNhanh @MaKhoXuat=@xuat,@MaKhoNhap=@nhap,@MaVatTu=@vatTu,@SoLuong=@soLuong,@MaNhanVien=@nhanVien,@GhiChu=@ghiChu", new { xuat, nhap, vatTu, soLuong, nhanVien, ghiChu }, tx), true);
}

