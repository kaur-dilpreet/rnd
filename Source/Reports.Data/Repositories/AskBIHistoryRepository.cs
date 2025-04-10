using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Reports.Data.Repositories
{
    public class AskBIHistoryRepository : NH.Repository<Core.Domain.Entities.AskBIHistory>, IAskBIHistoryRepository
    {
        public AskBIHistoryRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
