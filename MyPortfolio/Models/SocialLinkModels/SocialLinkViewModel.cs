namespace MyPortfolio.Models.SocialLinkModels
{
    public class SocialLinkViewModel
    {
        public int Id { get; set; }
        public string PlatformName { get; set; } = null!;
        public string URL { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
