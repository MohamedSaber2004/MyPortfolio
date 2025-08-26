using AutoMapper;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.SkillDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.ExperienceModels;
using DataAccessLayer.Models.SkillModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BusinessLogicLayer.Services.Classes
{
    public class SkillService(IUnitOfWork _unitOfWork,
                              IMapper _mapper,
                              IHttpContextAccessor _httpContextAccessor) : ISkillService
    {
        private readonly IGenericRepository<Skill, int> _skillRepo = _unitOfWork.GetRepository<Skill, int>();
        public async Task<int> AddSkillAsync(SkillDto skillDto)
        {
            var (userId, userName) = GetCurrentUser();
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create an skill without an authenticated user.");

            var skill = _mapper.Map<Skill>(skillDto);
            skill.UserId = userId;
            skill.CreatedOn = DateTime.Now;
            skill.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            await _skillRepo.AddAsync(skill);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteSkillAsync(int skillId)
        {
            var skill = await _skillRepo.GetByIDAsync(skillId);
            if (skill == null || skill.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            skill.IsDeleted = true;
            skill.LastModifiedOn = DateTime.Now;
            skill.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _skillRepo.Update(skill);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(string? skillSearchValue = null, bool includeDeleted = false)
        {
            skillSearchValue = skillSearchValue?.Trim();

            Expression<Func<Skill, bool>> filter = skillSearchValue switch
            {
                { Length: > 0 } when includeDeleted =>
                    e => e.SkillName.ToLower().Contains(skillSearchValue.ToLower()),
                { Length: > 0 } =>
                    e => !e.IsDeleted &&
                         e.SkillName.ToLower().Contains(skillSearchValue.ToLower()),
                _ when includeDeleted => e => true,
                _ => e => !e.IsDeleted
            };

            var skills = await _skillRepo.GetAllAsync(filter);
            return _mapper.Map<IEnumerable<SkillDto>>(skills);
        }

        public async Task<SkillDetailsDto?> GetSkillByIdAsync(int skillId, bool includeDeleted = false)
        {
            var skill = await _skillRepo.GetByIDAsync(skillId);
            if (skill is null) return null;
            if (!includeDeleted && skill.IsDeleted) return null;
            return _mapper.Map<SkillDetailsDto>(skill);
        }

        public async Task<bool> RestoreSkillAsync(int skillId)
        {
            var skill = await _skillRepo.GetByIDAsync(skillId);
            if (skill == null || !skill.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            skill.IsDeleted = false;
            skill.LastModifiedOn = DateTime.Now;
            skill.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _skillRepo.Update(skill);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> UpdateSkillAsync(SkillDto skillDto)
        {
            if (skillDto is null) throw new ArgumentNullException(nameof(skillDto));
            if (skillDto.Id <= 0) throw new ArgumentException("A valid skill Id is required for update.", nameof(skillDto.Id));

            var existing = await _skillRepo.GetByIDAsync(skillDto.Id);
            if (existing is null) return 0;
            if (existing.IsDeleted) return 0;

            _mapper.Map(skillDto, existing);
            var (userId, userName) = GetCurrentUser();
            existing.LastModifiedOn = DateTime.Now;
            existing.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _skillRepo.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        private (string? userId, string? userName) GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null) return (null, null);
            return (user.FindFirstValue(ClaimTypes.NameIdentifier), user.Identity?.Name);
        }
    }
}
