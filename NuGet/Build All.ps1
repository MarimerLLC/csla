##
##	Create Modular CSLA NuGet based on UI Technology (Based on Discussions with Rocky & JH)
##  =======================================================================================
##  
##  “CSLA .NET - Core” NuGet
##  •	Contents:
##      o	Csla.dll (.NET 4 and .NET 4.5)
##      o	Csla.dll (Silverlight 5)
##      o	Csla.dll (WinPRT - Windows Phone 8)
##      o	Csla.dll (WinRT - Windows RT)
##  •	Dependencies
##      o	Dependencies for .NET4 and SL5 on Async Targetting packages
##      o	(The rest has no dependencies on other NuPacks)
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
##      o	Csla.Xaml.dll (Silverlight 5)
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
##      o	Csla.Xaml.dll (WinPrt)
##  •	Dependencies
##      o	“CSLA .NET - Core” NuGet
##  
##  “CSLA .NET - Windows Runtime” NuGet
##  •	Contents:
##      o	Csla.Xaml.dll (WinRt)
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
##  
##  “CSLA Templates” NuGet
##  •	Contents:
##      o	Snippets and Templates
##  •	Dependencies
##      o	None

param( [System.String] $commandLineOptions )

function OutputCommandLineUsageHelp()
{
	Write-Host "Build all NuGet packages."
    Write-Host "============================"
    Write-Host "Usage: Build All.ps1 [/PreRelease:<PreReleaseVersion>]"
    Write-Host ">E.g.: Build All.ps1"
	Write-Host ">E.g.: Build All.ps1 /PreRelease:RC1"
}

function Pause ($Message="Press any key to continue...")
{
    Write-Host -NoNewLine $Message
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Write-Host ""
}

## Process CommandLine options
if ( [System.String]::IsNullOrEmpty($commandLineOptions) -ne $true )
{
	if ( $commandLineOptions.StartsWith("/PreRelease:", [System.StringComparison]::OrdinalIgnoreCase) )
	{
		$preRelease = $commandLineOptions.Substring( "/PreRelease:".Length )
	}
	else
	{
		OutputCommandLineUsageHelp
		return
	}
}

try 
{
    ## Initialise
    ## ----------
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    $originalLocation = Get-Location
    ## Temporarily exclude "Windows Phone" until WP8 SDK arrives
    ## $packages = @("Core", "ASP.NET", "ASP.NET MVC", "Silverlight", "Windows Forms", "Windows Phone", "WPF", "Windows Runtime", "Templates")  # Leave out "Workflow" until Rocky is happy with it for CSLA 4.x
    $packages = @("Core", "ASP.NET", "ASP.NET MVC", "Silverlight", "Windows Forms", "WPF", "Windows Runtime", "Templates")  # Leave out "Workflow" until Rocky is happy with it for CSLA 4.x
    
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
    $packages | ForEach { & ".\Build.ps1" $_ $commandLineOptions }
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