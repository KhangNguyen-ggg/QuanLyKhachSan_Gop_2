using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;

namespace DAL_QuanlyKhachSan.TienIch
{
    public class HienThi
    {
        private readonly string connStr;

        public HienThi(string connStr)
        {
            this.connStr = connStr;
        }

        public T HienThiTheoId<T>(T duLieu) where T : class
        {
            var khoa = typeof(T).GetProperties()
                .First(p => p.GetCustomAttribute<KeyAttribute>() != null);

            using (var connection = new SqlConnection(connStr))
            {
                return connection.Get<T>(khoa.GetValue(duLieu));
            }
        }

        public List<T> HienThiTatCa<T>() where T : class
        {
            using (var connection = new SqlConnection(connStr))
            {
                return connection.GetAll<T>().ToList();
            }
        }
    }
}
