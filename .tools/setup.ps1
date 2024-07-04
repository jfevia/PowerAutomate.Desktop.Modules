$certAuthority = $env:COMPUTERNAME
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=$certAuthority" -CertStoreLocation "Cert:\LocalMachine\My"
$certPasswordStr = -join ((0..9) + (65..90) + (97..122) | Get-Random -Count 16 | ForEach-Object {[char]$_})
$certPassword = ConvertTo-SecureString -String $certPasswordStr -Force -AsPlainText
Export-PfxCertificate -Cert "Cert:\LocalMachine\My\$($cert.Thumbprint)" -FilePath "certificate.pfx" -Password $certPassword
Import-PfxCertificate -CertStoreLocation "Cert:\LocalMachine\Root" -FilePath "certificate.pfx" -Password $certPassword
Remove-Item "certificate.pfx"
dotnet tool install --global sign --version 0.9.1-beta.24325.5