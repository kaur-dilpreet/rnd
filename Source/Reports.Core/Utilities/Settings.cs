using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Utilities
{
    public class Settings : ISettings
    {
        public String GetSettings(Core.Domain.Enumerations.AppSettings setting)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[setting.ToString()] != null)
            {
                return System.Configuration.ConfigurationManager.AppSettings[setting.ToString()].ToString();
            }
            else
            {
                throw new Core.Domain.Exceptions.InvalidParameterException(String.Format("{0} does not exist in AppSettings", setting));
            }
        }

        public static String GetSettingsStatic(Core.Domain.Enumerations.AppSettings setting)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[setting.ToString()] != null)
            {
                return System.Configuration.ConfigurationManager.AppSettings[setting.ToString()].ToString();
            }
            else
            {
                throw new Core.Domain.Exceptions.InvalidParameterException(String.Format("{0} does not exist in AppSettings", setting));
            }
        }

        public static Core.Domain.Enumerations.Environment GetServerEnvironment()
        {
            return (Core.Domain.Enumerations.Environment)Enum.Parse(typeof(Core.Domain.Enumerations.Environment), System.Configuration.ConfigurationManager.AppSettings["Environment"].ToString());
        }
    }
}
