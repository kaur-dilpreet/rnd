using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using Reports.Core.Helpers.NHAttributes;

namespace Reports.NH.NHibernateMaps.Conventions
{
    public class LengthConvention : AttributePropertyConvention<LengthAttribute>
    {
        protected override void Apply(LengthAttribute attribute, IPropertyInstance instance)
        {
            instance.Length(attribute.Length);
        }
    }
}
