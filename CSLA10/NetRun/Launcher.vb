Imports System.Reflection
Imports System.Security
Imports System.Security.Policy
Imports System.Security.Permissions

Public Class Launcher
  Inherits MarshalByRefObject

  Private mAppURL As String
  Private mAppDir As String
  Private mAppName As String

  Private mGroupExisted As Boolean

  Public Sub RunApp(ByVal AppURL As String)

    ' before we do anything, invoke the workaround
    ' for the serialization bug
    SerializationWorkaround()

    Try
      ' get and parse the URL for the app we are
      ' launching
      mAppURL = AppURL
      mAppDir = GetAppDirectory(mAppURL)
      mAppName = GetAppName(mAppURL)

      SetSecurity()

      ' load the assembly into our AppDomain
      Dim asm As [Assembly]
      asm = [Assembly].LoadFrom(AppURL)

      ' run the program by invoking its entry point
      asm.EntryPoint.Invoke(asm.EntryPoint, Nothing)

    Finally
      RemoveSecurity()
    End Try

  End Sub

#Region " Serialization bug workaround "

  Private Sub SerializationWorkaround()

    ' hook up the AssemblyResolve
    ' event so deep serialization works properly
    ' this is a workaround for a bug in the .NET runtime
    Dim currentDomain As AppDomain = AppDomain.CurrentDomain

    AddHandler currentDomain.AssemblyResolve, _
      AddressOf ResolveEventHandler

  End Sub

  Private Function ResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As [Assembly]

    ' get a list of all the assemblies loaded in our appdomain
    Dim list() As [Assembly] = AppDomain.CurrentDomain.GetAssemblies()

    ' search the list to find the assemby that was not found automatically
    ' and return the assembly from the list
    Dim asm As [Assembly]

    For Each asm In list
      If asm.FullName = args.Name Then
        Return asm
      End If
    Next

  End Function

#End Region

#Region " SetSecurity to FullTrust "

  Private Sub SetSecurity()

    Dim ph As System.Collections.IEnumerator
    Dim pl As System.Security.Policy.PolicyLevel
    Dim found As Boolean

    ' retrieve the security policy hierarchy
    ph = SecurityManager.PolicyHierarchy

    ' loop through to find the Machine level sub-tree
    Do While ph.MoveNext
      pl = CType(ph.Current, PolicyLevel)
      If pl.Label = "Machine" Then
        found = True
        Exit Do
      End If
    Loop

    If found Then
      ' see if the codegroup for this app already exists
      ' as a machine-level entry
      Dim cg As CodeGroup
      For Each cg In pl.RootCodeGroup.Children
        If cg.Name = mAppName Then
          ' codegroup already exists
          ' we assume it is set to a valid
          ' permission level
          mGroupExisted = True
          Exit Sub
        End If
      Next

      ' the codegroup doesn't already exist, so 
      ' we'll add a url group with FullTrust
      mGroupExisted = False
      Dim ucg As UnionCodeGroup = _
        New UnionCodeGroup(New UrlMembershipCondition(mAppDir & "/*"), _
        New PolicyStatement(New NamedPermissionSet("FullTrust")))
      ucg.Description = "Temporary entry for " & mAppURL
      ucg.Name = mAppName
      pl.RootCodeGroup.AddChild(ucg)
      SecurityManager.SavePolicy()
    End If

  End Sub

#End Region

#Region " RemoveSecurity "

  Private Sub RemoveSecurity()

    ' if the group existed before NetRun was used
    ' we want to leave the group intact, so we
    ' can just exit
    If mGroupExisted Then Exit Sub

    ' on the other hand, if the group didn't already
    ' exist then we need to remove it now that
    ' the business application is closed
    Dim ph As System.Collections.IEnumerator
    Dim pl As System.Security.Policy.PolicyLevel
    Dim found As Boolean

    ' retrieve the security policy hierarchy
    ph = SecurityManager.PolicyHierarchy

    ' loop through to find the Machine level sub-tree
    Do While ph.MoveNext
      pl = CType(ph.Current, PolicyLevel)
      If pl.Label = "Machine" Then
        found = True
        Exit Do
      End If
    Loop

    If found Then
      ' see if the codegroup for this app exists
      ' as a machine-level entry
      Dim cg As CodeGroup
      For Each cg In pl.RootCodeGroup.Children
        If cg.Name = mAppName Then
          ' codegroup exits - remove it
          pl.RootCodeGroup.RemoveChild(cg)
          SecurityManager.SavePolicy()
          Exit For
        End If
      Next
    End If

  End Sub

#End Region

End Class
