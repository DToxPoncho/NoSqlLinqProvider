﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoSqlLinqProvider;

namespace NoSqlLinqProviderTest
{
    internal static class WebServiceHelper
    {
        private static int numResults = 200;
        private static bool mustHaveImage = false;

        internal static Place[] GetPlacesFromTerraServerEquals(List<string> locations)
        {
            
            // Limit the total number of Web service calls. 
            if (locations.Count > 5)
                throw new InvalidQueryException("This query requires more than five separate calls to the Web service. Please decrease the number of locations in your query.");

            List<Place> allPlaces = new List<Place>();

            // For each location, call the Web service method to get data. 
            foreach (string location in locations)
            {
                Place[] places = CallGetPlaceListMethod(location, true);
                allPlaces.AddRange(places);
            }

            return allPlaces.ToArray();
        }

        internal static Place[] GetPlacesFromTerraServerStartsWith(List<string> locations)
        {

            // Limit the total number of Web service calls. 
            if (locations.Count > 5)
                throw new InvalidQueryException("This query requires more than five separate calls to the Web service. Please decrease the number of locations in your query.");

            List<Place> allPlaces = new List<Place>();

            // For each location, call the Web service method to get data. 
            foreach (string location in locations)
            {
                Place[] places = CallGetPlaceListMethod(location, false);
                allPlaces.AddRange(places);
            }

            return allPlaces.ToArray();
        }
        private static Place[] CallGetPlaceListMethod(string location, bool getEqualOnly)
        {
            TerraServiceSoapClient client = new TerraServiceSoapClient();
            Place[] placeFacts = null;

            try
            {
                // Call the Web service method "GetPlaceList".
                placeFacts = client.GetPlaceList(location, numResults, mustHaveImage, getEqualOnly);

                // If there are exactly 'numResults' results, they are probably truncated. 
                if (placeFacts.Length == numResults)
                    throw new Exception("The results have been truncated by the Web service and would not be complete. Please try a different query.");

                // Create Place objects from the PlaceFacts objects returned by the Web service.
                Place[] places = new Place[placeFacts.Length];
                for (int i = 0; i < placeFacts.Length; i++)
                {
                    places[i] = new Place(
                        placeFacts[i].Name,
                        placeFacts[i].State,
                        placeFacts[i].PlaceType);
                }

                // Close the WCF client.
                client.Close();

                return places;
            }
            catch (TimeoutException timeoutException)
            {
                client.Abort();
                throw;
            }
            catch (System.ServiceModel.CommunicationException communicationException)
            {
                client.Abort();
                throw;
            }
        }
    }
}