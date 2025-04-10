using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;


namespace Reports.NH
{
    public class ExtendedLinqtoHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public ExtendedLinqtoHqlGeneratorsRegistry()
        {
            this.Merge(new AddDaysGenerator());
        }
    }

    public class AddDaysGenerator : BaseHqlGeneratorForMethod
    {
        public AddDaysGenerator()
        {
            SupportedMethods = new[]
                {
                ReflectionHelper.GetMethodDefinition<DateTime>(d => d.AddDays((double) 0))
            };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return treeBuilder.MethodCall("AddDays", visitor.Visit(targetObject).AsExpression(), visitor.Visit(arguments[0]).AsExpression());
        }
    }

    public class MsSql2012CustomDialect : MsSql2012Dialect
    {
        public MsSql2012CustomDialect()
        {
            RegisterFunction("AddDays", new SQLFunctionTemplate(NHibernateUtil.DateTime, "dateadd(day,?2,?1)"));
        }
    }
}
