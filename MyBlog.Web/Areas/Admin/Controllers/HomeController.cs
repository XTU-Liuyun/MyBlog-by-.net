using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            int?adminid=HttpContext.Session.GetInt32("adminid");
            if(adminid==null)
            {
                return Redirect("/admin/Login/");
            }
            return View();
        }
        public IActionResult Top()
        {
            return View();
        }
        public IActionResult Left()
        {
            return View();
        }
        public IActionResult Welcome()
        {
            DAL.AdminDAL admindal= new DAL.AdminDAL();
            List<Model.Admin> list = admindal.GetList("");
            return View(list);
        }
        
    }
}
