using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;

namespace DAL_QuanlyKhachSan.HeThong
{
    public class DAL_VaiTro
    {
        private readonly string str = CauHinhKetNoi.Chuoi;
        private readonly HienThi hienThi;
        private readonly Them them;
        private readonly Sua sua;
        private readonly Xoa xoa;
        private readonly ThuThiLenhCoDieuKien lenh;
        public DAL_VaiTro()
        {
            hienThi = new HienThi(str);
            them = new Them(str);
            sua = new Sua(str);
            xoa = new Xoa(str);
            lenh = new ThuThiLenhCoDieuKien(str);
        }
        public List<VaiTro> hienThiVTAll()
        {
            VaiTro vaiTro = new VaiTro();
            return hienThi.HienThiTatCa<VaiTro>();

        }
        public bool ThemVT(VaiTro vaiTro)
        {
            Them them = new Them(str);
            return them.ThemKhongDieuKien(vaiTro);
        }
        public bool SuaVT(VaiTro vaiTro)
        {
            Sua sua = new Sua(str);
            return sua.SuaTheoId(vaiTro);
        }
        public bool XoaVT(VaiTro vaiTro)
        {
            Xoa xoa = new Xoa(str);
            return xoa.XoaTheoId(vaiTro);
        }
        public VaiTro HienThiVT(VaiTro vaiTro)
        {
            HienThi hienThi = new HienThi(str);
            return hienThi.HienThiTheoId(vaiTro);
        }
        public bool Themquyen(VaiTro_Quyen vaiTro_Quyen) 
        {
            Them them = new Them(str);
            return them.ThemKhongDieuKien(vaiTro_Quyen);
        }
    }
}
