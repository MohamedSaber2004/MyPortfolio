using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Helpers;
using MyPortfolio.Helpers.CustomerServiceModels;
using System.Security.Claims;

namespace MyPortfolio.Controllers
{
    public class AccountController(UserManager<User> _userManager,
                                   RoleManager<Role> _roleManager,
                                   SignInManager<User> _signInManager,
                                   IMailService _mailService) : Controller
    {
        #region Register
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
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

                await _userManager.AddToRoleAsync(user, E_Role.User.ToString());
                TempData["Message"] = "Your account has been created successfully. You can now sign in.";

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

            var normalizedEmail = _userManager.NormalizeEmail(viewModel.Email);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user is not null)
            {
                var Result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);

                if (Result.Succeeded)
                {
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

            if (!result.Succeeded)
            {
                TempData["Message"] = "Google authentication failed.";
                return RedirectToAction(nameof(Login));
            }

            var claims = result.Principal?.Identities?.FirstOrDefault()?.Claims;
            if (claims == null)
            {
                TempData["Message"] = "Google authentication failed.";
                return RedirectToAction(nameof(Login));
            }

            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                TempData["Message"] = "Unable to retrieve email from Google account.";
                return RedirectToAction(nameof(Login));
            }

            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var normalizedEmail = _userManager.NormalizeEmail(emailClaim.Value);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
            {
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

                await _userManager.AddToRoleAsync(user, E_Role.User.ToString());
                TempData["Message"] = "Your account has been created successfully. You can now sign in.";
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
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

                var normalizedEmail = _userManager.NormalizeEmail(viewModel.Email);
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
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
