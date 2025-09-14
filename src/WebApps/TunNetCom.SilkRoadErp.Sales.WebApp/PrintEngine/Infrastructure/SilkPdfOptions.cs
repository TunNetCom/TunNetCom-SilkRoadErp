using Microsoft.Playwright;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public class SilkPdfOptions
{
    public string? Format { get; set; }
    public bool? DisplayHeaderFooter { get; set; }
    public string? HeaderTemplate { get; set; }
    public string? FooterTemplate { get; set; }
    public string? MarginTop { get; set; }
    public string? MarginBottom { get; set; }
    public string? MarginLeft { get; set; }
    public string? MarginRight { get; set; }
    public float? Scale { get; set; }
    public bool? PreferCSSPageSize { get; set; }
    public string? PageRanges { get; set; }
    public string? Width { get; set; }
    public string? Height { get; set; }
    public bool? PrintBackground { get; set; }
    public bool? Landscape { get; set; }

    internal PagePdfOptions ToPlaywrightOptions()
    {
        return new PagePdfOptions
        {
            Format = Format,
            DisplayHeaderFooter = DisplayHeaderFooter,
            HeaderTemplate = HeaderTemplate,
            FooterTemplate = FooterTemplate,
            Scale = Scale,
            PreferCSSPageSize = PreferCSSPageSize,
            PageRanges = PageRanges,
            Width = Width,
            Height = Height,
            PrintBackground = PrintBackground,
            Landscape = Landscape,
            Margin = new Margin
            {
                Top = MarginTop,
                Bottom = MarginBottom,
                Left = MarginLeft,
                Right = MarginRight
            }
        };
    }

    public static SilkPdfOptions Default => new()
    {
        Format = "A4",
        DisplayHeaderFooter = true,
        MarginTop = "20px",
        MarginBottom = "20px",
        MarginLeft = "20px",
        MarginRight = "20px",
        PrintBackground = true
    };
}