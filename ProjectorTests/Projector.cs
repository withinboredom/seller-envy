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
    class ProjectorTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void hereWeGo()
        {
            var projector = new EventStoreStuff();
            Guid Id = Guid.NewGuid();
            Guid tenant = Guid.NewGuid();
            projector.SendCommand<InventoryAggregate, CreateInventoryCenter>(new CreateInventoryCenter
            {
                Id = Id,
                Tenant = tenant
            }).Wait();

            var agg = projector.GetAggregate<InventoryAggregate>(Id);
            agg.Wait();

            Assert.AreEqual(tenant, agg.Result.Tenant);
        }
    }
}
