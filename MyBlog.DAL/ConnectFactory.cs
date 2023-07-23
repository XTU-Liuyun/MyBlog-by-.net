using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace MyBlog.DAL
{
    //数据库连接
    internal class ConnectFactory
    {
        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(@"server=.\sqlexpress;uid=Liuyun;pwd=liuzhiha15326987;database=Blog;");
            connection.Open();

            return connection;
        }
    }
}
