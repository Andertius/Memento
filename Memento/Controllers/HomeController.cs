using Microsoft.AspNetCore.Mvc;

namespace Memento.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route(nameof(About))]
        public IActionResult About()
            => View();
    }
}
