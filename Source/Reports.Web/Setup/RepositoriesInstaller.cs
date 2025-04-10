using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Reports.NH;

namespace Reports.Web.Setup
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Reports.Data").Where(Component.IsInNamespace(@"Reports.Data")).WithService.DefaultInterfaces().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("Reports.Data").Where(Component.IsInNamespace(@"Reports.Data.Repositories")).WithService.DefaultInterfaces().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("Reports.Data").Where(Component.IsInNamespace(@"Reports.Data.Crawlers")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Data").Where(Component.IsInNamespace(@"Reports.Data.Vertica.Repositories")).WithService.DefaultInterfaces().LifestyleTransient());

            container.Register(
                Component.For<ISessionFactory>()
                .UsingFactoryMethod(() => NHibernateInitializer.Initialize().BuildSessionFactory())
                .LifeStyle.Singleton
            );
        }
    }
}