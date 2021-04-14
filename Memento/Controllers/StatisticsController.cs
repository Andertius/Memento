using Memento.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Memento.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MementoDbContext _context;
        private readonly UserManager<User> userManager;

        public StatisticsController(MementoDbContext context, UserManager<User> userMgr)
        {
            _context = context;
            userManager = userMgr;
        }

        [Authorize]
        public IActionResult Statistics()
        {
            //you code here
            return View();
        }

        [Authorize]
        public ActionResult Graphs()
        {
            Day = 30;
            return PartialView();
        }

        [Authorize]
        public ActionResult ProgressBars()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetHours(int dayNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetHours(userId, dayNumber);
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAverageHours()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetAverageHours(userId, Day);
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetCards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetCards(userId, Day);
            return Json(data);
        }

        public int Day { get; set; }

    }
}
