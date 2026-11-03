using Microsoft.AspNetCore.Authorization;
using MyPortfolio.Helpers;
using MyPortfolio.Models.ProfileMediaModels;

namespace MyPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileMediaController(IWebHostEnvironment _environment) : Controller
    {
        public IActionResult Index() => View(Build());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile? PortraitFile)
        {
            if (PortraitFile is null || PortraitFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Choose an image file first.");
                return View("Index", Build());
            }

            var stored = HeroPortrait.Store(_environment, PortraitFile, out var error);
            if (stored is null)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View("Index", Build());
            }

            TempData["Message"] = "Portrait saved — the public page serves it on the next load.";
            return RedirectToAction(nameof(Index));
        }

        private ProfileMediaVm Build()
        {
            var file = HeroPortrait.Current(_environment);

            return new ProfileMediaVm
            {
                FileName = file?.Name,
                Url = HeroPortrait.PublicUrl(_environment),
                SizeLabel = file is null ? null : $"{Math.Round(file.Length / 1024d)} KB",
                UpdatedOn = file?.LastWriteTimeUtc
            };
        }
    }
}
