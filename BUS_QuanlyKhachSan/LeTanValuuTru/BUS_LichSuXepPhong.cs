using ET_QuanLyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.LeTanValuuTru;

public class BUS_LichSuXepPhong
{
    private readonly DAL_LichSuXepPhong dal = new DAL_LichSuXepPhong();
    public System.Data.DataView hienThiLSXPAll(string tuKhoa = "", string trangThai = "")
    {
        return BUS_KiemTra.LocBang(dal.hienThiLSXPAll(), tuKhoa, trangThai);
    }
}
