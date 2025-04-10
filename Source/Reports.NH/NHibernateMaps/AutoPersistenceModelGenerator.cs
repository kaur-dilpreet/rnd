using System;
using System.Linq;
using Reports.NH.NHibernateMaps.Conventions;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Reports.NH.NHibernateMaps
{
    public class AutoPersistenceModelGenerator
    {
        public AutoPersistenceModel Generate()
        {
            var mappings = new AutoPersistenceModel();
            mappings.AddEntityAssembly(typeof(Reports.Core.Domain.Entities.Entity).Assembly).Where(GetAutoMappingFilter);
            mappings.Conventions.Setup(GetConventions());
            mappings.Setup(GetSetup());
            mappings.IgnoreBase<Reports.Core.Domain.Entities.Entity>();
            //mappings.IgnoreBase(typeof(EntityWithTypedId<>));
            mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();
            return mappings;
        }

        private Action<AutoMappingExpressions> GetSetup()
        {
            return c =>
            {
                c.FindIdentity = type => type.Name == "Id";
            };
        }

        private Action<IConventionFinder> GetConventions()
        {
            return c =>
            {
                c.Add<PrimaryKeyConvention>();
                c.Add<ReferenceConvention>();
                c.Add<TableNameConvention>();
                c.Add<LengthConvention>();
                c.Add<CascadeAllConvention>();
            };
        }

        public class TableNameConvention : IClassConvention
        {
            public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
            {
                instance.Table(Inflector.Net.Inflector.Pluralize(instance.EntityType.Name));
            }
        }

        public class CascadeAllConvention :
            IHasOneConvention, //Actually Apply the convention
            IHasManyConvention,
            IReferenceConvention,
            IHasManyToManyConvention,
            IHasOneConventionAcceptance, //Test to see if we should use the convention
            IHasManyConventionAcceptance, //I think we could skip these since it will always be true
            IReferenceConventionAcceptance, //adding them for reference later
            IHasManyToManyConventionAcceptance
        {

            //One to One
            public void Accept(IAcceptanceCriteria<IOneToOneInspector> criteria)
            {
                //criteria.Expect(x => (true));
            }

            public void Apply(IOneToOneInstance instance)
            {
                instance.Cascade.All();
            }

            //One to Many
            public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> criteria)
            {
                //criteria.Expect(x => (true));
            }

            public void Apply(IOneToManyCollectionInstance instance)
            {
                instance.Cascade.All();
            }

            //Many to One
            public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
            {
                // criteria.Expect(x => (true));
            }

            public void Apply(IManyToOneInstance instance)
            {
                instance.Cascade.All();
            }

            //Many to Many
            public void Accept(IAcceptanceCriteria<IManyToManyCollectionInspector> criteria)
            {
                // criteria.Expect(x => (true));
            }

            public void Apply(IManyToManyCollectionInstance instance)
            {
                instance.Cascade.All();
            }
        }


        /// <summary>
        /// Provides a filter for only including types which inherit from the IEntityWithTypedId interface.
        /// </summary>
        private bool GetAutoMappingFilter(Type t)
        {
            return t.GetInterfaces().Any(x =>
                        x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(Reports.Core.Domain.Entities.IEntityWithTypedId<>));
        }
    }

}
