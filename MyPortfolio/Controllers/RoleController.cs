using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyPortfolio.Models.ManagerModels.RoleModels;
using System;
using System.Threading.Tasks;

namespace MyPortfolio.Controllers
{
    public class RoleController(RoleManager<Role> _roleManager,
                                IWebHostEnvironment _environment,
                                ILogger<RoleController> _logger) : Controller
    {
        public IActionResult Index(string RoleSearchName)
        {
            var roles = string.IsNullOrEmpty(RoleSearchName)
                            ? _roleManager.Roles
                            : _roleManager.Roles.Where(R => R.Name!.Contains(RoleSearchName));

            var rolesViewModel = roles.Select(role => new RoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name ?? string.Empty,
                IsDeleted = role.IsDeleted
            }).ToList();
            return View(rolesViewModel);
        }

        #region Create New Role
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditRoleViewModel roleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(roleViewModel);
            }

            var name = roleViewModel.RoleName?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(roleViewModel.RoleName), "Role name is required.");
                return View(roleViewModel);
            }

            try
            {
                if (await _roleManager.RoleExistsAsync(name))
                {
                    ModelState.AddModelError(nameof(roleViewModel.RoleName), "A role with this name already exists.");
                    return View(roleViewModel);
                }

                var identityRole = new Role()
                {
                    Name = roleViewModel.RoleName?.Trim(),
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = User?.Identity?.Name ?? "System",
                    LastModifiedOn = null,
                    LastModifiedBy = null
                };

                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["Message"] = $"{identityRole.Name} Role Is Created Successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                else
                {
                    _logger.LogError(ex, "Error creating role {RoleName}", name);
                    TempData["Message"] = "An unexpected error occurred while creating the role.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(roleViewModel);
        }
        #endregion

        #region Details Of Role
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id is null) return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            var roleViewModel = new RoleViewModel()
            {
                Id = id,
                RoleName = role.Name ?? string.Empty,
                IsDeleted = role.IsDeleted
            };

            return View(roleViewModel);
        }
        #endregion

        #region Edit Role
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            var model = new CreateEditRoleViewModel
            {
                RoleName = role.Name ?? string.Empty,
                AvailableRoles = await _roleManager.Roles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name!,
                        Text = r.Name!,
                        Selected = r.Id == id
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, CreateEditRoleViewModel model)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _roleManager.Roles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new SelectListItem { Value = r.Name!, Text = r.Name! })
                    .ToListAsync();
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();

            var newName = model.RoleName?.Trim() ?? string.Empty;

            if (!string.Equals(role.Name, newName, StringComparison.Ordinal))
            {
                if (await _roleManager.RoleExistsAsync(newName))
                {
                    ModelState.AddModelError(nameof(model.RoleName), "A role with this name already exists.");
                    model.AvailableRoles = await _roleManager.Roles
                        .Where(r => !r.IsDeleted)
                        .Select(r => new SelectListItem { Value = r.Name!, Text = r.Name! })
                        .ToListAsync();
                    return View(model);
                }

                role.Name = newName;
                role.LastModifiedOn = DateTime.UtcNow;
                role.LastModifiedBy = User?.Identity?.Name ?? "System";

                var updateResult = await _roleManager.UpdateAsync(role);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    model.AvailableRoles = await _roleManager.Roles
                        .Where(r => !r.IsDeleted)
                        .Select(r => new SelectListItem { Value = r.Name!, Text = r.Name! })
                        .ToListAsync();
                    return View(model);
                }
            }

            TempData["Message"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role is null) return NotFound();


                role.IsDeleted = true;
                role.LastModifiedOn = DateTime.UtcNow;
                role.LastModifiedBy = User?.Identity?.Name ?? "System";

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Role deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }

                role.IsDeleted = false;

                TempData["Message"] = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete role with id {RoleId}", id);

                if (_environment.IsDevelopment())
                {
                    TempData["Message"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }

                return View("ErrorView", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound();

            role.IsDeleted = false;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                TempData["Message"] = "Failed to restore the role.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Message"] = "Role restored successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
