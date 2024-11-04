using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    //[ApiController]
    //[Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
