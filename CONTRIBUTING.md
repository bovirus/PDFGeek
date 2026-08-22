# Contributing to PDFGeek

Contributions are welcome — bug reports, fixes, translations and installer improvements especially.

## Licensing of contributions

PDFGeek is licensed under the **GNU General Public License v3.0** (see [LICENSE](LICENSE)). It was previously released as proprietary freeware, and was relicensed to GPL-3.0 in August 2026 so that outside contributions could be accepted cleanly.

**Inbound equals outbound.** By opening a pull request you agree that your contribution is licensed under GPL-3.0, the same terms as the rest of the project. You keep the copyright in what you wrote — you are granting the same licence everyone else already has.

There is no Contributor Licence Agreement to sign. That is deliberate: a CLA is what a *proprietary* project needs, because it has to collect rights it would not otherwise have. A project under a copyleft licence does not have that problem.

**Please do not modify [LICENSE](LICENSE) in a pull request.** The GPL text is verbatim and must stay that way; a change there will be rejected regardless of intent, and it makes the rest of the diff harder to review.

The PDFGeek name, logo and TechyGeeksHome branding are not covered by the GPL and remain ours. Fork the code freely — please do not ship a fork under the same name.

## What is most useful

- **Translations.** The installer supports multiple languages. Each language belongs in its own file under `installer/` rather than inside one large script. Translations of the application UI are welcome too.
- **Installer improvements.** `installer/` holds the Inno Setup script. Changes here need testing on a clean machine, including the uninstall path.
- **PDF operation bugs.** `src/PDFGeek/Services` holds every PDF operation with no UI dependency, which is what makes them testable.
- **Page-range parsing.** The `1-3, 5, 9-` parser is easy to break. If you touch it, add cases to the smoke tests.

## Ground rules

- The build must stay clean.
- Services stay free of UI dependencies. If a fix needs a dialog, the dialog goes in `Views`.
- Never write over the user's input file. Every operation writes to a new file.
- Nothing may phone home. The only network call in PDFGeek is the explicit **Check for updates** button, and it must stay that way.
- Explain *why* in comments, not *what*. The code already says what it does.

## Building

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```powershell
dotnet build PDFGeek.sln -c Release
dotnet run --project tests/PDFGeek.Smoke -c Release
```

Please run the smoke tests before opening a pull request — 29 checks against real PDFs, and they take seconds.

---

Questions: [open an issue](https://github.com/techygeekshome/PDFGeek/issues) or reach us through [techygeekshome.info](https://techygeekshome.info/contact/).
