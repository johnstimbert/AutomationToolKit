<#
.SYNOPSIS
    Loads credentials from a .env file and restores / tests the
    WebAutomationToolKit.BrowserTests project, which pulls
    WebAutomationToolKit from GitHub Packages.

.DESCRIPTION
    NuGet reads %GITHUB_ACTOR% and %GITHUB_TOKEN% from the OS environment.
    This script sources a local .env file into the current PowerShell
    process before calling dotnet, so the credentials are never stored
    inside nuget.config or committed to source control.

.PARAMETER EnvFile
    Path to the .env file.  Defaults to .env in the repo root.

.PARAMETER Target
    restore  – only restore packages (default)
    build    – restore then build
    test     – restore, build, and run tests

.EXAMPLE
    # From the repo root:
    .\scripts\Restore-BrowserTests.ps1
    .\scripts\Restore-BrowserTests.ps1 -Target test
    .\scripts\Restore-BrowserTests.ps1 -EnvFile C:\secrets\my.env -Target build
#>
[CmdletBinding()]
param(
    [string] $EnvFile = (Join-Path $PSScriptRoot ".." ".env"),
    [ValidateSet("restore", "build", "test")]
    [string] $Target  = "restore"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ── 1. Resolve the .env file ────────────────────────────────────────────────
$envPath = Resolve-Path -LiteralPath $EnvFile -ErrorAction SilentlyContinue

if (-not $envPath) {
    Write-Error @"
.env file not found at '$EnvFile'.

Copy .env.template to .env and fill in your GitHub credentials:
    GITHUB_ACTOR=your-github-username
    GITHUB_TOKEN=ghp_...
"@
    exit 1
}

# ── 2. Parse and inject variables into this process only ───────────────────
Write-Host "Loading credentials from $envPath" -ForegroundColor Cyan

Get-Content $envPath |
    Where-Object { $_ -match '^\s*[^#\s]' } |  # skip blanks / comments
    ForEach-Object {
        if ($_ -match '^\s*(?<key>[^=]+?)\s*=\s*(?<value>.+?)\s*$') {
            $key   = $Matches.key
            $value = $Matches.value
            # Set in the current process environment so dotnet inherits it.
            [System.Environment]::SetEnvironmentVariable($key, $value, "Process")
            Write-Verbose "  Set $key"
        }
    }

# ── 3. Confirm the required variables are present ──────────────────────────
foreach ($required in @("GITHUB_ACTOR", "GITHUB_TOKEN")) {
    if (-not [System.Environment]::GetEnvironmentVariable($required, "Process")) {
        Write-Error "'$required' was not found in $envPath. Add it and try again."
        exit 1
    }
}

# ── 4. Run the requested dotnet command ────────────────────────────────────
$project = Join-Path $PSScriptRoot ".." `
    "tests" "WebAutomationToolKit.BrowserTests" `
    "WebAutomationToolKit.BrowserTests.csproj"

switch ($Target) {
    "restore" {
        Write-Host "Restoring $project ..." -ForegroundColor Cyan
        dotnet restore $project
    }
    "build" {
        Write-Host "Building $project ..." -ForegroundColor Cyan
        dotnet restore $project
        dotnet build   $project --no-restore
    }
    "test" {
        Write-Host "Testing $project ..." -ForegroundColor Cyan
        dotnet restore $project
        dotnet build   $project --no-restore
        dotnet test    $project --no-build --verbosity normal
    }
}

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "Done." -ForegroundColor Green
