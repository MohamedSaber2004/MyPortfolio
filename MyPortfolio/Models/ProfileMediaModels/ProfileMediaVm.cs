namespace MyPortfolio.Models.ProfileMediaModels
{
    public sealed class ProfileMediaVm
    {
        public string? FileName { get; init; }
        public string? Url { get; init; }
        public string? SizeLabel { get; init; }
        public DateTime? UpdatedOn { get; init; }
        public bool HasImage => FileName is not null;
    }
}
