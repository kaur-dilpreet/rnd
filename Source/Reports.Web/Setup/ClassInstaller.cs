using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor; 

namespace Reports.Web.Setup
{
    public class ClassInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Hellper")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Email")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Utilities")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Logging")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.ErrorHandling")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Caching")).WithService.DefaultInterfaces().LifestyleSingleton());
            container.Register(Classes.FromAssemblyNamed("Reports.Core").Where(Component.IsInNamespace(@"Reports.Core.Encryption")).WithService.DefaultInterfaces().LifestyleSingleton());
        }
    }
}