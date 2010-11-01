##
##	Create Modular CSLA NuPack
##  ==========================
##  
##  “CSLA .NET - Core” NuPack
##  •	Contents:
##      o	Csla.dll (.NET)
##      o	Csla.dll (Silverlight 4)
##      o	Csla.dll (WP)
##  •	Dependencies
##      o	(Has no dependencies on other NuPacks)
##  
##  “CSLA .NET - ASP.NET” NuPack
##  •	Contents:
##      o	Csla.Web.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##  
##  “CSLA .NET - ASP.NET MVC” NuPack
##  •	Contents:
##      o	Csla.Web.Mvc.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##  
##  “CSLA .NET - Silverlight” NuPack
##  •	Contents:
##      o	Csla.Xaml.dll (Silverlight 4)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##
##  “CSLA .NET - Windows Forms” NuPack
##  •	Contents:
##      o	Csla.Windows.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##  
##  “CSLA .NET - Windows Phone” NuPack
##  •	Contents:
##      o	Csla.Xaml.dll (WP) – which is the Silverlight Csla.dll right? Or is this different in upcoming CSLA 4.1?
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##
##  “CSLA .NET - WPF” NuPack
##  •	Contents:
##      o	Csla.Xaml.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack
##  
##  “CSLA Workflow” NuPack
##  •	Contents:
##      o	Csla.Workflow.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuPack


function Pause ($Message="Press any key to continue...")
{
    Write-Host -NoNewLine $Message
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Write-Host ""
}

try 
{
    ## Initialise
    ## ----------
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    $originalLocation = Get-Location
    $packages = @("Core", "ASP.NET", "ASP.NET MVC", "Silverlight", "Windows Forms", "Windows Phone", "WPF")  # Leave out "Workflow" until Rocky is happy with it for CSLA 4.x
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black
    $host.UI.RawUI.ForegroundColor = [System.ConsoleColor]::White
    
    Write-Host "Create all CLSA .NET NuPack packages" -ForegroundColor White
    Write-Host "====================================" -ForegroundColor White
    
    ## NB - Cleanup destination folders
    ## --------------------------------
    Write-Host "Clean destination folders..." -ForegroundColor Yellow
    Remove-Item ".\Packages\*.nupkg" -Recurse -Force -ErrorAction SilentlyContinue
    
    ## Spawn off individual create processes...
    ## ---------------------------------------
    Set-Location "$originalLocation\Definition" ## Adjust current working directory since scripts are using relative paths
    $packages | ForEach { & ".\Create $_.ps1" }
    Write-Host "Create all done." -ForegroundColor Green
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
    ## Restore original values
    $host.UI.RawUI.BackgroundColor = $originalBackground
    $host.UI.RawUI.ForegroundColor = $originalForeground
    Set-Location $originalLocation
}
Pause # For debugging purposes