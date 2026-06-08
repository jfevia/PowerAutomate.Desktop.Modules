param(
    [string]
    $NuGetPackageRoot = "$env:USERPROFILE\.nuget\packages"
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$module = Join-Path $root "modules\Modules.GitHub.Actions"
$t4 = Join-Path $env:USERPROFILE ".dotnet\tools\t4.exe"

if (-not (Test-Path $t4)) {
    throw "dotnet-t4 is not installed. Run: dotnet tool install -g dotnet-t4"
}

$openapi = Join-Path $module "openapi.json"
if (-not (Test-Path $openapi)) {
    throw "$openapi is missing. Run .\.tools\update-github-openapi.ps1 first."
}

$cache = (Resolve-Path $NuGetPackageRoot).Path

$templates = Get-ChildItem -Path $module -Filter "*.tt"
foreach ($template in $templates) {
    $tempFile = Join-Path $module ($template.BaseName + ".regen.tt")
    try {
        $content = Get-Content -Path $template.FullName -Raw
        $content = $content.Replace('%NUGET_PACKAGES%', $cache.TrimEnd('\'))
        Set-Content -Path $tempFile -Value $content -NoNewline

        $extension = '.cs'
        if ($template.BaseName -eq 'Resources') {
            $extension = '.resx.generated'
        }
        $output = Join-Path $module ($template.BaseName + $extension)

        Write-Host "Generating $($template.Name) -> $($template.BaseName)$extension"
        & $t4 -o $output $tempFile
        if ($LASTEXITCODE -ne 0) {
            throw "t4 failed for $($template.Name) with exit code $LASTEXITCODE."
        }
    }
    finally {
        Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
    }
}
