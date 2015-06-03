Support\Paraffin.exe xCSSnippetsFragment.wxs -dir ..\Support\Snippets\cs -groupname CSSnippetsComp -alias "$(var.SolutionDir)..\Support\Snippets\cs" -dirref CSSnippets -norootdirectory 
Support\Paraffin.exe xLogosFragment.wxs -dir ..\Support\Logos -groupname LogosComponents -alias "$(var.SolutionDir)..\Support\Logos" -dirref Logos -norootdirectory
Support\Paraffin.exe xNet4Fragment.wxs -dir ..\Bin\Release\Net4 -groupname Net4Components -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Net4" -ext config -dirref Net4 -norootdirectory
Support\Paraffin.exe xNet45Fragment.wxs -dir ..\Bin\Release\Net45 -groupname Net45Components -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Net45" -ext config -dirref Net45 -norootdirectory
Support\Paraffin.exe xNet46Fragment.wxs -dir ..\Bin\Release\Net46 -groupname Net46Components -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Net46" -ext config -dirref Net46 -norootdirectory
Support\Paraffin.exe xVBSnippetsFragment.wxs -dir ..\Support\Snippets\vb -groupname VBSnippetsComp -alias "$(var.SolutionDir)..\Support\Snippets\vb" -dirref VBSnippets -norootdirectory
Support\Paraffin.exe xWinRTFragment.wxs -dir ..\Bin\Release\WinRT -groupname WinRTComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\WinRT" -ext config -dirref WinRT -norootdirectory
Support\Paraffin.exe xWinRTPhoneFragment.wxs -dir ..\Bin\Release\WinRT.Phone -groupname WinRTPhoneComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\WinRT.Phone" -ext config -dirref WinRT.Phone -norootdirectory
Support\Paraffin.exe xAndroidFragment.wxs -dir ..\Bin\Release\Android -groupname AndroidComponents -alias "$(var.SolutionDir)..\Bin\$(var.Configuration)\Android" -ext config -dirref Android -norootdirectory
