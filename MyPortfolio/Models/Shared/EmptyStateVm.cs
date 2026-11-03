namespace MyPortfolio.Models.Shared;

public sealed record EmptyStateVm
{
    public string Title { get; init; } = "Nothing here yet";

    public string? Text { get; init; }

    public string Icon { get; init; } = "layers";

    public string? ActionHref { get; init; }

    public string? ActionLabel { get; init; }

    public string ActionIcon { get; init; } = "plus";
}
