using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProvider
{
    class TerraServiceSoapClient
    {
        internal Place[] GetPlaceList(string location, int numResults, bool mustHaveImage)
        {
            Place[] places = new Place[] {
                new Place("Sacramento", "CA", PlaceType.CityTown)
                , new Place("Elk Grove", "CA", PlaceType.CityTown)
                , new Place("South Bend", "IN", PlaceType.CityTown)
            };
            return places.Where(x => x.Name == location || x.State == location).ToArray();
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
