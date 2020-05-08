param( $package, [System.String] $commandLineOptions )

function OutputCommandLineUsageHelp()
{
    Write-Host "Create a NuGet build output."
    Write-Host "============================"
    Write-Host "Usage: Create.ps1 ""<NuGet Package Name>"" [/PreRelease:<PreReleaseVersion>]"
    Write-Host ">E.g.: Create.ps1 ""Core"""
    Write-Host ">E.g.: Create.ps1 ""Core"" /PreRelease:RC1"
}

function Pause( $Message="Press any key to continue..." )
{
    Write-Host -NoNewLine $Message
    $null = $Host.UI.RawUI.ReadKey( "NoEcho,IncludeKeyDown" )
    Write-Host ""
}

function ChangeNuSpecVersion( $nuSpecFilePath, $version="0.0.0.0" )
{
    Write-Host "Dynamically setting NuSpec version: $Version" -ForegroundColor Yellow
    
    # Get full path or save operation fails when launched in standalone powershell
    $nuSpecFile = Get-Item $nuSpecFilePath | Select-Object -First 1
    
    # Bring the XML Linq namespace in
    [Reflection.Assembly]::LoadWithPartialName( "System.Xml.Linq" ) | Out-Null
    
    # Update the XML document with the new version
    $xDoc = [System.Xml.Linq.XDocument]::Load( $nuSpecFile.FullName )
    $versionNode = $xDoc.Descendants( "version" ) | Select-Object -First 1
    if ($versionNode -ne $null)
    {
        $versionNode.SetValue($version)
    }
    
    # Update the XML document dependencies with the new version
    $dependencies = $xDoc.Descendants( "dependency" )
    foreach( $dependency in $dependencies )
    {
        $idAttribute = $dependency.Attributes( "id" ) | Select-Object -First 1
        if ( $idAttribute -ne $null )
        {
            if ( $idAttribute.Value -eq "Csla" )
            {
                $dependency.SetAttributeValue( "version", "[$version]" )
            }
            if ( $idAttribute.Value -eq "Csla.Blazor" )
            {
                $dependency.SetAttributeValue( "version", "[$version]" )
            }
            if ( $idAttribute.Value -eq "Csla.AspNet" )
            {
                $dependency.SetAttributeValue( "version", "[$version]" )
            }
            if ( $idAttribute.Value -eq "CSLA-Core" )
            {
                $dependency.SetAttributeValue( "version", "[$version]" )
            }
            if ( $idAttribute.Value -eq "CSLA-ASP.NET" )
            {
                $dependency.SetAttributeValue( "version", "[$version]" )
            }
        }
    }
    
    # Save file
    $xDoc.Save( $nuSpecFile.FullName )
}

function UpdateNuSpecRepository( $nuSpecFilePath )
{
    # Get Git info
    $commit = git rev-parse HEAD
    $origin = git config --get remote.origin.url

    Write-Host "Dynamically setting NuSpec repository:" -ForegroundColor Yellow
    Write-Host "      origin: $origin" -ForegroundColor Yellow
    Write-Host "      commit: $commit" -ForegroundColor Yellow

    # Get full path or save operation fails when launched in standalone powershell
    $nuSpecFile = Get-Item $nuSpecFilePath | Select-Object -First 1

    # Read XML document
    [xml]$nuspec = Get-Content $nuSpecFile.FullName
    $repo = $nuspec.package.metadata.repository;

    # Generate repository tag if it doesn't exist
    if ( !$repo ) {
        $repo = $nuspec.CreateElement( "repository" );
        [void]$nuspec.package.metadata.AppendChild($repo);
    }

    # Add type attribute if it doesn't exist, and set it to "git"
    if ( !$repo.type ) { [void]$repo.Attributes.Append( $nuspec.CreateAttribute( "type" ) ) }
    $repo.type = "git"

    # Add url attribute if it doesn't exist, and set it to origin
    if ( !$repo.url ) { [void]$repo.Attributes.Append( $nuspec.CreateAttribute( "url" ) ) }
    $repo.url = $origin.ToString()

    # Add commit attribute if it doesn't exist, and set it to current HEAD hash
    if ( !$repo.commit ) { [void]$repo.Attributes.Append( $nuspec.CreateAttribute( "commit" ) ) }
    $repo.commit = $commit.ToString()

    # Save file
    $nuspec.Save( $nuSpecFile.FullName )
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
            [System.IO.Directory]::CreateDirectory( $targetDirectory ) | Out-Null
        }
        Copy-Item $file -Destination $targetDirectory
    }
}

## Validate input parameters
## -------------------------
if ( $package -eq $null )
{
    OutputCommandLineUsageHelp
    return
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
    if ( [System.String]::IsNullOrEmpty( $preRelease ) -ne $true )
    {
        Write-Host "Option: /PreRelease:$preRelease" -ForegroundColor Yellow
    }

    ## Update Package Version
    ## ----------------------    
    ## Before building NuGet package, extract CSLA Version number and update .NuSpec to automate versioning of .NuSpec document
    ## - JH: Not sure if I should get direct from source code file or from file version of compiled library instead.
    ## - JH: Going with product version in assembly for now
    $cslaAssembly = Get-ChildItem "$pathToBin\NET46\Csla.dll" | Select-Object -First 1
    ## - JH: If $preRelease is specified, then append it with a dash following the 3rd component of the quad-dotted-version number
    ##       Refer: http://docs.nuget.org/docs/Reference/Versioning 
    if ( [System.String]::IsNullOrEmpty( $preRelease ) -ne $true )
    {
        $productVersion = [System.String]::Format( "{0}.{1}.{2}-R{3}", $cslaAssembly.VersionInfo.ProductMajorPart, $cslaAssembly.VersionInfo.ProductMinorPart, $cslaAssembly.VersionInfo.ProductBuildPart, $preRelease )
    }
    else
    {
        $productVersion = [System.String]::Format( "{0}.{1}.{2}", $cslaAssembly.VersionInfo.ProductMajorPart, $cslaAssembly.VersionInfo.ProductMinorPart, $cslaAssembly.VersionInfo.ProductBuildPart )
    }
    ChangeNuSpecVersion "$basePath\$package.NuSpec" $productVersion

    ## Update Git Repository info
    ## ----------------------
    ## Before building NuGet package, extract git commit hash, and origin URL and update .NuSpec
    ## to automate repository attributes.
    UpdateNuSpecRepository "$basePath\$package.nuspec"
    
    ## Launch NuGet.exe to build package
    Write-Host "Build NuGet package: $package..." -ForegroundColor Yellow
    if ( $package -ne "Templates" ) { & $pathToNuGetPackager pack "$basePath\$package.NuSpec" -OutputDirectory ..\Packages -Symbols -SymbolPackageFormat snupkg }
    else { & $pathToNuGetPackager pack "$basePath\$package.NuSpec" -OutputDirectory ..\Packages }
    
    ## Publish package to Gallery using API Key
    ## JH - TODO

    ## Move NuGet package to Package Release folder"
    ## MT - This isn't required ...
    # Write-Host "Move package to ..\Packages folder..." -ForegroundColor Yellow
    # Move-Item "*nupkg" -Destination $pathToNuGetPackageOutput -Force
    
    ## Cleanup after ourselves
    ## JH - TODO

    Write-Host "Done." -ForegroundColor Green
}
catch 
{
    $baseException = $_.Exception.GetBaseException()
    if ( $_.Exception -ne $baseException )
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