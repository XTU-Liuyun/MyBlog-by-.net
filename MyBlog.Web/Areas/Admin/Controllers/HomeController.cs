﻿using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Web.Areas.Adnn1n.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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
