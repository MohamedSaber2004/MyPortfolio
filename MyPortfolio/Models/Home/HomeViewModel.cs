using BusinessLogicLayer.DTos.ContactDTos;
using BusinessLogicLayer.DTos.EducationDTos;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.ProjectDTos;
using BusinessLogicLayer.DTos.SkillDTos;
using BusinessLogicLayer.DTos.SocialLinksDTos;

namespace MyPortfolio.Models.Home
{
    public class HomeViewModel
    {
        public IReadOnlyList<SkillDto> Skills { get; init; } = [];
        public IReadOnlyList<ExperienceDto>? Experiences { get; init; }
        public IReadOnlyList<ProjectDto>? Projects { get; init; }
        public IReadOnlyList<SocialLinksDto>? SocialLinks { get; init; }
        public IReadOnlyList<ContactDto>? Contacts { get; init; }
        public IReadOnlyList<EducationDto>? Educations { get; init; }
    }
}