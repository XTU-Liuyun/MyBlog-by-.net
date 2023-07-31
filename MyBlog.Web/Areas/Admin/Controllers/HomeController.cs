using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}
