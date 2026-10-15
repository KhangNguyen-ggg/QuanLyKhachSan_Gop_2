using DAL_QuanlyKhachSan.TienIch;
using ET_QuanLyKhachSan;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_QuanlyKhachSan.NhanSu
{
    public class DAL_ChucVu
    {
        private readonly HienThi hienThi;
        private readonly Them them;
        private readonly ThuThiLenhCoDieuKien lenh;

        public DAL_ChucVu()
        {
            string chuoi = CauHinhKetNoi.Chuoi;
            hienThi = new HienThi(chuoi);
            them = new Them(chuoi);
            lenh = new ThuThiLenhCoDieuKien(chuoi);
        }

        public List<ChucVu> HienThiTatCaCV() => hienThi.HienThiTatCa<ChucVu>();
        public ChucVu HienThiTheoId(int ma) => hienThi.HienThiTheoId(new ChucVu { MaChucVu = ma });
        public bool Them(ChucVu chucVu) => them.ThemKhongDieuKien(chucVu);

        public bool TenDaTonTai(ChucVu chucVu)
        {
            return lenh.TruyVan<int>(
            "SELECT COUNT(1) FROM dbo.ChucVu WHERE TenChucVu = @TenChucVu AND MaChucVu <> @MaChucVu", chucVu).Single() > 0;
        }

        //xóa
        public bool Xoa(ChucVu chucVu)
        {
            try
            {
                return lenh.ThucThiLenh("DELETE FROM dbo.ChucVu WHERE MaChucVu = @MaChucVu", chucVu) == 1;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException(
                    "Không thể xóa chức vụ đang được nhân viên sử dụng. Có thể đổi trạng thái thành Ngừng hoạt động.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(
                    "Không thể xóa chức vụ. Vui lòng thử lại.", ex);
            }
        }

        //cập nhật
        public bool Sua(ChucVu chucVu)
        {
            try
            {
                return lenh.ThucThiLenh("UPDATE dbo.ChucVu SET " +
                    "TenChucVu = @TenChucVu," +
                    "Chucvu = @Chucvu, " +
                    "MoTa = @MoTa, " +
                    "TrangThai = @TrangThai " +
                    "WHERE MaChucVu = @MaChucVu", chucVu) == 1;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException(
                    "Không thể cập nhật chức vụ. Vui lòng thử lại.", ex);
            }
        }

    }
}
