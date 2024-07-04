$certAuthority = $env:COMPUTERNAME
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=$certAuthority" -CertStoreLocation "Cert:\CurrentUser\My"
$certPasswordStr = -join ((0..9) + (65..90) + (97..122) | Get-Random -Count 16 | ForEach-Object {[char]$_})
$certPassword = ConvertTo-SecureString -String $certPasswordStr -Force -AsPlainText
Export-PfxCertificate -Cert "Cert:\CurrentUser\My\$($cert.Thumbprint)" -FilePath "certificate.pfx" -Password $certPassword
Import-PfxCertificate -CertStoreLocation "Cert:\CurrentUser\Root" -FilePath "certificate.pfx" -Password $certPassword
dotnet tool install --global sign --version 0.9.1-beta.24325.5