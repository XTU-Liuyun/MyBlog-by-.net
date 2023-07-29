using Microsoft.AspNetCore.Mvc;
using MyBlog.DAL;

namespace MyBlog.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BlogController : Controller
	{
		DAL.BlogDAL dal = new DAL.BlogDAL();
		DAL.CategoryDAL cadal = new DAL.CategoryDAL();	
		public IActionResult Index()
		{
			List<Model.Blog> list = dal.GetList("");

			return View(list);
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
		public IActionResult Delete(int id)
		{
			dal.Delete(id);
			return Redirect("/admin/Blog/Index");
		}
	}
}
