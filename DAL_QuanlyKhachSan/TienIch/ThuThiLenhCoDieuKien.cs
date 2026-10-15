using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DAL_QuanlyKhachSan.TienIch
{
    public class ThuThiLenhCoDieuKien
    {
        private readonly string connStr;

        //Nhận và lưu chuỗi kết nối SQL Server để các phương thức khác sử dụng
        public ThuThiLenhCoDieuKien(string connStr)
        {
            this.connStr = connStr;
        }

        //Thực hiện các câu lệnh SQL không cần trả về danh sách dữ liệu, là INSERT, UPDATE, DELETE. Trả về số dòng bị ảnh hưởng
        public int ThucThiLenh(string sql, object thamSo = null)
        {
            using (var connection = new SqlConnection(connStr))
            {
                return connection.Execute(sql, thamSo);
            }
        }

        //Thực hiện các câu lệnh SQL trả về danh sách dữ liệu, là SELECT. Trả về danh sách đối tượng kiểu T
        public List<T> TruyVan<T>(string sql, object thamSo = null)
        {
            using (var connection = new SqlConnection(connStr))
            {
                return connection.Query<T>(sql, thamSo).ToList();
            }
        }

        //Thực hiện các câu lệnh SQL trả về danh sách dữ liệu, là SELECT. Trả về DataTable
        public DataTable TruyVanBang(string sql, object thamSo = null)
        {
            using var connection = new SqlConnection(connStr);
            connection.Open();
            using var reader = connection.ExecuteReader(sql, thamSo);
            var bang = new DataTable();
            bang.Load(reader);
            return bang;
        }
    }
}
