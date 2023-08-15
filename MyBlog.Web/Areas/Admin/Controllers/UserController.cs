using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;

namespace MyBlog.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class UserController : Controller
	{
		
		
		DAL.UserDAL dal = new DAL.UserDAL();
		DAL.CommentsDAL commentsdal = new DAL.CommentsDAL();
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Comments() 
		{
			return View();
		}
		public IActionResult List(int pageindex, int pagesize, string key, string start, string end, string number)
		{
			List<Model.User> list = dal.GetList("id asc", pagesize, pageindex, GetCond(key, start, end));
			ArrayList arr = new ArrayList();
            foreach (var item in list)
            {
                // 将创建日期和最后登录日期分别进行格式化
                string createdateStr = item.CreateDate.ToString("yyyy-MM-dd HH:mm");
                string lastloginStr = "未登录";

                if (item.LastLogin != DateTime.MinValue)
                {
                    lastloginStr = item.LastLogin.ToString("yyyy-MM-dd HH:mm");
                }

                // 将 item 中的各个字段的值添加到匿名对象中，然后将该对象添加到 arr 中
                arr.Add(new
                {
                    id = item.ID,
                    name = item.Name,
                    createdate = createdateStr,
                    email = item.Email,
                    lastlogin = lastloginStr
                });
            }

            return Json(arr);
		}
		public IActionResult CommentsList(int pageindex, int pagesize, int? key_id, int? key_blogid, int? key_userid, string start, string end, string accept)
		{
			List<Model.Comments> list = commentsdal.GetList("id asc", pagesize, pageindex, GetCond(key_id,key_blogid,key_userid, start, end,accept));
			ArrayList arr = new ArrayList();
			foreach (var item in list)
			{
				
                arr.Add(new { id = item.ID, blogid=item.BlogID, time = item.Time.ToString("yyyy-MM-dd HH:mm"), userid = item.UserID,body=Tool.GZipDecompressString(item.Body),accept=Tool.AcceptToString(item.Accept) });
			}

			return Json(arr);
		}
		
		public IActionResult GetTotalCount(string key, string start, string end)
		{

			int totalCount = dal.CalcCount(GetCond(key, start, end));
			return Content(totalCount.ToString());
		}
		public IActionResult GetCommentsTotalCount(int? key_id,int? key_blogid, int? key_userid, string start, string end, string accept)
		{
			Console.WriteLine("\naccept:" + accept + "\n");
			int totalCount = commentsdal.CalcCount(GetCond(key_id,key_blogid,key_userid, start, end,accept));
			return Content(totalCount.ToString());
		}

		private string GetCond(int? key_id, int? key_blogid, int? key_userid, string start, string end,string accept)
		{
			string cond = "1=1";
			if (key_id != null)
			{
				cond += $" and ID={key_id}";
			}
			if (key_blogid != null)
			{
				cond += $" and BlogID={key_blogid}";
			}
			if (key_userid != null)
			{
				cond += $" and UserID={key_userid}";
			}
			if (!string.IsNullOrEmpty(start))
			{
				DateTime d;
				if (DateTime.TryParse(start, out d))
				{
					cond += $" and Time>='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			if (!string.IsNullOrEmpty(end))
			{
				DateTime d;
				if (DateTime.TryParse(end, out d))
				{
					cond += $" and Time<='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			
			if (accept == "False")
			{
				cond += $" and Accept={0}";
			}
			else if (accept == "True")
			{
				cond += $" and Accept={1}";
			}
			
			
			return cond;
		}

		public string GetCond(string key, string start, string end)
		{
			string cond = "1=1";
			if (!string.IsNullOrEmpty(key))
			{
				key = Tool.GetSafeSQL(key);
				cond += $" and name like '%{key}%'";
			}
			if (!string.IsNullOrEmpty(start))
			{
				DateTime d;
				if (DateTime.TryParse(start, out d))
				{
					cond += $" and createdate>='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			if (!string.IsNullOrEmpty(end))
			{
				DateTime d;
				if (DateTime.TryParse(end, out d))
				{
					cond += $" and createdate<='{d.ToString("yyyy-MM-dd")}'";
				}
			}
			
			return cond;

		}
		[HttpPost]
		public IActionResult Delete(int id)
		{
			int b = dal.Delete(id);
			if (b > 0)
			{
				return Content("删除成功！");

			}
			else
			{
				return Content("删除失败.");
			}
		}
		[HttpPost]
		public IActionResult CommentsDelete(int id)
		{
			int b = commentsdal.Delete(id);
			if (b > 0)
			{
				return Content("删除成功！");

			}
			else
			{
				return Content("删除失败.");
			}
		}
		[HttpPost]
		public IActionResult CommentsAccept(int id)
		{
			commentsdal.Check(id);
			return Content("已通过审核！");
		}
	}
}
