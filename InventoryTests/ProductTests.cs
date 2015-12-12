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
    internal class ProductTests : BddTest.BaseTest<ProductAggregate>
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly Guid _tenant = Guid.NewGuid();
        private readonly string _productCategory = "Home";

        [Test(Description = "Describe a product")]
        public void AProductCanBeDescribed()
        {
            Test(
                Given(),
                When(new DescribeProduct
                {
                    Id = _id,
                    Title = "A Product",
                    ProductIds = new Dictionary<string, ProductIdType>
                    {
                        { "1234", ProductIdType.Upc }
                    },
                    Tags = new List<string> { "Tag" },
                    PictureUrls = new Dictionary<Uri, string>
                    {
                        { new Uri("http://apicture.com/test/png"), "A description" }
                    },
                    Sku = "1234",
                    ProductCategory = _productCategory,
                    Tenant = _tenant
                }),
                Then(new ProductDescribed
                {
                    Id = _id,
                    Title = "A Product",
                    ProductIds = new Dictionary<string, ProductIdType>
                    {
                        { "1234", ProductIdType.Upc }
                    },
                    Tags = new List<string> { "Tag" },
                    PictureUrls = new Dictionary<Uri, string>
                    {
                        { new Uri("http://apicture.com/test/png"), "A description" }
                    },
                    Sku = "1234",
                    ProductCategory = _productCategory,
                    Tenant = _tenant
                })
                );
        }
    }
}
