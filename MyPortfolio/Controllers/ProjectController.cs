using BusinessLogicLayer.DTos.ProjectDTos;
using BusinessLogicLayer.Services.Interfaces;
using MyPortfolio.Models.ProjectModels;
using System.Threading.Tasks;

namespace MyPortfolio.Controllers
{
    public class ProjectController(IProjectService _projectService,
                                   IWebHostEnvironment _environment,
                                   ILogger<ProjectController> _logger) : Controller
    {
        public async Task<IActionResult> Index(string? projectSearchName)
        {
            // Pass includeDeleted: true to show all projects (deleted and not deleted)
            var projects = await _projectService.GetAllProjects(projectSearchName, includeDeleted: true);
            var projectsViewModel = projects.Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ExistingProjectImage = p.ExistingProjectImage,
                ProjectURL = p.ProjectURL,
                IsDeleted = p.IsDeleted
            }).ToList();
            return View(projectsViewModel);
        }

        #region Create New Project
        [HttpGet]
        public IActionResult Create() => View(new ProjectViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectViewModel projectViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var projectDto = new CreatedOrUpdatedProjectDto
                    {
                        Id = projectViewModel.Id,
                        Title = projectViewModel.Title,
                        Description = projectViewModel.Description,
                        StartDate = projectViewModel.StartDate,
                        EndDate = projectViewModel.EndDate,
                        ProjectURL = projectViewModel.ProjectURL,
                        ProjectImage = projectViewModel.ProjectImage,
                        IsDeleted = projectViewModel.IsDeleted
                    };
                    int result = await _projectService.AddProject(projectDto);
                    string message = result > 0
                        ? $"Project With Title ({projectViewModel.Title}) Is Created Successfully"
                        : $"Project With Title ({projectViewModel.Title}) Can't Be Created";

                    TempData["Message"] = message;
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
            return View(projectViewModel);
        }
        #endregion

        #region Details Of Project
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var project = await _projectService.GetProjectById(id.Value);
            return project is null? NotFound() : View(project);
        }
        #endregion

        #region Update Project
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var project = await _projectService.GetProjectById(id.Value);
            if (project is null) return NotFound();

            var projectViewModel = new ProjectViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectURL = project.ProjectURL,
                ExistingProjectImage = project.ProjectImage,
                IsDeleted = project.IsDeleted
            };
            return View(projectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, ProjectViewModel projectViewModel)
        {
            if (!id.HasValue) return BadRequest();
            if (id.Value != projectViewModel.Id)
            {
                ModelState.AddModelError(string.Empty, "Mismatched project id.");
                return View(projectViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var projectDto = new CreatedOrUpdatedProjectDto
                    {
                        Id = projectViewModel.Id,
                        Title = projectViewModel.Title,
                        Description = projectViewModel.Description,
                        StartDate = projectViewModel.StartDate,
                        EndDate = projectViewModel.EndDate,
                        ProjectURL = projectViewModel.ProjectURL,
                        ProjectImage = projectViewModel.ProjectImage, // optional upload
                        IsDeleted = projectViewModel.IsDeleted
                    };

                    var result = await _projectService.UpdateProject(projectDto);
                    if (result > 0)
                    {
                        TempData["Message"] = $"Project With Title ({projectViewModel.Title}) Is Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Project could not be updated.");
                }
                catch (Exception ex)
                {
                    if (_environment.IsDevelopment())
                        ModelState.AddModelError(string.Empty, ex.Message);
                    else
                        _logger.LogError(ex, "Failed to update project with id {ProjectId}", id);
                }
            }

            return View(projectViewModel);
        }
        #endregion

        #region Delete Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await _projectService.DeleteProject(id);

                TempData["Message"] = success
                    ? "Project deleted successfully."
                    : "Project could not be deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete project with id {ProjectId}", id);

                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while deleting the project.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var success = await _projectService.RestoreProject(id);
                TempData["Message"] = success
                    ? "Project restored successfully."
                    : "Project could not be restored.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore project with id {ProjectId}", id);
                TempData["Message"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while restoring the project.";
                return RedirectToAction(nameof(Index));
            }
        } 
        #endregion
    }
}
