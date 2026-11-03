using System.Globalization;
using BusinessLogicLayer.DTos.ContactDTos;
using BusinessLogicLayer.DTos.EducationDTos;
using BusinessLogicLayer.DTos.ExperienceDTos;
using BusinessLogicLayer.DTos.ProjectDTos;
using BusinessLogicLayer.DTos.SkillDTos;
using BusinessLogicLayer.DTos.SocialLinksDTos;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.RoleModels;
using DataAccessLayer.Models.Shared.Enums;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Models.DashboardModels;

namespace MyPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IProjectService projectService,
        ISkillService skillService,
        IExperienceService experienceService,
        IEducationService educationService,
        ISocialLinkService socialLinkService,
        IContactService contactService,
        ICvDocumentService cvService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var projects = (await projectService.GetAllProjects(null, true)).ToList();
            var skills = (await skillService.GetAllSkillsAsync(null, true)).ToList();
            var experiences = (await experienceService.GetAllExperiencesAsync(null, true)).ToList();
            var educations = (await educationService.GetAllEducationsAsync(null, true)).ToList();
            var socialLinks = (await socialLinkService.GetAllSocialLinksAsync(null, true)).ToList();
            var contacts = (await contactService.GetAllContactsAsync(null, true)).ToList();
            var cv = await cvService.GetLatestCvAsync();

            var liveProjects = projects.Where(p => !p.IsDeleted).ToList();
            var liveSkills = skills.Where(s => !s.IsDeleted).ToList();
            var liveExperiences = experiences.Where(e => !e.IsDeleted).ToList();
            var liveEducations = educations.Where(e => !e.IsDeleted).ToList();
            var liveLinks = socialLinks.Where(s => !s.IsDeleted).ToList();
            var liveContacts = contacts.Where(c => !c.IsDeleted).ToList();

            var levelTotals = liveSkills
                .GroupBy(s => s.ProficiencyLevel)
                .ToDictionary(g => g.Key, g => g.Count());

            var yearTotals = liveProjects
                .Where(p => p.StartDate.Year > 2000)
                .GroupBy(p => p.StartDate.Year)
                .OrderByDescending(g => g.Key)
                .Take(6)
                .OrderBy(g => g.Key)
                .ToList();
            var yearMax = yearTotals.Select(g => g.Count()).DefaultIfEmpty(0).Max();

            var entries = liveProjects
                .Select(p => (p.StartDate, new Entry("folder", p.Title, SpanOf(p.StartDate, p.EndDate), "Project", p.Id)))
                .Concat(liveExperiences.Select(e => (e.StartDate, new Entry("briefcase", $"{e.Role} · {e.CompanyName}", SpanOf(e.StartDate, e.EndDate), "Expereience", e.Id))))
                .Concat(liveEducations.Select(e => (e.StartDate, new Entry("cap", $"{e.Degree}, {e.InstitutionName}", SpanOf(e.StartDate, e.EndDate), "Education", e.Id))))
                .OrderByDescending(x => x.Item1)
                .Take(7)
                .Select(x => x.Item2)
                .ToList();

            var emailChannel = liveContacts.Any(c => string.Equals(c.ContactType, "email", StringComparison.OrdinalIgnoreCase));
            var phoneChannel = liveContacts.Any(c => string.Equals(c.ContactType, "phone", StringComparison.OrdinalIgnoreCase));

            return View(new DashboardViewModel
            {
                UserName = User.Identity?.Name ?? "there",
                Users = await userManager.Users.CountAsync(u => !u.IsDeleted),
                Roles = await roleManager.Roles.CountAsync(),
                Cv = cv,
                TotalActive = liveProjects.Count + liveSkills.Count + liveExperiences.Count +
                              liveEducations.Count + liveLinks.Count + liveContacts.Count,
                Counters =
                [
                    Group<ProjectDto>("Projects", "folder", "Project", liveProjects, projects),
                    Group<SkillDto>("Skills", "layers", "Skill", liveSkills, skills),
                    Group<ExperienceDto>("Experience", "briefcase", "Expereience", liveExperiences, experiences),
                    Group<EducationDto>("Education", "cap", "Education", liveEducations, educations),
                    Group<SocialLinksDto>("Social links", "share", "SocialLink", liveLinks, socialLinks),
                    Group<ContactDto>("Contact channels", "mail", "Contact", liveContacts, contacts)
                ],
                SkillLevels =
                [
                    .. Enum.GetValues<ProficiencyLevels>()
                           .OrderByDescending(level => levelTotals.GetValueOrDefault(level))
                           .Select(level =>
                           {
                               var count = levelTotals.GetValueOrDefault(level);
                               var share = liveSkills.Count == 0 ? 0 : (int)Math.Round(count * 100d / liveSkills.Count);
                               return new Slice(Display.Level(level), count, share);
                           })
                ],
                ProjectsByYear =
                [
                    .. yearTotals.Select(g => SliceOf(g.Key.ToString(CultureInfo.InvariantCulture), g.Count(), yearMax))
                ],
                Entries = entries,
                Platforms = [.. liveLinks.Select(s => s.PlatformName).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(n => n)],
                Checks =
                [
                    new Check("pdf", "A CV is available for download", cv is not null, "CvDocument"),
                    new Check("folder", "At least one project is published", liveProjects.Count > 0, "Project"),
                    new Check("image", "Every project has a cover image", liveProjects.All(p => !string.IsNullOrWhiteSpace(p.ExistingProjectImage)), "Project"),
                    new Check("layers", "At least six skills are listed", liveSkills.Count >= 6, "Skill"),
                    new Check("briefcase", "Work history is filled in", liveExperiences.Count > 0, "Expereience"),
                    new Check("cap", "Education is filled in", liveEducations.Count > 0, "Education"),
                    new Check("mail", "An email channel is set", emailChannel, "Contact"),
                    new Check("phone", "A phone channel is set", phoneChannel, "Contact"),
                    new Check("share", "At least one social profile is linked", liveLinks.Count > 0, "SocialLink")
                ]
            });
        }

        private static CounterGroup Group<T>(string label, string icon, string controller, IReadOnlyList<T> live, IReadOnlyList<T> all)
            => new(label, icon, controller, live.Count, all.Count - live.Count);

        private static Slice SliceOf(string label, int value, int max)
            => new(label, value, max == 0 ? 0 : (int)Math.Round(value * 100d / max));

        private static string SpanOf(DateOnly start, DateOnly end)
        {
            static string Format(DateOnly d) => d.ToString("MMM yyyy", CultureInfo.InvariantCulture);
            if (start.Year <= 1900) return "—";
            return end.Year <= 1900 || end > DateOnly.FromDateTime(DateTime.Today)
                ? $"{Format(start)} → now"
                : $"{Format(start)} → {Format(end)}";
        }
    }
}
