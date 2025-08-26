using AutoMapper;
using BusinessLogicLayer.DTos.EducationDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.EducationModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BusinessLogicLayer.Services.Classes
{
    public class EducationService(IUnitOfWork _unitOfWork,
                                  IMapper _mapper,
                                  IHttpContextAccessor _httpContextAccessor) : IEducationService
    {
        private readonly IGenericRepository<Education,int> _educationRepo = _unitOfWork.GetRepository<Education,int>();

        public async Task<int> AddEducationAsync(CreateOrUpdateEducationDto educationDto)
        {
            var education = _mapper.Map<Education>(educationDto);

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            if(string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create a education without an authenticated user.");

            education.UserId = userId;
            education.CreatedOn = DateTime.Now;
            education.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            await _educationRepo.AddAsync(education);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteEducationAsync(int educationId)
        {
            var education = await _educationRepo.GetByIDAsync(educationId);
            if(education == null || education.IsDeleted) return false;

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            education.IsDeleted = true;
            education.LastModifiedOn = DateTime.Now;
            education.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _educationRepo.Update(education);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RestoreEducationAsync(int educationId)
        {
            var education = await _educationRepo.GetByIDAsync(educationId);
            if (education == null || !education.IsDeleted) return false;

            var user = _httpContextAccessor?.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;

            education.IsDeleted = false;
            education.LastModifiedOn = DateTime.Now;
            education.LastModifiedBy = !string.IsNullOrWhiteSpace (userName) ? userName : userId;

            _educationRepo.Update(education);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EducationDto>> GetAllEducationsAsync(string? educationSearchValue, bool includeDeleted = false)
        {
            Expression<Func<Education, bool>> filter;
            if (!string.IsNullOrWhiteSpace(educationSearchValue))
            {
                var lowered = educationSearchValue.ToLower();
                if (includeDeleted)
                    filter = e => e.InstitutionName.ToLower().Contains(lowered) ||
                                  e.Degree.ToLower().Contains(lowered) ||
                                  e.FieldOfStudy.ToLower().Contains(lowered);
                else
                    filter = e => (e.InstitutionName.ToLower().Contains(lowered) ||
                                  e.Degree.ToLower().Contains(lowered) ||
                                  e.FieldOfStudy.ToLower().Contains(lowered)) && !e.IsDeleted;
            }
            else
                filter = includeDeleted ? e => true : e => !e.IsDeleted;

            var educations = await _educationRepo.GetAllAsync(filter);
            return _mapper.Map<IEnumerable<EducationDto>>(educations);
        }

        public async Task<EducationDetailsDto?> GetEducationByIdAsync(int educationId)
        {
            var education = await _educationRepo.GetByIDAsync(educationId);
            return education is null? null: _mapper.Map<EducationDetailsDto>(education);
        }


        public async Task<int> UpdateEducationAsync(CreateOrUpdateEducationDto educationDto)
        {
            if (educationDto is null) throw new ArgumentNullException(nameof(educationDto));
            if (educationDto.Id <= 0) throw new ArgumentException("A valid education Id is required for update.", nameof(educationDto.Id));

            var education = await _educationRepo.GetByIDAsync(educationDto.Id);
            if (education is null) return 0;
            if (education.IsDeleted) return 0;

            _mapper.Map(educationDto, education);

            education.LastModifiedOn = DateTime.Now;
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.Identity?.Name;
            education.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName! : userId;

            _educationRepo.Update(education);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
