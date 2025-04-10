using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Caches.SysCache2;
using Reports.NH.NHibernateMaps;
using System;

namespace Reports.NH
{
    public class NHibernateInitializer
    {   
        private Configuration Configuration { get; set; }
        public static Configuration Initialize()
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerConnection"].ConnectionString;
            
            Configuration configuration = Fluently.Configure()
                                                .Database(MsSqlConfiguration.MsSql2012
                                                    .Dialect<NH.MsSql2012CustomDialect>()
                                                    .ConnectionString(connectionString))
                                                .Cache(c => c.ProviderClass<SysCacheProvider>().UseSecondLevelCache().UseQueryCache())
                                                .Mappings(m => m.AutoMappings.Add(new AutoPersistenceModelGenerator().Generate()))
                                                //.ExportTo(@"C:\rec\"))
                                                .CurrentSessionContext<LazySessionContext>()
                                                .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                                .ExposeConfiguration(c =>
                                                {
                                                    c.LinqToHqlGeneratorsRegistry<ExtendedLinqtoHqlGeneratorsRegistry>();
                                                })

                                                .BuildConfiguration();

            return configuration;
        }

        public Configuration GetConfiguration()
        {
            if (this.Configuration == null)
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerConnection"].ConnectionString;

                this.Configuration = Fluently.Configure()
                                                    .Database(MsSqlConfiguration.MsSql2012
                                                        .Dialect<NH.MsSql2012CustomDialect>()
                                                        .ConnectionString(connectionString))
                                                    .Cache(c => c.ProviderClass<SysCacheProvider>().UseSecondLevelCache().UseQueryCache())
                                                    .Mappings(m => m.AutoMappings.Add(new AutoPersistenceModelGenerator().Generate()))
                                                    //.ExportTo(@"C:\rec\"))
                                                    .CurrentSessionContext<LazySessionContext>()
                                                    .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                                    .ExposeConfiguration(c =>
                                                    {
                                                        c.LinqToHqlGeneratorsRegistry<ExtendedLinqtoHqlGeneratorsRegistry>();
                                                    })

                                                    .BuildConfiguration();
            }

            return this.Configuration;
        }
    }
}