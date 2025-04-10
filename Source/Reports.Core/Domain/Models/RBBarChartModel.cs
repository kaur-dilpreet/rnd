using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class RBBarChartModel
    {
        public RBBarChartModel()
        {
            this.DataSeries = new List<RBBarChartDataSerie>();
        }

        public String Title { get; set; }
        public String ContainerId { get; set; }
        public String ValueTitle { get; set; }
        public List<RBBarChartDataSerie> DataSeries { get; set; }
    }

    public class RBBarChartDataSerie
    {
        public String Name { get; set; }
        public float Percentage { get; set; }
        public Int32 Value { get; set; }
    }
}
