using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Utilities
{
    public interface ISettings
    {
        String GetSettings(Core.Domain.Enumerations.AppSettings setting);
    }
}
