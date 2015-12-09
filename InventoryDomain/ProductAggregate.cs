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
    public class ProductAggregate : Aggregate,
        IHandleCommand<DescribeProduct>,
        IApplyEvent<ProductDescribed>
    {
        public Guid Tenant { get; set; }
        public string Title { get; set; }
        public ConcurrentDictionary<string, ProductIdType> ProductIds { get; set; } 
        public ConcurrentBag<string> Tags { get; set; }
        public ConcurrentDictionary<Uri, string> Photos { get; set; }
        public string Sku { get; set; }
        public string ProductCategory { get; set; }

        public ProductAggregate()
        {
            ProductIds = new ConcurrentDictionary<string, ProductIdType>();
            Tags = new ConcurrentBag<string>();
            Photos = new ConcurrentDictionary<Uri, string>();
        }

        public IEnumerable Handle(DescribeProduct c)
        {
            yield return new ProductDescribed
            {
                Tenant = c.Tenant,
                Id = c.Id,
                PictureUrls = c.PictureUrls,
                ProductCategory = c.ProductCategory,
                ProductIds = c.ProductIds,
                Sku = c.Sku,
                Tags = c.Tags,
                Title = c.Title
            };
        }

        public void Apply(ProductDescribed e)
        {
            Tenant = e.Tenant;
            Title = e.Title;
            ProductIds = new ConcurrentDictionary<string, ProductIdType>(e.ProductIds);
            Tags = new ConcurrentBag<string>(e.Tags);
            Photos = new ConcurrentDictionary<Uri, string>(e.PictureUrls);
            Sku = e.Sku;
            ProductCategory = e.ProductCategory;
        }
    }
}
