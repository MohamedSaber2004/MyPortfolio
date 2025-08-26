using BusinessLogicLayer.DTos.EducationDTos;
using MyPortfolio.Models.EducationModels;

namespace MyPortfolio.Controllers
{
    public class EducationController(IEducationService _educationService,
                                     IWebHostEnvironment _environment,
                                     ILogger<EducationController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? educationSearchName)
        {
            var education = await _educationService.GetAllEducationsAsync(educationSearchName, includeDeleted: true);
            var educationViewModel = education.Select(e => new EducationViewModel
            { 
                Id = e.Id,
                Degree = e.Degree,
                InstitutionName = e.InstitutionName, 
                FieldOfStudy = e.FieldOfStudy,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description,
                IsDeleted = e.IsDeleted
            }).ToList();

            return View(educationViewModel);
        }

        #region Create New Education
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EducationViewModel educationViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var educationDto = new CreateOrUpdateEducationDto()
                    {
                        Id = educationViewModel.Id,
                        Degree = educationViewModel.Degree,
                        FieldOfStudy = educationViewModel.FieldOfStudy,
                        InstitutionName = educationViewModel.InstitutionName,
                        StartDate = educationViewModel.StartDate,
                        EndDate = educationViewModel.EndDate,
                        Description = educationViewModel.Description,
                        IsDeleted = educationViewModel.IsDeleted
                    };

                    int result = await _educationService.AddEducationAsync(educationDto);
                    string message = result > 0 ?
                           $"Education With Institution Name {educationViewModel.InstitutionName} Is Created Successfully" :
                           $"Education With Institution Name {educationViewModel.InstitutionName} Can't Be Created";

                    TempData["message"] = message;
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    if(_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex.Message);
                }
            }

            return View(educationViewModel);
        }
        #endregion

        #region Details Of Education
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute]int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var education = await _educationService.GetEducationByIdAsync(Id.Value);
            return education is null? NotFound(): View(education);
        }
        #endregion

        #region Edit Education
        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var education = await _educationService.GetEducationByIdAsync(Id.Value);
            if (education is null) return NotFound();

            var contactViewModel = new EducationViewModel()
            {
                Id = education.Id,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                Description = education.Description,
                EndDate = education.EndDate,
                InstitutionName = education.InstitutionName,
                StartDate = education.StartDate,
                IsDeleted = education.IsDeleted
            };

            return View(contactViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int? Id,EducationViewModel educationViewModel)
        {
            if (!Id.HasValue) return BadRequest();
            if (Id.Value != educationViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched education id.");
                return View(educationViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var educationDto = new CreateOrUpdateEducationDto()
                    {
                        Id = educationViewModel.Id,
                        Degree = educationViewModel.Degree,
                        FieldOfStudy = educationViewModel.FieldOfStudy,
                        InstitutionName = educationViewModel.InstitutionName,
                        StartDate = educationViewModel.StartDate,
                        EndDate = educationViewModel.EndDate,
                        Description = educationViewModel.Description,
                        IsDeleted = educationViewModel.IsDeleted
                    };

                    var result = await _educationService.UpdateEducationAsync(educationDto);

                    if (result > 0)
                    {
                        TempData["Message"] = $"Education With Institution Name ({educationViewModel.InstitutionName}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Education could not be updated.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update education with id {educationId}", Id);
                }
            }

            return View(educationViewModel);
        }
        #endregion

        #region Delete Education
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
            try
            {
                var success = await _educationService.DeleteEducationAsync(Id);

                TempData["Message"] = success
                    ? "Education deleted successfully."
                    : "Education could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete education with id {educationId}", Id);

                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while deleting the education.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromRoute]int Id)
        {
            if (Id <= 0)
                return BadRequest();

            try
            {
                var success = await _educationService.RestoreEducationAsync(Id);
                TempData["Message"] = success
                    ? "Education restored successfully."
                    : "Education could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore education with id {educationId}", Id);
                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while restoring the education.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
