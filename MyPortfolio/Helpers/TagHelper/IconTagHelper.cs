using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyPortfolio.Helpers.TagHelpers;

/// <summary>
/// &lt;ui-icon name="home" /&gt; renders a sprite-backed svg glyph.
/// </summary>
[HtmlTargetElement("ui-icon", TagStructure = TagStructure.WithoutEndTag)]
public class IconTagHelper : TagHelper
{
    private const string Fallback = "link";

    /// <summary>Symbol name without the i- prefix, e.g. "home", "chevR", "folder".</summary>
    public string Name { get; set; } = Fallback;

    /// <summary>sm | md | lg | xl</summary>
    public string? Size { get; set; }

    /// <summary>accent | success | danger | info | warning</summary>
    public string? Tone { get; set; }

    /// <summary>When set the glyph is announced to assistive tech.</summary>
    public string? Label { get; set; }

    public string? Class { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var name = IconSprite.Exists(Name) ? Name : Fallback;

        var classes = new List<string> { "icon" };
        if (!string.IsNullOrWhiteSpace(Size))
        {
            classes.Add("icon--" + Size);
        }
        if (!string.IsNullOrWhiteSpace(Tone))
        {
            classes.Add("text-" + Tone);
        }
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        output.TagName = "svg";
        // A self-closing <ui-icon/> would carry the <use> child straight through into an empty <svg/>;
        // the sprite glyph only renders when the element is expanded.
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", string.Join(' ', classes));
        output.Attributes.SetAttribute("focusable", "false");
        if (string.IsNullOrWhiteSpace(Label))
        {
            output.Attributes.SetAttribute("aria-hidden", "true");
        }
        else
        {
            output.Attributes.SetAttribute("role", "img");
            output.Attributes.SetAttribute("aria-label", Label);
        }
        output.Content.SetHtmlContent($"<use href=\"#i-{name}\"></use>");
    }
}
