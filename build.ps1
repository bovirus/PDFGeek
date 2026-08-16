# One-command release build for PDFGeek.
#
#   .\build.ps1              publish + compile the installer
#   .\build.ps1 -SkipSetup   publish only
#
# Needs the .NET 8 SDK, and Inno Setup 6 (free) on PATH for the installer step.

param([switch]$SkipSetup)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "Running tests..." -ForegroundColor Cyan
dotnet run --project "$root\tests\PDFGeek.Smoke" -c Release
if ($LASTEXITCODE -ne 0) { throw "Tests failed - not building a release." }

Write-Host "Publishing portable win-x64 build..." -ForegroundColor Cyan
dotnet publish "$root\src\PDFGeek\PDFGeek.csproj" -c Release -r win-x64 -o "$root\publish"
if ($LASTEXITCODE -ne 0) { throw "Publish failed." }

New-Item -ItemType Directory -Force -Path "$root\dist" | Out-Null
Copy-Item "$root\publish\PDFGeek.exe" "$root\dist\PDFGeek.exe" -Force

if (-not $SkipSetup) {
    $iscc = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if (-not $iscc) {
        $guess = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
        if (Test-Path $guess) { $iscc = $guess } else { $iscc = $null }
    } else { $iscc = $iscc.Source }

    if ($iscc) {
        Write-Host "Compiling installer..." -ForegroundColor Cyan
        & $iscc "$root\installer\PDFGeek.iss"
        if ($LASTEXITCODE -ne 0) { throw "Installer build failed." }
    } else {
        Write-Warning "Inno Setup not found - skipping installer. Get it from https://jrsoftware.org/isdl.php"
    }
}

Write-Host "`nSHA256 checksums for the release notes:" -ForegroundColor Green
Get-ChildItem "$root\dist" -File | ForEach-Object {
    "{0}  {1}" -f (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLower(), $_.Name
}
