param($installPath, $toolsPath, $package, $project)

	$vsVersions = @("2010", "2012", "2013", "2015", "2017", "2019")
	$cslaFolder = "Csla"
	$destinationDocumentsRoot = [System.Environment]::GetFolderPath( "MyDocuments" );

	# Uninstall for all versions of VS defined
	foreach ($vsVersion in $vsVersions)
	{
		# VS Version test - Test if User's Visual Studio destination path exists and skip if it doesn't
		$destinationUserVisualStudio = Join-Path $destinationDocumentsRoot "\Visual Studio $vsVersion"
		if ((Test-Path $destinationUserVisualStudio) -ne $true) { continue }
		
		$destinationSnippetsFolderCS = Join-Path $destinationUserVisualStudio "\Code Snippets\Visual C#\My Code Snippets\$cslaFolder"
		$destinationSnippetsFolderVB = Join-Path $destinationUserVisualStudio "\Code Snippets\Visual Basic\My Code Snippets\$cslaFolder"
		$destinationItemsFolderCS = Join-Path $destinationUserVisualStudio "\Templates\ItemTemplates\Visual C#\$cslaFolder"
		$destinationItemsFolderVB = Join-Path $destinationUserVisualStudio "\Templates\ItemTemplates\Visual Basic\$cslaFolder"

		Write-Host "Uninstalling templates and snippets for $cslaFolder from Visual Studio $vsVersion"
		Remove-Item -Recurse -Force $destinationSnippetsFolderCS
		Remove-Item -Recurse -Force $destinationSnippetsFolderVB
		Remove-Item -Recurse -Force $destinationItemsFolderCS
		Remove-Item -Recurse -Force $destinationItemsFolderVB
	}
