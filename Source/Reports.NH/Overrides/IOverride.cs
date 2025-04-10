using NHibernate.Mapping.ByCode;

namespace Reports.NH.Overrides
{
    internal interface IOverride
    {
        void Override(ModelMapper mapper);
    }
}
