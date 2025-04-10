using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Reports.Workers.Web.Setup
{
    public class BLLInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Reports.BLL").Where(Component.IsInNamespace(@"Reports.BLL.Providers")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.BLL").Where(Component.IsInNamespace(@"Reports.BLL.Workers")).WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}
