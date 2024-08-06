param($installPath, $toolsPath, $package, $project)
 
$generatorsPaths = Join-Path (Join-Path (Split-Path -Path $toolsPath -Parent) "analyzers" ) * -Resolve
 
foreach($generatorsPath in $generatorsPaths)
{
    # Install the language agnostic source generators.
    if (Test-Path $generatorsPath)
    {
        foreach ($generatorFilePath in Get-ChildItem $generatorsPath -Filter *.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($generatorFilePath.FullName)
            }
        }
    }
}
 
# $project.Type gives the language name like (C# or VB.NET)
$languageFolder = ""
if($project.Type -eq "C#")
{
    $languageFolder = "cs"
}
if($project.Type -eq "VB.NET")
{
    $languageFolder = "vb"
}
if($languageFolder -eq "")
{
    return
}
 
foreach($generatorsPath in $generatorsPaths)
{
    # Install language specific source generators.
    $languageGeneratorsPath = join-path $generatorsPath $languageFolder
    if (Test-Path $languageGeneratorsPath)
    {
        foreach ($generatorFilePath in Get-ChildItem $languageGeneratorsPath -Filter *.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($generatorFilePath.FullName)
            }
        }
    }
}
