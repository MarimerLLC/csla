function Pause ($Message="Press any key to continue...")
{
    Write-Host -NoNewLine $Message
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Write-Host ""
}

function ChangeNuSpecVersion ( $nuSpecFilePath, $version="0.0.0.0" )
{
    Write-Host "Dynamically setting NuSpec version: $Version" -ForegroundColor Yellow
    
    # Get full path or save operation fails when launched in standalone powershell
    $nuSpecFile = Get-Item $nuSpecFilePath | Select-Object -First 1
    
    # Bring the XML Linq namespace in
    [Reflection.Assembly]::LoadWithPartialName(“System.Xml.Linq”) | Out-Null
    
    # Update the XML document with the new version
    $xDoc = [System.Xml.Linq.XDocument]::Load($nuSpecFile.FullName)
    $versionNode = $xDoc.Descendants("version") | Select-Object -First 1
    if ($versionNode -ne $null)
    {
        $versionNode.SetValue($version)
    }
    
    # Update the XML document dependencies with the new version
    $dependencies = $xDoc.Descendants("dependency")
    foreach( $dependency in $dependencies )
    {
        $idAttribute = $dependency.Attributes("id") | Select-Object -First 1
        if ($idAttribute -ne $null)
        {
            if ($idAttribute.Value -eq "CSLA .NET - Core")
            {
                $dependency.SetAttributeValue("version", $version)
            }
        }
    }
    
    # Save file
    $xDoc.Save($nuSpecFile.FullName)
}

function CopyMaintainingSubDirectories( $basePath, $includes, $targetBasePath )
{
    $basePathLength = [System.IO.Path]::GetFullPath( $basePath ).Length - 1
    $filesToCopy = Get-ChildItem "$basePath\*" -Include $includes -Recurse
    #$filesToCopy | Write-Host -ForegroundColor DarkGray # Debug.Print
    foreach( $file in $filesToCopy )
    { 
        $targetDirectory = Join-Path $targetBasePath $file.Directory.FullName.Substring( $basePathLength )
        if ( (Test-Path $targetDirectory) -ne $true)
        { 
            [System.IO.Directory]::CreateDirectory($targetDirectory) | Out-Null
        }
        Copy-Item $file -Destination $targetDirectory
    }
}

try 
{
    ## Initialise
    ## ----------
    ## (Always use relative paths)
    $package = "WPF"
    $configuration = "Release"
    $basePath = Get-Location
    $pathToBin = [System.IO.Path]::GetFullPath( "$basePath\..\..\Bin\$configuration" )
    $pathToNuGetLib = [System.IO.Path]::GetFullPath( "$basePath\$package" )
    $pathToNuGetPackager = [System.IO.Path]::GetFullPath( "$basePath\..\Tools\NuGet.exe" )
    $pathToNuGetPackageReleaseFolder = [System.IO.Path]::GetFullPath( "$basePath\..\Packages" )
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black

    Write-Host "Build NuGet packages for: $package" -ForegroundColor White
    Write-Host "=========================" -ForegroundColor White

    ## Cleanup previous build outputs
    ## ------------------------------
    & ".\Clean.ps1" $package

    ## Copy the build outputs and group per platform
    ## --------------------------------------------
    Write-Host "Copy CSLA build outputs to NuGet Lib\<PlatForm><Version> folders..." -ForegroundColor Yellow

    ## .NET
    $includes = @("Csla.Xaml.dll*", "Csla.Xaml.resources*.dll", "Csla.Xaml.XML*")
    CopyMaintainingSubDirectories "$pathToBin\Client\" $includes "$pathtoNuGetLib\Lib\NET4.0" 
    
    ## Create NuGet package
    ## ----------------------
    ## Before building NuGet package, extract CSLA Version number and update .NuSpec to automate versioning of .NuSpec document
    ## - JH: Not sure if I should get direct from source code file or from file version of compiled library instead.
    ## - JH: Going with product version in assembly for now
    $cslaAssembly = Get-ChildItem "$pathToBin\Client\Csla.dll" | Select-Object -First 1
    $productVersion = $cslaAssembly.VersionInfo.ProductVersion
    ChangeNuSpecVersion "$pathtoNuGetLib\$package.NuSpec" $productVersion
    
    ## Launch NuGet.exe
    Write-Host "Build NuGet package: $package..." -ForegroundColor Yellow
    & $pathToNuGetPackager pack "$pathtoNuGetLib\$package.NuSpec"

    ## Move NuGet package to Package Release folder
    Move-Item "*.nupkg" -Destination $pathToNuGetPackageReleaseFolder -Force

    Write-Host "Done." -ForegroundColor Green
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
}
#Pause # For debugging purposes