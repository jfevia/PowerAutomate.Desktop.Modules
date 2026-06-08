$root = Split-Path -Parent $PSScriptRoot
$dest = Join-Path $root "modules\Modules.GitHub.Actions\openapi.json"
$src = "https://raw.githubusercontent.com/github/rest-api-description/main/descriptions/api.github.com/api.github.com.json"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri $src -OutFile $dest -UseBasicParsing
