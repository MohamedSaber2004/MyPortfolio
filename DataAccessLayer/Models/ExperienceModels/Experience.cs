
namespace DataAccessLayer.Models.ExperienceModels
{
    public class Experience : BaseEntity<int>
    {
        public string CompanyName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
