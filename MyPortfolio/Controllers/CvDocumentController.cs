using BusinessLogicLayer.DTos.CvDocumentDTos;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MyPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CvDocumentController(ICvDocumentService _cvDocumentService,
                                      IWebHostEnvironment _environment,
                                      ILogger<CvDocumentController> _logger) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var current = await _cvDocumentService.GetLatestCvAsync();
            return View(current);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadCvDocumentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var current = await _cvDocumentService.GetLatestCvAsync();
                return View("Index", current);
            }

            try
            {
                var userName = User?.Identity?.Name ?? "Unknown";
                var result = await _cvDocumentService.UploadCvAsync(dto, userName);

                TempData["Message"] = result > 0
                    ? "CV uploaded successfully."
                    : "CV upload failed. Please ensure the file is a valid PDF under 2 MB.";
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                    ModelState.AddModelError(string.Empty, ex.Message);
                else
                    _logger.LogError(ex, "CV upload failed");

                var current = await _cvDocumentService.GetLatestCvAsync();
                return View("Index", current);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
