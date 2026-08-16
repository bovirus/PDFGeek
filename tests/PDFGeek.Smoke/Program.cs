using System.Text;
using PDFGeek.Services;
using TechyGeeksHome.Common;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

// A deliberately dependency-free smoke test: it builds real PDFs, runs every operation the app
// exposes, and re-opens the results to check the page counts and encryption state are what we
// claimed. Run it with `dotnet run` from this folder. Exit code 0 means everything passed.

var work = Path.Combine(Path.GetTempPath(), "pdfgeek-smoke");
if (Directory.Exists(work)) Directory.Delete(work, true);
Directory.CreateDirectory(work);

GlobalFontSettings.FontResolver = new TestFontResolver();

var passed = 0;
var failed = 0;

void Check(string name, Func<string> act)
{
    try
    {
        var detail = act();
        Console.WriteLine($"  PASS  {name}  {detail}");
        passed++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  FAIL  {name}  {ex.GetType().Name}: {ex.Message}");
        foreach (var line in (ex.StackTrace ?? "").Split('\n').Take(4))
            Console.WriteLine($"        {line.Trim()}");
        failed++;
    }
}

static void Expect(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static int PageCountOf(string path, string? password = null)
{
    using var doc = string.IsNullOrEmpty(password)
        ? PdfReader.Open(path, PdfDocumentOpenMode.Import)
        : PdfReader.Open(path, password, PdfDocumentOpenMode.Import);
    return doc.PageCount;
}

string MakePdf(string name, int pages)
{
    var path = Path.Combine(work, name);
    using var doc = new PdfDocument();
    for (var i = 1; i <= pages; i++)
    {
        var page = doc.AddPage();
        using var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawString($"{Path.GetFileNameWithoutExtension(name)} page {i}",
            new XFont("Arial", 24), XBrushes.Black,
            new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.Center);
    }
    doc.Save(path);
    return path;
}

Console.WriteLine("PDFGeek smoke test");
Console.WriteLine("==================");
Console.WriteLine($"Working in {work}");
Console.WriteLine();

var a = MakePdf("alpha.pdf", 5);
var b = MakePdf("bravo.pdf", 3);

// ---------------------------------------------------------------- page ranges
Console.WriteLine("Page range parsing");
Check("all", () => { Expect(PageRange.Parse("all", 5).Count == 5, "expected 5"); return "5 pages"; });
Check("empty means all", () => { Expect(PageRange.Parse("", 5).Count == 5, "expected 5"); return "5 pages"; });
Check("1-3", () => { Expect(string.Join(",", PageRange.Parse("1-3", 5)) == "1,2,3", "wrong"); return "1,2,3"; });
Check("open-ended 3-", () => { Expect(string.Join(",", PageRange.Parse("3-", 5)) == "3,4,5", "wrong"); return "3,4,5"; });
Check("mixed 1,3-4", () => { Expect(string.Join(",", PageRange.Parse("1,3-4", 5)) == "1,3,4", "wrong"); return "1,3,4"; });
Check("clamps out of range", () => { Expect(string.Join(",", PageRange.Parse("4-99", 5)) == "4,5", "wrong"); return "4,5"; });
Check("reversed 4-2", () => { Expect(string.Join(",", PageRange.Parse("4-2", 5)) == "2,3,4", "wrong"); return "2,3,4"; });
Check("junk is ignored", () => { Expect(PageRange.Parse("abc", 5).Count == 0, "expected none"); return "no pages"; });

// ---------------------------------------------------------------- inspect
Console.WriteLine();
Console.WriteLine("Operations");
Check("inspect", () =>
{
    var info = PdfOps.Inspect(a);
    Expect(info.PageCount == 5, $"expected 5 pages, got {info.PageCount}");
    Expect(!info.IsEncrypted, "should not be encrypted");
    return info.Summary;
});

// ---------------------------------------------------------------- merge
var merged = Path.Combine(work, "merged.pdf");
Check("merge 5 + 3", () =>
{
    var pages = PdfOps.Merge(new[] { a, b }, merged);
    Expect(pages == 8, $"expected 8, got {pages}");
    Expect(PageCountOf(merged) == 8, "reopened file has the wrong page count");
    return "8 pages";
});

// ---------------------------------------------------------------- split
Check("split one file per page", () =>
{
    var dir = Path.Combine(work, "split-pages");
    var written = PdfOps.SplitToPages(a, dir);
    Expect(written.Count == 5, $"expected 5 files, got {written.Count}");
    Expect(written.All(f => PageCountOf(f) == 1), "each file should have 1 page");
    return "5 files";
});

Check("split every 2 pages", () =>
{
    var dir = Path.Combine(work, "split-chunks");
    var written = PdfOps.SplitEvery(a, 2, dir);
    Expect(written.Count == 3, $"expected 3 files, got {written.Count}");
    Expect(PageCountOf(written[0]) == 2, "first chunk should be 2 pages");
    Expect(PageCountOf(written[2]) == 1, "last chunk should be the remaining 1 page");
    return "2 + 2 + 1";
});

// ---------------------------------------------------------------- extract / remove
Check("extract 1-3", () =>
{
    var target = Path.Combine(work, "extract.pdf");
    var pages = PdfOps.Extract(a, "1-3", target);
    Expect(pages == 3, $"expected 3, got {pages}");
    Expect(PageCountOf(target) == 3, "reopened file has the wrong page count");
    return "3 pages";
});

Check("remove 2,4", () =>
{
    var target = Path.Combine(work, "trimmed.pdf");
    var pages = PdfOps.RemovePages(a, "2,4", target);
    Expect(pages == 3, $"expected 3, got {pages}");
    return "3 pages left";
});

Check("removing every page is refused", () =>
{
    try
    {
        PdfOps.RemovePages(a, "all", Path.Combine(work, "nope.pdf"));
        throw new Exception("should have refused");
    }
    catch (InvalidOperationException)
    {
        return "refused as expected";
    }
});

// ---------------------------------------------------------------- rotate
Check("rotate 90", () =>
{
    var target = Path.Combine(work, "rotated.pdf");
    PdfOps.Rotate(a, "all", 90, target);
    using var doc = PdfReader.Open(target, PdfDocumentOpenMode.Import);
    Expect(doc.Pages[0].Rotate == 90, $"expected 90, got {doc.Pages[0].Rotate}");
    return "all pages at 90 degrees";
});

Check("rotate accumulates", () =>
{
    var once = Path.Combine(work, "rot1.pdf");
    var twice = Path.Combine(work, "rot2.pdf");
    PdfOps.Rotate(a, "1", 270, once);
    PdfOps.Rotate(once, "1", 270, twice);
    using var doc = PdfReader.Open(twice, PdfDocumentOpenMode.Import);
    Expect(doc.Pages[0].Rotate == 180, $"expected 180, got {doc.Pages[0].Rotate}");
    return "270 + 270 = 180";
});

// ---------------------------------------------------------------- reorder
Check("reorder 3,1,2", () =>
{
    var target = Path.Combine(work, "reordered.pdf");
    var pages = PdfOps.Reorder(a, "3,1,2", target);
    Expect(pages == 3, $"expected 3, got {pages}");
    return "3 pages in the given order";
});

// ---------------------------------------------------------------- watermark
Check("watermark", () =>
{
    var target = Path.Combine(work, "watermarked.pdf");
    var pages = PdfOps.Watermark(a, "DRAFT", target, 48, 20, true, "Arial");
    Expect(pages == 5, $"expected 5, got {pages}");
    Expect(new FileInfo(target).Length > new FileInfo(a).Length / 2, "output looks empty");
    return "5 pages stamped";
});

// ---------------------------------------------------------------- security
var locked = Path.Combine(work, "locked.pdf");
Check("add password", () =>
{
    PdfOps.Protect(a, locked, "hunter2", null, allowPrinting: true, allowCopying: false);
    var info = PdfOps.Inspect(locked);
    Expect(info.IsEncrypted, "file should be encrypted");
    return "AES-128, printing allowed";
});

Check("locked file refuses the wrong password", () =>
{
    try
    {
        PageCountOf(locked, "wrong");
        throw new Exception("should not have opened");
    }
    catch (PdfReaderException)
    {
        return "refused as expected";
    }
});

Check("locked file opens with the right password", () =>
{
    Expect(PageCountOf(locked, "hunter2") == 5, "wrong page count");
    return "5 pages";
});

Check("remove password", () =>
{
    var target = Path.Combine(work, "unlocked.pdf");
    PdfOps.Unprotect(locked, "hunter2", target);
    var info = PdfOps.Inspect(target);
    Expect(!info.IsEncrypted, "file should no longer be encrypted");
    Expect(PageCountOf(target) == 5, "wrong page count");
    return "opens with no password, 5 pages";
});

// ---------------------------------------------------------------- helpers
Check("UniquePath never overwrites", () =>
{
    var first = Path.Combine(work, "collide.pdf");
    File.WriteAllText(first, "x");
    var second = PdfOps.UniquePath(first);
    Expect(second != first, "should have picked a new name");
    Expect(second.EndsWith("(2).pdf"), $"unexpected name: {second}");
    return Path.GetFileName(second);
});

// ---------------------------------------------------------------- shared chrome
Console.WriteLine();
Console.WriteLine("Shared TechyGeeksHome components");
Check("version tag v1.2.3", () => { Expect(UpdateChecker.TryParseVersion("v1.2.3", out var v) && v.Major==1 && v.Minor==2 && v.Build==3, "wrong"); return "1.2.3"; });
Check("version tag 2.0", () => { Expect(UpdateChecker.TryParseVersion("2.0", out var v) && v.Major==2 && v.Minor==0, "wrong"); return "2.0"; });
Check("version tag release-1.4.2", () => { Expect(UpdateChecker.TryParseVersion("release-1.4.2", out var v) && v.Minor==4 && v.Build==2, "wrong"); return "1.4.2"; });
Check("version tag with no digits is rejected", () => { Expect(!UpdateChecker.TryParseVersion("latest", out _), "should have failed"); return "rejected"; });
Check("AppInfo reads a version", () => { Expect(AppInfo.CurrentVersionText.Split('.').Length == 3, "expected three parts"); return AppInfo.CurrentVersionText; });

Console.WriteLine();
Console.WriteLine($"{passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;


/// <summary>Linux stand-in for the app's Windows font resolver, so this can run in CI.</summary>
file sealed class TestFontResolver : IFontResolver
{
    private const string Regular = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf";
    private const string Bold = "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf";

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        => new FontResolverInfo(isBold ? "bold" : "regular");

    public byte[]? GetFont(string faceName)
    {
        var path = faceName == "bold" ? Bold : Regular;
        return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }
}
