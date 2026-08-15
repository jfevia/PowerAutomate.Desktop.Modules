param(
    [string]
    $Configuration = 'Release',

    [string]
    $ResultsDirectory = "$PSScriptRoot\..\artifacts\coverage",

    [double]
    $LineThreshold = 100,

    [double]
    $BranchThreshold = 100,

    [switch]
    $SkipTests
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path "$PSScriptRoot\.."
$solution = Join-Path $repositoryRoot 'PowerAutomate.Desktop.slnx'
$runSettings = Join-Path $repositoryRoot 'tests\coverlet.runsettings'

if (-not $SkipTests) {
    if (Test-Path $ResultsDirectory) {
        Remove-Item $ResultsDirectory -Recurse -Force
    }

    dotnet test $solution `
        --configuration $Configuration `
        --nologo `
        --collect:"XPlat Code Coverage" `
        --settings $runSettings `
        --results-directory $ResultsDirectory

    if ($LASTEXITCODE -ne 0) {
        throw "Unit tests failed with exit code $LASTEXITCODE."
    }
}

$reports = @(Get-ChildItem $ResultsDirectory -Recurse -Filter 'coverage.cobertura.xml' -ErrorAction SilentlyContinue)
if ($reports.Count -eq 0) {
    throw "No coverage reports were produced under '$ResultsDirectory'. Every test project must reference coverlet.collector."
}

# The gate is only meaningful if every assembly it claims to watch actually reported.
[xml]$settings = Get-Content $runSettings
$expectedModules = @()
$includeFilter = $settings.RunSettings.DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include
if ($includeFilter) {
    $expectedModules = @([regex]::Matches($includeFilter, '\[([^\]]+)\]') | ForEach-Object { $_.Groups[1].Value })
}

$modules = @{}
$gaps = New-Object System.Collections.Generic.List[string]

foreach ($report in $reports) {
    [xml]$document = Get-Content $report.FullName

    foreach ($package in $document.coverage.packages.package) {
        $name = $package.name
        if (-not $modules.ContainsKey($name)) {
            $modules[$name] = [pscustomobject]@{
                Name            = $name
                CoveredLines    = 0
                TotalLines      = 0
                CoveredBranches = 0
                TotalBranches   = 0
            }
        }

        # Cobertura repeats each line under the method as well, so only class level lines are counted.
        $lines = @(Select-Xml -Xml $package -XPath './classes/class/lines/line')
        $covered = @($lines | Where-Object { [int]$_.Node.hits -gt 0 }).Count
        $entry = $modules[$name]

        # A module can appear in several reports; the one with the most hits wins.
        if ($covered -ge $entry.CoveredLines) {
            $entry.CoveredLines = $covered
            $entry.TotalLines = $lines.Count

            $branchLines = @($lines | Where-Object { $_.Node.'condition-coverage' })
            $totalBranches = 0
            $coveredBranches = 0
            foreach ($line in $branchLines) {
                if ($line.Node.'condition-coverage' -match '\((\d+)/(\d+)\)') {
                    $coveredBranches += [int]$Matches[1]
                    $totalBranches += [int]$Matches[2]
                }
            }

            $entry.CoveredBranches = $coveredBranches
            $entry.TotalBranches = $totalBranches
        }
    }
}

function Get-Percentage([int]$covered, [int]$total) {
    if ($total -eq 0) { return 100 }
    return [math]::Round(100 * $covered / $total, 2)
}

$totalLines = 0
$totalCoveredLines = 0
$totalBranches = 0
$totalCoveredBranches = 0

Write-Host ''
Write-Host ('{0,-62} {1,18} {2,18}' -f 'Module', 'Line', 'Branch')
Write-Host ('-' * 100)

foreach ($module in ($modules.Values | Sort-Object Name)) {
    $linePercentage = Get-Percentage $module.CoveredLines $module.TotalLines
    $branchPercentage = Get-Percentage $module.CoveredBranches $module.TotalBranches

    Write-Host ('{0,-62} {1,10} {2,6}% {3,10} {4,6}%' -f
        $module.Name,
        "$($module.CoveredLines)/$($module.TotalLines)", $linePercentage,
        "$($module.CoveredBranches)/$($module.TotalBranches)", $branchPercentage)

    if ($linePercentage -lt $LineThreshold) {
        $gaps.Add("$($module.Name): line coverage $linePercentage% is below the required $LineThreshold%.")
    }

    if ($branchPercentage -lt $BranchThreshold) {
        $gaps.Add("$($module.Name): branch coverage $branchPercentage% is below the required $BranchThreshold%.")
    }

    $totalLines += $module.TotalLines
    $totalCoveredLines += $module.CoveredLines
    $totalBranches += $module.TotalBranches
    $totalCoveredBranches += $module.CoveredBranches
}

Write-Host ('-' * 100)
Write-Host ('{0,-62} {1,10} {2,6}% {3,10} {4,6}%' -f
    'TOTAL',
    "$totalCoveredLines/$totalLines", (Get-Percentage $totalCoveredLines $totalLines),
    "$totalCoveredBranches/$totalBranches", (Get-Percentage $totalCoveredBranches $totalBranches))
Write-Host ''

foreach ($expected in $expectedModules) {
    if (-not $modules.ContainsKey($expected)) {
        $gaps.Add("$expected is watched by the coverage filter but produced no report; its tests may have stopped running.")
    }
}

if ($gaps.Count -gt 0) {
    foreach ($gap in $gaps) {
        Write-Host "##[error]$gap"
    }

    throw "Coverage is below the required threshold."
}

Write-Host "Coverage meets the required $LineThreshold% line and $BranchThreshold% branch threshold."
