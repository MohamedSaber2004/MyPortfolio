using MyPortfolio.Helpers;
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
        private readonly ICvDocumentService _cvDocumentService;
        private readonly IWebHostEnvironment _environment;

        public HomeController(
            ILogger<HomeController> logger,
            ISkillService skillService,
            IExperienceService experienceService,
            IProjectService projectService,
            ISocialLinkService socialLinkService,
            IContactService contactService,
            IEducationService educationService,
            ICvDocumentService cvDocumentService,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _skillService = skillService;
            _experienceService = experienceService;
            _projectService = projectService;
            _socialLinkService = socialLinkService;
            _contactService = contactService;
            _educationService = educationService;
            _cvDocumentService = cvDocumentService;
            _environment = environment;
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
                Educations = (await _educationService.GetAllEducationsAsync(null)).ToList(),
                CvFileName = await _cvDocumentService.GetCvFileNameAsync(),
                PortraitUrl = HeroPortrait.PublicUrl(_environment)
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code)
        {
            // Images and stylesheets never ask for HTML, so they get an empty 404 instead of a page of markup.
            var accept = HttpContext.Request.Headers.Accept.ToString();
            if (accept.Length > 0 && !accept.Contains("text/html", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            // Status pages re-execute with ?code=…, the exception handler arrives with 500 already set.
            var status = code ?? (HttpContext.Response.StatusCode >= 400 ? HttpContext.Response.StatusCode : 500);
            return View(new ErrorViewModel { StatusCode = status });
        }
    }
}
