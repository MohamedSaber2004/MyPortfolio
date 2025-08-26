using BusinessLogicLayer.DTos.ExperienceDTos;
using MyPortfolio.Models.ExperienceModels;

namespace MyPortfolio.Controllers
{
    public class ExpereienceController(IExperienceService _experienceService,
                                       IWebHostEnvironment _environment,
                                       ILogger<ExpereienceController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? experienceSearchName)
        {
            var experiences = await _experienceService.GetAllExperiencesAsync(experienceSearchName, includeDeleted: true);
            var experienceViewModel = experiences.Select(e => new ExperienceViewModel
            {
                Id = e.Id,
                CompanyName = e.CompanyName,
                Role = e.Role,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description,
                IsDeleted = e.IsDeleted
            }).ToList();

            return View(experienceViewModel);
        }

        #region Create New Experience
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExperienceViewModel experienceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var educationDto = new CreateOrUpdateExperienceDto()
                    {
                        Id = experienceViewModel.Id,
                        CompanyName = experienceViewModel.CompanyName,
                        Role = experienceViewModel.Role,
                        StartDate = experienceViewModel.StartDate,
                        EndDate = experienceViewModel.EndDate,
                        Description = experienceViewModel.Description,
                        IsDeleted = experienceViewModel.IsDeleted,
                    };

                    int result = await _experienceService.AddExperienceAsync(educationDto);
                    string message = result > 0 ?
                           $"Experience With Company Name {experienceViewModel.CompanyName} Is Created Successfully" :
                           $"Experience With Company Name {experienceViewModel.CompanyName} Can't Be Created";

                    TempData["message"] = message;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }

            return View(experienceViewModel);
        }
        #endregion

        #region Details Of Experience
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var experience = await _experienceService.GetExperienceByIdAsync(Id.Value);
            return experience is null ? NotFound() : View(experience);
        }
        #endregion

        #region Edit Experience
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();
            var experience = await _experienceService.GetExperienceByIdAsync(Id.Value);
            if (experience == null)
                return NotFound();
            var experienceViewModel = new ExperienceViewModel()
            {
                Id = experience.Id,
                CompanyName = experience.CompanyName,
                Role = experience.Role,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description,
                IsDeleted = experience.IsDeleted
            };
            return View(experienceViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int? Id, ExperienceViewModel experienceViewModel)
        {
            if (!Id.HasValue) return BadRequest();
            if (Id.Value != experienceViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched experience id.");
                return View(experienceViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var educationDto = new CreateOrUpdateExperienceDto()
                    {
                        Id = experienceViewModel.Id,
                        CompanyName = experienceViewModel.CompanyName,
                        Role = experienceViewModel.Role,
                        StartDate = experienceViewModel.StartDate,
                        EndDate = experienceViewModel.EndDate,
                        Description = experienceViewModel.Description,
                        IsDeleted = experienceViewModel.IsDeleted
                    };

                    var result = await _experienceService.UpdateExperienceAsync(educationDto);

                    if (result > 0)
                    {
                        TempData["Message"] = $"Experience With Company Name ({experienceViewModel.CompanyName}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Experience could not be updated.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update experience with id {experienceId}", Id);
                }
            }

            return View(experienceViewModel);
        }
        #endregion

        #region Delete Experience
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
            try
            {
                var success = await _experienceService.DeleteExperienceAsync(Id);

                TempData["Message"] = success
                    ? "Experience deleted successfully."
                    : "Experience could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete experience with id {experienceId}", Id);

                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while deleting the experience.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromRoute] int Id)
        {
            if (Id <= 0)
                return BadRequest();

            try
            {
                var success = await _experienceService.RestoreExperienceAsync(Id);
                TempData["Message"] = success
                    ? "Experience restored successfully."
                    : "Experience could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore experience with id {experienceId}", Id);
                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while restoring the experience.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
