@echo off
IF "%CI_CONFIG%"=="" (SET CI_CONFIG="Release")
IF "%CI_VERSION%"=="" (SET CI_VERSION="local")

del /S /F /Q src\FoundationDbNet\bin
del /S /F /Q src\FoundationDbNet\obj
del /F *.nupkg

dotnet restore --force FoundationDbNet.sln
dotnet build --no-restore -c %CI_CONFIG% --version-suffix %CI_VERSION% FoundationDbNet.sln

REM dotnet test --no-build --verbosity=normal test/FoundationDbNet.Tests/FoundationDbNet.Tests.csproj

REM -o ..\..\ needed due to bug in dotnet tooling.
dotnet pack -o ..\..\ --no-build --include-symbols -c %CI_CONFIG% --version-suffix %CI_VERSION% src/FoundationDbNet/FoundationDbNet.csproj