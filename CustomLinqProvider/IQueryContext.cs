using System.Linq.Expressions;

namespace CustomLinqProvider
{
    public interface IQueryContext
    {
        object Execute(Expression expression, bool isEnumerable);
    }
}
