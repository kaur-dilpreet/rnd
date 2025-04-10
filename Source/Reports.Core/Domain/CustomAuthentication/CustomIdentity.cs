using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Reports.BLL.CustomAuthentication
{
    public class CustomIdentity : ICustomIdentity
    {
        public CustomIdentity(String name)
        {
            this.Name = name;
        }

        public String AuthenticationType
        {
            get
            {
                return "Custom";
            }
        }

        public Boolean IsAuthenticated
        {
            get
            {
                return !String.IsNullOrEmpty(this.Name);
            }
        }

        public String Name
        {
            get;
            private set;
        }
    }
}
