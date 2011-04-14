param($package) 

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
            if ($idAttribute.Value -eq "CSLA-Core")
            {
                $dependency.SetAttributeValue("version", "[$version]" )
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

if ($package -eq $null)
{
    Write-Host "Create a NuGet build output."
    Write-Host "============================"
    Write-Host "Usage: Create.ps1 ""<NuGet Package Name>"""
    Write-Host ">E.g.: Create.ps1 ""Core"""
    return
}

try 
{
    ## Initialise
    ## ----------
    $configuration = "Release"
    $basePath = Get-Location
    $pathToBin = [System.IO.Path]::GetFullPath( "$basePath\..\..\Bin\$configuration" )
    $pathToNuGetPackager = [System.IO.Path]::GetFullPath( "$basePath\..\Tools\NuGet.exe" )
    $pathToNuGetPackageOutput = [System.IO.Path]::GetFullPath( "$basePath\..\Packages" )
    $originalBackground = $host.UI.RawUI.BackgroundColor
    $originalForeground = $host.UI.RawUI.ForegroundColor
    
    $host.UI.RawUI.BackgroundColor = [System.ConsoleColor]::Black
    $host.UI.RawUI.ForegroundColor = [System.ConsoleColor]::White

    Write-Host "Build NuGet packages for: " -ForegroundColor White -NoNewLine
    Write-Host $package -ForegroundColor Cyan
    Write-Host "=========================" -ForegroundColor White

    ## Update Package Version
    ## ----------------------    
    ## Before building NuGet package, extract CSLA Version number and update .NuSpec to automate versioning of .NuSpec document
    ## - JH: Not sure if I should get direct from source code file or from file version of compiled library instead.
    ## - JH: Going with product version in assembly for now
    $cslaAssembly = Get-ChildItem "$pathToBin\Client\Csla.dll" | Select-Object -First 1
    $productVersion = $cslaAssembly.VersionInfo.ProductVersion
    ChangeNuSpecVersion "$basePath\$package.NuSpec" $productVersion
    
    ## Launch NuGet.exe to build package
    Write-Host "Build NuGet package: $package..." -ForegroundColor Yellow -NoNewLine
    & $pathToNuGetPackager pack "$basePath\$package.NuSpec"
    
    ## Publish package to Gallery using API Key
    ## JH - TODO

    ## Move NuGet package to Package Release folder"
    Write-Host "Move package to ..\Packages folder..." -ForegroundColor Yellow
    Move-Item "*.nupkg" -Destination $pathToNuGetPackageOutput -Force
    
    ## Cleanup after ourselves
    ## JH - TODO

    Write-Host "Done." -ForegroundColor Green
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
    $host.UI.RawUI.BackgroundColor = $originalBackground
    $host.UI.RawUI.ForegroundColor = $originalForeground
}
#Pause # For debugging purposes