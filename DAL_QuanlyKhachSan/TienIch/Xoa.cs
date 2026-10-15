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
    public class Xoa
    {
        private readonly string connStr;

        public Xoa(string connStr)
        {
            this.connStr = connStr;
        }

        public bool XoaTheoId<T>(T duLieu) where T : class
        {
            try {
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Delete(duLieu);
                    
                }
                return true;
            }
            catch (Exception ex) { return false; }
            
        }

        public bool XoaSachCaBang<T>() where T : class
        {
            try {
                using (var connection = new SqlConnection(connStr))
                {
                    connection.DeleteAll<T>();
                }
                return true;
            }
            catch (Exception ex) { return false; }
            
        }
    }
}
