using Dapper.Contrib.Extensions;

namespace ET_QuanLyKhachSan;

// Thêm: giữ đúng khóa MaLoaiVatTu của bảng cũ; MaVatTu chỉ dùng ở bảng nghiệp vụ.
public class VatTuHienThi : VatTu
{
    public string Nhom { get; set; } = "";
    public string KhoMacDinh { get; set; } = "";
}
public class DinhMucHienThi : TieuHaoVatTu
{
    public string DichVu { get; set; } = "";
    public string VatTu { get; set; } = "";
    public string DonVi { get; set; } = "";
}
public class NhaCungCapVatTuHienThi : VatTu_NhaCungCap
{
    public string VatTu { get; set; } = "";
    public string NhaCungCap { get; set; } = "";
}
public class TonKhoHienThi
{
    public int MaKho { get; set; }
    public int MaVatTu { get; set; }
    public string Kho { get; set; } = "";
    public string VatTu { get; set; } = "";
    public string Nhom { get; set; } = "";
    public string DonVi { get; set; } = "";
    public decimal Ton { get; set; }
    public decimal ToiThieu { get; set; }
    public string CanhBao => Ton < ToiThieu ? "Dưới tối thiểu" : "Đủ tồn";
}
[Table("PhieuNhapXuat")]
public class PhieuNhapXuat
{
    [Key] public long MaPhieu { get; set; }
    public string LoaiPhieu { get; set; } = "NHAP";
    public int MaKho { get; set; }
    public int? MaNhaCungCap { get; set; }
    public int MaNhanVien { get; set; }
    public DateTime NgayPhieu { get; set; } = DateTime.Now;
    public string MucDich { get; set; } = "";
    public string GhiChu { get; set; } = "";
    public string TrangThai { get; set; } = "MOI";
    public int? MaNguoiXacNhan { get; set; }
    public DateTime? NgayXacNhan { get; set; }
}
public class PhieuKhoHienThi : PhieuNhapXuat
{
    public string Kho { get; set; } = "";
    public string NhaCungCap { get; set; } = "";
    public string NhanVien { get; set; } = "";
    public string Loai => LoaiPhieu switch { "NHAP" => "Nhập kho", "XUAT" => "Xuất kho", "DIEU_CHINH_TANG" => "Điều chỉnh tăng", _ => "Điều chỉnh giảm" };
    public decimal TongTien { get; set; }
    public long SoDong { get; set; }
}
[Table("ChiTietPhieuKho")]
public class ChiTietPhieuKho
{
    [Key] public long MaChiTietPhieuKho { get; set; }
    public long MaPhieu { get; set; }
    public int MaVatTu { get; set; }
    public decimal SoLuong { get; set; }
    public decimal? DonGia { get; set; }
    public string GhiChu { get; set; } = "";
}
public class ChiTietPhieuKhoHienThi : ChiTietPhieuKho
{
    public string TenVatTu { get; set; } = "";
    public string DonViTinh { get; set; } = "";
    public decimal ThanhTien { get; set; }
}
[Table("PhieuCapVatTuNoiBo")]
public class PhieuCapVatTuNoiBo
{
    [Key] public long MaPhieuCap { get; set; }
    public int MaKho { get; set; }
    public int MaNhanVien { get; set; }
    public string MucDich { get; set; } = "KHAC";
    public long? MaDatPhong { get; set; }
    public long? MaSuDungDichVu { get; set; }
    public DateTime NgayCap { get; set; } = DateTime.Now;
    public string GhiChu { get; set; } = "";
    public string TrangThai { get; set; } = "MOI";
    public DateTime? NgayXacNhan { get; set; }
}
public class CapNoiBoHienThi : PhieuCapVatTuNoiBo
{
    public string Kho { get; set; } = "";
    public string NhanVien { get; set; } = "";
    public string ThamChieu => MaDatPhong.HasValue ? $"Đặt phòng #{MaDatPhong}" : MaSuDungDichVu.HasValue ? $"Sử dụng dịch vụ #{MaSuDungDichVu}" : "";
    public long SoDong { get; set; }
    public decimal TongSoLuong { get; set; }
}
[Table("ChiTietCapVatTuNoiBo")]
public class ChiTietCapVatTuNoiBo
{
    [Key] public long MaChiTietCap { get; set; }
    public long MaPhieuCap { get; set; }
    public int MaVatTu { get; set; }
    public decimal SoLuong { get; set; }
}
public class ChiTietCapHienThi : ChiTietCapVatTuNoiBo
{
    public string TenVatTu { get; set; } = "";
    public string DonViTinh { get; set; } = "";
}
public class ThamChieuKho
{
    public long Ma { get; set; }
    public string Ten { get; set; } = "";
}
public class NhatKyKhoHienThi
{
    public DateTime ThoiGian { get; set; }
    public decimal BienDong { get; set; }
    public decimal TonTruoc { get; set; }
    public decimal TonSau { get; set; }
    public string LoaiChungTu { get; set; } = "";
    public long MaChungTu { get; set; }
    public string GhiChu { get; set; } = "";
}

