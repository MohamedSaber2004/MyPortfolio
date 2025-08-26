namespace MyPortfolio.Models.ExperienceModels
{
    public class ExperienceViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
