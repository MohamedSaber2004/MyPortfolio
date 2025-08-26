namespace MyPortfolio.Models.ProjectModels
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string ProjectURL { get; set; } = null!;
        public IFormFile? ProjectImage { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? LastModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
