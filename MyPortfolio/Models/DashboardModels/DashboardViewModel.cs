using BusinessLogicLayer.DTos.CvDocumentDTos;

namespace MyPortfolio.Models.DashboardModels
{
    public sealed class DashboardViewModel
    {
        public required string UserName { get; init; }
        public IReadOnlyList<CounterGroup> Counters { get; init; } = [];
        public int Users { get; init; }
        public int Roles { get; init; }
        public IReadOnlyList<Slice> SkillLevels { get; init; } = [];
        public IReadOnlyList<Slice> ProjectsByYear { get; init; } = [];
        public IReadOnlyList<Entry> Entries { get; init; } = [];
        public IReadOnlyList<Check> Checks { get; init; } = [];
        public CvDocumentDto? Cv { get; init; }
        public IReadOnlyList<string> Platforms { get; init; } = [];
        public int TotalActive { get; init; }
    }

    public sealed record CounterGroup(string Label, string Icon, string Controller, int Active, int Archived);

    public sealed record Slice(string Label, int Value, int Percent);

    public sealed record Entry(string Icon, string Text, string When, string Controller, int Id);

    public sealed record Check(string Icon, string Text, bool Passed, string Controller);
}
