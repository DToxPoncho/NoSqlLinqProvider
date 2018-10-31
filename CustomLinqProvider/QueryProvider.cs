using System;
using System.Linq;
using System.Linq.Expressions;

namespace CustomLinqProvider
{
    public class QueryProvider : IQueryProvider
    {
        private readonly IQueryContext queryContext;
        public QueryProvider(IQueryContext queryContext)
        {
            this.queryContext = queryContext;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(OrderedQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        // Queryable's collection-returning standard query operators call this method. 
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new OrderedQueryable<TResult>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return queryContext.Execute(expression, false);
        }

        // Queryable's "single value" standard query operators call this method.
        // It is also called from QueryableTerraServerData.GetEnumerator(). 
        public TResult Execute<TResult>(Expression expression)
        {
            //bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            //return (TResult)TerraServerQueryContext.Execute(expression, IsEnumerable);
            return (TResult)queryContext.Execute(expression, (typeof(TResult).Name == "IEnumerable`1"));
        }
    }
}
