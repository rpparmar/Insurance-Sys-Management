using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
