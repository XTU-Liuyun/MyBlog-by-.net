using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Model
{
    public class Category
    {
        public int ID { get; set; }
        public DateTime CreateDate { get; set; }

        public string Name { get; set; }
        public string Number { get; set; }
        public string PNumber { get; set; }
        public string Remark { get; set; }
        
    }
}
