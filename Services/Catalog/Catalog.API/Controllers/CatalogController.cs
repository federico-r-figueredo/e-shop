using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eShop.Services.Catalog.API.Extensions;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.IntegrationEvents;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using eShop.Services.Catalog.API.Model;
using eShop.Services.Catalog.API.Settings;
using eShop.Services.Catalog.API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace eShop.Services.Catalog.API.Controllers {
    [ApiController]
    [Route("api/v1/catalog")]
    public class CatalogController : ControllerBase {
        private readonly CatalogContext catalogContext;
        private readonly CatalogSettings catalogSettings;
        private readonly ICatalogIntegrationEventService catalogIntegrationEventService;

        public CatalogController(CatalogContext catalogContext,
            IOptionsSnapshot<CatalogSettings> catalogSettings,
            ICatalogIntegrationEventService catalogIntegrationEventService) {
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            this.catalogSettings = catalogSettings.Value;
            this.catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogContext));

            catalogContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(List<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Items([FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0, string ids = null) {
            if (!string.IsNullOrEmpty(ids)) {
                List<CatalogItem> catalogItems = await GetItemsByIDAsync(ids);

                if (!catalogItems.Any()) {
                    return BadRequest($"{nameof(ids)} value invalid. Must be comma-separated list of numbers");
                }

                return Ok(catalogItems);
            }

            long itemCount = await this.catalogContext.CatalogItems.LongCountAsync();

            List<CatalogItem> paginatedItems = await
                this.catalogContext
                    .CatalogItems
                    .OrderBy(x => x.Name)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

            paginatedItems = ChangeURIPlaceholder(paginatedItems);

            PaginatedItemsViewModel viewModel =
                new PaginatedItemsViewModel(
                    pageIndex, pageSize, itemCount, paginatedItems
                );

            return Ok(viewModel);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogItem>> ItemByID(int id) {
            if (id <= 0) {
                return BadRequest();
            }

            CatalogItem catalogItem = await
                this.catalogContext
                    .CatalogItems
                    .SingleOrDefaultAsync(x => x.ID == id);

            string pictureBaseURI = this.catalogSettings.PictureBaseURL;
            bool isAzureStorageEnabled = this.catalogSettings.AzureStorageEnabled;

            catalogItem.FillProductURL(pictureBaseURI, isAzureStorageEnabled);

            if (catalogItem == null) return NotFound();

            return catalogItem;
        }

        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel>> ItemsWithName(string name,
            [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0) {
            long totalCatalogItems = await
                this.catalogContext
                    .CatalogItems
                    .Where(x => x.Name.Contains(name))
                    .LongCountAsync();

            List<CatalogItem> paginatedCatalogItems = await
                this.catalogContext
                    .CatalogItems
                    .Where(x => x.Name.Contains(name))
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

            paginatedCatalogItems = ChangeURIPlaceholder(paginatedCatalogItems);

            return new PaginatedItemsViewModel(pageIndex, pageSize, totalCatalogItems, paginatedCatalogItems);
        }

        // GET api/v1/[controller]/items/type/1/brand[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items/type/{catalogTypeID:int}/brand/{catalogBrandID:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel>> ItemsByTypeIDAndBrandID(
            int catalogTypeID, int? catalogBrandID, [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0
        ) {
            IQueryable<CatalogItem> root = this.catalogContext.CatalogItems;

            root = root.Where(x => x.CatalogTypeID == catalogTypeID);

            if (catalogBrandID.HasValue) {
                root = root.Where(x => x.CatalogBrandID == catalogBrandID);
            }

            long catalogItemCount = await root.LongCountAsync();

            List<CatalogItem> pagintatedCatalogItems = await
                root.Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

            pagintatedCatalogItems = ChangeURIPlaceholder(pagintatedCatalogItems);

            return new PaginatedItemsViewModel(pageIndex, pageSize, catalogItemCount, pagintatedCatalogItems);
        }

        // GET api/v1/[controller]/items/type/all/brand[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("items/type/all/brand/{catalogBrandID:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel), (int)HttpStatusCode.OK)]
        public async Task<PaginatedItemsViewModel> ItemsByBrandID(int? catalogBrandID,
            [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0) {
            IQueryable<CatalogItem> root = this.catalogContext.CatalogItems;

            if (catalogBrandID.HasValue) {
                root = root.Where(x => x.CatalogBrandID == catalogBrandID);
            }

            long itemsCount = await root.LongCountAsync();

            List<CatalogItem> paginatedItems = await
                root.Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();

            return new PaginatedItemsViewModel(pageIndex, pageSize, itemsCount, paginatedItems);
        }

        // GET api/v1/[controller]/types
        [HttpGet]
        [Route("types")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogType>>> Types() {
            return await this.catalogContext.CatalogTypes.ToListAsync();
        }

        // GET api/v1/[controller]/brands
        [HttpGet]
        [Route("brands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogBrand>>> Brands() {
            return await this.catalogContext.CatalogBrands.ToListAsync();
        }

        // PUT api/v1/[controller]/items
        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateProduct([FromBody] CatalogItem productToUpdate) {
            CatalogItem catalogItem = await
                this.catalogContext
                    .CatalogItems
                    .SingleOrDefaultAsync(x => x.ID == productToUpdate.ID);

            if (catalogItem == null) {
                return NotFound(
                    new { Message = $"Item with ID {productToUpdate.ID} not found." }
                );
            }

            decimal oldPrice = catalogItem.Price;
            bool shouldRaiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            // Update current product
            catalogItem = productToUpdate;
            this.catalogContext.CatalogItems.Update(catalogItem);

            if (shouldRaiseProductPriceChangedEvent) { // Save product's data and publish integration event through the event bus if price has changed

                // Create integration event to be published through the event bus
                ProductPriceChangedIntegrationEvent priceChangedEvent =
                    new ProductPriceChangedIntegrationEvent(
                        catalogItem.ID,
                        productToUpdate.Price,
                        oldPrice
                    );

                // Achieving atomicity between original Catalog database operation and the
                // IntegrationEventLog thanks to a local transaction.
                await this.catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                await this.catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            } else {
                await this.catalogContext.SaveChangesAsync();
            }

            return CreatedAtAction("ItemByID", new { id = productToUpdate.ID }, null);
        }

        // POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateItem([FromBody] CatalogItem itemToCreate) {
            CatalogItem item = new CatalogItem() {
                CatalogBrandID = itemToCreate.CatalogBrandID,
                CatalogTypeID = itemToCreate.CatalogTypeID,
                Description = itemToCreate.Description,
                Name = itemToCreate.Name,
                PictureFileName = itemToCreate.PictureFileName,
                Price = itemToCreate.Price
            };

            this.catalogContext.CatalogItems.Add(item);

            await this.catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByID), new { id = item.ID }, null);
        }

        // DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteItem(int id) {
            CatalogItem item = await
                this.catalogContext.CatalogItems.SingleOrDefaultAsync(x => x.ID == id);

            if (item == null) {
                return NotFound();
            }

            this.catalogContext.CatalogItems.Remove(item);

            await this.catalogContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task<List<CatalogItem>> GetItemsByIDAsync(string ids) {
            IEnumerable<(bool Ok, int Value)> numericIDs =
                ids.Split(',').Select(x => (Ok: int.TryParse(x, out int y), Value: y));

            if (!numericIDs.All(x => x.Ok)) {
                return new List<CatalogItem>();
            }

            IEnumerable<int> idsToSelect = numericIDs.Select(x => x.Value);

            List<CatalogItem> catalogItems = await
                this.catalogContext
                    .CatalogItems
                    .Where(x => idsToSelect.Contains(x.ID))
                    .ToListAsync();

            catalogItems = ChangeURIPlaceholder(catalogItems);

            return catalogItems;
        }

        private List<CatalogItem> ChangeURIPlaceholder(List<CatalogItem> catalogItems) {
            string baseURI = this.catalogSettings.PictureBaseURL;
            bool isAzureStorageEnabled = this.catalogSettings.AzureStorageEnabled;

            foreach (CatalogItem catalogItem in catalogItems) {
                catalogItem.FillProductURL(baseURI, isAzureStorageEnabled: isAzureStorageEnabled);
            }

            return catalogItems;
        }
    }
}