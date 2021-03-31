using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace Memento.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Settings()
            => View(new SettingsModel());

        [Route("Statistics")]
        public IActionResult Statistics()
        {
            //you code here
            return View("Statistics");
        }
    }
}
