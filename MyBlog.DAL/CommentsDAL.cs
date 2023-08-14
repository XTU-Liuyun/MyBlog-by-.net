using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyBlog.DAL
{
    public class CommentsDAL
    {
        public int Insert(Model.Comments element)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {

                int resid = connection.ExecuteScalar<int>($"Insert into comments(BlogID,userId,Body,Accept) values ({element.BlogID},{element.UserID},'{element.Body}',0)");
                return resid;
            }
        }
        public int Check(int id)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {

                int resid = connection.ExecuteScalar<int>($"update comments set Accept=1 where ID={id}");
                return resid;
            }
        }
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="cond">sql条件语句</param>
        /// <returns></returns>
        public List<Model.Comments> GetList(string cond)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                string sql = "select * from comments";
                if (!string.IsNullOrEmpty(cond))
                {
                    sql += $" where {cond}";
                }
                var list = connection.Query<Model.Comments>(sql).ToList();
                return list;
            }
        }
        ///<summary>
		///评论分页，使用offset,mssql2012以后有用
		/// </summary> 
		/// <param name="orderstr">如：yydate desc,yytime asc,id desc,必须形成唯一性</param>
		/// <param name="PageSize">页大小</param>
		/// <param name="PageIndex">页索引</param>
		/// <param name="strWhere">条件</param>
		/// <returns></returns>
		public List<Model.Comments> GetList(string orderstr, int PageSize, int PageIndex, string strWhere)
        {
            if (!string.IsNullOrEmpty(strWhere))
            {
                strWhere = " where " + strWhere;
            }
            string sql = //mssql:/*string.Format
                /*(
					"select * from [comments] {0} order by {1} offset {2} rows fetch next {3} rows only",
					strWhere,
					orderstr,
					(PageIndex - 1) * PageSize,
					PageSize
				);*/
                $"select * from comments {strWhere} order by {orderstr} limit {(PageIndex - 1) * PageSize},{PageSize}";
            List<Model.Comments> list = new List<Model.Comments>();
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                Console.Write(sql);
                list = connection.Query<Model.Comments>(sql).ToList();
            }
            return list;
        }

        public int CalcCount(string cond)
        {
            string sql = "select count(1) from comments";
            if (!string.IsNullOrEmpty(cond))
            {
                sql += $" where {cond}";
            }
            Console.WriteLine("\n该评论计算sql为" + sql);
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                int res = connection.ExecuteScalar<int>(sql);
                return res;
            }
        }
    }
}
