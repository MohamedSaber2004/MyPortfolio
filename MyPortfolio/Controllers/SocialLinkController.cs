using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.SocialLinksDTos;
using MyPortfolio.Models.ExperienceModels;
using MyPortfolio.Models.SocialLinkModels;

namespace MyPortfolio.Controllers
{
    public class SocialLinkController(ISocialLinkService _socialLinkService,
                                      IWebHostEnvironment _environment,
                                      ILogger<SocialLinkController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? socialLinkSearchName)
        {
            var socialLinks = await _socialLinkService.GetAllSocialLinksAsync(socialLinkSearchName, includeDeleted: true);
            var socialLinkViewModel = socialLinks.Select(e => new SocialLinkViewModel
            {
                Id = e.Id,
                PlatformName = e.PlatformName,
                URL = e.URL,
                IsDeleted = e.IsDeleted
            }).ToList();

            return View(socialLinkViewModel);
        }

        #region Create New SocialLink
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SocialLinkViewModel socialLinkViewModel)
        {
            if(!ModelState.IsValid) return View(socialLinkViewModel);
            try
            {
                var socialLinkeDto = new CreateOrUpdateSocialLinksDto()
                {
                    Id = socialLinkViewModel.Id,
                    PlatformName = socialLinkViewModel.PlatformName,
                    URL = socialLinkViewModel.URL,
                    IsDeleted = socialLinkViewModel.IsDeleted,
                };

                int result = await _socialLinkService.AddSocialLinkAsync(socialLinkeDto);
                string message = result > 0 ?
                        $"Experience With Platform Name {socialLinkViewModel.PlatformName} Is Created Successfully" :
                        $"Experience With Platform Name {socialLinkViewModel.PlatformName} Can't Be Created";

                TempData["message"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    _logger.LogError(ex.Message);
                return View(socialLinkViewModel);
            }
        }
        #endregion

        #region Details Of SocialLink
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? Id)
        {
            if (!Id.HasValue) return BadRequest();

            var socialLink = await _socialLinkService.GetSocialLinkByIdAsync(Id.Value);
            return socialLink is null ? NotFound() : View(socialLink);
        }
        #endregion

        #region Edit SocialLink
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();
            var socialLink = await _socialLinkService.GetSocialLinkByIdAsync(Id.Value);
            if (socialLink == null)
                return NotFound();
            var socialLinkViewModel = new SocialLinkViewModel()
            {
                Id = socialLink.Id,
                PlatformName = socialLink.PlatformName,
                URL = socialLink.URL,
                IsDeleted = socialLink.IsDeleted
            };
            return View(socialLinkViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? Id, SocialLinkViewModel socialLinkViewModel)
        {
            if (!Id.HasValue) return BadRequest();
            if (Id.Value != socialLinkViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched social link id.");
                return View(socialLinkViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var socialLinkDto = new CreateOrUpdateSocialLinksDto()
                    {
                        Id = socialLinkViewModel.Id,
                        PlatformName = socialLinkViewModel.PlatformName,
                        URL = socialLinkViewModel.URL,
                        IsDeleted = socialLinkViewModel.IsDeleted
                    };

                    var result = await _socialLinkService.UpdateSocialLinkAsync(socialLinkDto);

                    if (result > 0)
                    {
                        TempData["Message"] = $"Social Lik With Platform Name ({socialLinkViewModel.PlatformName}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Social link could not be updated.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update social link with id {socialLinkId}", Id);
                }
            }

            return View(socialLinkViewModel);
        }
        #endregion

        #region Delete SocialLink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            try
            {
                var success = await _socialLinkService.DeleteSocialLinkAsync(Id);

                TempData["Message"] = success
                    ? "Social link deleted successfully."
                    : "Social link could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete social link with id {socialLinkId}", Id);

                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while deleting the social link.";
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
                var success = await _socialLinkService.RestoreSocialLinkAsync(Id);
                TempData["Message"] = success
                    ? "Social link restored successfully."
                    : "Social link could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore social link with id {socialLinkId}", Id);
                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while restoring the social link.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
