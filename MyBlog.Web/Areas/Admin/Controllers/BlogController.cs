using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.CodeAnalysis.CodeMetrics;
using MyBlog.DAL;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MyBlog.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	/// <summary>
	/// 博客后台管理
	/// </summary>
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
		/// 分类
		/// </summary>
		/// <returns></returns>
		public IActionResult Category()
		{
			#region 生成结点数据json
			ViewBag.nodejson = new DAL.CategoryDAL().GetTreeJson();
			#endregion
			ViewBag.calist = cadal.GetList("");
			return View();
		}
		/// <summary>
		/// 分类删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult categoryDel(int id)
		{
			
			int b = cadal.Delete(id);
			
			if(b>0)
			{
				return Content("删除成功！");
			}
			return Content("删除失败！");
		}
		
		/// <summary>
		/// 分类增加
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult categoryAdd(int pid,string caname)
		{
			caname=Tool.GetSafeSQL(caname);
			string pnumber = "0";
			
			if(pid!=0)
			{
				Model.Category pca=cadal.GetModel(pid);
				if(pca!=null)
				{
					if(pca.PNumber!="0")
					{
						return Json(new { status = "n", info = "只允许添加二级分类，无法添加！" });
					}
					pnumber = pca.Number;
					if(cadal.CalcCount($"PNumber='{pca.PNumber}' and Name='{caname}'")>0)
					{
						return Json(new { status = "n", info = "已有同名分类，无法添加！" });
					}
				}
			}
			else
			{
				if (cadal.CalcCount($"PNumber='0' and Name='{caname}'") > 0)
				{
					return Json(new { status = "n", info = "已有同名分类，无法添加！" });
				}
			}
			string number = cadal.GenBH(pnumber, 2);
			cadal.Insert(new Model.Category() { Name = caname, PNumber = pnumber, Number = number });
			return Json(new { status = "y", info = "新增分类成功!" });
		}
		/// <summary>
		/// 分类编辑
		/// </summary>
		/// <returns></returns>
		public IActionResult categoryMod(int pid,string caname,int id)
		{
			Model.Category ca=cadal.GetModel(id);
			if(ca==null)
			{
				return Json(new { status = "n", info = "分类ID传入错误!" });
			}
			string number = ca.Number;
			string pnumber=ca.PNumber;
			int source_pid = ca.PNumber == "0" ? 0 : cadal.GetModelNumber(ca.PNumber).ID;
			Model.Category pca=cadal.GetModel(pid);
			if(pca!=null)
			{
				if(pca.ID!=source_pid)
				{
					//需要修改低级分类，重新生成编号
					pnumber = pca.Number;
					number = cadal.GenBH(pnumber, 2);
				}
			}
			else if(pid==0)
			{
				pnumber = "0";
				number = cadal.GenBH(pnumber, 2);
			}
			ca.Name = caname;
			ca.PNumber = pnumber;
			ca.Number = number;
			int b = cadal.Update(ca);
			if(b>0)
			{
				return Json(new { status = "y", info = "分类修改成功!" });
			}
			else
			{
				return Json(new { status = "n", info = "修改失败!" });
			}
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
            Console.WriteLine("运行2\n");
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
			Console.WriteLine("运行1\n");
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
