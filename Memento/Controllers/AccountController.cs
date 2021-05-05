using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using Memento.Configuration;
using Memento.Models;
using Memento.Models.ViewModels.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Memento.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly EmailConfig emailConfig;
        private readonly MementoDbContext context;

        private User user;
        public AccountController(UserManager<User> userMgr, SignInManager<User> signInMgr, IOptions<EmailConfig> opts, MementoDbContext context)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            emailConfig = opts.Value;
            this.context = context;
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
                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        return View("Error", new AccountErrorModel()
                        {
                            Title = "Error",
                            Message = "To log in, first confirm your email, by going to the link we've sent you."
                        });
                    }

                    await signInManager.SignOutAsync();

                    if ((await signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, false, false)).Succeeded)
                    {
                        var data = context.Statistics.Where(u => u.UserId == user.Id).ToList();

                        DateTime lastEntry = data[^1].Date;
                        lastEntry = new DateTime(data[^1].Date.Year, data[^1].Date.Month, data[^1].Date.Day);
                        DateTime today = DateTime.UtcNow;

                        var comparator = new DateTime(today.Year, today.Month, today.Day);

                        if(lastEntry != comparator)
                        {
                            var average = 0.0;

                            for (int i = 0; i < data.Count; i++)
                            {
                                average += data[i].HoursPerDay;
                            }

                            average /= data.Count;

                            var newStats = new UserStats
                            {
                                UserId = user.Id,
                                HoursPerDay = 0,
                                CardsPerDay = 0,
                                AverageHoursPerDay = (float)average,
                                Date = comparator,
                            };

                            context.Statistics.Add(newStats);
                            context.SaveChanges();
                        }

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
                user = new User { UserName = signupModel.UserName, Email = signupModel.Email };
                IdentityResult result = await userManager.CreateAsync(user, signupModel.Password);

                if (result.Succeeded)
                {
                    if ((await signInManager.PasswordSignInAsync(user.UserName, signupModel.Password, false, false)).Succeeded)
                    {
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                        SendEmailConfirmationEmail(
                            $"Dear {user.UserName},\nHere is the link to confirm your email: {confirmationLink}",
                            user.Email);

                        var newStats = new UserStats
                        {
                            UserId = user.Id,
                            HoursPerDay = 0,
                            CardsPerDay = 0,
                            AverageHoursPerDay = 0,
                            Date = DateTime.Now,
                        };

                        context.Statistics.Add(newStats);
                        context.SaveChanges();

                        return View("Error", new AccountErrorModel() {
                            Title = "Registration successful",
                            Message = "To log in, first confirm your email, by going to the link we've sent to your email address."
                        });
                    }
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId is null || token is null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return RedirectToAction("index", "home");
            }

            if ((await userManager.ConfirmEmailAsync(user, token)).Succeeded)
            {
                var settings = new Settings
                {
                    UserId = user.Id,
                    HoursPerDay = 0,
                    CardsPerDay = 0,
                    CardsOrder = 0,
                    DarkTheme = false,
                    ShowImages = true
                };

                await context.AddAsync(settings);
                await context.SaveChangesAsync();

                return View();
            }

            return View("Error", new AccountErrorModel()
            {
                Title = "Cannot confirm email.",
                Message = "There was a problem confirming your email, please try again later.",
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token is null || email is null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user is not null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }

                    return View("ResetPasswordConfirmation");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
            => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user is not null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

                    SendPasswordResetEmail(
                        $"Dear {user.UserName},\nHere is the link to reset your password: {passwordResetLink}",
                        user.Email);

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        private void SendEmailConfirmationEmail(string message, string to)
        {
            using var mailMessage = new MailMessage(emailConfig.Address, to)
            {
                Subject = "Email Confirmation",
                Body = message,
            };

            SendEmail(mailMessage);
        }

        private void SendPasswordResetEmail(string message, string to)
        {
            using var mailMessage = new MailMessage(emailConfig.Address, to)
            {
                Subject = "Password reset",
                Body = message,
            };

            SendEmail(mailMessage);
        }

        private void SendEmail(MailMessage mailMessage)
        {
            var smtp = new SmtpClient(emailConfig.Host, emailConfig.Port)
            {
                Credentials = new NetworkCredential()
                {
                    UserName = emailConfig.Address,
                    Password = emailConfig.Password,
                },

                EnableSsl = true,
            };

            smtp.Send(mailMessage);
        }
    }
}
