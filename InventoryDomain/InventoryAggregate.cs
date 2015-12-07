using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Inventory;
using Objects;

namespace InventoryDomain
{
    public class InventoryAggregate : Aggregate,
        IHandleCommand<CreateInventoryCenter>,
        IHandleCommand<AddItemToInventory>,
        IHandleCommand<ReserveItem>,
        IHandleCommand<RemoveItem>,
        IApplyEvent<InventoryCenterCreated>,
        IApplyEvent<AddedItemToInventory>,
        IApplyEvent<ItemReserved>,
        IApplyEvent<ItemRemoved>
    {
        private class Location
        {
            public Guid Id { get; set; }
            public int NumberItems { get; set; }

            public int ReservedItems { get; set; }
        }

        private class Product
        {
            public Guid Id { get; set; }
            public List<Location> Locations { get; set; }

            public Product()
            {
                Locations = new List<Location>();
            } 
        }

        private Guid _tenant;
        private readonly ConcurrentBag<Product> _inventory;

        public InventoryAggregate()
        {
            _inventory = new ConcurrentBag<Product>();
        }

        public IEnumerable Handle(CreateInventoryCenter c)
        {
            //todo: Check that id isn't already created?
            yield return new InventoryCenterCreated
            {
                Id = c.Id,
                Tenant = c.Tenant
            };
        }

        public void Apply(InventoryCenterCreated e)
        {
            _tenant = e.Tenant;
        }

        public IEnumerable Handle(AddItemToInventory c)
        {
            yield return new AddedItemToInventory
            {
                ProductIdType = c.ProductIdType,
                Id = c.Id,
                Condition = c.Condition,
                Cost = c.Cost,
                Location = c.Location,
                Product = c.Product,
                ProductId = c.ProductId,
                Quantity = c.Quantity
            };
        }

        public void Apply(AddedItemToInventory e)
        {
            var items = from product in _inventory
                where product.Id == e.Product
                select product;

            if (!items.Any())
            {
                _inventory.Add(new Product
                {
                    Id = e.Product
                });
            }

            var locations = from product in items
                from location in product.Locations
                where location.Id == e.Location
                select location;

            if (locations.Any())
            {
                locations.AsParallel().ForAll(location => location.NumberItems += e.Quantity);
            }
            else
            {
                items.AsParallel().ForAll(l => l.Locations.Add(new Location
                {
                    Id = e.Location,
                    NumberItems = e.Quantity,
                    ReservedItems = 0
                }));
            }
        }

        public IEnumerable Handle(ReserveItem c)
        {
            var items = from product in _inventory
                where product.Id == c.Product
                from location in product.Locations
                where location.NumberItems - location.ReservedItems >= c.Quantity
                      && c.Locations.Contains(location.Id)
                select location;

            if (!items.Any())
                throw new Exception("Item not available for reserving");
            
            yield return new ItemReserved
            {
                Product = c.Product,
                Id = c.Id,
                Quantity = c.Quantity,
                Locations = c.Locations
            };
        }

        public void Apply(ItemReserved e)
        {
            var items = from product in _inventory
                where product.Id == e.Product
                from loc in product.Locations
                where e.Locations.Contains(loc.Id)
                select loc;

            items.AsParallel().ForAll(loc =>
            {
                loc.ReservedItems += e.Quantity;
            });
        }

        public IEnumerable Handle(RemoveItem c)
        {
            var items = from product in _inventory
                where product.Id == c.Product
                from loc in product.Locations
                where c.Locations.Contains(loc.Id) && loc.ReservedItems >= c.Quantity
                select loc;
            if (!items.Any())
            {
                throw new Exception("Item not reserved!");
            }

            yield return new ItemRemoved
            {
                Id = c.Id,
                Product = c.Product,
                Quantity = c.Quantity,
                Locations = c.Locations
            };
        }

        public void Apply(ItemRemoved e)
        {
            var items = from prod in _inventory
                where prod.Id == e.Product
                from loc in prod.Locations
                where e.Locations.Contains(loc.Id)
                select loc;

            items.AsParallel().ForAll(loc =>
            {
                loc.ReservedItems -= e.Quantity;
                loc.NumberItems -= e.Quantity;
            });
        }
    }
}
