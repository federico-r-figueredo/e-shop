using System.IO;
using System.Net;
using System.Threading.Tasks;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.Catalog.API.Controllers {
    public class PicController : ControllerBase {
        private readonly IWebHostEnvironment environment;
        private readonly CatalogContext catalogContext;

        public PicController(IWebHostEnvironment environment, CatalogContext catalogContext) {
            this.environment = environment;
            this.catalogContext = catalogContext;
        }

        // GET api/v1/catalog/items/<id>/pic
        [HttpGet]
        [Route("api/v1/catalog/items/{catalogItemID:int}/pic")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetImage(int catalogItemID) {
            if (catalogItemID <= 0) {
                return BadRequest();
            }

            CatalogItem item = await
                this.catalogContext
                    .CatalogItems
                    .SingleOrDefaultAsync(x => x.ID == catalogItemID);

            if (item == null) return NotFound();

            string webRoot = this.environment.WebRootPath;
            string path = Path.Combine(webRoot, "img", item.PictureFileName);

            string imageFileExtension = Path.GetExtension(item.PictureFileName);
            string mimeType = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            byte[] buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimeType);
        }

        private string GetImageMimeTypeFromImageFileExtension(string imageFileExtension) {
            string mimeType;

            switch (imageFileExtension) {
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".bmp":
                    mimeType = "image/bmp";
                    break;
                case ".tiff":
                    mimeType = "image/tiff";
                    break;
                case ".wmf":
                    mimeType = "image/wmf";
                    break;
                case ".jp2":
                    mimeType = "image/jp2";
                    break;
                case ".svg":
                    mimeType = "image/svg+xml";
                    break;
                default:
                    mimeType = "application/octet-stream";
                    break;
            }

            return mimeType;
        }
    }
}