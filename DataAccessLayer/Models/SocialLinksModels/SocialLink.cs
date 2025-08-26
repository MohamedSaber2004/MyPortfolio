
namespace DataAccessLayer.Models.SocialLinksModels
{
    public class SocialLink:BaseEntity<int>
    {
        public string PlatformName { get; set; } = null!;
        public string URL { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
