using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class PaginationModel
    {
        public Int32 CurrentPage { get; set; }
        public String Target { get; set; }
        public Int32 Count { get; set; }
        public String RefreshUrl { get; set; }
        public Int32 PageSize { get; set; }
    }
}
