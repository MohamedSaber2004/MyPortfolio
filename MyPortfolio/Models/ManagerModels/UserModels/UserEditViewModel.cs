namespace MyPortfolio.Models.ManagerModels.UserModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string FullName { get; set; } = null!;
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }
}
