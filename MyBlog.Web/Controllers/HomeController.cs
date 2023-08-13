using Microsoft.AspNetCore.Mvc;
using MyBlog.Web.Models;
using System.Diagnostics;
using System.Text;

namespace MyBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Private()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username,string password)
        {
            Console.WriteLine(username + ":" + password);
            username = Tool.GetSafeSQL(username);
            password = Tool.MD5Hash(password);
            Console.WriteLine(username+" " + password); 
            Model.User user = new DAL.UserDAL().Login(username, password);
            if (user == null)
            {
                return Content("<script>alert('登录失败,用户名不存在或者密码错误!');location.href='/home/login'</script>","text/html",Encoding.UTF8);

            }
            HttpContext.Session.SetInt32("userid", user.ID);
            HttpContext.Session.SetString("username", user.Name);
            return Redirect("/blog/artical");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string username,string password, string email)
        {
            Console.WriteLine("注册的账号为:"+username + ":" + password);
            username = Tool.GetSafeSQL(username);
            password = Tool.MD5Hash(password);

            int usernum=new DAL.UserDAL().Register(username, password,email); 
            
            if(usernum==-1)
            {
                return Content("<script>alert('该用户名已存在!');location.href='/home/register'</script>", "text/html", Encoding.UTF8);
            }
            else if(Tool.CheckEmail(email)==false)
            {
                return Content("<script>alert('请输入正确的邮箱!');location.href='/home/register'</script>", "text/html", Encoding.UTF8);
            }
            return Redirect("/home/login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}