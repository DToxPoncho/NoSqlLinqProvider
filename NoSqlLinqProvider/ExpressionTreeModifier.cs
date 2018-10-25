using NoSqlLinqProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProvider
{
    public class ExpressionTreeModifier<T> : ExpressionVisitor
    {
        private IQueryable<T> queryablePlaces;

        public ExpressionTreeModifier(IQueryable<T> places)
        {
            this.queryablePlaces = places;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (c.Type == typeof(OrderedQueryable<T>))
                return Expression.Constant(this.queryablePlaces);
            else
                return c;
        }
    }
}
