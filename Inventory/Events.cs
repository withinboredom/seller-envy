using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory
{
    /// <summary>
    /// A inventory center was tied to a tenant
    /// </summary>
    public class InventoryCenterCreated
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
    /// Adds a new item into inventory
    /// </summary>
    public class AddedItemToInventory
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
    /// Fired when an item gets reserved
    /// </summary>
    public class ItemReserved
    {
        /// <summary>
        /// The inventory center id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The product being reserved
        /// </summary>
        public Guid Product { get; set; }

        /// <summary>
        /// The quantity of the product reserved
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Locations to reserve
        /// </summary>
        public List<Guid> Locations { get; set; } 
    }

    /// <summary>
    /// Indicates an item was removed from inventory
    /// </summary>
    public class ItemRemoved
    {
        /// <summary>
        /// The center's id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The product removed
        /// </summary>
        public Guid Product { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The location from which it was removed
        /// </summary>
        public List<Guid> Locations { get; set; }
    }
}
