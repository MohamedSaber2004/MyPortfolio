
using BusinessLogicLayer.DTos.EducationDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IEducationService
    {
        Task<int> AddEducationAsync(CreateOrUpdateEducationDto educationDto);

        Task<int> UpdateEducationAsync(CreateOrUpdateEducationDto educationDto);

        Task<bool> DeleteEducationAsync(int educationId);

        Task<bool> RestoreEducationAsync(int educationId);

        Task<EducationDetailsDto?> GetEducationByIdAsync(int educationId);

        Task<IEnumerable<EducationDto>> GetAllEducationsAsync(string? educationSearchValue,bool includeDeleted = false);
    }
}
