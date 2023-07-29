using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Model
{
    public class Blog
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// MarkDown
        /// </summary>
        public string Body_md { get; set; }
        /// <summary>
        /// 访问量
        /// </summary>
        public int VisitNum { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark{ get; set; }
        /// <summary>
        /// 排序号 
        /// </summary>
        public int Sort { get; set; }
    }
}
