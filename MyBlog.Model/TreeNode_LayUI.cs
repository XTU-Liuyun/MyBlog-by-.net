using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Model
{
	/// <summary>
	/// layui树形菜单模型
	/// </summary>
	public class TreeNode_LayUI
	{
		public int id { set; get; }
		public string title { set; get; }	
		public bool spread { set; get; }
		public List<TreeNode_LayUI> children { set; get; }	
	}
}
