using Microsoft.AspNetCore.Mvc;

namespace no_DI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : Controller
    {
        
        [HttpGet]
        public IActionResult Index()
        {
            return Json(new { id=1 });
        }
    }
}
