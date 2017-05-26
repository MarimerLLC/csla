##
##	Create Modular CSLA NuGet based on UI Technology (Based on Discussions with Rocky & Jaans)
##  ==========================================================================================
##  
##  �CSLA .NET - Core� NuGet
##  �	Contents:
##      o	Csla.dll (.NET 4 and .NET 4.5)
##      o	Csla.dll (WinRT - Windows Phone WinRT)
##      o	Csla.dll (WinRT - Windows RT)
##      o	Csla.dll (MonoAndroid - Xamarin Android)
##      o	Csla.dll (MonoIos - Xamarin iOS Classic)
##      o	Csla.dll (XamarinIos - Xamarin iOS Unified
##  �	Dependencies
##      o	Dependencies for .NET4 and SL5 on Async Targetting packages
##      o Dependencies on EntityFramework v4/v5 for CSLA .NET - Data EFx" packages
##      o	(The rest has no dependencies on other NuPacks)
##  
##  �CSLA .NET - ASP.NET� NuGet
##  �	Contents:
##      o	Csla.Web.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - ASP.NET MVC 4� NuGet
##  �	Contents:
##      o	Csla.Web.Mvc4.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - ASP.NET MVC 5� NuGet
##  �	Contents:
##      o	Csla.Web.Mvc5.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - Windows Forms� NuGet
##  �	Contents:
##      o	Csla.Windows.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - Windows Phone WinRT� NuGet
##  �	Contents:
##      o	Csla.Xaml.dll (WinRT)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - Windows Runtime� NuGet
##  �	Contents:
##      o	Csla.Xaml.dll (WinRt)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##
##  �CSLA .NET - WPF� NuGet
##  �	Contents:
##      o	Csla.Xaml.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA .NET - MonoAndroid� NuGet
##  �	Contents:
##      o	Csla.Axml.dll (MonoAndroid)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##
##  �CSLA .NET - Xamarin.iOS� NuGet
##  �	Contents:
##      o	Csla.Iosui.dll (Mono iOS UI)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##
##  �CSLA Workflow� NuGet
##  �	Contents:
##      o	Csla.Workflow.dll (.NET)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##  
##  �CSLA Templates� NuGet
##  �	Contents:
##      o	Snippets and Templates
##  �	Dependencies
##      o	None
##  
##  �CSLA .NET - Data EF4" NuGet
##  �	Contents:
##      o	Csla.Data.EF4.dll (WinRt)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##      o	�EntityFramework� (v4) NuGet
##  
##  �CSLA .NET - Data EF5" NuGet
##  �	Contents:
##      o	Csla.Data.EF5.dll (WinRt)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##      o	�EntityFramework� (v5) NuGet
##  
##  �CSLA .NET - Data EF6" NuGet
##  �	Contents:
##      o	Csla.Data.EF6.dll (WinRt)
##  �	Dependencies
##      o	�CSLA .NET - Core� NuGet
##      o	�EntityFramework� (v6) NuGet
##  
##  �CSLA .NET - Legacy Validation� NuGet
##  �	Contents:
##      o	Csla.Validation.dll (.NET 4 and .NET 4.5)
##      o	Csla.Validation.dll (WinRT - Windows Phone WinRT)
##      o	Csla.Validation.dll (WinRT - Windows RT)
##      o	Csla.Validation.dll (MonoAndroid - Xamarin Android)
##      o	Csla.Validation.dll (XamarinIos - Xamarin iOS)
##  �	Dependencies
##      o	Dependencies for .NET4 and SL5 on Async Targetting packages
##      o Dependencies on EntityFramework v4/v5 for CSLA .NET - Data EFx" packages
##      o	(The rest has no dependencies on other NuPacks)

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
    $packages = @("Core", "XamarinForms", "AspNetCore MVC", "ASP.NET", "ASP.NET MVC 5", "ASP.NET MVC 4", "Windows Forms", "UWP", "Windows Phone WinRT", "WPF", "Windows Runtime", "MonoAndroid", "XamarinIos", "Data EF4", "Data EF5", "Data EF6",  "Data EF7", "Templates", "UpdateValidation")  
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black
    $host.UI.RawUI.ForegroundColor = [System.ConsoleColor]::White
    
    Write-Host "Build All CLSA .NET NuGet packages" -ForegroundColor White
    Write-Host "==================================" -ForegroundColor White

    Write-Host "Creating Packages folder" -ForegroundColor Yellow
    mkdir Packages -ErrorAction Ignore

    ## NB - Cleanup destination package folder
    ## ---------------------------------------
    Write-Host "Clean destination folders..." -ForegroundColor Yellow
    Remove-Item ".\Packages\*.nupkg" -Recurse -Force -ErrorAction SilentlyContinue
    
    ## RDL - Copy definition files to temp folder
    ## ------------------------------------------
    Write-Host "Copy NuSpec files to working directory..." -ForegroundColor Yellow
    mkdir deftmp  -ErrorAction Ignore
    Remove-Item ".\deftmp\*" -Recurse -Force -ErrorAction SilentlyContinue
    Copy -Recurse $originalLocation\Definition\* $originalLocation\deftmp
    
    ## Spawn off individual build processes...
    ## ---------------------------------------
    Set-Location "$originalLocation\deftmp" ## Adjust current working directory since scripts are using relative paths
    $packages | ForEach { & ".\Build.ps1" $_ $commandLineOptions }

    Set-Location "$originalLocation" 
    Remove-Item "deftmp" -Recurse -Force -ErrorAction SilentlyContinue
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
Pause # For debugging purpose