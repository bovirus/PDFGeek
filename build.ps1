# One-command release build for PDFGeek.
#
#   .\build.ps1                       run tests, publish, compile the installer
#   .\build.ps1 -SkipSetup            publish only
#   .\build.ps1 -Iscc "C:\path\ISCC.exe"   use a specific Inno Setup compiler
#
# Needs the .NET 8 SDK. The installer step needs Inno Setup 6 (free):
#   winget install JRSoftware.InnoSetup

param(
    [switch]$SkipSetup,
    [string]$Iscc
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

# ---------------------------------------------------------------- Inno Setup discovery
# Inno Setup does NOT add itself to PATH, and depending on how it was installed it can land in
# Program Files, Program Files (x86), or a per-user Programs folder. So: registry first, then
# the usual suspects, then give up with something actionable.
function Find-InnoSetup {
    param([string]$Explicit)

    if ($Explicit) {
        if (Test-Path $Explicit) { return $Explicit }
        throw "ISCC.exe not found at the path you passed: $Explicit"
    }

    $onPath = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }

    # Version-agnostic on purpose: this originally hardcoded "Inno Setup 6" and promptly failed
    # on a machine with Inno Setup 7. Match any version and prefer the newest.
    $uninstallRoots = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
    )
    $fromRegistry =
        foreach ($uninstallRoot in $uninstallRoots) {
            Get-ChildItem $uninstallRoot -ErrorAction SilentlyContinue |
                Where-Object { $_.PSChildName -like "Inno Setup *_is1" } |
                ForEach-Object {
                    $location = (Get-ItemProperty $_.PSPath -Name InstallLocation -ErrorAction SilentlyContinue).InstallLocation
                    if ($location) { Join-Path $location "ISCC.exe" }
                }
        }
    $hit = $fromRegistry | Where-Object { $_ -and (Test-Path $_) } | Sort-Object -Descending | Select-Object -First 1
    if ($hit) { return $hit }

    $searchRoots = @(
        $env:ProgramFiles,
        ${env:ProgramFiles(x86)},
        (Join-Path $env:LOCALAPPDATA "Programs")
    ) | Where-Object { $_ -and (Test-Path $_) }

    $fromFolders =
        foreach ($searchRoot in $searchRoots) {
            Get-ChildItem $searchRoot -Directory -Filter "Inno Setup *" -ErrorAction SilentlyContinue |
                ForEach-Object { Join-Path $_.FullName "ISCC.exe" }
        }
    # Sorting descending puts "Inno Setup 7" ahead of "Inno Setup 6".
    $hit = $fromFolders | Where-Object { Test-Path $_ } | Sort-Object -Descending | Select-Object -First 1
    if ($hit) { return $hit }

    # Last resort: a bounded search of the likely roots.
    foreach ($searchRoot in $searchRoots) {
        $found = Get-ChildItem -Path $searchRoot -Filter "ISCC.exe" -Recurse -Depth 3 -ErrorAction SilentlyContinue |
                 Select-Object -First 1
        if ($found) { return $found.FullName }
    }

    return $null
}

# ---------------------------------------------------------------- build
Write-Host "Running tests..." -ForegroundColor Cyan
dotnet run --project "$root\tests\PDFGeek.Smoke" -c Release
if ($LASTEXITCODE -ne 0) { throw "Tests failed - not building a release." }

Write-Host "`nPublishing portable win-x64 build..." -ForegroundColor Cyan
dotnet publish "$root\src\PDFGeek\PDFGeek.csproj" -c Release -r win-x64 -o "$root\publish"
if ($LASTEXITCODE -ne 0) { throw "Publish failed." }

New-Item -ItemType Directory -Force -Path "$root\dist" | Out-Null
Copy-Item "$root\publish\PDFGeek.exe" "$root\dist\PDFGeek.exe" -Force

if (-not $SkipSetup) {
    $compiler = Find-InnoSetup -Explicit $Iscc

    if ($compiler) {
        Write-Host "`nCompiling installer with $compiler" -ForegroundColor Cyan
        & $compiler "$root\installer\PDFGeek.iss"
        if ($LASTEXITCODE -ne 0) { throw "Installer build failed." }
    } else {
        Write-Warning @"
Inno Setup was not found. Looked on PATH, in the uninstall registry keys, and for any
"Inno Setup *" folder under Program Files, Program Files (x86) and %LOCALAPPDATA%\Programs.

Install it with:  winget install JRSoftware.InnoSetup
Or, if it is already installed somewhere unusual, find it and pass it in:
  Get-ChildItem C:\ -Filter ISCC.exe -Recurse -ErrorAction SilentlyContinue | Select -First 1 -Expand FullName
  .\build.ps1 -Iscc "<that path>"
"@
    }
}

Write-Host "`nSHA256 checksums for the release notes:" -ForegroundColor Green
Get-ChildItem "$root\dist" -File | ForEach-Object {
    "{0}  {1}" -f (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLower(), $_.Name
}
