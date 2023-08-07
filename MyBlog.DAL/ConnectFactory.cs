using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using MySqlConnector;

namespace MyBlog.DAL
{
    //数据库连接
    internal class ConnectFactory
    {
        public static DbConnection GetOpenConnection()
        {
            //Mysql连接:"server=localhost; user id=root; password=root; database=blogcore"
            //var connection = new SqlConnection(@"server=.\sqlexpress;uid=Liuyun;pwd=liuzhiha15326987;database=Blog;");
            var connection = new MySqlConnection("server=localhost; user id=root; password=Liuzhiha15326987; database=blogcore");
            connection.Open();

            return connection;
        }
    }
}
