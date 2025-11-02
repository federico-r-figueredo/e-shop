using System;
using eShop.Services.Catalog.API.Infrastructure.Exceptions;

namespace eShop.Services.Catalog.API.Model {
    public class CatalogItem {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        internal string PictureURI { get; set; }
        public int CatalogTypeID { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CatalogBrandID { get; set; }
        public CatalogBrand CatalogBrand { get; set; }

        // Quanity in stock
        public int AvailableStock { get; set; }

        // Available stock at which we should reorder
        public int RestockThreshold { get; set; }

        // Maximum number of units that can be in-stock at any time 
        // (due ot physical / logistical constraints in warehouses)
        public int MaxStockThreshold { get; set; }

        // True is item is in reorder
        public bool IsOnReorder { get; set; }

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
        public int RemoveStock(int quantityDesired) {
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
        public int AddStock(int quantity) {
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