mkdir $PSScriptRoot\native
Start-BitsTransfer -Source "https://www.foundationdb.org/downloads/5.1.7/windows/libfdb_c_5.1.7.dll" -Destination $PSScriptRoot\native\libfdb_c_5.1.7.dll