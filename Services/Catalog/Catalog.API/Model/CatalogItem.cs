using System;
using eShop.Services.Catalog.API.Infrastructure.Exceptions;

namespace eShop.Services.Catalog.API.Model {
    internal class CatalogItem {
        internal int ID { get; set; }
        internal string Name { get; set; }
        internal string Description { get; set; }
        internal decimal Price { get; set; }
        internal string PictureFileName { get; set; }
        internal string PictureURI { get; set; }
        internal int CatalogTypeID { get; set; }
        internal CatalogType CatalogType { get; set; }
        internal int CatalogBrandID { get; set; }
        internal CatalogBrand CatalogBrand { get; set; }

        // Quanity in stock
        internal int AvailableStock { get; set; }

        // Available stock at which we should reorder
        internal int RestockThreshold { get; set; }

        // Maximum number of units that can be in-stock at any time 
        // (due ot physical / logistical constraints in warehouses)
        internal int MaxStockThreshold { get; set; }

        // True is item is in reorder
        internal bool IsOnReorder { get; set; }

        /// <summary>
        /// Decrements the quantity of a particular item in inventory and ensures the
        /// RestockThreshold hasn't been breached. If so, a RestockRequest is generated
        /// in CheckThreshold.
        /// 
        /// If there is sufficient stock of an item, then the integer returned at the end
        /// of this call should be the same as quantityDesired. In the event that there is
        /// not sufficient stock available, the method will remove whatever stock is 
        /// available and return that quantity to the client. In this case, it is 
        /// responsability of the client to determine if the amount that is returned is the
        /// same as quantityDesired. It is invalid to pass in a negative number.
        /// </summary>
        /// <param name="quantityDesired"></param>
        /// <returns>int: Returns the number actually removed from stock.</returns>
        internal int RemoveStock(int quantityDesired) {
            if (this.AvailableStock == 0) {
                throw new CatalogDomainException($"Empty stock, product item {Name} is sold out.");
            }

            if (quantityDesired <= 0) {
                throw new CatalogDomainException($"Item units desired should be greater than zero.");
            }

            int stockUnitsRemoved = Math.Min(quantityDesired, this.AvailableStock);
            this.AvailableStock -= stockUnitsRemoved;
            return stockUnitsRemoved;
        }

        /// <summary>
        /// Increments the quanity of a particular item in the inventory
        /// </summary>
        /// <returns>int: Returns the quantity that has been added to stock</returns>
        internal int AddStock(int quantity) {
            int originalStock = this.AvailableStock;

            // The quantity that the client is trying to add to stock is greater than what
            // can be physically accomodated in the warehouse.
            if ((this.AvailableStock + quantity) > this.MaxStockThreshold) {
                // For now, this method only adds new units up to the maximum stock threshold.
                // In an expanded version of this application, we could include tracking for
                // the remaining units and store information about overstock elsewhere.
                this.AvailableStock += (this.MaxStockThreshold - this.AvailableStock);
            } else {
                this.AvailableStock += quantity;
            }

            this.IsOnReorder = false;

            return this.AvailableStock - originalStock;
        }
    }
}