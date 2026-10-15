using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan
{
    public class DAL_Taikhoang
    {
        private readonly string str = CauHinhKetNoi.Chuoi;
        private readonly HienThi hienThi;
        private readonly Them them;
        private readonly Sua sua;
        private readonly Xoa xoa;
        private readonly ThuThiLenhCoDieuKien lenh;
        public DAL_Taikhoang()
        {
            hienThi = new HienThi(str);
            them = new Them(str);
            sua = new Sua(str);
            xoa = new Xoa(str);
            lenh = new ThuThiLenhCoDieuKien(str);
        }
        public List<dynamic> hienThiTKAll()=>lenh.TruyVan<dynamic>("EXEC dbo.sp_HienThitaiKhoang");
        public bool ThemTK(TaiKhoan taiKhoan)
        {
            return them.ThemKhongDieuKien(taiKhoan);
        }
        public bool SuaTK(TaiKhoan taiKhoan)
        {
            return sua.SuaTheoId(taiKhoan);
        }
        public bool XoaTK(TaiKhoan taiKhoan)
        {
            return xoa.XoaTheoId(taiKhoan);
        }
        public TaiKhoan HienThiTK(TaiKhoan taiKhoan)
        {
            return hienThi.HienThiTheoId(taiKhoan);
        }
        public bool Khoa(int ma) 
        {
            TaiKhoan tai= hienThi.HienThiTheoId(new TaiKhoan {MaTaiKhoan = ma });
            tai.TrangThai = "Khoa";
            return sua.SuaTheoId(tai); ;
        }
        public bool mo(int ma)
        {
            TaiKhoan tai = hienThi.HienThiTheoId(new TaiKhoan { MaTaiKhoan = ma });
            tai.TrangThai = "HoatDong";
            return sua.SuaTheoId(tai);
        }
        public List<NhanVien> HienThiNVALL()
        {
            return hienThi.HienThiTatCa<NhanVien>();
        }
    }
}
