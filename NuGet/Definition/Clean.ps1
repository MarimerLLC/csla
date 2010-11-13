param($package) 

if ($package -eq $null)
{
    Write-Host "Clean a NuGet build output."
    Write-Host "============================"
    Write-Host "Usage: Clean.ps1 ""<NuGet Name>"""
    Write-Host "E.g.: Clean.ps1 ""Core"""
    return
}

try 
{
    ## Initialise
    ## ----------
    $configuration = "Release"
    $basePath = Get-Location
    $pathToNuGetLib = [System.IO.Path]::GetFullPath( "$basePath\$package" )
    
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black
    $host.UI.RawUI.ForegroundColor = [System.ConsoleColor]::White

    Write-Host "Clean NuGet build outputs for: $package" -ForegroundColor White
    Write-Host "==============================" -ForegroundColor White

    ## NB - Cleanup destination folders
    ## --------------------------------
    Write-Host "Clean destination folders..." -ForegroundColor Yellow
    Remove-Item "$pathToNuGetLib\Lib\NET4.0\*" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "$pathToNuGetLib\Lib\SL4.0\*" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "$pathToNuGetLib\Lib\WP7.0\*" -Recurse -Force -ErrorAction SilentlyContinue ## According to NuGet team, Profiles aren't supported yet and SL4.0 is to be used for WP7
    Remove-Item "$pathToNuGetLib\*.nupkg" -Recurse -Force -ErrorAction SilentlyContinue

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