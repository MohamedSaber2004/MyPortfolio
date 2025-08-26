using AutoMapper;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.SocialLinksDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.ExperienceModels;
using DataAccessLayer.Models.SocialLinksModels;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BusinessLogicLayer.Services.Classes
{
    public class SocialLinkService(IUnitOfWork _unitOfWork,
                                   IMapper _mapper,
                                   IHttpContextAccessor _httpContextAccessor) : ISocialLinkService
    {
        private readonly IGenericRepository<SocialLink,int> _socialLinksRepo = _unitOfWork.GetRepository<SocialLink,int>();
        public async Task<int> AddSocialLinkAsync(CreateOrUpdateSocialLinksDto socialLinksDto)
        {
            var (userId, userName) = GetCurrentUser();
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Cannot create an social Link without an authenticated user.");

            var socialLink = _mapper.Map<SocialLink>(socialLinksDto);
            socialLink.UserId = userId;
            socialLink.CreatedOn = DateTime.Now;
            socialLink.CreatedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            await _socialLinksRepo.AddAsync(socialLink);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteSocialLinkAsync(int socialLinkId)
        {
            var socialLink = await _socialLinksRepo.GetByIDAsync(socialLinkId);
            if (socialLink == null || socialLink.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            socialLink.IsDeleted = true;
            socialLink.LastModifiedOn = DateTime.Now;
            socialLink.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _socialLinksRepo.Update(socialLink);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SocialLinksDto>> GetAllSocialLinksAsync(string? socialLinkSearchValue = null, bool includeDeleted = false)
        {
             socialLinkSearchValue = socialLinkSearchValue?.Trim();

            Expression<Func<SocialLink, bool>> filter = socialLinkSearchValue switch
            {
                { Length: > 0 } when includeDeleted =>
                    e => e.PlatformName.ToLower().Contains(socialLinkSearchValue.ToLower()),
                { Length: > 0 } =>
                    e => !e.IsDeleted &&
                         (e.PlatformName.ToLower().Contains(socialLinkSearchValue.ToLower())),
                _ when includeDeleted => e => true,
                _ => e => !e.IsDeleted
            };

            var socialLinks = await _socialLinksRepo.GetAllAsync(filter);
            return _mapper.Map<IEnumerable<SocialLinksDto>>(socialLinks);
        }

        public async Task<SocialLinksDetailsDto?> GetSocialLinkByIdAsync(int socialLinkId, bool includeDeleted = false)
        {
            var socialLink = await _socialLinksRepo.GetByIDAsync(socialLinkId);
            if (socialLink is null) return null;
            if (!includeDeleted && socialLink.IsDeleted) return null;
            return _mapper.Map<SocialLinksDetailsDto>(socialLink);
        }

        public async Task<bool> RestoreSocialLinkAsync(int socialLinkId)
        {
            var socialLink = await _socialLinksRepo.GetByIDAsync(socialLinkId);
            if (socialLink == null || !socialLink.IsDeleted) return false;

            var (userId, userName) = GetCurrentUser();
            socialLink.IsDeleted = false;
            socialLink.LastModifiedOn = DateTime.Now;
            socialLink.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _socialLinksRepo.Update(socialLink);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> UpdateSocialLinkAsync(CreateOrUpdateSocialLinksDto socialLinksDto)
        {
            if (socialLinksDto is null) throw new ArgumentNullException(nameof(socialLinksDto));
            if (socialLinksDto.Id <= 0) throw new ArgumentException("A valid social Link Id is required for update.", nameof(socialLinksDto.Id));

            var existing = await _socialLinksRepo.GetByIDAsync(socialLinksDto.Id);
            if (existing is null) return 0;
            if (existing.IsDeleted) return 0;

            _mapper.Map(socialLinksDto, existing);
            var (userId, userName) = GetCurrentUser();
            existing.LastModifiedOn = DateTime.Now;
            existing.LastModifiedBy = !string.IsNullOrWhiteSpace(userName) ? userName : userId;

            _socialLinksRepo.Update(existing);
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
