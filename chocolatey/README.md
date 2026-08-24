# Chocolatey package — pdfgeek

PDFGeek has no Chocolatey presence yet. This is the package source for it, kept in the
repository so `packageSourceUrl` points somewhere real and a new version is a two-command job.

## What it does

The package does **not** embed the application. `chocolateyinstall.ps1` downloads
`PDFGeekSetup.exe` from the GitHub release for the matching tag, verifies it against a SHA-256
checksum, and runs it silently. `chocolateyuninstall.ps1` finds Inno Setup's own uninstaller
through the uninstall registry key rather than guessing a path.

Because the installer is downloaded rather than embedded, this package must **not** contain a
`tools\VERIFICATION.txt`. That file is only for packages that ship a binary inside the nupkg —
including one is exactly what the Ultimate Settings Panel 8.0.0 submission was rejected for.

PDFGeek is GPL-3.0, so the nuspec uses the modern `<license type="expression">` element rather
than the deprecated `licenseUrl`. DiskGeek and USP are proprietary freeware and have no SPDX
expression available, which is why theirs still use `licenseUrl`.

## First submission

The first push of a brand-new package goes into Chocolatey's moderation queue and is reviewed by
a human, so expect a few days and possibly a round of comments.

```powershell
# from this folder
choco pack
choco push pdfgeek.1.0.1.nupkg --source https://push.chocolatey.org/
```

`choco push` needs the API key from your community.chocolatey.org account
(`choco apikey --key <key> --source https://push.chocolatey.org/`, once per machine).

## Checklist for a new release

1. Cut the GitHub release and note the `PDFGeekSetup.exe` asset URL.
2. Take the hash straight from the release's own `SHA256SUMS.txt` and put it, with the new URL,
   in `tools/chocolateyinstall.ps1`.
3. Bump `<version>` and `<releaseNotes>` in the nuspec.
4. `choco pack`, then install locally from the nupkg and check the uninstall path works too.
5. `choco push`.

The 1.0.1 values currently in place:

| | |
|---|---|
| Asset | `PDFGeekSetup.exe`, 41,427,019 bytes |
| SHA-256 | `8b5e44dddd390686fea1c111804aa629702e7a09a65cde83af465d36098b435d` |
| Installer | Inno Setup 6.7 — silent args `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-` |

The hash above was verified against the release's published `SHA256SUMS.txt` and by re-hashing
the downloaded asset, not taken on trust from one source.
