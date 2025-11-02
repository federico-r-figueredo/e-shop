using eShop.Services.Catalog.API.Model;

namespace eShop.Services.Catalog.API.Extensions {
    internal static class CatalogItemExtensions {
        internal static void FillProductURL(this CatalogItem catalogItem,
            string pictureBaseURL, bool isAzureStorageEnabled) {
            if (catalogItem != null) {
                catalogItem.PictureURI = isAzureStorageEnabled
                    ? pictureBaseURL + catalogItem.PictureFileName
                    : pictureBaseURL.Replace("[0]", catalogItem.ID.ToString());
            }
        }
    }
}