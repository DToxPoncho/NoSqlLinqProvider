using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CustomLinqProvider
{
    public class OrderedQueryable<TData> : IOrderedQueryable<TData>
    {
        #region Constructors
        /// <summary> 
        /// This constructor is called by the client to create the data source. 
        /// </summary> 
        public OrderedQueryable(IQueryContext queryContext)
        {
            Initialize(new QueryProvider(queryContext), null);
        }

        public OrderedQueryable(IQueryProvider provider)
        {
            Initialize(provider, null);
        }

        public OrderedQueryable(IQueryProvider provider, Expression expression)
        {
            Initialize(provider, expression);
        }

        private void Initialize(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (expression != null && !typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
                throw new ArgumentException(String.Format("Not assignable from {0}", expression.Type), "expression");
            Provider = provider;
            Expression = expression ?? Expression.Constant(this);
        }
        #endregion

        #region Properties

        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get { return typeof(TData); }
        }

        #endregion

        #region Enumerators
        public IEnumerator<TData> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
        }
        #endregion
    }
}
