
namespace DataAccessLayer.Models.ContactModels
{
    public class Contact : BaseEntity<int>
    {
        public string ContactType { get; set; } = string.Empty;
        public string ContactValue { get; set; } = string.Empty;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
