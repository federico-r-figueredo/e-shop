
using Microsoft.AspNetCore.Mvc;

namespace EShop.Services.Ordering.API.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HomeController : Controller {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index() {
            return new RedirectResult("~/swagger");
        }
    }
}