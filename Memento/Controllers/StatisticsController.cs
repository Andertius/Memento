using Memento.Models;
using Memento.Models.ViewModels;

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
        public async Task<IActionResult> Statistics()
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            var data = _context.Statistics.Where(u => u.UserId == loggedInUser.Id).ToList();

            DateTime lastEntry = data[^1].Date;

            lastEntry = new DateTime(data[^1].Date.Year, data[^1].Date.Month, data[^1].Date.Day);

            DateTime today = DateTime.UtcNow;

            DateTime comparator = new DateTime(today.Year, today.Month, today.Day);

            if (lastEntry != comparator)
            {
                var average = 0.0;

                for (int i = 0; i < data.Count; i++)
                {
                    average += data[i].HoursPerDay;
                }

                average /= data.Count;

                var newStats = new UserStats
                {
                    UserId = loggedInUser.Id,
                    HoursPerDay = 0,
                    CardsPerDay = 0,
                    AverageHoursPerDay = (float)average,
                    Date = comparator,
                };

                _context.Statistics.Add(newStats);
                _context.SaveChanges();
            }
            return View(new StatisticsModel { Username = loggedInUser.UserName });
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

            var debugJson = Json(data);
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAverageHours(int dayNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetAverageHours(userId, dayNumber);
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetCards(int dayNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetCards(userId, dayNumber);
            return Json(data);
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetTodayStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stats = new GetUserStats(_context);

            var data = stats.GetTodayStats(userId);

            var debugJson = Json(data);

            return Json(data);
        }

        public int Day { get; set; }

    }
}
