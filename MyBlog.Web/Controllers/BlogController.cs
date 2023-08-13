using Microsoft.AspNetCore.DataProtection.KeyManagement;
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
		public IActionResult Artical(string key,string number)
		{
            int? userid = HttpContext.Session.GetInt32("userid");
            if (userid == null)
            {
                return Redirect("/Home/Login/");
            }
            ViewBag.calist = new DAL.CategoryDAL().GetList("");
			ViewBag.toplist = new DAL.BlogDAL().GetTop10List();
			ViewBag.search_key = key;
			ViewBag.search_number = number;
			return View();
		}
		public IActionResult List(int pageindex, int pagesize,string key,string number)
		{
			List<Model.Blog> list = dal.GetList("sort asc,id desc", pagesize, pageindex,GetCond(key,number));
			ArrayList arr = new ArrayList();
			foreach (var item in list)
			{
				//Console.WriteLine("item:"+item.Body);
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
			dal.AddVisted(id);
			return View(b);
		}
		/// <summary>
		/// 获取总记录数
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IActionResult GetTotalCount(string key,string number)
		{

			int totalCount = dal.CalcCount(GetCond(key, number));
			Console.WriteLine("totalCount=:" + totalCount);
			return Content(totalCount.ToString());
		}
		/// <summary>
		/// 拼接sql条件
		/// </summary>
		/// <param name="key"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public string GetCond(string key,  string number)
		{
			string cond = "1=1";
			if (!string.IsNullOrEmpty(key))
			{
				key = Tool.GetSafeSQL(key);
				cond += $" and title like '%{key}%'";
			}
			
			if (!string.IsNullOrEmpty(number))
			{
				number = Tool.GetSafeSQL(number);
				cond += $" and number='{number}'";
			}
			Console.WriteLine("该cond为:" + cond);
			return cond;
		}
	}
}
