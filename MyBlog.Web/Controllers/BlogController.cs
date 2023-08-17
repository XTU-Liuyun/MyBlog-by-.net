using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Net.Http.Headers;
using MyBlog.DAL;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace MyBlog.Web.Controllers
{
	public class BlogController : Controller
	{
        Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment;
        public BlogController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
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
			DAL.UserDAL user = new DAL.UserDAL();	
			List<Model.Comments> list = commentsdal.GetList("time asc", pagesize, pageindex,$"BlogID={blogid} and Accept={1}");
			ArrayList arr = new ArrayList();
			foreach (var item in list)
			{
				arr.Add(new { id = item.ID, blogid = item.BlogID, username = user.GetName(item.UserID),body =(item.Body),time=item.Time.ToString("yyyy-MM-dd hh:mm")});
				Console.WriteLine(item.ID + " " + item.BlogID + " " + user.GetName(item.UserID) + " " + (item.Body) + "\n");
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
				int commentsnum = commentsdal.CalcCount($"blogid={item.ID} and accept={1}");
                //Console.WriteLine("item:"+item.Body);
                arr.Add(new { id = item.ID, title = item.Title, createDate = item.CreateDate.ToString("yyyy-MM-dd"), visitNum = item.VisitNum, name = item.Name, desc = Tool.StringTruncat(Tool.GetNoHTMLString((item.Body)), 60, "..."), cover = BlogDAL.GetCover(item.ID),commentsnum=commentsnum});
            }

            return Json(arr);
        }
        /// <summary>
        /// 阅读
        /// </summary>
        /// <returns></returns> 
        public IActionResult Show(int id)
		{
            Model.Blog b = dal.GetModel(id);
            Model.BlogAndComments a = new Model.BlogAndComments() { Blog = b, Comments = new Model.Comments() };
            if (a.Blog == null)
            {
                return Content("Error");
            }
            Console.WriteLine($"\nShow {id}");
			
            Console.WriteLine('\n' + "标题为:" + a.Blog.Title);
           
            
            dal.AddVisted(id);
			return View(a);
		}
		[AutoValidateAntiforgeryToken]
		[HttpPost]
		public IActionResult Show(Model.Comments m) 
		{
            var commentBody = Request.Form["Comments.Body"];
			m.Body = commentBody;
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
			
            int totalCount = commentsdal.CalcCount($"blogid={id} and Accept={1}");
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
        [HttpPost]
        public IActionResult Upload()
        {

            var imgFile = Request.Form.Files[0];
            long size = 0;
            string tempname = "";
            var filename = ContentDispositionHeaderValue
                            .Parse(imgFile.ContentDisposition)
                            .FileName;
            filename = filename.ToString().Trim('"');
            var extname = filename.Substring(filename.LastIndexOf('.'), filename.Length - filename.LastIndexOf('.'));
            var filename1 = System.Guid.NewGuid().ToString().Substring(0, 6) + extname;
            tempname = filename1;
            var path = hostingEnvironment.WebRootPath;
            Console.WriteLine("path:" + path+"\n");
            string dir = DateTime.Now.ToString("yyyyMMdd");
            if (!System.IO.Directory.Exists(path + $@"/upload/{dir}"))
            {
                System.IO.Directory.CreateDirectory(path + $@"/upload/{dir}");
            }
            filename = path + $@"/upload/{dir}/{filename1}";
            size += imgFile.Length;
            using (FileStream fs = System.IO.File.Create(filename.ToString()))
            {
                imgFile.CopyTo(fs);
                fs.Flush();
            }
            
            return Json(new { location = $"/upload/{dir}/{filename1}" }); ;
        }
    }
}
