using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Reports.Data.Repositories;

namespace Reports.NH
{
    public class Repository<T> : RepositoryWithTypedId<T, long>, IRepository<T> where T : class
    {
        public Repository(ISessionFactory sessionFactory) : base(sessionFactory) { }
    }

    public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class
    {
        private readonly ISessionFactory _sessionFactory;

        public ISessionFactory SessionFactory {
            get
            {
                return _sessionFactory;
            }
        }

        public void ExecuteNonQuery(String query)
        {
            var sqlQuery = this.Session.CreateQuery(query);

            sqlQuery.ExecuteUpdate();
        }

        public RepositoryWithTypedId(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory may not be null");

            _sessionFactory = sessionFactory;
        }

        protected virtual ISession Session
        {
            get
            {
                ISession session = _sessionFactory.GetCurrentSession();

                if (session == null)
                {
                    session = _sessionFactory.OpenSession();
                }

                return session;
            }
        }

        public virtual IDbContext DbContext
        {
            get
            {
                return new DbContext(_sessionFactory);
            }
        }

        public virtual void RollBack()
        {
            if (Session.Transaction != null && Session.Transaction.IsActive)
            {
                Session.Transaction.Rollback();
            }
        }

        public virtual void Commit()
        {
            if (Session.Transaction != null && Session.Transaction.IsActive)
            {
                Session.Transaction.Commit();
                Session.BeginTransaction();
            }
        }

        public virtual IQuery CreateQuery(String queryString)
        {
            return Session.CreateQuery(queryString);
        }

        public virtual T Get(TId id)
        {
            return Session.Get<T>(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return Session.Query<T>();
        }

        public virtual T SaveOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }

        public virtual T Save(T entity)
        {
            Session.Save(entity);
            return entity;
        }

        public virtual T Update(T entity)
        {
            Session.Update(entity);
            return entity;
        }

        public virtual T Merge(T entity)
        {
            Session.Merge(entity);
            return entity;
        }

        /// <summary>
        /// This method is specially created in order to pass a manual id for the entity in the database
        /// for example ugcCampaign, Promotion, Brand
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T SaveWithId(T entity, TId id)
        {
            Session.Save(entity, id);
            return entity;
        }


        /// <summary>
        /// This deletes the object and commits the deletion imassettely.  We don't want to delay deletion
        /// until a transaction commits, as it may throw a foreign key constraint exception which we could
        /// likely handle and inform the user about.  Accordingly, this tries to delete right away; if there
        /// is a foreign key constraint preventing the deletion, an exception will be thrown.
        /// </summary>
        public virtual void Delete(T entity)
        {
            Session.Delete(entity);
            Session.Flush();
        }
    }
}
