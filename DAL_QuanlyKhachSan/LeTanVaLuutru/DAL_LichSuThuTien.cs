using System.Reflection;
using Dapper.Contrib.Extensions;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.LeTanVaLuutru;

public class DAL_LichSuThuTien
{
    private readonly string str = CauHinhKetNoi.Chuoi;
    private ThuThiLenhCoDieuKien lenh => new ThuThiLenhCoDieuKien(str);
    public System.Data.DataTable hienThiLSTTAll() => lenh.TruyVanBang("EXEC dbo.sp_HienThiLichSuThuTien");
}
