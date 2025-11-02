using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogAPI;
using eShop.Services.Catalog.API.Extensions;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.Model;
using eShop.Services.Catalog.API.Settings;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CatalogAPI.Catalog;

namespace eShop.Services.Catalog.API.Controllers.gRPC {
    public class CatalogService : CatalogBase {
        private readonly CatalogContext catalogContext;
        private readonly CatalogSettings catalogSettings;
        private readonly ILogger<CatalogService> logger;

        public CatalogService(CatalogContext catalogContext,
            IOptionsSnapshot<CatalogSettings> catalogSettings,
            ILogger<CatalogService> logger) {
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            this.catalogSettings = catalogSettings.Value;
            this.logger = logger;
        }

        public override async Task<CatalogItemResponse> GetItemByID(
            CatalogItemRequest request, ServerCallContext context) {
            this.logger.LogInformation("Begin gRPC call CatalogService.GetItemByID for CatalogItem with ID = {ID}", request.Id);
            if (request.Id <= 0) {
                context.Status = new Status(StatusCode.FailedPrecondition, $"ID must be > 0 (received {request.Id})");
                return null;
            }

            CatalogItem item = await
                this.catalogContext
                    .CatalogItems
                    .SingleOrDefaultAsync(x => x.ID == request.Id);

            string baseURI = this.catalogSettings.PictureBaseURL;
            bool isAzureStorageEnabled = this.catalogSettings.AzureStorageEnabled;
            item.FillProductURL(baseURI, isAzureStorageEnabled);

            if (item == null) {
                context.Status = new Status(StatusCode.NotFound, $"CatalogItem with ID = {request.Id} does not exist.");
                return null;
            }

            return new CatalogItemResponse() {
                Id = item.ID,
                Description = item.Description,
                AvailableStock = item.AvailableStock,
                MaxStockThreshold = item.MaxStockThreshold,
                Name = item.Name,
                OnReorder = item.IsOnReorder,
                PictureFileName = item.PictureFileName,
                PictureUri = item.PictureURI,
                Price = (double)item.Price,
                RestockThreshold = item.RestockThreshold
            };
        }

        public override async Task<PaginatedItemsResponse> GetItemsByIDs(
            CatalogItemsRequest request, ServerCallContext context) {
            if (!string.IsNullOrEmpty(request.Ids)) {
                List<CatalogItem> items = await GetItemsByIDs(request.Ids);

                context.Status = !items.Any()
                    ? new Status(StatusCode.NotFound, $"ids value invalid. Must be comma-separated list of numbers")
                    : new Status(StatusCode.OK, string.Empty);

                return this.MapToResponse(items);
            }

            long itemsCount = await this.catalogContext.CatalogItems.LongCountAsync();

            List<CatalogItem> paginatedItems = await
                this.catalogContext
                    .CatalogItems
                    .OrderBy(x => x.Name)
                    .Skip(request.PageSize * request.PageIndex)
                    .Take(request.PageSize)
                    .ToListAsync();

            paginatedItems = ChangeURIPlaceholder(paginatedItems);

            PaginatedItemsResponse response = this.MapToResponse(
                paginatedItems,
                itemsCount,
                request.PageIndex,
                request.PageSize
            );

            return response;
        }

        private PaginatedItemsResponse MapToResponse(List<CatalogItem> items) {
            return this.MapToResponse(items, items.Count, 1, items.Count);
        }

        private PaginatedItemsResponse MapToResponse(List<CatalogItem> items, long count,
            int pageIndex, int pageSize) {
            PaginatedItemsResponse response = new PaginatedItemsResponse() {
                Count = count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            items.ForEach(x => {
                CatalogAPI.CatalogBrand brand = x.CatalogBrand == null
                    ? null
                    : new CatalogAPI.CatalogBrand() {
                        Id = x.CatalogBrand.ID,
                        Name = x.CatalogBrand.Brand
                    };

                CatalogAPI.CatalogType type = x.CatalogType == null
                    ? null
                    : new CatalogAPI.CatalogType() {
                        Id = x.CatalogType.ID,
                        Type = x.CatalogType.Type
                    };

                response.Data.Add(new CatalogItemResponse() {
                    Id = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    Price = (double)x.Price,
                    AvailableStock = x.AvailableStock,
                    MaxStockThreshold = x.MaxStockThreshold,
                    OnReorder = x.IsOnReorder,
                    PictureFileName = x.PictureFileName,
                    PictureUri = x.PictureURI,
                    RestockThreshold = x.RestockThreshold,
                    CatalogBrand = brand,
                    CatalogType = type
                });
            });

            return response;
        }

        private async Task<List<CatalogItem>> GetItemsByIDs(string ids) {
            IEnumerable<(bool Ok, int Value)> numIDs =
                ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIDs.All(x => x.Ok)) {
                return new List<CatalogItem>();
            }

            IEnumerable<int> idsToSelect = numIDs.Select(x => x.Value);

            List<CatalogItem> items = await
                this.catalogContext
                    .CatalogItems
                    .Where(x => idsToSelect.Contains(x.ID))
                    .ToListAsync();

            items = ChangeURIPlaceholder(items);

            return items;
        }

        private List<CatalogItem> ChangeURIPlaceholder(List<CatalogItem> items) {
            string baseURI = this.catalogSettings.PictureBaseURL;
            bool isAzureStorageEnabled = this.catalogSettings.AzureStorageEnabled;

            foreach (CatalogItem item in items) {
                item.FillProductURL(baseURI, isAzureStorageEnabled);
            }

            return items;
        }
    }
}