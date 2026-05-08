using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using MyPortfolio.Helpers;
using MyPortfolio.Helpers.CustomerServiceModels;
using System.Security.Claims;

namespace MyPortfolio.Controllers
{
    public class AccountController(UserManager<User> _userManager,
                                   RoleManager<Role> _roleManager,
                                   SignInManager<User> _signInManager,
                                   IMailService _mailService,
                                   IRoleChangeRequestService _roleChangeRequestService) : Controller
    {
        #region Register
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var isFirstUser = !_userManager.Users.Any();

            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = $"{registerViewModel.FirstName} {registerViewModel.LastName}",
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                CreatedBy = "SYSTEM",
                LastModifiedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                foreach (var role in Enum.GetValues(typeof(E_Role)).Cast<E_Role>())
                {
                    if (!await _roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await _roleManager.CreateAsync(new Role
                        {
                            Name = role.ToString(),
                            CreatedBy = "SYSTEM",
                            LastModifiedBy = "SYSTEM",
                            CreatedOn = DateTime.UtcNow,
                            LastModifiedOn = DateTime.UtcNow,
                            IsDeleted = false
                        });
                    }
                }

                if (isFirstUser)
                {
                    await _userManager.AddToRoleAsync(user, E_Role.Admin.ToString());
                    TempData["Message"] = "Registration successful. You are the first user and have been assigned as Admin.";
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, E_Role.Pending.ToString());

                    // إنشاء طلب تغيير الصلاحية تلقائياً
                    var userRole = await _roleManager.FindByNameAsync(E_Role.User.ToString());
                    if (userRole != null)
                    {
                        await _roleChangeRequestService.CreateRequestAsync(user.Id, userRole.Id);
                    }

                    TempData["Message"] = "Registration successful. Please wait for admin approval.";
                }

                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerViewModel);
            }
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = await _userManager.FindByEmailAsync(viewModel.Email);

            if (user is not null)
            {
                var isPending = await _userManager.IsInRoleAsync(user, "Pending");
                if (isPending)
                {
                    ModelState.AddModelError(string.Empty, "Your account is pending approval. Please wait for admin confirmation.");
                    return View(viewModel);
                }

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                var Result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);

                if (Result.Succeeded)
                {
                    if (isAdmin)
                    {
                        TempData["Message"] = "Welcome, Admin!";
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Login");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            var claims = result.Principal?.Identities?.FirstOrDefault()?.Claims;
            if (claims != null)
            {
                var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (emailClaim != null)
                {
                    var user = await _userManager.FindByEmailAsync(emailClaim.Value);

                    if (user == null)
                    {
                        var isFirstUser = !_userManager.Users.Any();

                        user = new User
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = emailClaim.Value,
                            Email = emailClaim.Value,
                            FullName = nameClaim?.Value ?? emailClaim.Value,
                            CreatedBy = "SYSTEM",
                            LastModifiedBy = "SYSTEM",
                            CreatedOn = DateTime.UtcNow,
                            LastModifiedOn = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        var createResult = await _userManager.CreateAsync(user);

                        if (!createResult.Succeeded)
                        {
                            TempData["Message"] = "An error occurred while creating your account.";
                            return RedirectToAction(nameof(Login));
                        }

                        foreach (var role in Enum.GetValues(typeof(E_Role)).Cast<E_Role>())
                        {
                            if (!await _roleManager.RoleExistsAsync(role.ToString()))
                            {
                                await _roleManager.CreateAsync(new Role
                                {
                                    Name = role.ToString(),
                                    CreatedBy = "SYSTEM",
                                    LastModifiedBy = "SYSTEM",
                                    CreatedOn = DateTime.UtcNow,
                                    LastModifiedOn = DateTime.UtcNow,
                                    IsDeleted = false
                                });
                            }
                        }

                        if (isFirstUser)
                        {
                            await _userManager.AddToRoleAsync(user, E_Role.Admin.ToString());
                            TempData["Message"] = "Registration successful. You are the first user and have been assigned as Admin.";
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, E_Role.Pending.ToString());

                            // إنشاء طلب تغيير الصلاحية تلقائياً
                            var userRole = await _roleManager.FindByNameAsync(E_Role.User.ToString());
                            if (userRole != null)
                            {
                                await _roleChangeRequestService.CreateRequestAsync(user.Id, userRole.Id);
                            }
                        }
                    }

                    var isPending = await _userManager.IsInRoleAsync(user, "Pending");
                    if (isPending)
                    {
                        TempData["Message"] = "Your account is pending approval. Please wait for admin confirmation.";
                        return RedirectToAction(nameof(Login));
                    }

                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (isAdmin)
                    {
                        TempData["Message"] = "Welcome, Admin!";
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            TempData["Message"] = "Google authentication failed.";
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetPasswordLink(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.Email))
                {
                    ModelState.AddModelError(nameof(viewModel.Email), "Email is required.");
                    return View(nameof(ForgetPassword), viewModel);
                }

                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordLink = Url.Action("ResetPassword", "Account", new { email = viewModel.Email, token = token }, Request.Scheme);

                    var emailMessage = new EmailMessageFormat()
                    {
                        To = viewModel.Email,
                        Subject = "Reset Password",
                        Body = resetPasswordLink ?? "Password reset link"
                    };

                    try
                    {
                        await _mailService.SendEmailAsync(emailMessage);
                        return RedirectToAction("CheckYourInbox", "Account");
                    }
                    catch (ArgumentException ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Email sending failed: {ex.Message}");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.Email), "No user found with this email address.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Operation");
            }
            return View(nameof(ForgetPassword), viewModel);
        }

        [HttpGet]
        public IActionResult CheckYourInbox() => View();

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            string email = TempData["email"] as string ?? string.Empty;
            string token = TempData["token"] as string ?? string.Empty;

            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, viewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(nameof(ResetPassword), viewModel);
        }

        #endregion
    }
}
