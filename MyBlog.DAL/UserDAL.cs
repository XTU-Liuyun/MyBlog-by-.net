using Dapper;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MyBlog.DAL
{
    public class UserDAL
    {
        public User Login(string name, string password)
        {
            string sql = $"select * from user where name='{name}' and password='{password}'";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                Console.WriteLine(sql);
                var post = connection.Query<Model.User>(sql,
                  new { name = name, password = password }).FirstOrDefault();
                return post;
            }
        }
        public int Register(string name, string password,string email)
        {
            if(RegisterCheck(name)!=0)
            {
                return -1;
            }
           
            string sql = $"insert into user(name,password,email) values('{name}','{password}','{email}')";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                connection.Query<bool>(sql, new User { Name = name, Password = password,Email=email }).FirstOrDefault();
                return 0;
            }
            
        }
        public int RegisterCheck(string name)
        {
            string sql = $"select count(1) from user where name='{name}'";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                int resid = connection.Query<int>(sql, new User { Name = name }).First();
                return resid;
            }
        }
        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <returns></returns>
        public int GetUserNum(string time)
        {
            if(!string.IsNullOrEmpty(time))
            {
                time = $"where {time}";
            }
            string sql = $"select count(1) from user {time}";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                int res = connection.ExecuteScalar<int>(sql);
                return res;
            }
        }
        /// <summary>
        /// 更新登录时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateLoginTime(int id)
        {

            string sql = $"update user set LastLogin=(select now()) where id={id}";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                bool res = connection.ExecuteScalar<bool>(sql);
                return res;
            }
        }
        
    }
}
