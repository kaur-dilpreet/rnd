using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            this.ReportCarousels = new List<ReportCarouselModel>();
        }

        public List<ReportCarouselModel> ReportCarousels { get; set; }
    }
}
