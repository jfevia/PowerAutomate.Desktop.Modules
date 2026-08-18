# Makes secondary public constructors of action types internal, so Power Automate Desktop's
# "Only one constructor can be defined for an action type" rule is satisfied while keeping the
# dependency-injection seam available to the module's own test assembly.
#
# Only types carrying an [Action], [ConditionAction] or [WaitAction] attribute are touched.
param(
    [switch] $WhatIf
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$changed = @()
$script:constructorsTouched = 0

$files = Get-ChildItem (Join-Path $root 'modules'), (Join-Path $root 'samples') -Recurse -Filter '*.cs' -File |
    Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' }

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    if ($text -notmatch '\[(Action|ConditionAction|WaitAction)\b') {
        continue
    }

    # Pair every class declaration with the text preceding it, so its attributes can be inspected.
    $classMatches = [regex]::Matches($text, '(?m)^\s*(?:public|internal)\s+(?:sealed\s+|abstract\s+|partial\s+)*class\s+(?<name>\w+)')
    $actionClasses = @()
    for ($i = 0; $i -lt $classMatches.Count; $i++) {
        $start = if ($i -eq 0) { 0 } else { $classMatches[$i - 1].Index + $classMatches[$i - 1].Length }
        $preamble = $text.Substring($start, $classMatches[$i].Index - $start)
        if ($preamble -match '\[(Action|ConditionAction|WaitAction)\b') {
            $actionClasses += $classMatches[$i].Groups['name'].Value
        }
    }

    if ($actionClasses.Count -eq 0) {
        continue
    }

    $updated = $text
    foreach ($name in ($actionClasses | Sort-Object -Unique)) {
        # The parameterless constructor stays public; it is the one the engine calls.
        $pattern = "(?m)^(?<indent>[ \t]*)public(?<gap>\s+)(?<name>$([regex]::Escape($name)))\((?<args>[^)]+)\)"
        $updated = [regex]::Replace($updated, $pattern, {
            param($m)
            $script:constructorsTouched++
            "$($m.Groups['indent'].Value)internal$($m.Groups['gap'].Value)$($m.Groups['name'].Value)($($m.Groups['args'].Value))"
        })
    }

    if ($updated -ne $text) {
        $changed += $file.FullName.Substring($root.Length + 1)
        if (-not $WhatIf) {
            $utf8Bom = New-Object System.Text.UTF8Encoding $true
            [System.IO.File]::WriteAllText($file.FullName, $updated, $utf8Bom)
        }
    }
}

"constructors changed: $($script:constructorsTouched)"
"files changed       : $($changed.Count)"
$changed | Sort-Object
