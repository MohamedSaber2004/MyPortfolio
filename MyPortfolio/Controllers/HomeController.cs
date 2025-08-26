using System.Diagnostics;
using MyPortfolio.Models;
using MyPortfolio.Models.Home;

namespace MyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISkillService _skillService;
        private readonly IExperienceService _experienceService;
        private readonly IProjectService _projectService;
        private readonly ISocialLinkService _socialLinkService;
        private readonly IContactService _contactService;
        private readonly IEducationService _educationService;

        public HomeController(
            ILogger<HomeController> logger,
            ISkillService skillService,
            IExperienceService experienceService,
            IProjectService projectService,
            ISocialLinkService socialLinkService,
            IContactService contactService,
            IEducationService educationService)
        {
            _logger = logger;
            _skillService = skillService;
            _experienceService = experienceService;
            _projectService = projectService;
            _socialLinkService = socialLinkService;
            _contactService = contactService;
            _educationService = educationService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Skills = (await _skillService.GetAllSkillsAsync()).ToList(),
                Experiences = (await _experienceService.GetAllExperiencesAsync()).ToList(),
                Projects = (await _projectService.GetAllProjects(null)).ToList(),
                SocialLinks = (await _socialLinkService.GetAllSocialLinksAsync()).ToList(),
                Contacts = (await _contactService.GetAllContactsAsync(null)).ToList(),
                Educations = (await _educationService.GetAllEducationsAsync(null)).ToList()
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
