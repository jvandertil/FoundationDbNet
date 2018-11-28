param (
    [switch]$ForceBITS = $false
)

# Unfortunately required to enable TLS 1.2. Should be auto negotiated...
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Download-File($Url, $Destination) 
{
    $bitsService = Get-Service BITS
    
    if ( ! ($bitsService.Status -like 'Running'  ) )
    {
        Write-Host "BITS appears to be disabled, using WebClient"
    
        try
        {
            $wc = New-Object System.Net.WebClient
            $wc.DownloadFile($Url, $Destination)
        } finally {
            $wc.Dispose()
        }
    }
    else
    {
        Start-BitsTransfer -Source $Url -Destination $Destination
    }
}

$rootFolder = Join-Path $PSScriptRoot ".."
$nativeFolder = Join-Path $rootFolder "native"

if( ! (Test-Path $nativeFolder) )
{
    mkdir $nativeFolder
}

function Download-NativeLibrary($Version)
{
    $dllName = "libfdb_c_$Version.dll"

    $url = "https://www.foundationdb.org/downloads/$Version/windows/$dllName"
    $destination = Join-Path $nativeFolder $dllName

    Download-File -Url $url -Destination $destination
}

if ( $ForceBITS )
{
    Write-Host "FORCE: Starting BITS service"
    Start-Service BITS
}

Download-NativeLibrary -Version 5.2.5
Download-NativeLibrary -Version 6.0.15
