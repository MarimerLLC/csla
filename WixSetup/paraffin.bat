Support\Paraffin.exe CSSnippetsFragment.wxs -dir ..\Support\Snippets\cs -groupname CSSnippetsComp -alias "$(var.SolutionDir)..\Support\Snippets\cs" -dirref CSSnippets -norootdirectory
Support\Paraffin.exe LogosFragment.wxs -dir ..\Support\Logos -groupname LogosComponents -alias "$(var.SolutionDir)..\Support\Logos" -dirref Logos -norootdirectory
Support\Paraffin.exe Net4Fragment.wxs -dir ..\Bin\Release\Net4 -groupname Net4Components -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Net4" -ext config -dirref Net4 -norootdirectory
Support\Paraffin.exe NetFragment.wxs -dir ..\Bin\Release\Net -groupname NetComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Net" -ext config -dirref Net -norootdirectory
Support\Paraffin.exe SilverlightFragment.wxs -dir ..\Bin\Release\Silverlight -groupname SilverlightComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Silverlight" -ext config -dirref Silverlight -norootdirectory
Support\Paraffin.exe VBSnippetsFragment.wxs -dir ..\Support\Snippets\vb -groupname VBSnippetsComp -alias "$(var.SolutionDir)..\Support\Snippets\vb" -dirref VBSnippets -norootdirectory
Support\Paraffin.exe WinPRTFragment.wxs -dir ..\Bin\Release\WinPRT -groupname WinPRTComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\WinPRT" -ext config -dirref WinPRT -norootdirectory
Support\Paraffin.exe WinRTFragment.wxs -dir ..\Bin\Release\WinRT -groupname WinRTComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\WinRT" -ext config -dirref WinRT -norootdirectory
