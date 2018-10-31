using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CustomLinqProvider
{
    public class TerraServerQueryContext<T> : ExpressionVisitor, IQueryContext
    {
        private IQueryOperations<T> queryOperations;
        private Expression Expression { get; set; }
        private List<string> locations;
        private enum FilterType { None, StartsWith, Equals }
        private FilterType filterType = FilterType.Equals;
        public TerraServerQueryContext(IQueryOperations<T> queryOperations)
        {
            this.queryOperations = queryOperations;
        }
        public List<string> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<string>();
                    this.Visit(this.Expression);
                }
                return this.locations;
            }
        }
        // Executes the expression tree that is passed to it. 
        public object Execute(Expression expression, bool IsEnumerable)
        {
            this.Expression = expression;
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            // Get the place name(s) to query the Web service with.
            List<string> locations = this.Locations;
            if (locations.Count == 0)
                throw new InvalidQueryException("You must specify at least one place name in your query.");

            T[] places = null;

            // Call the Web service and get the results.
            if (filterType == FilterType.Equals)
                places = queryOperations.Where(locations);

            // Copy the IEnumerable places to an IQueryable.
            IQueryable<T> queryablePlaces = places.AsQueryable<T>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            ExpressionTreeModifier<T> treeCopier = new ExpressionTreeModifier<T>(queryablePlaces);
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            if (IsEnumerable)
                return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            else
                return queryablePlaces.Provider.Execute(newExpressionTree);
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), propertyInfo.Name))
                    {
                        locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), propertyInfo.Name));
                        filterType = FilterType.Equals;
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
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(T), propertyInfo.Name))
                    {
                        locations.Add(ExpressionTreeHelpers.GetValueFromExpression(m.Arguments[0]));
                        filterType = FilterType.StartsWith;
                        return m;
                    }
                }
            }
            return base.VisitMethodCall(m);
        }
        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}
