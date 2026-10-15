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
    public class Them
    {
        private readonly string connStr;

        public Them(string connStr)
        {
            this.connStr = connStr;
        }

        public bool ThemKhongDieuKien<T>(T duLieu) where T : class
        {
            try
            {
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Insert(duLieu);

                }
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
