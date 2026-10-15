using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL_QuanlyKhachSan.NhanSu;
using ET_QuanLyKhachSan;

namespace BUS_QuanlyKhachSan.NhanSu
{
    public class BUS_ChucVu
    {
        private readonly DAL_ChucVu dal = new DAL_ChucVu();

        public static bool TuKhoaTimKiemHopLe(string tuKhoa) =>
            Regex.IsMatch(tuKhoa, @"\A(?:\p{L}\p{M}*|\p{Nd}| )*\z");

        public List<ChucVu> HienThiTatCa(string tuKhoa = "", bool? trangThai = null)
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

            return dal.HienThiTatCaCV().Where(x =>
            {
                bool khopTuKhoa = ChuaTuKhoa(x.TenChucVu) || ChuaTuKhoa(x.MoTa ?? "");
                bool khopTrangThai = !trangThai.HasValue || x.TrangThai == trangThai.Value;
                return khopTuKhoa && khopTrangThai;
            })
                .OrderBy(x => x.MaChucVu).ToList();
        }

        public ChucVu HienThiTheoId(int ma)
        {
            BUS_KiemTra.Ma(ma, "chức vụ");
            return BUS_KiemTra.Chon(dal.HienThiTheoId(ma), "chức vụ còn tồn tại");
        }

        public static void KiemTra(ChucVu chucVu)
        {
            BUS_KiemTra.Chon(chucVu, "chức vụ");// mục đích là để tránh lỗi null reference khi gọi các thuộc tính của chucVu.
            BUS_KiemTra.YeuCau(chucVu.MaChucVu >= 0, "Mã chức vụ không hợp lệ.");
            chucVu.TenChucVu = BUS_KiemTra.Chuoi(chucVu.TenChucVu, "tên chức vụ", 100, true);
            chucVu.MoTa = BUS_KiemTra.Chuoi(chucVu.MoTa, "mô tả", 500);
            BUS_KiemTra.Tien(chucVu.Chucvu, "Mức lương");

        }

        public void Luu(ChucVu chucVu)
        {
            KiemTra(chucVu);
            BUS_KiemTra.YeuCau(!dal.TenDaTonTai(chucVu), "Tên chức vụ đã tồn tại. Vui lòng nhập tên khác.");
            bool daLuu = chucVu.MaChucVu == 0 ? dal.Them(chucVu) : dal.Sua(chucVu);
            BUS_KiemTra.KetQua(daLuu);
        }

        public void Xoa(int ma)
        {
            BUS_KiemTra.Ma(ma, "chức vụ");
            ChucVu chucVu = dal.HienThiTheoId(ma);
            BUS_KiemTra.Chon(chucVu, "chức vụ còn tồn tại");
            bool daXoa = dal.Xoa(chucVu);
            BUS_KiemTra.KetQua(daXoa);
        }
    }
}
