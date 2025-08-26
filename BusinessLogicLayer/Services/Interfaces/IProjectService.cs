using BusinessLogicLayer.DTos.ProjectDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IProjectService
    {
        Task<int> AddProject(CreatedOrUpdatedProjectDto projectDto);
        Task<int> UpdateProject(CreatedOrUpdatedProjectDto projectDto);
        Task<bool> DeleteProject(int projectId);
        Task<bool> RestoreProject(int projectId);
        Task<ProjectDetailsDto?> GetProjectById(int projectId);
        Task<IEnumerable<ProjectDto>> GetAllProjects(string? projectSearchName, bool includeDeleted = false);
    }
}
