using System;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Memento.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userMgr, SignInManager<User> signInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
        }

        public ViewResult Login(string returnUrl)
            => View(new LoginModel
            {
                ReturnUrl = returnUrl
            });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByNameAsync(loginModel.Name);

                if (user is not null)
                {
                    await signInManager.SignOutAsync();

                    if ((await signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect(loginModel?.ReturnUrl ?? "/");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid name or password");
            return View(loginModel);
        }

        [Authorize]
        [Route("/account/logout")]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        public ViewResult Signup()
            => View(new SignupModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupModel signupModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = signupModel.UserName, Email = signupModel.Email };
                IdentityResult result = await userManager.CreateAsync(user, signupModel.Password);

                if (result.Succeeded)
                {
                    if ((await signInManager.PasswordSignInAsync(user.UserName, signupModel.Password, false, false)).Succeeded)
                    {
                        return Redirect("/");
                    }
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View();
        }

        //public ViewResult ChangePassword()
        //    => View();

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User user = await userManager.FindByEmailAsync(changePasswordModel.Email);

        //        if (user is not null)
        //        {
        //            var result = await userManager.ResetPasswordAsync(user, token:, changePasswordModel.NewPassword);

        //            if (result.Succeeded)
        //            {
        //                return RedirectToPage("/account/login");
        //            }

        //            foreach (IdentityError err in result.Errors)
        //            {
        //                ModelState.AddModelError("", err.Description);
        //            }
        //        }
        //    }

        //    return View();
        //}
    }
}
