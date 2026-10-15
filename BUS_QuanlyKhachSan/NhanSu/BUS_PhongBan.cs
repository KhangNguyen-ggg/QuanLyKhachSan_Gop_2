using BUS_QuanlyKhachSan.LeTanValuuTru;
using DAL_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace BUS_QuanlyKhachSan.NhanSu;

public class BUS_PhongBan
{
    private readonly DAL_PhongBan dal = new();

    public List<PhongBan> HienThiTatCa(string tuKhoa = "", bool? trangThai = null)
    {
        tuKhoa = (tuKhoa ?? "").Trim();
        return dal.HienThiTatCa().Where(x =>
            (x.TenPhongBan.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase) ||
             (x.MoTa ?? "").Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase)) &&
            (!trangThai.HasValue || x.TrangThai == trangThai.Value))
            .OrderBy(x => x.MaPhongBan).ToList();
    }

    public PhongBan HienThiTheoId(int ma)
    {
        BUS_KiemTra.Ma(ma, "phòng ban");
        return BUS_KiemTra.Chon(dal.HienThiTheoId(ma), "phòng ban còn tồn tại");
    }

    public static void KiemTra(PhongBan phongBan)
    {
        BUS_KiemTra.Chon(phongBan, "phòng ban");
        BUS_KiemTra.YeuCau(phongBan.MaPhongBan >= 0, "Mã phòng ban không hợp lệ.");
        phongBan.TenPhongBan = BUS_KiemTra.Chuoi(phongBan.TenPhongBan, "tên phòng ban", 100, true);
        phongBan.MoTa = BUS_KiemTra.Chuoi(phongBan.MoTa, "mô tả", 500);
    }

    public void Luu(PhongBan phongBan)
    {
        KiemTra(phongBan);
        bool daLuu = phongBan.MaPhongBan == 0 ? dal.Them(phongBan) : dal.Sua(phongBan);
        BUS_KiemTra.KetQua(daLuu);
    }
}
