# PDFGeek — handover to Claude Code

Everything below is the state of the project as built in Cowork on 16 Aug 2026. Open this
folder in Claude Code on the Windows machine and start at "First run".

## Update, 16 Aug — renamed, plus shared About and update check

- Renamed **PdfGeek → PDFGeek** everywhere: folders, projects, namespaces, assembly name.
  Rebuilt clean from scratch; no stale artefacts.
- Added **`src/TechyGeeksHome.Common`**, a small shared library holding the About window and the
  update checker. It is deliberately Avalonia-plus-BCL only, so the folder can be copied
  straight into the next Geek app's repo. Wiring it into a new app is three steps: copy the
  folder, add a project reference, and write an `AppMetadata.cs` (see PDFGeek's for the shape).
- Both are reachable from the sidebar: **Check for updates** and **About PDFGeek**, with the
  running version underneath.
- Smoke suite is now **29 checks** and covers the release-tag version parser.

**One thing that needs your eye:** the update check is PDFGeek's only network call, and the
README previously promised the app never opens a socket. That claim is now corrected rather
than quietly dropped — the README explains exactly what the check sends, that it only runs on
click, and that nothing is downloaded or installed automatically. It is deliberately
**manual-only, with no check-on-startup**, because "your documents never leave your machine" is
the entire pitch and a silent call home on every launch would undercut it. If you would rather
it checked automatically, that is a one-line change in `MainWindow`, but I would make it an
opt-in setting that defaults to off.

I have not seen DiskGeek's About window, so this is built to the brand tokens rather than
copied from it. If you want them identical, paste me DiskGeek's About XAML or connect the
`OneDrive\TGH` folder and I will match it exactly — and since it is now shared code, matching
it once fixes it for all 22 tools.

## What exists

A complete, compiling Avalonia app with all six PDF tools implemented and unit-verified.
Built and published clean for `win-x64`; the smoke harness passes 24/24 against real PDFs.

**It has never been run as a GUI.** The Cowork sandbox is Linux, so the window has never been
drawn. The PDF logic is proven; the layout, spacing, theme and drag-and-drop are not.

## First run

```powershell
cd PDFGeek
dotnet run --project src/PDFGeek
```

Then work through the checklist below. Expect layout problems, not logic problems.

## What to check first, in order

1. **Does the window render?** The dark theme, the sidebar, the six tool panels.
2. **Does switching tools work?** Clicking the sidebar toggles `IsVisible` on six StackPanels
   in `MainWindow.axaml.cs → ShowPanel`. Crude but predictable.
3. **Drag and drop.** Written but never exercised. `OnDrop` adds to the merge list when the
   Merge tool is selected, otherwise it fills the current tool's file box. Avalonia's
   `e.Data.GetFiles()` behaviour is the bit most likely to need adjusting.
4. **The watermark tool specifically.** It is the only feature that needs a font, and it goes
   through `WindowsFontResolver`, which reads TTFs straight out of `C:\Windows\Fonts`. That
   path has never been executed — on Linux the smoke test swaps in its own DejaVu resolver.
   If watermarking throws, this is why.
5. **"Show last result"** shells out to `explorer.exe /select,"path"`. Untested on Windows.
6. **File pickers** use Avalonia's `StorageProvider`. Check the PDF filter actually filters.

## Known rough edges, deliberately left

- **44MB executable.** Self-contained .NET plus Avalonia. `PublishTrimmed` would get it to
  roughly 25-30MB but trimming can strip types that Avalonia and PDFsharp reach by reflection,
  so it needs real testing before it goes in. Worth doing before release if you care about the
  download size.
- **No icon.** `docs/logo.png` is referenced by the README but does not exist yet, and the exe
  has no embedded icon. Needs the TGH badge treatment — navy `#0A0D16`, accent `#38BDF8`.
- **No progress reporting.** Operations run on a background thread via `RunAsync` and the
  status bar says "Merging…", but a 500-page file gives no progress. Fine for v1.
- **Merge list has no drag-to-reorder** — it has Move up / Move down buttons instead.
- **Compress is not implemented.** It was deliberately cut from v1; doing it properly means
  re-encoding images, which is a different job. Do not add it under launch pressure.

## A bug worth knowing about

The smoke test caught this before it shipped, and it will bite again if anyone adds a new
operation: **PDFsharp seals a document when you call `Save()`**. Reading `doc.PageCount` after
saving throws `InvalidOperationException`. Five of the six operations had this bug. Capture
anything you want to report *before* the save. There is a comment in `PdfOps.Merge` marking it.

## Rules this project is built to

- **Nothing costs money.** No code-signing certificate, no paid libraries, no services.
- **Portable single exe.** No installer, no admin rights, no registry writes. This is what
  makes PortableApps.com and The Portable Freeware Collection viable submission channels,
  which the installer-based DiskGeek could not use.
- **No network code at all.** "Nothing leaves your PC" is the entire pitch, so it has to be
  literally true and checkable in the source.
- **Never modify the input file.** Every operation writes a new file the user chose.

## Dependencies and licences

| Package | Version | Licence | Why |
|---|---|---|---|
| Avalonia | 11.2.3 | MIT | UI |
| PDFsharp | 6.1.1 | MIT | Every PDF operation |

Both permissive, both safe to ship in a free closed-source binary. No AGPL anywhere — MuPDF and
iText were deliberately avoided for exactly that reason.

## When it runs properly

1. `dotnet publish src/PDFGeek -c Release -r win-x64 -o publish`
2. Test the exe on a machine without the .NET SDK installed.
3. Then the launch checklist in `TechyGeeksHome-Afternoon-Builds.xlsx` → Launch Checklist:
   GitHub release → winget → Chocolatey → directories → AlternativeTo → Dev.to → Product Hunt.

Screenshots to generate once and reuse everywhere: one at 1920×1080 minimum for Softonic, one
portrait or square for Pinterest, plus the icon at 128px and 256px.

## Suggested "alternative to" tags for listings

Smallpdf, iLovePDF, Sejda, PDF24, PDFsam, Adobe Acrobat.

The line that does the work in every listing and post:

> Every online PDF tool wants you to upload your documents and caps what you can do until you
> pay. This one runs on your machine, with no limits and nothing to upgrade to.
