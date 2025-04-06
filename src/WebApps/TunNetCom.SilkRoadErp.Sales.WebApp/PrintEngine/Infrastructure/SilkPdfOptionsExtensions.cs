using Microsoft.Playwright;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public static class SilkPdfOptionsExtensions
{
    public static PagePdfOptions ToPlaywrightOptions(this SilkPdfOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new PagePdfOptions
        {
            Format = options.Format,
            DisplayHeaderFooter = options.DisplayHeaderFooter,
            HeaderTemplate = options.HeaderTemplate,
            FooterTemplate = options.FooterTemplate,
            Scale = options.Scale,
            PreferCSSPageSize = options.PreferCSSPageSize,
            PageRanges = options.PageRanges,
            Width = options.Width,
            Height = options.Height,
            PrintBackground = options.PrintBackground,
            Landscape = options.Landscape,
        };
    }
}
