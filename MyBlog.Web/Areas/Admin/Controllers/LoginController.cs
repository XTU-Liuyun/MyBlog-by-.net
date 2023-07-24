using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
