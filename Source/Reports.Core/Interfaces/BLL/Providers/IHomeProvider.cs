using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface IHomeProvider
    {
        Core.Domain.Models.HomeModel GetHomeModel(Int64 userId);
    }
}
