param(
    [string]
    $Filter
)

$certAuthority = $env:COMPUTERNAME
$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" | Where-Object { $_.Issuer -like "*$($certAuthority)*" }
$certFingerprint = $cert.GetCertHashString("SHA256")
sign code certificate-store $Filter --certificate-fingerprint $certFingerprint