param($installPath, $toolsPath, $package, $project)

	Write-Host "Installing CSLA Snippets and templates..."
  
	function InstallFiles ($destination,  $files) 
	{
		# Create destination of not exists
		if ((Test-Path $destination) -ne $true)
		{
			New-Item $destination -ItemType directory
		}

		Write-Host "...installing to $destination for Visual Studio $vsVersion."
		Copy-Item $files -Destination $destination
	}
	
	$vsVersions = @("2010", "2012", "2013", "2015", "2017", "2019")
	$cslaFolder = "Csla"
	$sourceSnippetsCS = "$toolsPath\Snippets\cs\*.snippet"
	$sourceSnippetsVB = "$toolsPath\Snippets\Vb\*.snippet"
	$sourceTemplatesCS = "$toolsPath\Templates\cs\*.zip"
	$sourceTemplatesVB = "$toolsPath\Templates\vb\*.zip"
	$destinationDocumentsRoot = [System.Environment]::GetFolderPath( "MyDocuments" );

  	# Install for all versions of VS defined
	foreach ($vsVersion in $vsVersions) 
	{
		# VS Version test - Test if User's Visual Studio destination path exists and skip if it doesn't
		$destinationUserVisualStudio = Join-Path $destinationDocumentsRoot "\Visual Studio $vsVersion"
		if ((Test-Path $destinationUserVisualStudio) -ne $true) { continue }
		
		$destinationSnippetsFolderCS = Join-Path $destinationUserVisualStudio "\Code Snippets\Visual C#\My Code Snippets\$cslaFolder"
		$destinationSnippetsFolderVB = Join-Path $destinationUserVisualStudio "\Code Snippets\Visual Basic\My Code Snippets\$cslaFolder"
		$destinationItemsFolderCS = Join-Path $destinationUserVisualStudio "\Templates\ItemTemplates\Visual C#\$cslaFolder"
		$destinationItemsFolderVB = Join-Path $destinationUserVisualStudio "\Templates\ItemTemplates\Visual Basic\$cslaFolder"

		InstallFiles $destinationSnippetsFolderCS $sourceSnippetsCS
		InstallFiles $destinationSnippetsFolderVB $sourceSnippetsVB
		InstallFiles $destinationItemsFolderCS $sourceTemplatesCS
		InstallFiles $destinationItemsFolderVB $sourceTemplatesVB
	}
