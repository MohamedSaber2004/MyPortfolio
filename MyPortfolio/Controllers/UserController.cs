using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using DataAccessLayer.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Models.ManagerModels.UserModels;
using System;

using Microsoft.AspNetCore.Authorization;

namespace MyPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(UserManager<User> _userManager,
                                RoleManager<Role> _roleManager,
                                IWebHostEnvironment _environment,
                                ILogger<UserController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? SearchUserName)
        {
            IQueryable<User> querableUsers;
            if(string.IsNullOrEmpty(SearchUserName))
            {
                querableUsers = _userManager.Users;
            }
            else
            {
                querableUsers = _userManager.Users
                                .Where(u => u.FullName.ToLower().Contains(SearchUserName.ToLower()));
            }

            var users = await querableUsers.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new UserViewModel()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList(),
                    IsDeleted = user.IsDeleted
                });
            }

            return View(userViewModels);
        }

        #region Details Of Users
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            if (id is null)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserDetailsViewModel()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                CreatedBy = user.CreatedBy,
                CreatedOn = user.CreatedOn,
                LastModifiedBy = user.LastModifiedBy,
                LastModifiedOn = user.LastModifiedOn,
                IsDeleted = user.IsDeleted,
            };

            return View(userViewModel);
        }
        #endregion

        #region Edit User
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = allRoles.Select(role => new RoleSelectionViewModel
                {
                    RoleName = role!,
                    IsSelected = userRoles.Contains(role!)
                }).ToList()
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is null) return NotFound();

                user.FullName = viewModel.FullName;
                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;

                user.LastModifiedOn = DateTime.Now;
                user.LastModifiedBy = User?.Identity?.Name ?? "System";

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(viewModel);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var selectedRoles = viewModel.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to remove old roles.");
                    return View(viewModel);
                }

                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                {
                    _logger.LogError(ex, "Error while updating user.");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
                }

                return View(viewModel);
            }
        }
        #endregion

        #region Delete User
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserDetailsViewModel()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = userRoles.ToList(),
                CreatedBy = user.CreatedBy,
                CreatedOn = user.CreatedOn,
                LastModifiedBy = user.LastModifiedBy,
                LastModifiedOn = user.LastModifiedOn,
                IsDeleted = user.IsDeleted
            };

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is null)
                    return NotFound();

                user.IsDeleted = true;
                user.LastModifiedOn = DateTime.Now;
                user.LastModifiedBy = User?.Identity?.Name ?? "System";

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    user.IsDeleted = false;

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    TempData["Error"] = "Something went wrong while deleting the user.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                TempData["Message"] = "User marked as deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    _logger.LogError(ex, $"Error occurred while soft-deleting user with ID: {id}");
                    TempData["Error"] = "An unexpected error occurred while deleting the user.";
                }

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            user.IsDeleted = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                TempData["Error"] = "Failed to restore the user.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Message"] = "User restored successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
