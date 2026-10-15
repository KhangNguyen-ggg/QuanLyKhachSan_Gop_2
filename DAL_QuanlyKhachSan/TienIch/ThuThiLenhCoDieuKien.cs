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

        public ThuThiLenhCoDieuKien(string connStr)
        {
            this.connStr = connStr;
        }

        public int ThucThiLenh(string sql, object thamSo = null)
        {
            using (var connection = new SqlConnection(connStr))
            {
                return connection.Execute(sql, thamSo);
            }
        }

        public List<T> TruyVan<T>(string sql, object thamSo = null)
        {
            using (var connection = new SqlConnection(connStr))
            {
                return connection.Query<T>(sql, thamSo).ToList();
            }
        }
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
