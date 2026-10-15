using DAL_QuanlyKhachSan.TienIch;
using DAL;
using ET_QuanLyKhachSan;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 public long MaBangLuong { get; set; } // SQL: bigint IDENTITY(1,1) NOT NULL
        public int MaNhanVien { get; set; } // SQL: int NOT NULL
        public int Thang { get; set; } // SQL: int NOT NULL
        public int Nam { get; set; } // SQL: int NOT NULL
        public decimal LuongCoBan { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienCong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienTangCa { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienThuong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienPhuCap { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TienKhauTru { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal Thue { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal TongLuong { get; set; } // SQL: decimal(18, 2) NOT NULL
        public decimal ThucLinh { get; set; } // SQL: decimal(18, 2) NOT NULL
        public DateTime NgayLap { get; set; } // SQL: datetime2(0) NOT NULL
        public string TrangThai { get; set; } = ""; // SQL: varchar(20) NOT NULL
        public string GhiChu { get; set; } = ""; // SQL: nvarchar(500) NULL
 */


namespace DAL.NhanSu  
{
    public class DAL_BangLuong
    {
        private readonly string str = CauHinhKetNoi.Chuoi;
        private readonly HienThi hienThi;
        private readonly Them them;
        private readonly Sua sua;
        private readonly Xoa xoa;
        private readonly ThuThiLenhCoDieuKien lenh;

        public DAL_BangLuong()
        {
            hienThi = new HienThi(str);
            them = new Them(str);
            xoa = new Xoa(str);
            sua = new Sua(str);
            lenh = new ThuThiLenhCoDieuKien(str);
        }

        public List<dynamic> HienThiBangLuongAll() => lenh.TruyVan<dynamic>("EXEC dbo.usp_HienThiBangLuong");
        public List<BangLuong> HienThiBangLuong(BangLuong duLieu) => hienThi.HienThiTatCa<BangLuong>();
        public BangLuong HienThiBangLuongTheoId(BangLuong duLieu) => hienThi.HienThiTheoId(duLieu);
        //Thêm bảng lương mới
        public bool ThemBangLuong(BangLuong bangLuong)
        {
            return them.ThemKhongDieuKien(bangLuong);
        }

        //Cập nhật bảng lương
        public bool SuaBangLuong(BangLuong bangLuong)
        {
            return sua.SuaTheoId(bangLuong);
        }
        //xóa bảng lương
        public bool XoaBangLuong(BangLuong bangLuong)
        {
            return xoa.XoaTheoId(bangLuong);
        }

        

    }
}
