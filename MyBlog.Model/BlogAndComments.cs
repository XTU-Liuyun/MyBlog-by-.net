using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Model
{
    public class BlogAndComments
    {
        public MyBlog.Model.Blog Blog { get; set; }
        public MyBlog.Model.Comments Comments { get; set; }
    }
}
