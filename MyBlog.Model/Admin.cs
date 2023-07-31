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
    public class Admin
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        
    }
}
