namespace MyPortfolio.Models.ContactModels
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public string ContactType { get; set; } = null!;
        public string ContactValue { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
