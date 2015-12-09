using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory;
using Objects;

namespace InventoryDomain
{
    public class LocationAggregate : Aggregate,
        IHandleCommand<DescribeLocation>,
        IHandleCommand<ChangeLocationCapacity>,
        IApplyEvent<DescribedLocation>,
        IApplyEvent<DecreaseCapacityAtLocation>,
        IApplyEvent<IncreaseCapacityAtLocation>,
        ISubscribeTo<AddedItemToInventory>
    {
        public string Name { get; set; }
        public LocationType LocationType { get; set; }
        public uint Capacity { get; set; }
        public ConcurrentBag<Guid> Items { get; set; }
        public Guid Tenant { get; set; }

        public int NumberItems => Items.Count;

        public LocationAggregate()
        {
            Items = new ConcurrentBag<Guid>();
        }

        public IEnumerable Handle(DescribeLocation c)
        {
            yield return new DescribedLocation
            {
                LocationType = c.LocationType,
                Id = c.Id,
                LocationName = c.LocationName,
                Tenant = c.Tenant
            };
        }

        public void Apply(DescribedLocation e)
        {
            Name = e.LocationName;
            LocationType = e.LocationType;
            Tenant = e.Tenant;
        }

        public IEnumerable HandleExternalEvent(AddedItemToInventory e)
        {
            if (e.Quantity + Items.Count > Capacity)
            {
                yield return new LocationOverCapacityAlert
                {
                    Id = e.Location
                };
            }

            yield return new AddedItemToLocation
            {
                Id = e.Location,
                Product = e.Product,
                Quantity = e.Quantity
            };
        }

        public IEnumerable Handle(ChangeLocationCapacity c)
        {
            if (c.ToCapacity > Capacity)
            {
                yield return new IncreaseCapacityAtLocation
                {
                    Id = c.Id,
                    NewCapacity = c.ToCapacity
                };
            }
            else
            {
                yield return new DecreaseCapacityAtLocation
                {
                    Id = c.Id,
                    NewCapacity = c.ToCapacity
                };
            }
        }

        public void Apply(DecreaseCapacityAtLocation e)
        {
            Capacity = e.NewCapacity;
        }

        public void Apply(IncreaseCapacityAtLocation e)
        {
            Capacity = e.NewCapacity;
        }
    }
}
