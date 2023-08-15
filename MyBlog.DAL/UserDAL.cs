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
        public string GetName(int id)
        {
            if(id==0)
            {
                return "游客";
            }
            string sql = $"select name from user where id={id}";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                string res = connection.ExecuteScalar<string>(sql);
                return res;
            }
        }
        /// <summary>
        /// 获取user数
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
		public int CalcCount(string cond)
		{
			Console.WriteLine("1该cond为:" + cond);
			string sql = "select count(1) from user ";
			if (!string.IsNullOrEmpty(cond))
			{
				sql += $"where {cond}";
			}
			Console.WriteLine("该sql为" + sql);
			using (var connection = ConnectFactory.GetOpenConnection())
			{
				int res = connection.ExecuteScalar<int>(sql);
				return res;
			}
		}
		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="cond"></param>
		/// <returns></returns>
		public List<Model.User> GetList(string cond)
		{
			using (var connection = ConnectFactory.GetOpenConnection())
			{
				string sql = "select * from user";
				if (!string.IsNullOrEmpty(cond))
				{
					sql += $" where {cond}";
				}
				var list = connection.Query<Model.User>(sql).ToList();
				return list;
			}
		}
		public List<Model.User> GetList(string orderstr, int PageSize, int PageIndex, string strWhere)
		{
			if (!string.IsNullOrEmpty(strWhere))
			{
				strWhere = " where " + strWhere;
			}
			string sql = //mssql:/*string.Format
				/*(
					"select * from [user] {0} order by {1} offset {2} rows fetch next {3} rows only",
					strWhere,
					orderstr,
					(PageIndex - 1) * PageSize,
					PageSize
				);*/
				$"select * from user {strWhere} order by {orderstr} limit {(PageIndex - 1) * PageSize},{PageSize}";
			List<Model.User> list = new List<Model.User>();
			using (var connection = ConnectFactory.GetOpenConnection())
			{
				Console.Write(sql);
				list = connection.Query<Model.User>(sql).ToList();
			}
			return list;
		}
		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id">所删除的id</param>
		/// <returns></returns>
		public int Delete(int id)
		{
			using (var connection = ConnectFactory.GetOpenConnection())
			{
				int res = connection.Execute(@"delete from user where id = @id", new { id = id });
				return res;
			}
		}
	}
}
