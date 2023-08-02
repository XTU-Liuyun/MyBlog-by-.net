using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using MyBlog.DAL;
using System.Collections;
using System.Collections.Generic;

namespace MyBlog.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BlogController : Controller
	{
		DAL.BlogDAL dal = new DAL.BlogDAL();
		DAL.CategoryDAL cadal = new DAL.CategoryDAL();	
		public IActionResult Index()
		{
			ViewBag.calist = cadal.GetList("");
			
			
			return View();
		}
		/// <summary>
		/// 获取总记录数
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IActionResult GetTotalCount(string key,string start,string end,string number)
		{
			
			int totalCount = dal.CalcCount(GetCond(key,start,end,number));
			return Content(totalCount.ToString());
		}

		public string GetCond(string key,string start,string end,string number)
		{
			string cond = "1=1";
			if (!string.IsNullOrEmpty(key))
			{
				key = Tool.GetSafeSQL(key);
				cond += $" and title like '%{key}%'";
			}
			if (!string.IsNullOrEmpty(start))
			{
				DateTime d;
				if (DateTime.TryParse(start, out d))
				{
					cond += $" and createdate>='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			if (!string.IsNullOrEmpty(end))
			{
				DateTime d;
				if (DateTime.TryParse(end, out d))
				{
					cond += $" and createdate<='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			if (!string.IsNullOrEmpty(number))
			{
				number = Tool.GetSafeSQL(number);
				cond += $" and number='{number}'";
			}
			Console.WriteLine("该cond为:"+cond);
			return cond;
		}

		public IActionResult List(int pageindex,int pagesize, string key, string start, string end, string number)
		{
			List<Model.Blog> list = dal.GetList("sort asc,id desc", pagesize, pageindex,GetCond(key,start,end,number));
			ArrayList arr=new ArrayList();
			foreach(var item in list)
			{
				arr.Add(new {id=item.ID,title=item.Title,createDate=item.CreateDate.ToString("yyyy-MM-dd"),visitNum=item.VisitNum,name=item.Name});
			}
			
			return Json(arr);
		}
		public IActionResult Add(int? id)
		{
			ViewBag.calist=cadal.GetList("");
			Model.Blog m=new Model.Blog();
			if(id!=null)
			{
				m = dal.GetModel(id.Value);
			}
			return View(m); 
		}
		[AutoValidateAntiforgeryToken]
		[HttpPost]
		public IActionResult Add(Model.Blog m)
		{
			Model.Category ca = cadal.GetModelNumber(m.Number);
			if(ca!=null)
			{
				m.Name= ca.Name;
			}
			if(m.ID==0)
			{
				dal.Insert(m);
			}
			else
			{
				dal.Update(m);
			}
			return Redirect("/admin/Blog/Index");
		}
		[HttpPost]
		public IActionResult Delete(int id)
		{
			int b=dal.Delete(id);
			if(b>0)
			{
				return Content("删除成功！");

			}
			else
			{
				return Content("删除失败.");
			}
		}
	}
}
