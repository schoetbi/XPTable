## rewrite this in powerhshell
# git log --pretty=oneline -n1 > gitstatus.txt
# git status >> gitstatus.txt
# 
# git rev-parse --short HEAD > githash.txt
# set /p HASH=<githash.txt
# echo using System.Reflection; > AssemblyInfo.githash.cs
# echo [assembly: AssemblyInformationalVersion("git:%HASH%")] >> AssemblyInfo.githash.cs
# exit 0

$gitlog = git log --pretty=oneline -n1
$gitstatus = git status
$githash = git rev-parse --short HEAD

$gitlog = git log --pretty=oneline -n1
$gitstatus = git status
$githash = git rev-parse --short HEAD

$assemblyInfoContent = @"
using System.Reflection;

[assembly: AssemblyInformationalVersion("git:$githash")]
"@

$assemblyInfoContent | Out-File AssemblyInfo.githash.cs
