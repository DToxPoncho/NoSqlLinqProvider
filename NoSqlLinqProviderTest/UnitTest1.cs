using System;
using System.Collections.Generic;
using System.Linq;
using NoSqlLinqProvider;
using NUnit.Framework;

namespace NoSqlLinqProviderTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void PlacesAreEqualsTest()
        {
            var terraPlaces = new OrderedQueryable<Place>(new TerraServerQueryContext<Place>());

            var query = from place in terraPlaces
                        where place.Name == "Sacramento"
                        || place.State == "IN"
                        select place;
            List<string> results = new List<string>();
            foreach (Place place in query)
                results.Add(place.Name);
            Assert.AreEqual("Sacramento", results[0]);
            Assert.AreEqual("South Bend", results[1]);
        }
        [Test]
        public void PlacesAreStartsWithTest()
        {
            var terraPlaces = new OrderedQueryable<Place>(new TerraServerQueryContext<Place>());

            var query = from place in terraPlaces
                        where place.Name.StartsWith("Sac")
                        select place;
            List<string> results = new List<string>();
            foreach (Place place in query)
                results.Add(place.Name);
            Assert.AreEqual("Sacramento", results[0]);
            Assert.AreEqual(results.Count(), 1);
        }
    }
}
