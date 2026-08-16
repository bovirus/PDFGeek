using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFGeek.Services;

/// <summary>
/// Parses the page-range syntax people already expect from print dialogs:
/// "1-3, 5, 9-" and "all". Everything is 1-based on the way in and on the way out.
/// </summary>
public static class PageRange
{
    public static IReadOnlyList<int> Parse(string? input, int pageCount)
    {
        if (pageCount <= 0) return Array.Empty<int>();

        if (string.IsNullOrWhiteSpace(input) ||
            input.Trim().Equals("all", StringComparison.OrdinalIgnoreCase))
            return Enumerable.Range(1, pageCount).ToList();

        var pages = new SortedSet<int>();

        foreach (var rawPart in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var part = rawPart.Trim();
            if (part.Length == 0) continue;

            var dash = part.IndexOf('-');
            if (dash < 0)
            {
                if (int.TryParse(part, out var single) && single >= 1 && single <= pageCount)
                    pages.Add(single);
                continue;
            }

            var leftText = part[..dash].Trim();
            var rightText = part[(dash + 1)..].Trim();

            var start = leftText.Length == 0 ? 1
                : int.TryParse(leftText, out var s) ? s : 1;
            var end = rightText.Length == 0 ? pageCount
                : int.TryParse(rightText, out var e) ? e : pageCount;

            if (start > end) (start, end) = (end, start);

            start = Math.Max(1, start);
            end = Math.Min(pageCount, end);

            for (var i = start; i <= end; i++) pages.Add(i);
        }

        return pages.ToList();
    }

    public static string Describe(IReadOnlyList<int> pages)
    {
        if (pages.Count == 0) return "no pages";
        return pages.Count == 1 ? "1 page" : $"{pages.Count} pages";
    }
}
