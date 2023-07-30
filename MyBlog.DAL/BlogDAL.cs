using Dapper;
using System;
using System.Collections.Generic;
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

                int resid = connection.Query<int>(@"INSERT INTO [dbo].[Blog]
           (
           [Title]
           ,[Body]
           ,[Body_md]
           ,[VisitNum]
           ,[Number]
           ,[Name]
           ,[Remark]
           ,[Sort])
     VALUES
           (@Title
		   ,@Body
		   ,@Body_md
           ,@VisitNum
		   ,@Number
		   ,@Name
		   ,@Remark
		   ,@Sort);select @@IDENTITY;",
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
                int res=connection.Execute(@"delete from Blog where id = @id", new { id = id });
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
            using(var connection = ConnectFactory.GetOpenConnection())
            {
                string sql = "select * from Blog";
                if(!string.IsNullOrEmpty(cond))
                {
                    sql += $" where {cond}";
                }
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
                var post = connection.Query<Model.Blog>("select * from Blog where id = @id",
                  new { id =id }).First();
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
                int res = connection.Execute(@"UPDATE [dbo].[Blog]
                                               SET 
                                                  [Title] =@Title
                                                  ,[Body] = @Body
                                                  ,[Body_md] = @Body_md
                                                  ,[VisitNum] = @VisitNum
                                                  ,[Number] = @Number
                                                  ,[Name] = @Name
                                                  ,[Remark] = @Remark
                                                  ,[Sort] = @Sort
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
            string sql = "select count(1) from blog";
            if(!string.IsNullOrEmpty(cond))
            {
                sql += $"where {cond}";
            }
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
			string sql = string.Format
                (
					"select * from [blog] {0} order by {1} offset {2} rows fetch next {3} rows only",
					strWhere,
					orderstr,
					(PageIndex - 1) * PageSize,
					PageSize
				);
			List<Model.Blog> list = new List<Model.Blog>();
			using (var connection =ConnectFactory.GetOpenConnection())
			{
				list = connection.Query<Model.Blog>(sql).ToList();
			}
			return list;
		}
	}
}
