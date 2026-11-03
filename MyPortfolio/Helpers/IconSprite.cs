using System.Text.RegularExpressions;

namespace MyPortfolio.Helpers;


public static class IconSprite
{
    private static readonly Regex Symbols = new(@"<symbol id=""i-([A-Za-z0-9]+)""", RegexOptions.Compiled);
    private static readonly HashSet<string> Known = [];

    public static string Content { get; private set; } = string.Empty;

    public static void Load(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.WebRootPath ?? "wwwroot", "assets", "icon-sprite.svg");
        if (!File.Exists(path))
        {
            return;
        }

        var raw = File.ReadAllText(path);
        Content = raw;
        Known.Clear();
        foreach (Match match in Symbols.Matches(raw))
        {
            Known.Add(match.Groups[1].Value);
        }
    }

    public static bool Exists(string name) => Known.Contains(name);
}
