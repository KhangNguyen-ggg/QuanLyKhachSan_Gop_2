using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_LichSuThuTien
{
    private readonly DAL_LichSuThuTien dal = new DAL_LichSuThuTien();
    public System.Data.DataView hienThiLSTTAll(string tuKhoa = "", string trangThai = "")
    {
        return BUS_KiemTra.LocBang(dal.hienThiLSTTAll(), tuKhoa, trangThai);
    }
}
