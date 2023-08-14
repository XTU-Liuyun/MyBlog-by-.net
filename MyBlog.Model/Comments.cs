using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Model
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class Comments
    {
        public int ID { get; set; }
        public int BlogID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public int UserID { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public bool Accept { get; set; }

        public DateTime Time { get; set; }
    }
}
