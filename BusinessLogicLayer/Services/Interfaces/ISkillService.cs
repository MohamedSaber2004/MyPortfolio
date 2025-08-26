using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.SkillDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface ISkillService
    {
        Task<int> AddSkillAsync(SkillDto skillDto);
        Task<int> UpdateSkillAsync(SkillDto skillDto);
        Task<bool> DeleteSkillAsync(int skillId);
        Task<bool> RestoreSkillAsync(int skillId);
        Task<SkillDetailsDto?> GetSkillByIdAsync(int skillId, bool includeDeleted = false);
        Task<IEnumerable<SkillDto>> GetAllSkillsAsync(string? skillSearchValue = null, bool includeDeleted = false);
    }
}
