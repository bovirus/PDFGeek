using System;
using Avalonia;

namespace PDFGeek;

internal static class Program
{
    /// <summary>PDFs passed on the command line, e.g. from "Open with" or dropping onto the exe.</summary>
    public static string[] StartupFiles { get; private set; } = System.Array.Empty<string>();

    [STAThread]
    public static void Main(string[] args)
    {
        StartupFiles = System.Array.FindAll(args,
            a => a.EndsWith(".pdf", System.StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(a));

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
