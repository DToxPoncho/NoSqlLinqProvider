using System.Collections.Generic;

namespace CustomLinqProvider
{
    public interface IQueryOperations<T>
    {
        T[] Where(List<string> locations);
        void StartsWith();
    }
}
