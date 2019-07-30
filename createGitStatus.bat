git log --pretty=oneline -n1 > %1
git status >> %1

git rev-parse --short HEAD > githash.txt
set /p HASH=<githash.txt
echo using System.Reflection; > %2
echo [assembly: AssemblyConfiguration("git:%HASH%")] >> %2