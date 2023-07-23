using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Web.Areas.Adnn1n.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
