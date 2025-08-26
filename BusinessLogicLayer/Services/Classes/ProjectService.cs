using AutoMapper;
using BusinessLogicLayer.DTos.ProjectDTos;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.Services.Special;
using DataAccessLayer.Models.ProjectModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services.Classes
{
    public class ProjectService(IUnitOfWork _unitOfWork,
                                IMapper _mapper,
                                IAttachmentService _attachmentService,
                                IHttpContextAccessor _httpContextAccessor) : IProjectService
    {
        public async Task<int> AddProject(CreatedOrUpdatedProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create a project without an authenticated user.");

            project.UserId = userId;
            project.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

            if (projectDto.ProjectImage is not null)
                project.ProjectImage = _attachmentService.Upload(projectDto.ProjectImage, "Images/Projects");

            await _unitOfWork.GetRepository<Project, int>().AddAsync(project);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteProject(int projectId)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIDAsync(projectId);
            if (project == null || project.IsDeleted)
                return false;

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            project.IsDeleted = true;
            project.LastModifiedOn = DateTime.Now;
            project.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

            _unitOfWork.GetRepository<Project, int>().Update(project);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreProject(int projectId)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIDAsync(projectId);
            if (project == null || !project.IsDeleted)
                return false;

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            project.IsDeleted = false;
            project.LastModifiedOn = DateTime.Now;
            project.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

            _unitOfWork.GetRepository<Project, int>().Update(project);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjects(string? projectSearchName, bool includeDeleted = false)
        {
            Expression<Func<Project, bool>> filter;

            if (!string.IsNullOrWhiteSpace(projectSearchName))
            {
                var lowered = projectSearchName.ToLower();
                if (includeDeleted)
                    filter = p => p.Title.ToLower().Contains(lowered) || p.Description.ToLower().Contains(lowered);
                else
                    filter = p => (p.Title.ToLower().Contains(lowered) || p.Description.ToLower().Contains(lowered)) && !p.IsDeleted;
            }
            else
            {
                filter = includeDeleted ? p => true : p => !p.IsDeleted;
            }

            var projects = await _unitOfWork.GetRepository<Project, int>().GetAllAsync(filter);
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDetailsDto?> GetProjectById(int projectId)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIDAsync(projectId);
            return project is null ? null : _mapper.Map<ProjectDetailsDto>(project);
        }

        public async Task<int> UpdateProject(CreatedOrUpdatedProjectDto projectDto)
        {
            if (projectDto is null) throw new ArgumentNullException(nameof(projectDto));
            if (projectDto.Id <= 0) throw new ArgumentException("A valid project Id is required for update.", nameof(projectDto.Id));

            var repo = _unitOfWork.GetRepository<Project, int>();
            var project = await repo.GetByIDAsync(projectDto.Id);
            if (project is null) return 0;

            if (project.IsDeleted)
            {
                if (!projectDto.IsDeleted)
                {
                    project.IsDeleted = false;
                    project.LastModifiedOn = DateTime.Now;

                    var user = _httpContextAccessor.HttpContext?.User;
                    var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userName = user?.Identity?.Name;
                    project.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

                    repo.Update(project);
                    return await _unitOfWork.SaveChangesAsync();
                }
                return 0;
            }

            _mapper.Map(projectDto, project);

            if (projectDto.ProjectImage is { Length: > 0 })
            {
                if (!string.IsNullOrWhiteSpace(project.ProjectImage))
                    _attachmentService.Delete(project.ProjectImage);

                var newPath = _attachmentService.Upload(projectDto.ProjectImage, "Images/Projects");
                if (!string.IsNullOrWhiteSpace(newPath))
                    project.ProjectImage = newPath;
            }

            project.LastModifiedOn = DateTime.Now;
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = user?.Identity?.Name;
                project.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;
            }

            repo.Update(project);

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}