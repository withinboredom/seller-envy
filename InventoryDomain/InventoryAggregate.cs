using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
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
        IApplyEvent<ItemRemoved>,
        IRequire
    {
        internal class Location : LocationAggregate
        {
            public int ReservedItems { get; set; }


        }

        internal class Product : ProductAggregate
        {
            public List<Location> Locations { get; set; }

            public Product()
            {
                Locations = new List<Location>();
            }
        }

        public Guid Tenant { get; internal set; }
        private readonly ConcurrentBag<Product> _inventory;
        private IProcessor _processor;

        public IEnumerable<ProductAggregate> GetProductLocations(Guid aProduct)
        {
            return from product in _inventory
                   where product.Id == aProduct
                   select product;
        }

        public IEnumerable<ProductAggregate> GetProductsInLocation(Guid aLocation)
        {
            return (from product in _inventory
                    from location in product.Locations
                    where location.Id == aLocation
                    select product).Distinct();
        }

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
            Tenant = e.Tenant;
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
                    Id = e.Product,
                });
            }

            var locations = from product in items
                            from location in product.Locations
                            where location.Id == e.Location
                            select location;

            if (!locations.Any())
            {
                items.AsParallel().ForAll(l => l.Locations.Add(new Location
                {
                    Id = e.Location,
                    ReservedItems = 0
                }));
            }
        }

        private async Task<ConcurrentBag<Product>> LoadInventory(ConcurrentBag<Product> prods)
        {
            var replaceCopy = new ConcurrentBag<Product>();

            foreach (var item in _inventory)
            {
                var newItem = await _processor.GetAggregate<ProductAggregate>(item.Id).ConfigureAwait(false) as Product;
                foreach (var location in item.Locations)
                {
                    var newLocation = await _processor.GetAggregate<LocationAggregate>(location.Id).ConfigureAwait(false) as Location;
                    newItem.Locations.Add(newLocation);
                }
                replaceCopy.Add(newItem);
            }

            return replaceCopy;
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

        public async Task Initialize(IProcessor processor)
        {
            await Task.Run(() => _processor = processor).ConfigureAwait(false);
        }
    }
}
