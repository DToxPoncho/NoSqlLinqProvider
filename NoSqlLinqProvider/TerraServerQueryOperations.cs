using CustomLinqProvider;
using System;
using System.Collections.Generic;

namespace NoSqlLinqProvider
{
    public class TerraServerQueryOperations : IQueryOperations<Place>
    {
        public void StartsWith()
        {
            Console.WriteLine("This is Terra Server Starts With");
        }

        public Place[] Where(List<string> locations)
        {
            Console.WriteLine("This is Terra Server Where");
            return WebServiceHelper.GetPlacesFromTerraServer(locations);
        }
    }
}
