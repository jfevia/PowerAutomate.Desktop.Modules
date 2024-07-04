param(

    [ValidateScript({Test-Path $_ -PathType Container})]
	[string]
	$sourceDir,
	
	[ValidateScript({Test-Path $_ -PathType Container})]
    [string]
    $cabOutputDir,

    [string]
    $cabFilename
)

$ddf = ".OPTION EXPLICIT
.Set CabinetName1=$cabFilename
.Set DiskDirectory1=$cabOutputDir
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
$ddfpath = ($env:TEMP + "\customModule.ddf")
$sourceDirLength = $sourceDir.Length;
$ddf += (Get-ChildItem $sourceDir -Filter "*.dll" | Where-Object { (!$_.PSIsContainer) -and ($_.Name -ne "Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.dll") } | Select-Object -ExpandProperty FullName | ForEach-Object { '"' + $_ + '" "' + ($_.Substring($sourceDirLength)) + '"' }) -join "`r`n"
$ddf | Out-File -Encoding UTF8 $ddfpath
makecab.exe /F $ddfpath
Remove-Item $ddfpath
Remove-Item "setup.inf"
Remove-Item "setup.rpt"