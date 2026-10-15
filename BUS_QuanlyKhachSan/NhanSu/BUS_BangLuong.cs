using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ET_QuanLyKhachSan;
using DAL.NhanSu;

namespace BUS_QuanlyKhachSan.NhanSu;

// Bớt: giữ tên hàm cũ nhưng dùng lại BUS chung để tính lương và chặn sửa/xóa lương đã thanh toán.
public class BUS_BangLuong
{
    private readonly BUS_NhanSu bus = new();
    public List<dynamic> HienThiBangLuongAll() => bus.BangLuong().Cast<dynamic>().ToList();
    public List<BangLuong> HienThiBangLuong(BangLuong duLieu) => bus.BangLuong().Cast<BangLuong>().ToList();
    public BangLuong HienThiBangLuongTheoId(BangLuong duLieu) => bus.TheoId<BangLuong>(duLieu.MaBangLuong);
    public bool ThemBangLuong(BangLuong duLieu) { bus.Luu(duLieu); return true; }
    public bool SuaBangLuong(BangLuong duLieu) { bus.Luu(duLieu); return true; }
    public bool XoaBangLuong(BangLuong duLieu) { bus.Xoa<BangLuong>(duLieu.MaBangLuong); return true; }
}
