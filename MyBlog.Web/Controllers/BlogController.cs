using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using MyBlog.DAL;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace MyBlog.Web.Controllers
{
	public class BlogController : Controller
	{
		/// <summary>
		/// 前台博客控制器
		/// </summary>
		/// <returns></returns>
		DAL.BlogDAL dal = new DAL.BlogDAL();
		DAL.CommentsDAL commentsdal = new DAL.CommentsDAL();	
		public IActionResult Artical(string key,string number)
		{
            int? userid = HttpContext.Session.GetInt32("userid");
            if (userid == null)
            {
                return Redirect("/Home/Login/");
            }
            ViewBag.calist = new DAL.CategoryDAL().GetList("");
			ViewBag.toplist = new DAL.BlogDAL().GetTop10List();
			ViewBag.search_key = key;
			ViewBag.search_number = number;
			return View();
		}
		/// <summary>
		/// 获取评论列表
		/// </summary>
		/// <param name="pageindex"></param>
		/// <param name="pagesize"></param>
		/// <param name="blogid"></param>
		/// <returns></returns>
		public IActionResult CommentsList(int pageindex, int pagesize,int blogid)
		{

			List<Model.Comments> list = commentsdal.GetList("time asc", pagesize, pageindex,$"BlogID={blogid}");
			ArrayList arr = new ArrayList();
			foreach (var item in list)
			{
				arr.Add(new { id = item.ID, blogid = item.BlogID, userid = item.UserID,body = Tool.GZipDecompressString(item.Body),time=item.Time});
			}

			return Json(arr);
		}
		/// <summary>
		/// 获取文章列表
		/// </summary>
		/// <param name="pageindex"></param>
		/// <param name="pagesize"></param>
		/// <param name="key"></param>
		/// <param name="number"></param>
		/// <returns></returns>
        public IActionResult List(int pageindex, int pagesize, string key, string number)
        {
            List<Model.Blog> list = dal.GetList("sort asc,id desc", pagesize, pageindex, GetCond(key, number));
            ArrayList arr = new ArrayList();
            foreach (var item in list)
            {
                //Console.WriteLine("item:"+item.Body);
                arr.Add(new { id = item.ID, title = item.Title, createDate = item.CreateDate.ToString("yyyy-MM-dd"), visitNum = item.VisitNum, name = item.Name, desc = Tool.StringTruncat(Tool.GetNoHTMLString(item.Body), 60, "..."), cover = BlogDAL.GetCover(item.ID) });
            }

            return Json(arr);
        }
        /// <summary>
        /// 阅读
        /// </summary>
        /// <returns></returns>
        public IActionResult Show(int id)
		{
			Console.WriteLine($"Show {id}");
			Model.Blog b=dal.GetModel(id);
			Model.BlogAndComments a=new Model.BlogAndComments() { Blog=b,Comments=new Model.Comments() };
			
			if (a.Blog == null)
			{
				return Content("Error");
			}
            Console.WriteLine('\n' + "标题为:" + a.Blog.Title);
            dal.AddVisted(id);
			return View(a);
		}
		[AutoValidateAntiforgeryToken]
		[HttpPost]
		public IActionResult Show(Model.Comments m) 
		{
            var commentBody = Request.Form["Comments.Body"];
			m.Body = Tool.GZipCompressString(commentBody);
			if(m.Body.Length==0)
			{
				return Content($"<script>alert('评论不能为空!');location.href='/blog/show/{m.BlogID}'</script>", "text/html", Encoding.UTF8);
            }
            MyBlog.DAL.CommentsDAL commentsdal=new CommentsDAL();
			commentsdal.Insert(m);
            return Content($"<script>alert('已发表评论,请等待管理员审核!');location.href='/blog/show/{m.BlogID}'</script>", "text/html", Encoding.UTF8);
        }
		/// <summary>
		/// 获取总记录数
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IActionResult GetTotalCount(string key,string number)
		{

			int totalCount = dal.CalcCount(GetCond(key, number));
			Console.WriteLine("totalCount=:" + totalCount);
			return Content(totalCount.ToString());
		}
		/// <summary>
		/// 获取总评论数
		/// </summary>
		/// <returns></returns>
        public IActionResult GetBlogCommentsCount(int id)
        {

            int totalCount = commentsdal.CalcCount($"blogid={id}");
            Console.WriteLine("BlogCommentstotalCount=:" + totalCount);
            return Content(totalCount.ToString());
        }
        /// <summary>
        /// 拼接sql条件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public string GetCond(string key,  string number)
		{
			string cond = "1=1";
			if (!string.IsNullOrEmpty(key))
			{
				key = Tool.GetSafeSQL(key);
				cond += $" and title like '%{key}%'";
			}
			
			if (!string.IsNullOrEmpty(number))
			{
				number = Tool.GetSafeSQL(number);
				cond += $" and number='{number}'";
			}
			Console.WriteLine("该cond为:" + cond);
			return cond;
		}
	}
}
