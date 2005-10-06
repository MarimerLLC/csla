Imports System.EnterpriseServices
Imports System.Runtime.InteropServices
Imports System.Reflection

Namespace Server.Hosts

  ''' <summary>
  ''' Exposes server-side DataPortal functionality
  ''' through Enterprise Services.
  ''' </summary>
  <EventTrackingEnabled(True)> _
  <ComVisible(True)> _
  Public MustInherit Class EnterpriseServicesPortal
    Inherits ServicedComponent

    Implements Server.IDataPortalServer

    Shared Sub New()
      SerializationWorkaround()
    End Sub

    Public Overridable Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

      Dim portal As New Server.DataPortal
      Return portal.Create(objectType, criteria, context)

    End Function

    Public Overridable Function Fetch(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch

      Dim portal As New Server.DataPortal
      Return portal.Fetch(criteria, context)

    End Function

    Public Overridable Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

      Dim portal As New Server.DataPortal
      Return portal.Update(obj, context)

    End Function

    Public Overridable Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

      Dim portal As New Server.DataPortal
      Return portal.Delete(criteria, context)

    End Function

#Region " Serialization bug workaround "

    Private Shared Sub SerializationWorkaround()

      ' hook up the AssemblyResolve
      ' event so deep serialization works properly
      ' this is a workaround for a bug in the .NET runtime
      Dim currentDomain As AppDomain = AppDomain.CurrentDomain

      AddHandler currentDomain.AssemblyResolve, _
        AddressOf ResolveEventHandler

    End Sub

    Private Shared Function ResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As [Assembly]

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

      ' if the assembly wasn't already in the appdomain, then try to load it
      Return [Assembly].Load(args.Name)

    End Function

#End Region

  End Class

End Namespace
