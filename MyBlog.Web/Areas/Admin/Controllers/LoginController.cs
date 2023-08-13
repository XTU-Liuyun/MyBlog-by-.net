using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string username,string password)
        {
           // Console.WriteLine(username + ',' + password);
            username=Tool.GetSafeSQL(username);
            password=Tool.MD5Hash(password);
            Model.Admin admin = new DAL.AdminDAL().Login(username,password);
            if(admin == null) 
            {
                return Content("登录失败,用户名或者密码错误！");
            }
            HttpContext.Session.SetInt32("adminid", admin.ID);
            HttpContext.Session.SetString("adminusername", admin.UserName);
            
            return Redirect("/admin/home/index");
        }
        
    }
}
