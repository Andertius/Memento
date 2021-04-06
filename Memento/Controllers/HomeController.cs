using Memento.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Memento.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Statistics")]
        public IActionResult Statistics()
        {
            //you code here
            return View("Statistics");
        }
        
        [Route("Play")]
        public IActionResult Play()
        {
            //you code here
            return View("Play");
        }
    }
}
