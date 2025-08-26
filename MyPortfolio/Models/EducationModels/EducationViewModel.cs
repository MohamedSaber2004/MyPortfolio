namespace MyPortfolio.Models.EducationModels
{
    public class EducationViewModel
    {
        public int Id { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string FieldOfStudy { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
