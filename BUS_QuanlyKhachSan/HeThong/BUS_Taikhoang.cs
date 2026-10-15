using BUS_QuanlyKhachSan.LeTanValuuTru;
using ET_QuanLyKhachSan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QuanlyKhachSan;
using DAL_QuanlyKhachSan.LeTanVaLuutru;

namespace BUS_QuanlyKhachSan.HeThong
{
    public class BUS_Taikhoang
    {
        private readonly DAL_Taikhoang dalTK = new DAL_Taikhoang();
        public List<dynamic> hienThiTKAll() 
        {
           return dalTK.hienThiTKAll();
        }
        public bool Khoa(int ma) 
        {
            try
            {
               return dalTK.Khoa(ma);
            }
            catch (Exception e) { return false; }
           
        }
        public string mo(int ma)
        {
            try
            {
                 dalTK.mo(ma);
                return "damo";
            }
            catch (Exception e) {  return e+""; }
        }
        public TaiKhoan HienThiTK(int ma)
        {
            BUS_KiemTra.Ma(ma, "tài khoản");
            return BUS_KiemTra.Chon(dalTK.HienThiTK(new TaiKhoan { MaTaiKhoan = ma }), "tài khoản còn tồn tại");
        }
        public List<NhanVien> HienThiNVALL()
        {
            return dalTK.HienThiNVALL();
        }
        public bool ThemTK(TaiKhoan taiKhoan)
        {
            BUS_KiemTra.Chon(taiKhoan, "tài khoản");
            BUS_KiemTra.Chuoi(taiKhoan.TenDangNhap, "tên đăng nhập", 50, true);
            BUS_KiemTra.Chuoi(taiKhoan.MatKhauHash, "mật khẩu", 1000, true);
            
            BUS_KiemTra.Chuoi(taiKhoan.TrangThai, "trạng thái", 20, true);
            BUS_KiemTra.Ma(taiKhoan.MaNhanVien, "nhân viên");
            BUS_KiemTra.Ngay(taiKhoan.NgayTao, "ngày tạo");
            return dalTK.ThemTK(taiKhoan);
        }
        public bool SuaTK(TaiKhoan taiKhoan)
        {
            BUS_KiemTra.Chon(taiKhoan, "tài khoản");
            BUS_KiemTra.Ma(taiKhoan.MaTaiKhoan, "mã tài khoản");
            BUS_KiemTra.Chuoi(taiKhoan.TenDangNhap, "tên đăng nhập", 50, true);
            BUS_KiemTra.Chuoi(taiKhoan.MatKhauHash, "mật khẩu", 1000, true);
            BUS_KiemTra.Chuoi(taiKhoan.TrangThai, "trạng thái", 20, true);
            BUS_KiemTra.Ma(taiKhoan.MaNhanVien, "nhân viên");
            return dalTK.SuaTK(taiKhoan);
        }
    }
}
