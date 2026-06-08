param(
    [string]
    $Filter
)

$certAuthority = $env:COMPUTERNAME

# Use X509Store API directly (Get-ChildItem Cert:\... depends on the Certificate
# provider which Windows PowerShell 5.1 fails to register when launched from
# cmd.exe with PowerShell 7 modules ahead of it in PSModulePath).
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'CurrentUser')
$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
try {
    $now = Get-Date
    $cert = $store.Certificates |
        Where-Object { $_.Issuer -like "*$certAuthority*" -and $_.NotAfter -gt $now } |
        Sort-Object NotAfter -Descending |
        Select-Object -First 1
}
finally {
    $store.Close()
}

if ($null -eq $cert) {
    throw "No non-expired code-signing certificate issued by '$certAuthority' found in Cert:\CurrentUser\My. Run .tools\setup.ps1."
}

$certFingerprint = $cert.GetCertHashString('SHA256')
sign code certificate-store $Filter --certificate-fingerprint $certFingerprint
if ($LASTEXITCODE -ne 0) {
    throw "sign tool exited with code $LASTEXITCODE."
}