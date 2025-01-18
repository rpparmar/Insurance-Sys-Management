using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FormControl() { return View(); }
    }
}
