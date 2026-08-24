$ErrorActionPreference = 'Stop'

# PDFGeek ships an Inno Setup installer that sets up for the current user, so no elevation is
# needed. The package downloads it from the GitHub release for the matching tag and verifies it
# against a SHA-256 checksum rather than embedding the binary. Because nothing is embedded, this
# package must NOT contain a tools\VERIFICATION.txt - that file is only for packages that ship a
# binary inside the nupkg, and including one is what the USP 8.0.0 submission was rejected for.
$packageArgs = @{
  packageName    = 'pdfgeek'
  fileType       = 'exe'
  url            = 'https://github.com/techygeekshome/PDFGeek/releases/download/v1.0.1/PDFGeekSetup.exe'
  checksum       = '8b5e44dddd390686fea1c111804aa629702e7a09a65cde83af465d36098b435d'
  checksumType   = 'sha256'
  silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
  validExitCodes = @(0, 3010, 1641)
}

Install-ChocolateyPackage @packageArgs
