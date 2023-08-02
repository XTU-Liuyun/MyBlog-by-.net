using Dapper;
using MyBlog.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.DAL
{
    //分类表数据操作
    public class CategoryDAL
    {
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="element"> 所需增加的内容 </param>
        public int Insert(Model.Category element)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {

                int resid = connection.Query<int>(@"INSERT INTO Category(Name,Number,PNumber,Remark) VALUES (@Name,@Number,@PNumber,@Remark);select @@IDENTITY;",
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
                int res=connection.Execute(@"delete from Category where id = @id", new { id = id });
                return res;
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<Model.Category> GetList(string cond) 
        {
            using(var connection = ConnectFactory.GetOpenConnection())
            {
                string sql = "select distinct * from Category";
                if(!string.IsNullOrEmpty(cond))
                {
                    sql += $" where {cond}";
                }
                var list = connection.Query<Model.Category>(sql).ToList();
                return list;
            }
        }
        /// <summary>
        /// 获取实体类 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Category GetModel(int id)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                var post = connection.Query<Model.Category>("select * from Category where id = @id",
                  new { id = id }).FirstOrDefault();
                return post;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int Update(Model.Category element)
        {
            using (var connection =ConnectFactory.GetOpenConnection())
            {
                int res = connection.Execute(@"update Category set Name=@Name,Number=@Number,PNumber=@PNumber,Remark=@Remark where id=@id", new { Name = element.Name, Number = element.Number, PNumber = element.PNumber, Remark = element.Remark,id=element.ID});
                    return res;
            }
        }
        /// <summary>
        /// 根据分类编号取实体类 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public Category GetModelNumber(string number)
        {
            using (var connection = ConnectFactory.GetOpenConnection())
            {
                var post = connection.Query<Model.Category>("select * from Category where Number=@bh",
                  new {bh= number }).First();
                return post;
            }
        }
        /// <summary>
        /// 取所有分类数据拼成json
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
		public string GetTreeJson()
		{
            List<Model.TreeNode_LayUI> list_return = new List<TreeNode_LayUI>();
			
			List<Model.Category> list = GetList("");
            var top = list.Where(a => a.PNumber == "0");
            foreach (var item in top) 
            {
                Model.TreeNode_LayUI node=new TreeNode_LayUI() { id=item.ID ,title=item.Name,spread=true};
				List<Model.TreeNode_LayUI> list_sub = new List<TreeNode_LayUI>();
				var sub=list.Where(a => a.PNumber == item.Number);
                foreach(var item2 in sub)
                {
					Model.TreeNode_LayUI node2 = new TreeNode_LayUI() { id = item2.ID, title = item2.Name, spread = true };
                    list_sub.Add(node2);
				}
                node.children = list_sub;
                list_return.Add(node);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(list_return); 
		}
	}
}
