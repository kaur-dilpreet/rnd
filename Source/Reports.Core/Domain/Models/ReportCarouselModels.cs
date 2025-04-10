using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class ReportCarouselModel
    {
        public ReportCarouselModel()
        {
            this.Reports = new List<ReportTileModel>();
        }

        public String Title { get; set; }
        public List<Core.Domain.Models.ReportTileModel> Reports { get; set; }
    }

    public class ReportTileModel
    {
        public Int64 Id { get; set; }
        public Guid UniqueId { get; set; }
        public String Title { get; set; }
        public String Logo { get; set; }
        public String ShortDescription { get; set; }
        public Boolean UserHasAccess { get; set; }
        public String Url { get; set; }
    }
}
