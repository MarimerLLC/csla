Imports System.IO
Imports System.Reflection
Imports System.Security
Imports System.Security.Policy
Imports System.Security.Permissions

Module Main

  Public Sub Main()

    Try
      ' launch the app based on the URL provided by the user
      RunAppliation(Microsoft.VisualBasic.Command)

    Catch ex As Exception
      Dim sb As New System.Text.StringBuilder()
      sb.Append("NetRun was unable to launch the application")
      sb.Append(vbCrLf)
      sb.Append(Microsoft.VisualBasic.Command)
      sb.Append(vbCrLf)
      sb.Append(vbCrLf)
      sb.Append(ex.ToString)
      MsgBox(sb.ToString, MsgBoxStyle.Exclamation)
    End Try

  End Sub

#Region " RunApplication "

  Private Sub RunAppliation(ByVal AppURL As String)

    ' create setup object for the new app domain 
    Dim setupDomain As New AppDomainSetup()
    With setupDomain
      ' give it a valid base path
      .ApplicationBase = CurrentDomainPath()
      ' give it a safe config file name
      .ConfigurationFile = AppURL + ".remoteconfig"
    End With

    ' create new application domain 
    Dim newDomain As AppDomain = _
      AppDomain.CreateDomain( _
      GetAppName(AppURL), Nothing, setupDomain)

    ' create launcher object in new appdomain
    Dim launcher As Launcher = _
      CType(newDomain.CreateInstanceAndUnwrap( _
      "NetRun", "NetRun.Launcher"), _
      Launcher)

    ' use launcher object from the new domain
    ' to launch the remote app in that appdomain
    launcher.RunApp(AppURL)

  End Sub

#End Region

#Region " GetCurrentDomainPath "

  Private Function CurrentDomainPath() As String

    ' get path of current assembly
    Dim currentPath As String = [Assembly].GetExecutingAssembly.CodeBase
    ' convert it to a URI for ease of use
    Dim currentURI As Uri = New Uri(currentPath)
    ' get the path portion of the URI
    Dim currentLocalPath As String = currentURI.LocalPath

    ' return the full name of the path
    Return New DirectoryInfo(currentLocalPath).Parent.FullName

  End Function

#End Region

#Region " URL parsing functions "

  Public Function GetAppDirectory(ByVal AppURL As String) As String

    ' get the path without prog name
    Dim appURI As New System.Uri(AppURL)
    Dim appPath As String = appURI.GetLeftPart(UriPartial.Path)
    Dim pos As Integer

    For pos = Len(appPath) To 1 Step -1
      If Mid(appPath, pos, 1) = "/" OrElse Mid(appPath, pos, 1) = "\" Then
        Return Left(appPath, pos - 1)
      End If
    Next
    Return ""

  End Function

  Public Function GetAppName(ByVal AppURL As String) As String

    ' get the prog name without path
    Dim appURI As New System.Uri(AppURL)
    Dim appPath As String = appURI.GetLeftPart(UriPartial.Path)
    Dim pos As Integer

    For pos = Len(appPath) To 1 Step -1
      If Mid(appPath, pos, 1) = "/" OrElse Mid(appPath, pos, 1) = "\" Then
        Return Mid(appPath, pos + 1)
      End If
    Next
    Return ""

  End Function

#End Region

End Module
