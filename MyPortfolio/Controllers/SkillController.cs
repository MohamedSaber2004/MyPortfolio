using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.SkillDTos;
using MyPortfolio.Models.ExperienceModels;

namespace MyPortfolio.Controllers
{
    public class SkillController(ISkillService _skillService,
                                 IWebHostEnvironment _environment,
                                 ILogger<SkillController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? skillSearchName)
        {
            var skills = await _skillService.GetAllSkillsAsync(skillSearchName, includeDeleted: true);
            var skillViewModel = skills.Select(s => new SkillDto
            {
                Id = s.Id,
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel,
                IsDeleted = s.IsDeleted
            }).ToList();

            return View(skillViewModel);
        }

        #region Create New Skill
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SkillDto skillViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var skillDto = new SkillDto()
                    {
                        Id = skillViewModel.Id,
                        SkillName = skillViewModel.SkillName,
                        ProficiencyLevel = skillViewModel.ProficiencyLevel,
                        IsDeleted = skillViewModel.IsDeleted,
                    };
                    int result = await _skillService.AddSkillAsync(skillDto);
                    string message = result > 0 ?
                           $"Skill With Name {skillViewModel.SkillName} Is Created Successfully" :
                           $"Skill With Name {skillViewModel.SkillName} Can't Be Created";
                    TempData["message"] = message;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(skillViewModel);
        }
        #endregion

        #region Details Of Skill
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var skill = await _skillService.GetSkillByIdAsync(Id.Value);
            return skill is null ? NotFound() : View(skill);
        }
        #endregion

        #region Edit Skill
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();
            var skill = await _skillService.GetSkillByIdAsync(Id.Value);
            if (skill == null)
                return NotFound();
            var skillViewModel = new SkillDto()
            {
                Id = skill.Id,
                SkillName = skill.SkillName,
                ProficiencyLevel = skill.ProficiencyLevel,
                IsDeleted = skill.IsDeleted
            };
            return View(skillViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? Id,SkillDto skillViewModel)
        {
            if (!Id.HasValue) return BadRequest();
            if (Id.Value != skillViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched skill id.");
                return View(skillViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var skillDto = new SkillDto()
                    {
                        Id = skillViewModel.Id,
                        SkillName = skillViewModel.SkillName,
                        ProficiencyLevel = skillViewModel.ProficiencyLevel,
                        IsDeleted = skillViewModel.IsDeleted
                    };

                    var result = await _skillService.UpdateSkillAsync(skillDto);

                    if (result > 0)
                    {
                        TempData["Message"] = $"Skill With Name ({skillViewModel.SkillName}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Skill could not be updated.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update skill with id {skillId}", Id);
                }
            }

            return View(skillViewModel);
        }
        #endregion

        #region Delete Skill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute]int Id)
        {
            try
            {
                var success = await _skillService.DeleteSkillAsync(Id);

                TempData["Message"] = success
                    ? "Skill deleted successfully."
                    : "Skill could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete skill with id {skillId}", Id);

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
                var success = await _skillService.RestoreSkillAsync(Id);
                TempData["Message"] = success
                    ? "Skill restored successfully."
                    : "Skill could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore skill with id {skillId}", Id);
                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while restoring the skill.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
