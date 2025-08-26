namespace MyPortfolio.Models.ManagerModels.UserModels
{
    public class UserManagerViewModel
    {
        public string Id { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }
    }
}
