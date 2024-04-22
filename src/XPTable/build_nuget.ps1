# delete bin and obj folder using powershell
Remove-Item -Recurse -Force bin
Remove-Item -Recurse -Force obj

powershell -ExecutionPolicy Bypass -File createGitStatus.ps1
dotnet build ..\..\XPTable.sln --configuration=release
dotnet gitversion /output file /outputfile gitversion.json

# get version from gitversion.json
$gitversion = Get-Content -Raw gitversion.json | ConvertFrom-Json
$version = $gitversion.NuGetVersion
nuget pack .\XPTable.nuspec -OutputDirectory .\nupkg -Version $version

