param(
    $Configuration = "Release",
    $VersionSuffix = "local"
)

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
Function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

$buildDir = $PSScriptRoot
$rootDir = Resolve-Path "$PSScriptRoot\.."
$testDir = Join-Path $rootDir test
$solutionName = "FoundationDbNet.sln"
$solutionPath = Join-Path $rootDir $solutionName
$buildConfiguration = $Configuration

Function Clean-Solution
{
    exec { & dotnet clean -c $buildConfiguration $solutionPath }
}

Function Restore-Packages
{
    exec { & dotnet restore -f $solutionPath }
}

Function Build-Solution
{
    exec { & dotnet build --no-restore --version-suffix=$VersionSuffix -c $buildConfiguration $solutionPath }
}

Function Run-TestProject($ProjectName)
{
    $project = Join-Path (Join-Path $testDir $ProjectName) "$ProjectName.csproj"

    exec { & dotnet test --no-build --verbosity=normal -c $buildConfiguration $project }
}

Function Run-Tests
{
    Run-TestProject -ProjectName FoundationDbNet.Tests
}

Function Create-Package
{
    exec { & dotnet pack -o $rootDir --no-build --include-symbols --version-suffix=$VersionSuffix -c $buildConfiguration $solutionPath }
}

Clean-Solution
Restore-Packages
Build-Solution
Run-Tests
Create-Package
