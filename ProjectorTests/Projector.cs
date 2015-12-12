using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory;
using InventoryDomain;
using NUnit.Framework;
using Projector;

namespace ProjectorTests
{
    [TestFixture]
    internal class ProjectorTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void HereWeGo()
        {
            var projector = new EventProcessor();
            var id = Guid.NewGuid();
            var tenant = Guid.NewGuid();
            projector.SendCommand<InventoryAggregate, CreateInventoryCenter>(new CreateInventoryCenter
            {
                Id = id,
                Tenant = tenant
            }).Wait();

            projector.SendCommand<InventoryAggregate, AddItemToInventory>(new AddItemToInventory
            {
                Condition = ProductCondition.New,
                Product = Guid.NewGuid(),
                Id = id,
                Location = Guid.NewGuid(),
                Quantity = 5,
                ProductIdType = ProductIdType.Upc,
                Cost = 200,
                ProductId = "1234"
            }).Wait();

            var agg = projector.GetAggregate<InventoryAggregate>(id);
            agg.Wait();

            Assert.AreEqual(tenant, agg.Result.Tenant);
        }
    }
}
