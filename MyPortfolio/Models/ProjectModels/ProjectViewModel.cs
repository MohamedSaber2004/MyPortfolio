namespace MyPortfolio.Models.ProjectModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string ProjectURL { get; set; } = null!;
        public IFormFile? ProjectImage { get; set; }
        public bool IsDeleted { get; set; }

        public string? ExistingProjectImage { get; set; }
    }
}
