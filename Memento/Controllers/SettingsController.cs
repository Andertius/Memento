using Memento.Models;
using Memento.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Memento.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly MementoDbContext context;

        public SettingsController(UserManager<User> userManager, MementoDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            User user = await userManager.GetUserAsync(User);
            Settings settings = context.Settings.Find(user.Id);

            if (settings is null)
            {
                return View(new SettingsModel());
            }

            return View(new SettingsModel
            {
                Username = user.UserName,
                HoursPerDay = settings.HoursPerDay,
                CardsPerDay = settings.CardsPerDay,
                CardsOrder = settings.CardsOrder.ToString(),
                DarkTheme = settings.DarkTheme,
                ShowImages = settings.ShowImages
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SettingsModel settingsModel)
        {
            User user = await userManager.GetUserAsync(User);
            Settings prevSettings = context.Settings.Find(user.Id);

            Settings settings = new Settings
            {
                UserId = user.Id,
                HoursPerDay = settingsModel.HoursPerDay,
                CardsPerDay = settingsModel.CardsPerDay,
                CardsOrder = Int32.Parse(settingsModel.CardsOrder),
                DarkTheme = settingsModel.DarkTheme,
                ShowImages = settingsModel.ShowImages
            };

            if (prevSettings != null)
            {
                context.Remove(prevSettings);
            }

            await context.AddAsync(settings);
            await context.SaveChangesAsync();

            return View(nameof(Settings), settingsModel);
        }
    }
}
