param($package) 

if ($package -eq $null)
{
    Write-Host "Clean a NuPack build output."
    Write-Host "============================"
    Write-Host "Usage: Clean.ps1 ""<NuPack Name>"""
    Write-Host "E.g.: Clean.ps1 ""Core"""
    return
}

try 
{
    ## Initialise
    ## ----------
    $configuration = "Release"
    $basePath = Get-Location
    $pathToNuPackLib = [System.IO.Path]::GetFullPath( "$basePath\$package" )
    
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black
    $host.UI.RawUI.ForegroundColor = [System.ConsoleColor]::White

    Write-Host "Clean NuPack build outputs for: $package" -ForegroundColor White
    Write-Host "===============================" -ForegroundColor White

    ## NB - Cleanup destination folders
    ## --------------------------------
    Write-Host "Clean destination folders..." -ForegroundColor Yellow
    Remove-Item "$pathToNuPackLib\Lib\NET4.0\*" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "$pathToNuPackLib\Lib\SL4.0\*" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "$pathToNuPackLib\Lib\WP7.0\*" -Recurse -Force -ErrorAction SilentlyContinue ## According to NuPack team, Profiles aren't supported yet and SL4.0 is to be used for WP7
    Remove-Item "$pathToNuPackLib\*.nupkg" -Recurse -Force -ErrorAction SilentlyContinue

    Write-Host "Clean operation done." -ForegroundColor Green
}
catch 
{
    Write-Host "An error ocurred" -ForegroundColor Red
    Write-Host "================" -ForegroundColor Red
    Write-Host "Details: $_.ToString()"
    Pause
} 
finally 
{
    $host.UI.RawUI.BackgroundColor = $originalBackground
    $host.UI.RawUI.ForegroundColor = $originalForeground
}
#Pause # For debugging purposes