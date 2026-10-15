using DAL_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BUS_QuanlyKhachSan.NhanSu;

public class BUS_PhongBan
{
    private readonly DAL_PhongBan dal = new DAL_PhongBan();

    public static bool TuKhoaTimKiemHopLe(string tuKhoa) =>
        Regex.IsMatch(tuKhoa, @"\A(?:\p{L}\p{M}*|\p{Nd}| )*\z");

    public List<PhongBan> HienThiTatCa(string tuKhoa = "", bool? trangThai = null)
    {
        tuKhoa = (tuKhoa ?? "").Trim();
        BUS_KiemTra.YeuCau(TuKhoaTimKiemHopLe(tuKhoa),
            "Từ khóa tìm kiếm chỉ được chứa chữ, số và khoảng trắng; không nhập ký tự đặc biệt.");

        // So sánh bỏ dấu và không phân biệt chữ hoa chữ thường.
        // Đ/đ là chữ riêng nên cần đổi về D/d trước khi so sánh bỏ dấu.
        tuKhoa = tuKhoa.Replace('đ', 'd').Replace('Đ', 'D');
        bool ChuaTuKhoa(string noiDung) => CultureInfo.InvariantCulture.CompareInfo.IndexOf(
            noiDung.Replace('đ', 'd').Replace('Đ', 'D'), tuKhoa,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

        return dal.HienThiTatCaPB().Where(x =>
        {
            bool khopTuKhoa = ChuaTuKhoa(x.TenPhongBan) || ChuaTuKhoa(x.MoTa ?? "");
            bool khopTrangThai = !trangThai.HasValue || x.TrangThai == trangThai.Value;
            return khopTuKhoa && khopTrangThai;
        })
            .OrderBy(x => x.MaPhongBan).ToList();
    }

    public PhongBan HienThiTheoId(int ma)
    {
        BUS_KiemTra.Ma(ma, "phòng ban");
        return BUS_KiemTra.Chon(dal.HienThiTheoId(ma), "phòng ban còn tồn tại");
    }

    public static void KiemTra(PhongBan phongBan)
    {
        BUS_KiemTra.Chon(phongBan, "phòng ban");// mục đích là để tránh lỗi null reference khi gọi các thuộc tính của phongBan.
        BUS_KiemTra.YeuCau(phongBan.MaPhongBan >= 0, "Mã phòng ban không hợp lệ.");
        phongBan.TenPhongBan = BUS_KiemTra.Chuoi(phongBan.TenPhongBan, "tên phòng ban", 100, true);
        phongBan.MoTa = BUS_KiemTra.Chuoi(phongBan.MoTa, "mô tả", 500);
    }

    public void Luu(PhongBan phongBan)
    {
        KiemTra(phongBan);
        BUS_KiemTra.YeuCau(!dal.TenDaTonTai(phongBan), "Tên phòng ban đã tồn tại. Vui lòng nhập tên khác.");
        bool daLuu = phongBan.MaPhongBan == 0 ? dal.Them(phongBan) : dal.Sua(phongBan);
        BUS_KiemTra.KetQua(daLuu);
    }

    public void Xoa(int ma)
    {
        BUS_KiemTra.Ma(ma, "phòng ban cần xóa");
        BUS_KiemTra.YeuCau(dal.Xoa(new PhongBan { MaPhongBan = ma }),
            "Phòng ban không còn tồn tại hoặc đã bị xóa.");
    }
}
