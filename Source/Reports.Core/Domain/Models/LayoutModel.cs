using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class LayoutModel
    {
        public String UserFullName { get; set; }
        public Int64 UserId { get; set; }
    }

    public class MenuModel
    {
        public String Title { get; set; }
        public String Url { get; set; }
    }
}
