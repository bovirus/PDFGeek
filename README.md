<div align="center">

<img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/icons/pdfgeek.png" alt="PDFGeek logo" width="96" height="96">

# PDFGeek

**A free, self-contained PDF toolkit for Windows — merge, split, rotate, watermark and protect PDFs without uploading a thing.**

[![Version](https://img.shields.io/badge/version-1.0.0-4c9bff)](https://github.com/techygeekshome/PDFGeek/releases)
[![Platform](https://img.shields.io/badge/platform-Windows-0078d4)](#-download--run)
[![License](https://img.shields.io/badge/license-proprietary%20freeware-b7791f)](LICENSE)
[![Made by TechyGeeksHome](https://img.shields.io/badge/made%20by-TechyGeeksHome-b191f2)](https://techygeekshome.info)
[![Support on Ko-fi](https://img.shields.io/badge/support-Ko--fi-ff5e5b)](https://ko-fi.com/techygeekshome)

[Download](#-download--run) · [Features](#-what-it-does) · [Screenshots](#-screenshots) · [Build from source](#-build-from-source) · [License](#-license)

</div>

---

PDFGeek does the everyday PDF jobs on your own machine, with none of the limits the web tools impose. Merge as many files as you like, split a document into pages or fixed-size chunks, pull pages out or drop them, rotate sideways scans, reorder a document, stamp a watermark across it, and add or remove password protection.

The online alternatives cap you and then ask for money — 25 files per merge, three tasks an hour, a subscription to unlock batch mode. PDFGeek has no caps to lift, because your documents never leave your computer in the first place.

No installer bloat, no bundled offers, no telemetry. 100% free, no Pro tier, no upsells.

## 📸 Screenshots

<p float="left">
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-merge.png" width="49%" />
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-split.png" width="49%" />
</p>
<p float="left">
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-extract.png" width="49%" />
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-rotate.png" width="49%" />
</p>
<p float="left">
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-watermark.png" width="49%" />
  <img src="https://raw.githubusercontent.com/techygeekshome/PDFGeek/main/screenshots/screenshot-password.png" width="49%" />
</p>

## ⬇️ Download & run

| What it is | Get it |
| --- | --- |
| Portable Windows app *(self-contained, .NET 8 / Avalonia UI)* | [**Download PDFGeek**](https://techygeekshome.info/pdfgeek/) — free |

A single `.exe`. No installer, no admin rights, nothing written to the registry — run it from anywhere, including a USB stick. To uninstall, delete the file.

> **Windows will warn you the first time you run it.** PDFGeek isn't code-signed, because a certificate costs money we'd rather not put behind a free tool. Click **More info → Run anyway**, or install it with `winget install TechyGeeksHome.PDFGeek` to skip the prompt entirely. The source is right here so you can see exactly what it does.

## ✨ What it does

- 🔗 **Merge** any number of PDFs into one, in the order you set — drag them in, reorder, done.
- ✂️ **Split** a document into one file per page, or into fixed-size chunks.
- 📄 **Extract or remove pages** using print-dialog page ranges (`1-3, 5, 9-`).
- 🔄 **Rotate** sideways scans by 90°, 180° or 270°, on selected pages or the whole document.
- 🔀 **Reorder** a document by listing the pages in the order you want them.
- 💧 **Watermark** every page with your own text, at whatever size, opacity and angle you like.
- 🔐 **Add a password** with AES-128 encryption, with control over printing and copying.
- 🔓 **Remove a password** from a document you can already open.
- 🔒 **Private** — your documents are processed locally and never uploaded, with no telemetry and no account.

### On the one network call

Clicking **Check for updates** makes a single request to GitHub's public releases API to compare version numbers. It sends no identifiers, no file names and no usage data, and it never downloads or installs anything on its own — if there's a newer version it just offers to open the page. Don't press it and PDFGeek makes no network connection at all.

## 🔧 Build from source

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```powershell
dotnet build PDFGeek.sln -c Release
```

To produce the portable, self-contained `win-x64` build:

```powershell
dotnet publish src/PDFGeek/PDFGeek.csproj -c Release -r win-x64 --self-contained true -o publish/win-x64
```

To run the test suite (29 checks against real PDFs):

```powershell
dotnet run --project tests/PDFGeek.Smoke -c Release
```

### Project layout

| Path | What's there |
| --- | --- |
| `src/PDFGeek/Services` | Every PDF operation and the page-range parser (no UI dependencies) |
| `src/PDFGeek/Views` | The Avalonia desktop UI |
| `src/TechyGeeksHome.Common` | Shared About window and update checker, used across all TechyGeeksHome apps |
| `tests/PDFGeek.Smoke` | Console harness that runs every operation against real PDFs |
| `tools/make-icon.py` | Generates the icon set from the brand tokens |
| `icons/` | App icon assets |

## ☕ Support

PDFGeek is free and always will be. If it saved you a subscription, you can [buy us a coffee on Ko-fi](https://ko-fi.com/techygeekshome) — welcome, but never expected.

## 🐛 Support & contributing

Found a bug or have a request? [Open an issue](https://github.com/techygeekshome/PDFGeek/issues) or [get in touch](https://techygeekshome.info/contact/).

## 📄 License

PDFGeek is free to download and use. This is proprietary freeware, not open source — see [LICENSE](LICENSE) for the full terms.

Built with [Avalonia](https://avaloniaui.net/) (MIT) and [PDFsharp](https://github.com/empira/PDFsharp) (MIT).

© 2026 TechyGeeksHome | Andrew Armstrong.

---

<div align="center">

Made with ❤️ by [**TechyGeeksHome**](https://techygeekshome.info)

[Website](https://techygeekshome.info) · [YouTube](https://www.youtube.com/channel/UCtEuFj1SMLiuRoucD1hv8dA) · [X](https://x.com/TechyGeeks1) · [Facebook](https://www.facebook.com/techygeeks.home) · [Instagram](https://www.instagram.com/andrewarmstrongtgh/)

</div>

---
