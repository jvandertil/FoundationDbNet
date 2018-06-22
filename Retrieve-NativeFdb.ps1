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

$nativeFolder = "$PSScriptRoot\native"

$fdbVersion = "5.1.7"
$dllName = "libfdb_c_$fdbVersion.dll"

$url = "https://www.foundationdb.org/downloads/$fdbVersion/windows/$dllName"
$destination = "$PSScriptRoot\native\$dllName"

if( ! (Test-Path $nativeFolder) )
{
    mkdir $nativeFolder
}

Download-File -Url $url -Destination $destination

