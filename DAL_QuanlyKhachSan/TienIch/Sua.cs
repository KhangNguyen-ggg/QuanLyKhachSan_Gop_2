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
    public class Sua
    {
        private readonly string connStr;

        public Sua(string connStr)
        {
            this.connStr = connStr;
        }

        //
        public bool SuaTheoId<T>(T duLieu) where T : class
        {
            try {
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Update(duLieu);
                }
                return true;
            }
            catch (Exception ex) { return false; }
           
        }
    }
}
