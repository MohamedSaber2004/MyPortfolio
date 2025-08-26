using BusinessLogicLayer.DTos.SocialLinksDTos;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface ISocialLinkService
    {
        Task<int> AddSocialLinkAsync(CreateOrUpdateSocialLinksDto socialLinksDto);
        Task<int> UpdateSocialLinkAsync(CreateOrUpdateSocialLinksDto socialLinksDto);
        Task<bool> DeleteSocialLinkAsync(int socialLinkId);
        Task<bool> RestoreSocialLinkAsync(int socialLinkId);
        Task<SocialLinksDetailsDto?> GetSocialLinkByIdAsync(int socialLinkId, bool includeDeleted = false);
        Task<IEnumerable<SocialLinksDto>> GetAllSocialLinksAsync(string? socialLinkSearchValue = null, bool includeDeleted = false);
    }
}
