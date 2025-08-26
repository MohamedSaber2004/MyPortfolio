
namespace DataAccessLayer.Models.ProjectModels
{
    public class Project : BaseEntity<int>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string ProjectURL { get; set; } = null!;
        public string? ProjectImage { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
