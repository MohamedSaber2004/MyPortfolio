using System.Globalization;
using DataAccessLayer.Models.Shared.Enums;

namespace MyPortfolio.Helpers;

public static class Display
{
    public static string Level(ProficiencyLevels level) => level switch
    {
        ProficiencyLevels.Advanced => "Advanced",
        ProficiencyLevels.Intermmediate => "Intermediate",
        _ => "Beginner"
    };

    public static int LevelPercent(ProficiencyLevels level) => level switch
    {
        ProficiencyLevels.Advanced => 100,
        ProficiencyLevels.Intermmediate => 67,
        _ => 34
    };

    public static int Months(DateOnly start, DateOnly end)
        => Math.Max((end.Year - start.Year) * 12 + (end.Month - start.Month) + 1, 0);

    public static string Tenure(DateOnly start, DateOnly end)
    {
        var months = Months(start, end);
        if (months == 0) return "—";
        if (months < 12) return $"{months} mo";
        var rest = months % 12;
        return rest == 0 ? $"{months / 12} yrs" : $"{months / 12} yrs {rest} mo";
    }

    public static string Period(DateOnly start, DateOnly end)
    {
        if (start.Year <= 1900) return "Dates to confirm";
        var from = Month(start);
        if (end.Year <= 1900 || end > DateOnly.FromDateTime(DateTime.Today)) return $"{from} → now";
        return $"{from} → {Month(end)}";
    }

    public static string Month(DateOnly value) => value.ToString("MMM yyyy", CultureInfo.InvariantCulture);

    public static string Stamp(DateTime value) => value.ToString("dd MMM yyyy · HH:mm", CultureInfo.InvariantCulture);

    public static string Stamp(DateTime? value) => value is null ? "—" : Stamp(value.Value);
}
