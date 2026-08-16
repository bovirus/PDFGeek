using System;
using System.Collections.Concurrent;
using System.IO;
using PdfSharp.Fonts;

namespace PDFGeek.Services;

/// <summary>
/// PDFsharp's core (non-GDI) package has no font handling of its own, so anything that draws
/// text onto a page - the watermark tool - needs a resolver registered first. This one reads
/// TTF files straight out of the Windows font directory, with the user's own font folder as a
/// fallback for machines where fonts were installed per-user.
/// </summary>
public sealed class WindowsFontResolver : IFontResolver
{
    private static readonly ConcurrentDictionary<string, byte[]> Cache = new();

    // Deliberately conservative: fonts that ship with every Windows install.
    private static readonly (string Family, string Regular, string Bold)[] Known =
    {
        ("arial",           "arial.ttf",   "arialbd.ttf"),
        ("segoe ui",        "segoeui.ttf", "segoeuib.ttf"),
        ("calibri",         "calibri.ttf", "calibrib.ttf"),
        ("times new roman", "times.ttf",   "timesbd.ttf"),
        ("verdana",         "verdana.ttf", "verdanab.ttf"),
        ("tahoma",          "tahoma.ttf",  "tahomabd.ttf"),
    };

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var wanted = (familyName ?? string.Empty).Trim().ToLowerInvariant();

        foreach (var (family, regular, bold) in Known)
        {
            if (family != wanted) continue;
            var file = isBold ? bold : regular;
            if (FindFontFile(file) is not null)
                return new FontResolverInfo(file);
        }

        // Anything we do not recognise falls back to Arial so the operation never hard-fails.
        var fallback = isBold ? "arialbd.ttf" : "arial.ttf";
        return FindFontFile(fallback) is null ? null : new FontResolverInfo(fallback);
    }

    public byte[]? GetFont(string faceName)
    {
        if (Cache.TryGetValue(faceName, out var cached)) return cached;

        var path = FindFontFile(faceName);
        if (path is null) return null;

        try
        {
            var bytes = File.ReadAllBytes(path);
            Cache[faceName] = bytes;
            return bytes;
        }
        catch
        {
            return null;
        }
    }

    private static string? FindFontFile(string fileName)
    {
        foreach (var dir in FontDirectories())
        {
            if (string.IsNullOrWhiteSpace(dir)) continue;
            var candidate = Path.Combine(dir, fileName);
            if (File.Exists(candidate)) return candidate;
        }
        return null;
    }

    private static string[] FontDirectories()
    {
        var windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        return new[]
        {
            string.IsNullOrEmpty(windows) ? string.Empty : Path.Combine(windows, "Fonts"),
            string.IsNullOrEmpty(localAppData) ? string.Empty : Path.Combine(localAppData, "Microsoft", "Windows", "Fonts"),
        };
    }
}

public static class FontSetup
{
    private static bool _done;

    public static void Register()
    {
        if (_done) return;
        try
        {
            GlobalFontSettings.FontResolver = new WindowsFontResolver();
        }
        catch
        {
            // A missing resolver only affects the watermark tool; everything else still works.
        }
        _done = true;
    }
}
