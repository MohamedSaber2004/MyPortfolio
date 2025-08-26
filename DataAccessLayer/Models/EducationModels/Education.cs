
namespace DataAccessLayer.Models.EducationModels
{
    public class Education : BaseEntity<int>
    {
        public string InstitutionName { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string FieldOfStudy { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
