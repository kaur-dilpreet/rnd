using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface ISharePointProvider
    {
        void CreateTicket(String userEmail, String title, String source, String question, String answer, DateTime creationDateTime, String issueDescription);
    }
}
