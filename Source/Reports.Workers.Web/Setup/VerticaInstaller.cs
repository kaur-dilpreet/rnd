using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Reports.Workers.Web.Setup
{
    public class VerticaInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Reports.Vertica").Where(Component.IsInNamespace(@"Reports.Vertica")).WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}