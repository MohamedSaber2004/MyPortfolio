using AutoMapper;
using BusinessLogicLayer.DTos.ContactDTos;
using BusinessLogicLayer.DTos.EducationDTos;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.ProjectDTos;
using BusinessLogicLayer.DTos.SkillDTos;
using BusinessLogicLayer.DTos.SocialLinksDTos;
using DataAccessLayer.Models.ContactModels;
using DataAccessLayer.Models.EducationModels;
using DataAccessLayer.Models.ExperienceModels;
using DataAccessLayer.Models.ProjectModels;
using DataAccessLayer.Models.SkillModels;
using DataAccessLayer.Models.SocialLinksModels;

namespace BusinessLogicLayer.Profiles
{
    public class mappingProfiles : Profile
    {
        public mappingProfiles() : base()
        {
            // Project mappings (READ)
            CreateMap<Project, ProjectDto>()
                .ForMember(d => d.ProjectImage, opt => opt.Ignore()) 
                .ForMember(d => d.ExistingProjectImage, opt => opt.MapFrom(s => s.ProjectImage));

            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(d => d.ProjectImage, opt => opt.MapFrom(s => s.ProjectImage));

            // Project mappings (CREATE / UPDATE)
            CreateMap<CreatedOrUpdatedProjectDto, Project>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ProjectImage, opt => opt.Ignore());

            // Contact mappings
            CreateMap<Contact, ContactDto>();
            CreateMap<Contact, ContactDetailsDto>();
            CreateMap<CreateOrUpdateContactDto, Contact>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            // Education mappings
            CreateMap<Education, EducationDto>();
            CreateMap<Education, EducationDetailsDto>();
            CreateMap<CreateOrUpdateEducationDto, Education>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            // Experience mappings
            CreateMap<Experience, ExperienceDto>();
            CreateMap<Experience, ExperienceDetailsDto>();
            CreateMap<CreateOrUpdateExperienceDto, Experience>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            // SocialLinks mappings
            CreateMap<SocialLink, SocialLinksDto>();
            CreateMap<SocialLink, SocialLinksDetailsDto>();
            CreateMap<CreateOrUpdateSocialLinksDto, SocialLink>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            // Skill mappings
            CreateMap<Skill, SkillDto>().ReverseMap();
            CreateMap<Skill, SkillDetailsDto>();
        }
    }
}
