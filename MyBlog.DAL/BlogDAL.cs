using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.DAL
{
    /// <summary>
    /// 博客表数据库操作类
    /// </summary>
    public class BlogDAL
    {
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="element"> 所需增加的内容 </param>
        public int Insert(Model.Blog element)
        {
            
            using (var connection = ConnectFactory.GetOpenConnection())
            {

                int resid = connection.Query<int>(@"INSERT INTO blog
           (
           Title
           ,Body
           ,Body_md
           ,VisitNum
           ,Number
           ,Name
           ,Remark
           ,Sort)
     VALUES
           (@Title
		   ,@Body
		   ,@Body_md
           ,@VisitNum
		   ,@Number
		   ,@Name
		   ,@Remark
		   ,@Sort);SELECT LAST_INSERT_ID();",//sqlserver:select @@IDENTITY;,
                 element).First();
                return resid;
            }
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
                int res=connection.Execute(@"delete from blog where id = @id", new { id = id });
                return res;
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<Model.Blog> GetList(string cond)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                string sql = "select * from blog";
                if (!string.IsNullOrEmpty(cond))
                {
                    sql += $" where {cond}";
                }
                var list = connection.Query<Model.Blog>(sql).ToList();
                return list;
            }
        }
        /// <summary>
        /// 获取前十文章
        /// </summary>
        /// <returns></returns>
        public List<Model.Blog> GetTop10List()
        {
            string sql = "SELECT * from (SELECT id,Title,VisitNum,DENSE_RANK()over(ORDER BY VisitNum DESC) rank1 from blog) t1 WHERE t1.rank1<=10 limit 10";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                var list = connection.Query<Model.Blog>(sql).ToList();
                return list;
            }
        }
        public List<Model.Blog> GetTop3List()
        {
            string sql = "SELECT * from (SELECT id,Title,CreateDate,Body,VisitNum,DENSE_RANK()over(ORDER BY VisitNum DESC) rank1 from blog) t1 WHERE t1.rank1<=10 limit 3";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                var list = connection.Query<Model.Blog>(sql).ToList();
                return list;
            }
        }
        /// <summary>
        /// 获取实体类 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Blog GetModel(int id) 
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                var post = connection.Query<Model.Blog>("select * from blog where id = @id",
                  new { id = id }).FirstOrDefault ();
                return post;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int Update(Model.Blog element)
        {
            using (var connection =ConnectFactory.GetOpenConnection())
            {
                int res = connection.Execute(@"UPDATE blog
                                               SET 
                                                  Title =@Title
                                                  ,Body = @Body
                                                  ,Body_md = @Body_md
                                                  ,VisitNum = @VisitNum
                                                  ,Number = @Number
                                                  ,Name = @Name
                                                  ,Remark = @Remark
                                                  ,Sort = @Sort
                                             WHERE ID=@ID", element);
                 return res;
            }
        }
        /// <summary>
        /// 计算记录数
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
       
		public int CalcCount(string cond)
		{
			Console.WriteLine("1该cond为:" + cond);
			string sql = "select count(1) from blog ";
            if(!string.IsNullOrEmpty(cond))
            {
                sql += $"where {cond}";
            }
            Console.WriteLine("该sql为"+sql);
			using (var connection = ConnectFactory.GetOpenConnection())
			{
				int res = connection.ExecuteScalar<int>(sql);   
				return res;
			}
		}
		///<summary>
		///分页，使用offset,mssql2012以后有用
		/// </summary> 
		/// <param name="orderstr">如：yydate desc,yytime asc,id desc,必须形成唯一性</param>
		/// <param name="PageSize">页大小</param>
		/// <param name="PageIndex">页索引</param>
		/// <param name="strWhere">条件</param>
		/// <returns></returns>
		public List<Model.Blog> GetList(string orderstr, int PageSize, int PageIndex, string strWhere)
		{
			if (!string.IsNullOrEmpty(strWhere))
			{
				strWhere = " where " + strWhere;
			}
            string sql = //mssql:/*string.Format
                /*(
					"select * from [blog] {0} order by {1} offset {2} rows fetch next {3} rows only",
					strWhere,
					orderstr,
					(PageIndex - 1) * PageSize,
					PageSize
				);*/
                $"select * from blog {strWhere} order by {orderstr} limit {(PageIndex - 1) * PageSize},{PageSize}";
			List<Model.Blog> list = new List<Model.Blog>();
			using (var connection =ConnectFactory.GetOpenConnection())
			{
                Console.Write(sql);
				list = connection.Query<Model.Blog>(sql).ToList();
			}
			return list;
		}
        /// <summary>
        /// 获取最后一篇文章时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastArticalTime()
        {
            string sql = "select max(CreateDate) from blog"; 
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                DateTime res = connection.ExecuteScalar<DateTime>(sql);
                return res;
            }
        }
        /// <summary>
        /// 获取访问量
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetTotalVisted(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                time = " where " + time;
            }
            string sql = $"select sum(VisitNum) from blog {time}";
           
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                int res = connection.ExecuteScalar<int>(sql);
                return res;
            }
        }
        /// <summary>
        /// 访问数加1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int AddVisted(int id)
        {
            string sql = $"update blog set VisitNum=VisitNum+1 where id={id}";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                int res = connection.ExecuteScalar<int>(sql);
                return res;
            }
        }
        /// <summary>
        /// 获取分类封面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetCover(int id)
        {
            string sql = $"select cover from category where number=(select number from blog where id={id})";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                string res = connection.ExecuteScalar<string>(sql);
                return res;
            }
        }
        
        public string GetMaxVisitNum()
        {
            string sql = "select Title from blog where VisitNum = (select max(VisitNum) from blog)";
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                string res = connection.ExecuteScalar<string>(sql);
                return res;
            }

        }
    }
}
