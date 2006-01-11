Namespace DataPortalClient

  Public MustInherit Class EnterpriseServicesProxy

    Implements DataPortalClient.IDataPortalProxy

    Protected MustOverride Function GetServerObject() As Server.Hosts.EnterpriseServicesPortal

    Public Overridable Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Create(objectType, criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Overridable Function Fetch(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Fetch(criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Overridable Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Update(obj, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Overridable Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Delete(criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Overridable ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

  End Class

End Namespace
