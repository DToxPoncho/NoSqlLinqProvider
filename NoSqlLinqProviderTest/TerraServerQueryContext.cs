using NoSqlLinqProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProviderTest
{
    class TerraServerQueryContext<T> : IQueryContext
    {
        // Executes the expression tree that is passed to it. 
        public object Execute(Expression expression, bool IsEnumerable)
        {
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
            ExpressionValueFinder<T> lf = new ExpressionValueFinder<T>(lambdaExpression.Body);
            List<string> locations = lf.Locations;
            if (locations.Count == 0)
                throw new InvalidQueryException("You must specify at least one place name in your query.");

            Place[] places = null;
            // Call the Web service and get the results.
            if (lf.WhereMethodName == WhereMethodName.Equals)
                places = WebServiceHelper.GetPlacesFromTerraServerEquals(locations);
            else
                places = WebServiceHelper.GetPlacesFromTerraServerStartsWith(locations);

            // Copy the IEnumerable places to an IQueryable.
            IQueryable<Place> queryablePlaces = places.AsQueryable<Place>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            ExpressionTreeModifier<Place> treeCopier = new ExpressionTreeModifier<Place>(queryablePlaces);
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            if (IsEnumerable)
                return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            else
                return queryablePlaces.Provider.Execute(newExpressionTree);
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}
