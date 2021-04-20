﻿using Memento.Models;

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
