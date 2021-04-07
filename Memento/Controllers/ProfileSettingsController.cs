using Memento.Models;
using Memento.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Memento.Controllers
{
    public class ProfileSettingsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly MementoDbContext context;

        public ProfileSettingsController(UserManager<User> userManager, MementoDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ProfileSettings()
        {
            User user = await userManager.GetUserAsync(User);
            ProfileSettingsModel profSetts = new ProfileSettingsModel
            {
                Username = user.UserName,
                Email = user.Email,
                NewPassword = string.Empty,
                PasswordConfirm = string.Empty,
                ProfilePicture = user.ProfilePicture is null ? "" : Convert.ToBase64String(user.ProfilePicture)
            };

            return View(profSetts);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileSettings(ProfileSettingsModel profSetts)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.GetUserAsync(User);
                var updatedUser = new User { Id = user.Id, UserName = profSetts.Username };
            }
            return View(nameof(ProfileSettings));
        }
    }
}
