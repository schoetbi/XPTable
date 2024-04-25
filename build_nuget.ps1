# delete bin and obj folder using powershell
Remove-Item -Recurse -Force src/XPTable/bin
Remove-Item -Recurse -Force src/XPTable/obj
dotnet restore
dotnet build XPTable.sln --configuration=release
dotnet pack src/XPTable/XPTable.csproj

