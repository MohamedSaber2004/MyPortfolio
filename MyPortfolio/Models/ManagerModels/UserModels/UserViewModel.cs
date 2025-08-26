namespace MyPortfolio.Models.ManagerModels.UserModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
        public bool IsDeleted { get; set; }
    }
}
