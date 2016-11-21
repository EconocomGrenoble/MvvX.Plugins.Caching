#$location  = "C:\Sources\NoSqlRepositories";
$location  = $env:APPVEYOR_BUILD_FOLDER

$locationNuspec = $location + "\nuspec"
$locationNuspec
	
Set-Location -Path $locationNuspec

"Packaging to nuget..."
"Build folder : " + $location

$strPath = $location + '\MvvX.Plugins.Caching\bin\Release\MvvX.Plugins.Caching.dll'

$VersionInfos = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($strPath)
$ProductVersion = $VersionInfos.ProductVersion
"Product version : " + $ProductVersion

"Update nuspec versions ..."	
$nuSpecFile =  $locationNuspec + '\MvvX.Plugins.Caching.nuspec'
(Get-Content $nuSpecFile) | 
Foreach-Object {$_ -replace "{BuildNumberVersion}", "$ProductVersion" } | 
Set-Content $nuSpecFile

"Generate nuget package ..."
.\NuGet.exe pack MvvX.Plugins.Caching.nuspec

$apiKey = $env:NuGetApiKey
	
#"Publish packages ..."	
.\NuGet push MvvX.Plugins.Caching.$ProductVersion.nupkg -Source https://www.nuget.org/api/v2/package -ApiKey $apiKey
