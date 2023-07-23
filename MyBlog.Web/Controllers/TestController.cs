using Microsoft.AspNetCore.Mvc;
using MyBlog.Web.Models;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace MyBlog.Web.Controllers
{
    public class TestController : Controller
    {
        

        public IActionResult Index()
        {
            string str = "";
            DAL.CategoryDAL cadal = new DAL.CategoryDAL();
            str = "新增分类的ID值:" + cadal.Insert(new Model.Category() { Name = "新分类3", Number = "123" }) + "\n";
            int b = cadal.Delete(2);
            str += "删除的id值为:" + b;
            Model.Category dal = cadal.GetModel(2);
            if(dal != null)
            {
                dal.Name = "新名称" + DateTime.Now;
                int b2 = cadal.Update(dal);
                str += "\n" + "修改的结果:" + b2;

            }
            List<Model.Category> list = cadal.GetList("");
            foreach(var item in list) 
            {
                str += "\n" + $"ID为{item.ID},名称为{item.Name}";
            }
            return Content(str);
        }

        
    }
}