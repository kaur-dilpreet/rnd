using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Reports.Data.Repositories
{
    public class CMOChatQuestionsRepository : NH.Repository<Core.Domain.Entities.CMOChatQuestion>, ICMOChatQuestionsRepository
    {
        public CMOChatQuestionsRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
