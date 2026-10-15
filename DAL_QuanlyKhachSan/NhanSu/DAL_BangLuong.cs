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


namespace DAL.NhanSu;

// Bớt tiện ích ghi cũ nuốt lỗi: dùng lại DAL chung, vẫn giữ chữ ký hàm để tương thích.
public class DAL_BangLuong
{
    private readonly DAL_QuanlyKhachSan.NhanSu.DAL_NhanSu dal = new();
    public List<dynamic> HienThiBangLuongAll() => dal.BangLuong().Cast<dynamic>().ToList();
    public List<BangLuong> HienThiBangLuong(BangLuong duLieu) => dal.TatCa<BangLuong>();
    public BangLuong HienThiBangLuongTheoId(BangLuong duLieu) => dal.TheoId<BangLuong>(duLieu.MaBangLuong);
    public bool ThemBangLuong(BangLuong duLieu) { dal.LuuBangLuong(duLieu); return true; }
    public bool SuaBangLuong(BangLuong duLieu) { dal.LuuBangLuong(duLieu); return true; }
    public bool XoaBangLuong(BangLuong duLieu) { dal.XoaBangLuong(duLieu.MaBangLuong); return true; }
}
