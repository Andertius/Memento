using Memento.Models;
using Memento.Models.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.IO;
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
                CurrentPassword = "",
                NewPassword = string.Empty,
                PasswordConfirm = string.Empty,
                ProfilePicture = user.ProfilePicture,
                NoPicture = user.ProfilePicture is null
            };

            return View(profSetts);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileSettings(ProfileSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.GetUserAsync(User);

                IdentityResult result = await userManager.SetUserNameAsync(user, model.Username);

                if (result.Succeeded)
                {
                    result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (model.ProfilePicture is not null)
                            {
                                ms.Read(model.ProfilePicture);
                                user.ProfilePicture = ms.ToArray();
                            }
                            else if (model.NoPicture)
                            {
                                user.ProfilePicture = null;
                            }
                        }

                        context.Users.Update(user);
                        context.SaveChanges();

                        return View(nameof(ProfileSettings));
                    }
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(nameof(ProfileSettings));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGeneralInfo(ProfileSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.GetUserAsync(User);

                IdentityResult result = await userManager.SetUserNameAsync(user, model.Username);

                if (result.Succeeded)
                {
                    return View(nameof(ProfileSettings));
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(nameof(ProfileSettings));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ProfileSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.GetUserAsync(User);

                IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    context.Users.Update(user);
                    context.SaveChanges();

                    return View(nameof(ProfileSettings));
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(nameof(ProfileSettings));
        }

        [HttpGet]
        public async Task<FileResult> GetPicture()
        {
            User user = await userManager.GetUserAsync(User);
            return File(user.ProfilePicture, "image/*");
        }
    }
}
