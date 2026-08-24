# Chocolatey package — pdfgeek

**PDFGeek is already on Chocolatey.** Version 1.0.0 was pushed on 17 August 2026 under the
`techygeekshome` maintainer account, and as of 24 August it is **still sitting in the moderation
queue** — the package page carries the "versions of this package awaiting moderation" banner.

What did not exist was the package *source*. The nuspec and scripts lived only on a local
machine, so `packageSourceUrl` had nowhere real to point and a new version meant reconstructing
them from memory. That is what this folder fixes.

## Do not push another version yet

1.0.0 has not cleared moderation. Pushing 1.0.1 on top of it puts two versions of a brand-new
package in the same queue, which slows review down rather than speeding it up, and moderators
are entitled to be annoyed by it.

**Check the moderation comments on the package page first.** A first-time package almost always
comes back with review notes, and an unanswered note is the usual reason one sits for weeks.
DiskGeek's has been queued since 6 August for what looks like exactly that.

Once 1.0.0 is approved, or a moderator asks for changes, this folder is what you edit.

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

## Pushing, when the time comes

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
5. Confirm nothing is still queued, then `choco push`.

The 1.0.1 values currently in place:

| | |
|---|---|
| Asset | `PDFGeekSetup.exe`, 41,427,019 bytes |
| SHA-256 | `8b5e44dddd390686fea1c111804aa629702e7a09a65cde83af465d36098b435d` |
| Installer | Inno Setup 6.7 — silent args `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-` |

The hash above was verified against the release's published `SHA256SUMS.txt` and by re-hashing
the downloaded asset, not taken on trust from one source.
