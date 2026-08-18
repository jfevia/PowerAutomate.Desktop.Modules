# Builds, packs, uploads, downloads, extracts and validates a custom module end to end, so a
# designer paste is never the first place a module contract break is discovered.
#
# The download and extract steps prove that what Dataverse serves is what the designer will load.
[CmdletBinding()]
param(
    [string] $Project = 'modules\Modules.OpenTibia.Actions\Modules.OpenTibia.Actions.csproj',
    [string] $Configuration = 'Release',
    [string] $Organization = 'https://orge1515288.crm4.dynamics.com',
    [string] $DesktopFlowModuleId,
    [switch] $SkipUpload
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Step { param([string] $Text) Write-Host "==> $Text" -ForegroundColor Cyan }

Step "Building and packing $Project"
& dotnet build $Project -c $Configuration -p:PackCustomModule=true --nologo -v minimal | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE." }

$projectDirectory = Split-Path -Parent (Join-Path $root $Project)
$cab = Get-ChildItem (Join-Path $projectDirectory "bin\$Configuration") -Filter '*.cab' | Select-Object -First 1
if (-not $cab) { throw "No .cab was produced under bin\$Configuration." }

$signature = Get-AuthenticodeSignature $cab.FullName
Write-Host "    cab       : $($cab.Name) $($cab.Length) bytes, signature $($signature.Status)"
if ($signature.Status -ne 'Valid') { throw "The cab is not validly signed: $($signature.Status)." }

Step 'Validating the freshly built assembly'
$validator = Join-Path $root 'tools\PowerAutomate.Desktop.ModuleValidator\bin\Release\PowerAutomate.Desktop.ModuleValidator.exe'
if (-not (Test-Path $validator)) {
    & dotnet build (Join-Path $root 'tools\PowerAutomate.Desktop.ModuleValidator\PowerAutomate.Desktop.ModuleValidator.csproj') -c Release --nologo -v quiet | Out-Null
}

$assembly = Get-ChildItem (Join-Path $projectDirectory "bin\$Configuration") -Filter 'PowerAutomate.Desktop.Modules.*.Actions.dll' | Select-Object -First 1
& $validator --assembly $assembly.FullName
if ($LASTEXITCODE -ne 0) { throw 'The module does not load cleanly; the designer would reject it.' }

if ($SkipUpload) {
    Step 'Skipping upload as requested'
    return
}

if (-not $DesktopFlowModuleId) { throw 'DesktopFlowModuleId is required unless -SkipUpload is passed.' }

Step 'Uploading to Dataverse'
$token = & az account get-access-token --resource $Organization --query accessToken -o tsv
if ($LASTEXITCODE -ne 0) { throw 'Could not acquire a Dataverse token. Run az login first.' }

$headers = @{ Authorization = "Bearer $token"; 'x-ms-file-name' = $cab.Name }
$response = Invoke-WebRequest -Uri "$Organization/api/data/v9.2/desktopflowmodules($DesktopFlowModuleId)/data" `
    -Method Patch -Headers $headers -ContentType 'application/octet-stream' -InFile $cab.FullName -TimeoutSec 180
Write-Host "    upload    : HTTP $($response.StatusCode)"

Step 'Downloading it back'
$downloaded = Join-Path $env:TEMP 'module-roundtrip.cab'
Invoke-WebRequest -Uri "$Organization/api/data/v9.2/desktopflowmodules($DesktopFlowModuleId)/data/`$value" `
    -Headers @{ Authorization = "Bearer $token" } -OutFile $downloaded -TimeoutSec 180

$localHash = (Get-FileHash $cab.FullName -Algorithm SHA256).Hash
$remoteHash = (Get-FileHash $downloaded -Algorithm SHA256).Hash
Write-Host "    round trip: $((Get-Item $downloaded).Length) bytes, hashes $(if ($localHash -eq $remoteHash) { 'match' } else { 'DIFFER' })"
if ($localHash -ne $remoteHash) { throw 'What Dataverse served back does not match what was uploaded.' }

Step 'Extracting the downloaded cab'
$extracted = Join-Path $env:TEMP 'module-roundtrip'
if (Test-Path $extracted) { Remove-Item $extracted -Recurse -Force }
New-Item -ItemType Directory -Path $extracted | Out-Null
& expand.exe $downloaded -F:* $extracted | Out-Null
if ($LASTEXITCODE -ne 0) { throw "expand.exe failed with exit code $LASTEXITCODE." }
Get-ChildItem $extracted -File | ForEach-Object { Write-Host "    $($_.Name) $($_.Length)" }

Step 'Validating what Dataverse actually served'
$served = Get-ChildItem $extracted -Filter 'PowerAutomate.Desktop.Modules.*.Actions.dll' | Select-Object -First 1
if (-not $served) { throw 'The extracted cab contains no module assembly.' }

& $validator --assembly $served.FullName
if ($LASTEXITCODE -ne 0) { throw 'The uploaded module does not load cleanly; the designer would reject it.' }

Write-Host 'ROUND TRIP OK' -ForegroundColor Green
