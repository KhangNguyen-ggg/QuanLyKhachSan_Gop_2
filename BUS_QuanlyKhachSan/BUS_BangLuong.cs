using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ET_QuanLyKhachSan;
using DAL.NhanSu;

namespace BUS
{
    public class BUS_BangLuong
    {
        private readonly DAL_BangLuong dal = new DAL_BangLuong();
        public List<dynamic> HienThiBangLuongAll() => dal.HienThiBangLuongAll();
        public List<BangLuong> HienThiBangLuong(BangLuong duLieu) => dal.HienThiBangLuong(duLieu);
        public BangLuong HienThiBangLuongTheoId(BangLuong duLieu) => dal.HienThiBangLuongTheoId(duLieu);
        public bool ThemBangLuong(BangLuong bangLuong)
        {
           
            return dal.ThemBangLuong(bangLuong);
        }

        public bool SuaBangLuong(BangLuong bangLuong)
        {
            return dal.SuaBangLuong(bangLuong);
        }
        public bool XoaBangLuong(BangLuong bangLuong)
        {
            return dal.XoaBangLuong(bangLuong);
        }

    }
}
