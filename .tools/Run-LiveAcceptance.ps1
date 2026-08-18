<#
.SYNOPSIS
    Runs the OpenTibia live acceptance harness against the dedicated test server.

.DESCRIPTION
    The harness is restricted to 127.0.0.1 ports 10101-10104. Every other OpenTibia
    server on this machine belongs to other work and must not be contacted.

.EXAMPLE
    .\Run-LiveAcceptance.ps1 -Scenario login
    .\Run-LiveAcceptance.ps1 -Scenario game -Character BotTester
    .\Run-LiveAcceptance.ps1 -Scenario full -Character BotTester
#>
[CmdletBinding()]
param(
    [ValidateSet('login', 'game', 'full', 'soak')]
    [string] $Scenario = 'login',

    [string] $Account = '11',

    [string] $Password = '1',

    [string] $Character = 'Account Manager',

    [int] $ObserveSeconds = 10,

    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path "$PSScriptRoot\.."
$project = Join-Path $repositoryRoot 'tools\OpenTibia.LiveHarness\OpenTibia.LiveHarness.csproj'
$exe = Join-Path $repositoryRoot "tools\OpenTibia.LiveHarness\bin\$Configuration\OpenTibia.LiveHarness.exe"
$itemsOtb = 'C:\Users\jfevi\RiderProjects\tfs-old-svn_padm\data\items\items.otb'

dotnet build $project -c $Configuration --nologo | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Harness build failed." }

# Item classification keeps the map parse aligned; without it stackable items desynchronize it.
$common = @('--account', $Account, '--password', $Password, '--out', (Join-Path $repositoryRoot 'artifacts\live'))
if (Test-Path $itemsOtb) { $common += @('--items-otb', $itemsOtb) }

switch ($Scenario) {
    'login' { $args = $common + @('--phase', 'login') }
    'game'  { $args = $common + @('--phase', 'game', '--character', $Character, '--observe-seconds', $ObserveSeconds) }
    'full'  { $args = $common + @('--phase', 'game', '--character', $Character, '--observe-seconds', $ObserveSeconds, '--say', 'hello', '--turn', '--walk') }
    'soak'  { $args = $common + @('--phase', 'game', '--character', $Character, '--observe-seconds', 300) }
}

Write-Host "> $exe $($args -join ' ')" -ForegroundColor Cyan
& $exe @args
exit $LASTEXITCODE
