using AutoMapper;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.ExperienceModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BusinessLogicLayer.Services.Classes
{
    public class ExperienceService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        IHttpContextAccessor _httpContextAccessor) : IExperienceService
    {
        private readonly IGenericRepository<Experience, int> _experienceRepo = _unitOfWork.GetRepository<Experience, int>();

        public async Task<int> AddExperienceAsync(CreateOrUpdateExperienceDto experienceDto)
        {
            var (userId, userName) = GetCurrentUser();
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create an experience without an authenticated user.");

            var experience = _mapper.Map<Experience>(experienceDto);
            experience.UserId = userId;
            experience.CreatedOn = DateTime.Now;
            experience.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            await _experienceRepo.AddAsync(experience);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateExperienceAsync(CreateOrUpdateExperienceDto experienceDto)
        {
            if (experienceDto is null) throw new ArgumentNullException(nameof(experienceDto));
            if (experienceDto.Id <= 0) throw new ArgumentException("A valid experience Id is required for update.", nameof(experienceDto.Id));

            var existing = await _experienceRepo.GetByIDAsync(experienceDto.Id);
            if (existing is null) return 0;
            if (existing.IsDeleted) return 0;

            _mapper.Map(experienceDto, existing);
            var (userId, userName) = GetCurrentUser();
            existing.LastModifiedOn = DateTime.Now;
            existing.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _experienceRepo.Update(existing);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteExperienceAsync(int experienceId)
        {
            var experience = await _experienceRepo.GetByIDAsync(experienceId);
            if (experience == null || experience.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            experience.IsDeleted = true;
            experience.LastModifiedOn = DateTime.Now;
            experience.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _experienceRepo.Update(experience);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreExperienceAsync(int experienceId)
        {
            var experience = await _experienceRepo.GetByIDAsync(experienceId);
            if (experience == null || !experience.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            experience.IsDeleted = false;
            experience.LastModifiedOn = DateTime.Now;
            experience.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _experienceRepo.Update(experience);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExperienceDto>> GetAllExperiencesAsync(string? experienceSearchValue = null, bool includeDeleted = false)
        {
            experienceSearchValue = experienceSearchValue?.Trim();

            Expression<Func<Experience, bool>> filter = experienceSearchValue switch
            {
                { Length: > 0 } when includeDeleted =>
                    e => e.CompanyName.ToLower().Contains(experienceSearchValue.ToLower()) ||
                         e.Role.ToLower().Contains(experienceSearchValue.ToLower()),
                { Length: > 0 } =>
                    e => !e.IsDeleted &&
                         (e.CompanyName.ToLower().Contains(experienceSearchValue.ToLower()) ||
                          e.Role.ToLower().Contains(experienceSearchValue.ToLower())),
                _ when includeDeleted => e => true,
                _ => e => !e.IsDeleted
            };

            var experiences = await _experienceRepo.GetAllAsync(filter);
            return _mapper.Map<IEnumerable<ExperienceDto>>(experiences);
        }

        public async Task<ExperienceDetailsDto?> GetExperienceByIdAsync(int experienceId, bool includeDeleted = false)
        {
            var experience = await _experienceRepo.GetByIDAsync(experienceId);
            if (experience is null) return null;
            if (!includeDeleted && experience.IsDeleted) return null;
            return _mapper.Map<ExperienceDetailsDto>(experience);
        }

        private (string? userId, string? userName) GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null) return (null, null);
            return (user.FindFirstValue(ClaimTypes.NameIdentifier), user.Identity?.Name);
        }
    }
}
