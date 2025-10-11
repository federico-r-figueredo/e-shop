using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eShop.Services.Catalog.API.Extensions;
using eShop.Services.Catalog.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace eShop.Services.Catalog.API.Infrastructure {
    internal class CatalogContextSeed {
        internal async Task SeedAsync(CatalogContext context, IWebHostEnvironment environment,
            IOptions<CatalogSettings> settings, ILogger<CatalogContextSeed> logger) {
            AsyncRetryPolicy policy = CreatePolicy(logger,
            nameof(CatalogContextSeed));

            await policy.ExecuteAsync(async () => {
                bool useCustomizationData = settings.Value.UseCustomizationData;
                string contentRootPath = environment.ContentRootPath;
                string picturePath = environment.WebRootPath;

                if (!context.CatalogBrands.Any()) {
                    await context.CatalogBrands.AddRangeAsync(
                        useCustomizationData
                            ? GetCatalogBrandsFromFile(contentRootPath, logger)
                            : GetPreconfiguredCatalogBrands()
                    );
                    await context.SaveChangesAsync();
                }

                if (!context.CatalogTypes.Any()) {
                    context.CatalogTypes.AddRange(
                        useCustomizationData
                            ? GetCatalogTypesFromFile(contentRootPath, logger)
                            : GetPreconfiguredCatalogTypes()
                    );
                    await context.SaveChangesAsync();
                }

                if (!context.CatalogItems.Any()) {
                    context.CatalogItems.AddRange(
                        useCustomizationData
                            ? GetCatalogItemsFromFile(contentRootPath, context, logger)
                            : GetPreconfiguredCatalogItems()
                    );
                    await context.SaveChangesAsync();
                }
            });
        }

        private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentRootPath,
            ILogger<CatalogContextSeed> logger) {
            string csvFileCatalogBrands = Path.Combine(contentRootPath, "Setup", "CatalogBrands.csv");

            if (!File.Exists(csvFileCatalogBrands)) {
                return GetPreconfiguredCatalogBrands();
            }

            string[] csvHeaders;
            try {
                string[] requiredHeaders = { "CatalogBrand" };
                csvHeaders = GetHeaders(csvFileCatalogBrands, requiredHeaders);
            } catch (Exception exception) {
                logger.LogError(exception, "EXCEPTION ERROR: {Message}", exception.Message);
                return GetPreconfiguredCatalogBrands();
            }

            return File
                .ReadAllLines(csvFileCatalogBrands)
                .Skip(1) // skip header row
                .SelectTry(x => CreateCatalogBrand(x))
                .OnCaughtException(ex => { logger.LogError(ex, "EXEPTION ERROR: {Message}", ex.Message); return null; })
                .Where(x => x != null);

        }

        private CatalogBrand CreateCatalogBrand(string brand) {
            brand = brand.Trim('"').Trim();

            if (string.IsNullOrEmpty(brand)) {
                throw new Exception("Catalog brand name is empty");
            }

            return new CatalogBrand() {
                Brand = brand
            };
        }

        private IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands() {
            return new List<CatalogBrand>() {
                new CatalogBrand() { Brand = "Azure" },
                new CatalogBrand() { Brand = ".NET" },
                new CatalogBrand() { Brand = "Visual Studio" },
                new CatalogBrand() { Brand = "SQL Server" },
                new CatalogBrand() { Brand = "Other" }
            };
        }

        private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentRootPath,
            ILogger<CatalogContextSeed> logger) {
            string csvCatalogTypes = Path.Combine(contentRootPath, "Setup", "CatalogTypes.csv");

            if (!File.Exists(csvCatalogTypes)) {
                return GetPreconfiguredCatalogTypes();
            }

            string[] csvHeaders;
            try {
                string[] requiredHeaders = { "CatalogType" };
                csvHeaders = GetHeaders(csvCatalogTypes, requiredHeaders);
            } catch (Exception exception) {
                logger.LogError(exception, "EXCEPTION ERROR: {Message}", exception.Message);
                return GetPreconfiguredCatalogTypes();
            }

            return File
                .ReadAllLines(csvCatalogTypes)
                .Skip(1)
                .SelectTry(x => CreateCatalogType(x))
                .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                .Where(x => x != null);
        }

        private CatalogType CreateCatalogType(string type) {
            type = type.Trim('"').Trim();

            if (string.IsNullOrEmpty(type)) {
                throw new Exception("Catalog type name is empty");
            }

            return new CatalogType() {
                Type = type
            };
        }

        private IEnumerable<CatalogType> GetPreconfiguredCatalogTypes() {
            return new List<CatalogType> {
                new CatalogType() { Type = "Mug" },
                new CatalogType() { Type = "T-Shirt" },
                new CatalogType() { Type = "Sheet" },
                new CatalogType() { Type = "USB Memory Stick" }
            };
        }

        private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentRootPath,
            CatalogContext context, ILogger<CatalogContextSeed> logger) {
            string csvCatalogItemsFile = Path.Combine(contentRootPath, "Setup", "CatalogItems.csv");

            if (!File.Exists(csvCatalogItemsFile)) {
                return GetPreconfiguredCatalogItems();
            }

            string[] csvHeaders;
            try {
                string[] requiredHeaders = {
                    "CatalogTypeName",
                    "CatalogBrandName",
                    "Description",
                    "Name",
                    "Price",
                    "PictureFileName"
                };
                string[] optionalHeaders = {
                    "AvailableStock",
                    "RestockTreshold",
                    "MaxStockTreshold",
                    "OnReorder"
                };
                csvHeaders = GetHeaders(csvCatalogItemsFile, requiredHeaders, optionalHeaders);
            } catch (Exception exception) {
                logger.LogError(exception, "EXCEPTION ERROR: {Message}", exception.Message);
                return GetPreconfiguredCatalogItems();
            }

            IDictionary<string, int> catalogTypeIDLookup = context.CatalogTypes.ToDictionary(x => x.Type, x => x.ID);
            IDictionary<string, int> catalogBrandIDLookup = context.CatalogBrands.ToDictionary(x => x.Brand, x => x.ID);

            return File
                .ReadAllLines(csvCatalogItemsFile)
                .Skip(1)
                .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                .SelectTry(column => CreateCatalogItem(column, csvHeaders, catalogTypeIDLookup, catalogBrandIDLookup))
                .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                .Where(x => x != null);
        }

        private CatalogItem CreateCatalogItem(string[] column, string[] headers,
            IDictionary<string, int> catalogTypeIDLookup, IDictionary<string, int> catalogBrandIDLookup) {
            if (column.Count() != headers.Count()) {
                throw new Exception($"Column count '{column.Count()}' not the same as headers count '{headers.Count()}'");
            }

            string catalogTypeName = column[Array.IndexOf(headers, "catalogtypename")].Trim('"').Trim();
            if (!catalogTypeIDLookup.ContainsKey(catalogTypeName)) {
                throw new Exception($"Type = {catalogTypeName} does not exist in CatalogTypes");
            }

            string catalogBrandName = column[Array.IndexOf(headers, "catalogbrandname")].Trim('"').Trim();
            if (!catalogBrandIDLookup.ContainsKey(catalogBrandName)) {
                throw new Exception($"Brand = {catalogBrandName} does not exist in Catalog Brands");
            }

            string priceString = column[Array.IndexOf(headers, "price")].Trim('"').Trim();
            if (!decimal.TryParse(priceString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price)) {
                throw new Exception($"Price = {priceString} is not a valid decimal number");
            }

            CatalogItem catalogItem = new CatalogItem() {
                CatalogTypeID = catalogTypeIDLookup[catalogTypeName],
                CatalogBrandID = catalogBrandIDLookup[catalogBrandName],
                Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim(),
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                Price = price,
                PictureFileName = column[Array.IndexOf(headers, "picturefilename")].Trim('"').Trim()
            };

            int availableStockIndex = Array.IndexOf(headers, "availablestock");
            if (availableStockIndex != -1) {
                string availableStockString = column[availableStockIndex].Trim('"').Trim();
                if (!string.IsNullOrEmpty(availableStockString)) {
                    if (int.TryParse(availableStockString, out int availableStock)) {
                        catalogItem.AvailableStock = availableStock;
                    } else {
                        throw new Exception($"AvailableStock = {availableStockString} is not a valid integer");
                    }
                }
            }

            int restockThresholdIndex = Array.IndexOf(headers, "restockthresholdindex");
            if (restockThresholdIndex != -1) {
                string restockThresholdString = column[restockThresholdIndex].Trim('"').Trim();
                if (int.TryParse(restockThresholdString, out int restockThreshold)) {
                    catalogItem.AvailableStock = restockThreshold;
                } else {
                    throw new Exception($"RestockThreshold = {restockThresholdString} is not a valid integer");
                }
            }

            int maxStockThresholdIndex = Array.IndexOf(headers, "maxstockthreshold");
            if (maxStockThresholdIndex != -1) {
                string maxStockThresholdString = column[maxStockThresholdIndex].Trim('"').Trim();
                if (int.TryParse(maxStockThresholdString, out int maxStockThreshold)) {
                    catalogItem.MaxStockThreshold = maxStockThreshold;
                } else {
                    throw new Exception($"MaxStockThreshold = {maxStockThresholdString} is not a valid integer");
                }
            }

            int isOnReorderIndex = Array.IndexOf(headers, "onreorder");
            if (isOnReorderIndex != -1) {
                string isOnReorderString = column[isOnReorderIndex].Trim('"').Trim();
                if (bool.TryParse(isOnReorderString, out bool isOnReorder)) {
                    catalogItem.IsOnReorder = isOnReorder;
                } else {
                    throw new Exception($"IsOnReorder = {isOnReorderString} is not a valid boolean");
                }
            }

            return catalogItem;
        }

        private IEnumerable<CatalogItem> GetPreconfiguredCatalogItems() {
            return new List<CatalogItem>() {
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = ".NET Bot Black Hoodie",
                    Name = ".NET Bot Black Hoodie",
                    Price = 19.5M,
                    PictureFileName = "1.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 1,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = ".NET Black & White Mug",
                    Name = ".NET Black & White Mug",
                    Price= 8.50M,
                    PictureFileName = "2.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Prism White T-Shirt",
                    Name = "Prism White T-Shirt",
                    Price = 12,
                    PictureFileName = "3.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = ".NET Foundation T-shirt",
                    Name = ".NET Foundation T-shirt",
                    Price = 12,
                    PictureFileName = "4.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 3,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Roslyn Red Sheet",
                    Name = "Roslyn Red Sheet",
                    Price = 8.5M,
                    PictureFileName = "5.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = ".NET Blue Hoodie",
                    Name = ".NET Blue Hoodie",
                    Price = 12,
                    PictureFileName = "6.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Roslyn Red T-Shirt",
                    Name = "Roslyn Red T-Shirt",
                    Price = 12,
                    PictureFileName = "7.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Kudu Purple Hoodie",
                    Name = "Kudu Purple Hoodie",
                    Price = 8.5M,
                    PictureFileName = "8.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 1,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Cup<T> White Mug",
                    Name = "Cup<T> White Mug",
                    Price = 12,
                    PictureFileName = "9.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 3,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = ".NET Foundation Sheet",
                    Name = ".NET Foundation Sheet",
                    Price = 12,
                    PictureFileName = "10.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 3,
                    CatalogBrandID = 2,
                    AvailableStock = 100,
                    Description = "Cup<T> Sheet",
                    Name = "Cup<T> Sheet",
                    Price = 8.5M,
                    PictureFileName = "11.png"
                },
                new CatalogItem() {
                    CatalogTypeID = 2,
                    CatalogBrandID = 5,
                    AvailableStock = 100,
                    Description = "Prism White TShirt",
                    Name = "Prism White TShirt",
                    Price = 12,
                    PictureFileName = "12.png"
                }
            };
        }

        private string[] GetHeaders(string csvFile, string[] requiredHeaders, string[] optionalHeaders = null) {
            string[] csvHeaders = File.ReadLines(csvFile).First().ToLowerInvariant().Split(',');

            if (csvHeaders.Count() < requiredHeaders.Count()) {
                throw new Exception(
                    $@"Required header count '{requiredHeaders.Count()}' is bigger than CSV 
                    header count '{csvHeaders.Count()}'"
                );
            }

            if (optionalHeaders != null
                && csvHeaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count())) {
                throw new Exception(
                    $@"CSV header count '{csvHeaders.Count()}' is larger than required 
                    '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}'
                    headers count"
                );
            }

            foreach (string requiredHeader in requiredHeaders) {
                if (!csvHeaders.Contains(requiredHeader.ToLowerInvariant())) {
                    throw new Exception(
                        $"Doesn't contain required header '{requiredHeader}'"
                    );
                }
            }

            return csvHeaders;
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<CatalogContextSeed> logger, string prefix,
            int retries = 3) {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, context) => {
                        logger.LogWarning(
                            exception,
                            @"[{prefix}] Exception {ExceptionType} with message {Message} 
                            detected on attempt {retry} of {retries}",
                            prefix,
                            exception.GetType().Name,
                            exception.Message,
                            retry,
                            retries
                        );
                    }
                );
        }
    }
}