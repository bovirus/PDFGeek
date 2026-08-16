<div align="center">

<img src="docs/logo.png" width="96" height="96" alt="PdfGeek" />

# PdfGeek

**Free, offline PDF tools for Windows. No limits, no uploads, no subscription.**

![Version](https://img.shields.io/badge/version-1.0.0-4c9bff)
![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078d4)
![License](https://img.shields.io/badge/license-Free-b7791f)
![Made by TechyGeeksHome](https://img.shields.io/badge/made%20by-TechyGeeksHome-b191f2)

</div>

---

Every PDF tool on the web wants you to upload your documents to their server, then caps what
you can do until you pay. iLovePDF stops you at 25 files. Sejda allows three tasks an hour.
Smallpdf wants a subscription. Acrobat wants £20 a month.

PdfGeek does the same jobs on your own machine. Nothing is uploaded, nothing is counted, and
there is no paid tier to upgrade to.

## What it does

| Tool | What it does |
|---|---|
| **Merge** | Combine any number of PDFs into one, in the order you set |
| **Split** | One file per page, or fixed-size chunks |
| **Extract & remove** | Pull out selected pages, or drop them and keep the rest |
| **Rotate & reorder** | Fix sideways scans, or rebuild the document in a new order |
| **Watermark** | Stamp text across every page, with adjustable size, opacity and angle |
| **Password** | Encrypt with AES-128, or remove protection from a file you can already open |

Page ranges work the way you already expect from a print dialog: `1-3, 5, 9-` or just `all`.

## Install

Download `PdfGeek.exe` from [Releases](../../releases) and run it. That is the whole install.

It is a single portable executable — no installer, no admin rights, nothing written to the
registry. Put it on a USB stick if you like. To uninstall, delete the file.

> **Windows will warn you the first time.** PdfGeek is not code-signed, because a certificate
> costs money we would rather not put behind a free tool. Click **More info → Run anyway**, or
> install it with `winget install TechyGeeksHome.PdfGeek` to skip the prompt. The source is
> here so you can check what it does, and every release is built from it.

## Your files stay yours

- No network code. PdfGeek never opens a socket.
- No telemetry, no analytics, no crash reporting, no account.
- Input files are never modified — every operation writes a new file where you choose.

## Building it yourself

Needs the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/techygeekshome/PdfGeek.git
cd PdfGeek

# run it
dotnet run --project src/PdfGeek

# run the smoke tests (24 checks against real PDFs)
dotnet run --project tests/PdfGeek.Smoke -c Release

# produce the portable single-file build
dotnet publish src/PdfGeek -c Release -r win-x64 -o publish
```

## How it is put together

```
src/PdfGeek/
  Services/PdfOps.cs      every PDF operation, no UI dependencies
  Services/PageRange.cs   the "1-3, 5, 9-" parser
  Services/FontSetup.cs   font resolver for the watermark tool
  Views/MainWindow.axaml  the entire interface
tests/PdfGeek.Smoke/      console harness that runs every operation on real files
```

`PdfOps` is deliberately free of UI code, so the smoke test compiles the exact same source that
ships rather than a copy of it.

Built with [Avalonia](https://avaloniaui.net/) (MIT) and
[PDFsharp](https://github.com/empira/PDFsharp) (MIT).

## Licence

Free to use, for anything, including at work. If it saved you a subscription, a
[donation](https://techygeekshome.info) is welcome but never expected.

---

<div align="center">
Made by <a href="https://techygeekshome.info">TechyGeeksHome</a>
</div>
