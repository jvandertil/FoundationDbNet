param(
    [string]$Configuration = "Release",
    [string]$VersionSuffix = "local",
    [string]$OutputDirectory = (Join-Path $PSScriptRoot ".."),
    [string]$TestResultOutputDirectory = (Join-Path $OutputDirectory "TestResults")
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
$rootDir = Join-Path $PSScriptRoot ".."
$testDir = Join-Path $rootDir "test"
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

Function Run-IntegrationTests($FdbServerVersion, $FdbApiVersion)
{
    $env:FdbServerVersion = "v" + ($FdbServerVersion -replace "\.", "_")
    $env:FdbApiVersion = $FdbApiVersion

    $projectDir = Join-Path $testDir "FoundationDbNet.Tests"
    $project = Join-Path $projectDir "FoundationDbNet.Tests.csproj"

    $projectTargets = (Join-Path (Join-Path $projectDir "bin") $Configuration) | ls

    $projectTargets | % { 
        cp "$rootDir\native\libfdb_c_$FdbServerVersion.dll" (Join-Path $_.FullName "libfdb_c.dll") 
    }

    $projectTargets | % {
        Write-Output "Running tests for $_ on FoundationDB server $FdbServerVersion with API level $FdbApiVersion."
        $timestamp = Get-Date -Format yyyyMMdd_HHmmss
        exec { & dotnet test --no-build -f $_ --logger="trx;LogFileName=IntegrationTests_$_`_$FdbServerVersion`_$FdbApiVersion`_$timestamp.trx;TestRunName=TestName" -r $TestResultOutputDirectory --verbosity=normal -c $buildConfiguration $project }
    }
}

Function Run-Tests
{
    Run-IntegrationTests -FdbServerVersion "5.2.5" -FdbApiVersion 520
#    Run-IntegrationTests -FdbServerVersion "6.0.15" -FdbApiVersion 520
    Run-IntegrationTests -FdbServerVersion "6.0.15" -FdbApiVersion 600
}

Function Create-Package
{
    exec { & dotnet pack -o $OutputDirectory --no-build --include-symbols --version-suffix=$VersionSuffix -c $buildConfiguration $solutionPath }
}

Clean-Solution
Restore-Packages
Build-Solution
Run-Tests
Create-Package
