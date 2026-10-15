using BUS_QuanlyKhachSan.LeTanValuuTru;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QuanlyKhachSan;
using DAL_QuanlyKhachSan.HeThong;
using ET_QuanLyKhachSan;

namespace BUS_QuanlyKhachSan.HeThong
{
    public class BUS_VaiTro
    {
        private readonly DAL_VaiTro dalVT = new DAL_VaiTro();
        public List<VaiTro> hienThiVTAll()
        {
            return dalVT.hienThiVTAll();
        }
        public bool ThemVT(VaiTro vaiTro)
        {
            return dalVT.ThemVT(vaiTro);
        }
        public VaiTro HienThiVT(VaiTro vaiTro)
        {
            return dalVT.HienThiVT(vaiTro);
        }
        public bool suaVT(VaiTro vaiTro)
        {
            return dalVT.SuaVT(vaiTro);
        }
        public void LuuVaiTro(VaiTro vaiTro)
        {
            BUS_KiemTra.Chon(vaiTro, "vai trò");
            vaiTro.TenVaiTro = BUS_KiemTra.Chuoi(vaiTro.TenVaiTro, "tên vai trò", 100, true);
            vaiTro.MoTa = BUS_KiemTra.Chuoi(vaiTro.MoTa, "mô tả", 500);
            bool daLuu = vaiTro.MaVaiTro == 0 ? dalVT.ThemVT(vaiTro) : dalVT.SuaVT(vaiTro);
            BUS_KiemTra.KetQua(daLuu);
        }
        public bool xoaVT(VaiTro vaiTro)
        {
            return dalVT.XoaVT(vaiTro);
        }
    }
}
