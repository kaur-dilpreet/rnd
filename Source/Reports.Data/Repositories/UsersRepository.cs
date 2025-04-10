using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Reports.Data.Repositories
{
    public class UsersRepository : NH.Repository<Core.Domain.Entities.User>, IUsersRepository
    {
        public UsersRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            
        }
    }
}
