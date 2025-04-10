using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Http;

namespace Reports.Workers.Web.Setup
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Reports.Workers.UI").BasedOn<IController>().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("Reports.Workers.UI").BasedOn<ApiController>().LifestyleTransient());
            container.Register(Classes.FromAssemblyNamed("Reports.Workers.UI").Where(Component.IsInNamespace(@"Reports.Workers.UI.Filters")).WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}