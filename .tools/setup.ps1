$certAuthority = $env:COMPUTERNAME
Write-Host "Got computer name"
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=$certAuthority" -CertStoreLocation "Cert:\LocalMachine\My"
Write-Host "Created self-signed certificate"
$certPasswordStr = -join ((0..9) + (65..90) + (97..122) | Get-Random -Count 16 | ForEach-Object {[char]$_})
$certPassword = ConvertTo-SecureString -String $certPasswordStr -Force -AsPlainText
Write-Host "Created certificate password"
Export-PfxCertificate -Cert "Cert:\LocalMachine\My\$($cert.Thumbprint)" -FilePath "certificate.pfx" -Password $certPassword
Write-Host "Exported certificate"
Import-PfxCertificate -CertStoreLocation "Cert:\LocalMachine\Root" -FilePath "certificate.pfx" -Password $certPassword
Write-Host "Imported certificate"
Remove-Item "certificate.pfx"
Write-Host "Deleted certificate"
dotnet tool install --global sign --version 0.9.1-beta.24325.5
Write-Host "Installed sign CLI"
Get-ExecutionPolicy
Write-Host "Got execution policy"