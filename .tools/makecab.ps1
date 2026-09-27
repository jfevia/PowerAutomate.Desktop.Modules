param(
    [ValidateScript({Test-Path $_ -PathType Container})]
	[string]
	$SourceDirectory,
	
	[ValidateScript({Test-Path $_ -PathType Container})]
    [string]
    $OutputDirectory,

    [string]
    $OutputFileName
)

$ddf = ".OPTION EXPLICIT
.Set CabinetName1=$OutputFileName
.Set DiskDirectory1=$OutputDirectory
.Set CompressionType=LZX
.Set Cabinet=on
.Set Compress=on
.Set CabinetFileCountThreshold=0
.Set FolderFileCountThreshold=0
.Set FolderSizeThreshold=0
.Set MaxCabinetSize=0
.Set MaxDiskFileCount=0
.Set MaxDiskSize=0
"
$ddfpath = ($env:TEMP + "\" + [System.IO.Path]::GetFileNameWithoutExtension($OutputFileName) + ".ddf")
$sourceDirLength = $SourceDirectory.Length;
$moduleFiles = @(Get-ChildItem $SourceDirectory -Filter "*.dll" -File | Where-Object { $_.Name -ne "Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.dll" })
$runtimeDirectory = Join-Path $SourceDirectory "runtimes"
$runtimeFiles = @()
if (Test-Path $runtimeDirectory -PathType Container) {
    $runtimeFiles = @(Get-ChildItem $runtimeDirectory -Filter "*.dll" -File -Recurse)
}
$ddf += (($moduleFiles + $runtimeFiles) | Select-Object -ExpandProperty FullName | ForEach-Object { '"' + $_ + '" "' + ($_.Substring($sourceDirLength)) + '"' }) -join "`r`n"
$ddf | Out-File -Encoding UTF8 $ddfpath
makecab.exe /F $ddfpath
Remove-Item $ddfpath
Remove-Item "setup.inf"
Remove-Item "setup.rpt"