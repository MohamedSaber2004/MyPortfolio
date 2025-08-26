using BusinessLogicLayer.DTos.ExperienceDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IExperienceService
    {
        Task<int> AddExperienceAsync(CreateOrUpdateExperienceDto experienceDto);
        Task<int> UpdateExperienceAsync(CreateOrUpdateExperienceDto experienceDto);
        Task<bool> DeleteExperienceAsync(int experienceId);
        Task<bool> RestoreExperienceAsync(int experienceId);
        Task<ExperienceDetailsDto?> GetExperienceByIdAsync(int experienceId, bool includeDeleted = false);
        Task<IEnumerable<ExperienceDto>> GetAllExperiencesAsync(string? experienceSearchValue = null, bool includeDeleted = false);
    }
}
