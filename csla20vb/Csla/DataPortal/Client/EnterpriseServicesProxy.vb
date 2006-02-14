Namespace DataPortalClient

  Public MustInherit Class EnterpriseServicesProxy

    Implements DataPortalClient.IDataPortalProxy

    Protected MustOverride Function GetServerObject() As _
      Server.Hosts.EnterpriseServicesPortal

    Public Function Create( _
      ByVal objectType As System.Type, ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Create(objectType, criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Fetch(criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Update(obj, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Dim svc As Server.Hosts.EnterpriseServicesPortal = GetServerObject()
      Try
        Return svc.Delete(criteria, context)

      Finally
        If svc IsNot Nothing Then
          svc.Dispose()
        End If
      End Try

    End Function

    Public ReadOnly Property IsServerRemote() As Boolean _
      Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

  End Class

End Namespace
