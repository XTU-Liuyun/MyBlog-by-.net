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
    public class User
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        public DateTime LastLogin { get; set; } 
    }
}
