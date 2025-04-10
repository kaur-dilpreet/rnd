using NHibernate;

namespace Reports.Data.Repositories
{
    public class AccessRequestsRepository : NH.Repository<Core.Domain.Entities.AccessRequest>, IAccessRequestsRepository
    {
        public AccessRequestsRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
