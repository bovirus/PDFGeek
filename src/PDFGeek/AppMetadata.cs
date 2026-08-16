using TechyGeeksHome.Common;

namespace PDFGeek;

/// <summary>
/// Everything the shared About window and update checker need to know about this app.
/// One place to edit when the product page moves or a dependency changes.
/// </summary>
public static class AppMetadata
{
    public static readonly AppInfo Info = new()
    {
        Name = "PDFGeek",
        Tagline = "Free, offline PDF tools for Windows",
        Description =
            "Merge, split, extract, rotate, watermark and password-protect PDFs on your own " +
            "machine. No file-count caps, no hourly limits, no watermarks on the output and " +
            "nothing uploaded to anyone's server.",
        GitHubOwner = "techygeekshome",
        GitHubRepo = "PDFGeek",
        ProductUrl = "https://techygeekshome.info/pdfgeek/",
        WebsiteUrl = "https://techygeekshome.info",
        DonateUrl = "https://techygeekshome.info",
        LicenceLine =
            "Free to use, including at work. No paid tier, no subscription, no upsell. " +
            "If it saved you a subscription, a donation is welcome but never expected.",
        Credits = new[]
        {
            new Credit("Avalonia", "MIT", "https://avaloniaui.net/"),
            new Credit("PDFsharp", "MIT", "https://github.com/empira/PDFsharp")
        }
    };
}
