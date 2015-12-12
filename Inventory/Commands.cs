using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objects;

namespace Inventory
{
    /// <summary>
    /// Creates a new inventory center for a tenant
    /// </summary>
    public class CreateInventoryCenter : ICommandEvent
    {
        /// <summary>
        /// The id of the center
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The id of the tenant
        /// </summary>
        public Guid Tenant { get; set; }
    }

    /// <summary>
    /// Describes the ways a product may be identified by
    /// </summary>
    public enum ProductIdType
    {
        Upc,
        Asin,
        Ean,
        Isbn
    }

    /// <summary>
    /// Describes a condition that a product may be
    /// </summary>
    public enum ProductCondition
    {
        New,
        LikeNew,
        VeryGood,
        Good,
        Acceptable
    }

    /// <summary>
    /// Adds a new item into inventory
    /// </summary>
    public class AddItemToInventory : ICommandEvent
    {
        /// <summary>
        /// The inventory center id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The way in which this product is identified
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The type of the product identifier
        /// </summary>
        public ProductIdType ProductIdType { get; set; }

        /// <summary>
        /// The number of this item being added to inventory
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The condition of this item
        /// </summary>
        public ProductCondition Condition { get; set; }

        /// <summary>
        /// The cost of this item
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// The location this item is stored at
        /// </summary>
        public Guid Location { get; set; }

        /// <summary>
        /// The product this item is associated with
        /// </summary>
        public Guid Product { get; set; }
    }

    /// <summary>
    /// Reserve an item in the inventory center
    /// </summary>
    public class ReserveItem : ICommandEvent
    {
        /// <summary>
        /// The inventory center id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The product we want to reserve
        /// </summary>
        public Guid Product { get; set; }

        /// <summary>
        /// The quantity to reserve
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Locations to reserve
        /// </summary>
        public List<Guid> Locations { get; set; } 
    }

    /// <summary>
    /// Remove a reserved item from inventory
    /// </summary>
    public class RemoveItem : ICommandEvent
    {
        /// <summary>
        /// The center's id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The product we are removing
        /// </summary>
        public Guid Product { get; set; }

        /// <summary>
        /// The quantity to remove
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Locations to remove
        /// </summary>
        public List<Guid> Locations { get; set; } 
    }

    /// <summary>
    /// The types of locations
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// A Bin may contain any number of items
        /// </summary>
        Bin,

        /// <summary>
        /// A Slot can only contain a certain number of items
        /// </summary>
        Slot
    }

    /// <summary>
    /// Describes a location
    /// </summary>
    public class DescribeLocation : ICommandEvent
    {
        /// <summary>
        /// The id of the location
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The descriptive name of this location
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// The type this location is
        /// </summary>
        public LocationType LocationType { get; set; }

        /// <summary>
        /// The tenant this location belongs to
        /// </summary>
        public Guid Tenant { get; set; }
    }

    /// <summary>
    /// Increases the capacity of a location
    /// </summary>
    public class ChangeLocationCapacity : ICommandEvent {
        /// <summary>
        /// The location id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The new capacity
        /// </summary>
        public uint ToCapacity { get; set; }
    }

    /// <summary>
    /// Describes a product
    /// </summary>
    public class DescribeProduct : ICommandEvent
    {
        /// <summary>
        /// The id of the product
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title of the product
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The ways this item can be identified by
        /// </summary>
        public Dictionary<string, ProductIdType> ProductIds { get; set; }

        /// <summary>
        /// The tags to help categorize this product
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// The urls for photos
        /// </summary>
        public Dictionary<Uri, string> PictureUrls { get; set; }

        /// <summary>
        /// The sku of the product
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// The category of the product
        /// </summary>
        public string ProductCategory { get; set; }

        /// <summary>
        /// The tenant this product belongs to
        /// </summary>
        public Guid Tenant { get; set; }
    }
}
