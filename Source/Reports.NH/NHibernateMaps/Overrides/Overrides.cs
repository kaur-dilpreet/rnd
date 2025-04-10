using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using Reports.Core.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Reports.NH.NHibernateMaps.Overrides
{
    public class GenAIHistoryOverride : IAutoMappingOverride<GenAIHistory>
    {
        /// <summary>
        /// Override the GenAIHistory mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<GenAIHistory> mapping)
        {
            mapping.Table("GenAIHistory");
            mapping.IgnoreProperty(x => x.LastModifiedBy);
            mapping.IgnoreProperty(x => x.LastModificationDateTime);
        }
    }

    public class CMOChatQuestionsOverride : IAutoMappingOverride<CMOChatQuestion>
    {
        /// <summary>
        /// Override the CMOChatQuestion mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<CMOChatQuestion> mapping)
        {
            mapping.IgnoreProperty(x => x.CreatedBy);
            mapping.IgnoreProperty(x => x.CreationDateTime);
            mapping.IgnoreProperty(x => x.LastModifiedBy);
            mapping.IgnoreProperty(x => x.LastModificationDateTime);
        }
    }

    public class CMOChatHistoryOverride : IAutoMappingOverride<CMOChatHistory>
    {
        /// <summary>
        /// Override the CMOChatHistory mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<CMOChatHistory> mapping)
        {
            mapping.Table("CMOChatHistory");
            mapping.Map(x => x.Question).CustomType("StringClob");
            mapping.Map(x => x.Answer).CustomType("StringClob");
            mapping.Map(x => x.SQLQuery).CustomType("StringClob");
            mapping.Map(x => x.Message).CustomType("StringClob");
            mapping.IgnoreProperty(x => x.LastModifiedBy);
            mapping.IgnoreProperty(x => x.LastModificationDateTime);
        }
    }
    
    public class AskBIHistoryOverride : IAutoMappingOverride<AskBIHistory>
    {
        /// <summary>
        /// Override the AskBIHistory mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<AskBIHistory> mapping)
        {
            mapping.Table("AskBIHistory");
            mapping.Map(x => x.Question).CustomType("StringClob");
            mapping.Map(x => x.Answer).CustomType("StringClob");
            mapping.Map(x => x.SQLQuery).CustomType("StringClob");
            mapping.Map(x => x.Message).CustomType("StringClob");
            mapping.Map(x => x.Data).CustomType("StringClob");
            mapping.IgnoreProperty(x => x.LastModifiedBy);
            mapping.IgnoreProperty(x => x.LastModificationDateTime);
        }
    }

    public class AccessRequestsOverride : IAutoMappingOverride<AccessRequest>
    {
        /// <summary>
        /// Override the AccessRequest mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<AccessRequest> mapping)
        {
            mapping.IgnoreProperty(x => x.LastModifiedBy);

            mapping.References<User>(x => x.CreatedByUser).ForeignKey("CreatedBy").Column("CreatedBy").Not.Insert().Not.Update().Cascade.None();
            mapping.References<User>(x => x.ResponseByUser).ForeignKey("ResponseBy").Column("ResponseBy").Not.Insert().Not.Update().Cascade.None();
        }
    }

    public class UsersOverride : IAutoMappingOverride<User>
    {
        /// <summary>
        /// Override the User mapping.
        /// </summary>
        /// <param name="mapping">Fluent NHibernate mapping</param>
        public void Override(AutoMapping<User> mapping)
        {
            mapping.IgnoreProperty(x => x.CreatedBy);
            mapping.IgnoreProperty(x => x.LastModifiedBy);
        }
    }
}
