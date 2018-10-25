using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NoSqlLinqProvider;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProviderTest
{
    public enum WhereMethodName { Equals, StartsWith }
    public class ExpressionValueFinder<T> : ExpressionVisitor
    {
        private Expression expression;
        private List<string> locations;
        public WhereMethodName WhereMethodName { get; set; }
        public ExpressionValueFinder(Expression exp)
        {
            this.expression = exp;
        }

        public List<string> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<string>();
                    this.Visit(this.expression);
                }
                return this.locations;
            }
        }
        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                WhereMethodName = WhereMethodName.Equals;
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), propertyInfo.Name))
                    {
                        locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), propertyInfo.Name));
                        return be;
                    }
                }
                return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(String) && m.Method.Name == "StartsWith")
            {
                WhereMethodName = WhereMethodName.StartsWith;
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(T), propertyInfo.Name))
                    {
                        locations.Add(ExpressionTreeHelpers.GetValueFromExpression(m.Arguments[0]));
                        return m;
                    }
                }
            }
            return base.VisitMethodCall(m);
        }
    }
}
