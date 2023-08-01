using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;

namespace MyBlog.Web.Controllers
{
	public class BlogController : Controller
	{
		/// <summary>
		/// 前台博客控制器
		/// </summary>
		/// <returns></returns>
		DAL.BlogDAL dal = new DAL.BlogDAL();
		public IActionResult Artical()
		{
			return View();
		}
		public IActionResult List(int pageindex, int pagesize)
		{
			List<Model.Blog> list = dal.GetList("sort asc,id desc", pagesize, pageindex,"");
			ArrayList arr = new ArrayList();
			foreach (var item in list)
			{
				Console.WriteLine("item:"+item.Body);
				arr.Add(new { id = item.ID, title = item.Title, createDate = item.CreateDate.ToString("yyyy-MM-dd"), visitNum = item.VisitNum, name = item.Name,desc=Tool.StringTruncat(Tool.GetNoHTMLString(item.Body),60,"...") });
			}

			return Json(arr);
		}
		/// <summary>
		/// 阅读
		/// </summary>
		/// <returns></returns>
		public IActionResult Show(int id)
		{
			Console.WriteLine($"Show {id}");
			Model.Blog b=dal.GetModel(id);
			if (b == null)
			{
				return Content("Error");
			}
			return View(b);
		}
	}
}
