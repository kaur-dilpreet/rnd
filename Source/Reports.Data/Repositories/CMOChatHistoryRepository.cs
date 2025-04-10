using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Reports.Data.Repositories
{
    public class CMOChatHistoryRepository : NH.Repository<Core.Domain.Entities.CMOChatHistory>, ICMOChatHistoryRepository
    {
        public CMOChatHistoryRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
