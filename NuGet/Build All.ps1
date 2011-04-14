##
##	Create Modular CSLA NuGet based on UI Technology (Based on Discussions with Rocky & JH)
##  =======================================================================================
##  
##  “CSLA .NET - Core” NuGet
##  •	Contents:
##      o	Csla.dll (.NET)
##      o	Csla.dll (Silverlight 4)
##      o	Csla.dll (WP)
##  •	Dependencies
##      o	(Has no dependencies on other NuPacks)
##  
##  “CSLA .NET - ASP.NET” NuGet
##  •	Contents:
##      o	Csla.Web.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##  
##  “CSLA .NET - ASP.NET MVC” NuGet
##  •	Contents:
##      o	Csla.Web.Mvc.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##  
##  “CSLA .NET - Silverlight” NuGet
##  •	Contents:
##      o	Csla.Xaml.dll (Silverlight 4)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##
##  “CSLA .NET - Windows Forms” NuGet
##  •	Contents:
##      o	Csla.Windows.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##  
##  “CSLA .NET - Windows Phone” NuGet
##  •	Contents:
##      o	Csla.Xaml.dll (WP) – which is the Silverlight Csla.dll right? Or is this different in upcoming CSLA 4.1?
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##
##  “CSLA .NET - WPF” NuGet
##  •	Contents:
##      o	Csla.Xaml.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##  
##  “CSLA Workflow” NuGet
##  •	Contents:
##      o	Csla.Workflow.dll (.NET)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet

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
    
    Write-Host "Build All CLSA .NET NuGet packages" -ForegroundColor White
    Write-Host "==================================" -ForegroundColor White
    
    ## NB - Cleanup destination package folder
    ## ---------------------------------------
    Write-Host "Clean destination folders..." -ForegroundColor Yellow
    Remove-Item ".\Packages\*.nupkg" -Recurse -Force -ErrorAction SilentlyContinue
    
    ## Spawn off individual build processes...
    ## ---------------------------------------
    Set-Location "$originalLocation\Definition" ## Adjust current working directory since scripts are using relative paths
    $packages | ForEach { & ".\Build.ps1" $_ }
    Write-Host "Build All - Done." -ForegroundColor Green
}
catch 
{
    $baseException = $_.Exception.GetBaseException()
    if ($_.Exception -ne $baseException)
    {
      Write-Host $baseException.Message -ForegroundColor Magenta
    }
    Write-Host $_.Exception.Message -ForegroundColor Magenta
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