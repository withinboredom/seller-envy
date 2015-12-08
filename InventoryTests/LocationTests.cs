using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory;
using InventoryDomain;
using NUnit.Framework;

namespace InventoryTests
{
    [TestFixture]
    [Category("Inventory")]
    internal class LocationTests : BddTest.BaseTest<LocationAggregate>
    {
        private readonly Guid _id = Guid.NewGuid();
        private const string TestLocation = "test";

        [SetUp]
        public void SetUp()
        {

        }

        private DescribeLocation AUserDescribesALocation(string locationName, LocationType locationType)
        {
            return new DescribeLocation
            {
                LocationName = locationName,
                LocationType = locationType
            };
        }

        private DescribedLocation AUserDescribedALocation(string locationName, LocationType locationType)
        {
            return new DescribedLocation
            {
                LocationName = locationName,
                LocationType = locationType
            };
        }

        private AddedItemToInventory AUserAddedItemToInventory(Guid inventory, Guid product)
        {
            return new AddedItemToInventory
            {
                Id = inventory,
                Product = product,
                Location = _id,
                ProductIdType = ProductIdType.Asin,
                Quantity = 1,
                Condition = ProductCondition.New,
                Cost = 100,
                ProductId = "1234"
            };
        }

        private AddedItemToLocation AnItemWasAddedToLocation(Guid inventory, Guid product)
        {
            return new AddedItemToLocation
            {
                Id = _id,
                Product = product,
                Quantity = 1
            };
        }

        [Test(Description = "A User Describes a Location")]
        public void DescribeLocation()
        {
            Test(Given(),
                When(AUserDescribesALocation(TestLocation, LocationType.Bin)),
                Then(AUserDescribedALocation(TestLocation, LocationType.Bin)));
        }

        [Test(Description = "An item is added to the location")]
        public void AddItemToLocation()
        {
            var inventory = Guid.NewGuid();
            var product = Guid.NewGuid();
            Test(
                Given(
                    AUserDescribedALocation(TestLocation, LocationType.Bin),
                new IncreaseCapacityAtLocation
                {
                    Id = _id,
                    NewCapacity = 1
                }
                ),
                When(AUserAddedItemToInventory(inventory, product)),
                Then(AnItemWasAddedToLocation(inventory, product))
                );
        }
    }
}
