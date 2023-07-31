using Dapper;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.DAL
{
    /// <summary>
    /// 管理员表数据库操作类
    /// </summary>
    public class AdminDAL
    {
        public Admin Login(string username, string password)
        {
            string sql = $"select * from admin where username='{username}' and password='{password}'";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                Console.WriteLine(sql);
                var post = connection.Query<Model.Admin>(sql,
                  new { username=username,password=password }).FirstOrDefault();
                return post;
            }
        }
    }
}
