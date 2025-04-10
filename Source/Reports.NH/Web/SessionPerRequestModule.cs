using System;
using System.Web;
using NHibernate;
using System.Collections.Generic;
// This is needed for the DependencyResolver...wish they would've just used Common Service Locator!
using System.Web.Mvc;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Reports.Core;

namespace Reports.NH.Web
{
    /// <summary>
    /// Taken from http://nhforge.org/blogs/nhibernate/archive/2011/03/03/effective-nhibernate-session-management-for-web-apps.aspx
    /// </summary>
    public class SessionPerRequestModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            _httpApplication = context;
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
        }

        private void ContextBeginRequest(object sender, EventArgs e)
        {
            foreach (ISessionFactory sessionFactory in GetSessionFactories())
            {
                ISessionFactory localFactory = sessionFactory;

                LazySessionContext.Bind(new Lazy<ISession>(() => BeginSession(localFactory)), sessionFactory);
            }
        }

        private static ISession BeginSession(ISessionFactory sessionFactory)
        {
            ISession session = sessionFactory.OpenSession();
            session.BeginTransaction();
            return session;
        }

        private void ContextEndRequest(object sender, EventArgs e)
        {
            foreach (ISessionFactory sessionfactory in GetSessionFactories())
            {
                ISession session = LazySessionContext.UnBind(sessionfactory);
                if (session == null) continue;
                EndSession(session);
            }
        }

        private static void EndSession(ISession session)
        {
            if (session.Transaction != null && session.Transaction.IsActive)
            {
                try
                {
                    session.Transaction.Commit();
                }
                catch (Exception)
                {
                    session.Transaction.Rollback();
                }
            }

            session.Dispose();
        }

        public void Dispose()
        {
            try
            {
                _httpApplication.BeginRequest -= ContextBeginRequest;
                _httpApplication.EndRequest -= ContextEndRequest;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Retrieves all ISessionFactory instances via IoC
        /// </summary>
        private IEnumerable<ISessionFactory> GetSessionFactories()
        {
            IEnumerable<ISessionFactory> sessionFactories = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<ISessionFactory>();

            if (sessionFactories == null || sessionFactories.Count() == 0)
                throw new TypeLoadException("At least one ISessionFactory has not been registered with IoC");

            return sessionFactories;
        }

        private HttpApplication _httpApplication;
    }
}
