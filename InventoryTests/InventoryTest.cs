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
    class InventoryTest : BddTest.BaseTest<InventoryAggregate>
    {
        private Guid _testId;
        private Guid _tenantId;
        private string _testString;
        private int _testOne;
        private int _testTwo;
        private Guid _locationId;
        private Guid _productId;

        [SetUp]
        public void Setup()
        {
            _testId = Guid.NewGuid();
            _tenantId = Guid.NewGuid();
            _locationId = Guid.NewGuid();
            _productId = Guid.NewGuid();
            _testString = "test";
            _testOne = 1;
            _testTwo = 5;
        }

        [Test]
        public void CreateInventoryCenter()
        {
            Test(
                Given(),
                When(new CreateInventoryCenter
                {
                    Id = _testId,
                    Tenant = _tenantId
                }),
                Then(new InventoryCenterCreated
                {
                    Id = _testId,
                    Tenant = _tenantId
                }));
        }

        [Test]
        public void AddItemToInventory()
        {
            Test(
                Given(new InventoryCenterCreated
                {
                    Id = _testId,
                    Tenant = _tenantId
                }),
                When(new AddItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }),
                Then(new AddedItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }));
        }

        [Test]
        public void CanAddMultipleItemsToSameLocation()
        {
            Test(
                Given(new InventoryCenterCreated
                {
                    Id = _testId,
                    Tenant = _tenantId
                }, new AddedItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }, new AddedItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }, new ItemReserved
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }),
                When(new RemoveItem
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }),
                Then(new ItemRemoved
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }));
        }

        [Test]
        public void RemoveItemFromInventory()
        {
            Test(
                Given(new InventoryCenterCreated
                {
                    Id = _testId,
                    Tenant = _tenantId
                }, new AddedItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }, new ItemReserved
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }),
                When(new RemoveItem
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }),
                Then(new ItemRemoved
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                })
                );
        }

        [Test]
        public void ReserveItemInInventory()
        {
            Test(
                Given(new InventoryCenterCreated
                {
                    Id = _testId,
                    Tenant = _tenantId
                }, new AddedItemToInventory
                {
                    Id = _testId,
                    ProductId = _testString,
                    ProductIdType = ProductIdType.Asin,
                    Quantity = _testOne,
                    Condition = ProductCondition.New,
                    Cost = _testTwo,
                    Location = _locationId,
                    Product = _productId
                }),
                When(new ReserveItem
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                }),
                Then(new ItemReserved
                {
                    Id = _testId,
                    Product = _productId,
                    Quantity = _testOne,
                    Locations = new List<Guid> { _locationId }
                })
                );
        }

        [Test]
        public void ItemCantBeRemovedUnlessReserved()
        {
            Assert.Pass();
        }

        [Test]
        public void ItemCantBeReservedIfAlreadyReserved()
        {
            Assert.Pass();
        }

        [Test]
        public void ItemCantBeRemovedIfAlreadyRemoved()
        {
            Assert.Pass();
        }
    }
}
