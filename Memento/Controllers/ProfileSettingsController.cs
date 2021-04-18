using Memento.Models;
using Memento.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfileSettings()
        {
            User user = await userManager.GetUserAsync(User);
            return View(new ProfileSettingsModel
            {
                Username = user.UserName,
                Email = user.Email,
                CurrentPassword = string.Empty,
                NewPassword = string.Empty,
                PasswordConfirm = string.Empty,
                NoPicture = user.ProfilePicture is null
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateGeneralInfo(ProfileSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.GetUserAsync(User);

                IdentityResult result = await userManager.SetUserNameAsync(user, model.Username);

                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(model.Email))
                        model.Email = user.Email;

                    return View(nameof(ProfileSettings), model);
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(nameof(ProfileSettings), model);
        }

        [Authorize]
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

                    if (string.IsNullOrEmpty(model.Username))
                        model.Username = user.UserName;

                    if (string.IsNullOrEmpty(model.Email))
                        model.Email = user.Email;

                    return View(nameof(ProfileSettings), model);
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(nameof(ProfileSettings), model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SavePicture(ProfileSettingsModel model)
        {
            User user = await userManager.GetUserAsync(User);

            using (MemoryStream ms = new MemoryStream())
            {
                if (model.ProfilePicture is not null)
                {
                    model.ProfilePicture.CopyTo(ms);
                    user.ProfilePicture = ms.ToArray();
                }
                else
                {
                    user.ProfilePicture = null;
                }
            }

            context.Users.Update(user);
            context.SaveChanges();

            if (string.IsNullOrEmpty(model.Username))
                model.Username = user.UserName;

            if (string.IsNullOrEmpty(model.Email))
                model.Email = user.Email;

            model.NoPicture = model.ProfilePicture is null;

            return View(nameof(ProfileSettings), model);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetPrevPicture()
        //{
        //    User user = await userManager.GetUserAsync(User);
        //    return View(new ProfileSettingsModel
        //    {
        //        Username = user.UserName,
        //        Email = user.Email,
        //        CurrentPassword = string.Empty,
        //        NewPassword = string.Empty,
        //        PasswordConfirm = string.Empty,
        //        NoPicture = user.ProfilePicture is null
        //    });
        //}

        [HttpGet]
        public async Task<FileResult> GetPicture()
        {
            User user = await userManager.GetUserAsync(User);
            return File(user.ProfilePicture, "image/*");
        }
    }
}
