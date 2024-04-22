# delete bin and obj folder using powershell
Remove-Item -Recurse -Force bin
Remove-Item -Recurse -Force obj

powershell -ExecutionPolicy Bypass -File createGitStatus.ps1
dotnet build .\XPTable.csproj --configuration=release
dotnet gitversion /output file /outputfile gitversion.json

# get version from gitversion.json
$version = (Get-Content -Raw gitversion.json | ConvertFrom-Json).LegacySemVer
nuget pack .\XPTable.nuspec -OutputDirectory .\nupkg -Version $version

