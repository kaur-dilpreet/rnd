using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Reports.Data.Repositories
{
    public class GenAIHistoryRepository : NH.Repository<Core.Domain.Entities.GenAIHistory>, IGenAIHistoryRepository
    {
        public GenAIHistoryRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
