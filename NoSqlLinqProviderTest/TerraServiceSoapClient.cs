using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProviderTest
{
    class TerraServiceSoapClient
    {
        internal Place[] GetPlaceList(string location, int numResults, bool mustHaveImage, bool getEqualOnly)
        {
            Place[] places = new Place[] {
                new Place("Sacramento", "CA", PlaceType.CityTown)
                , new Place("Elk Grove", "CA", PlaceType.CityTown)
                , new Place("South Bend", "IN", PlaceType.CityTown)
            };
            if (getEqualOnly == true)
                return places.Where(x => x.Name == location || x.State == location).ToArray();
            return places.Where(x => x.Name.StartsWith(location) || x.State.StartsWith(location)).ToArray();
        }

        internal void Close()
        {
            return;
        }

        internal void Abort()
        {
            return;
        }
    }
}
