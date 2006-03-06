Namespace DataPortalClient

  ''' <summary>
  ''' Implements a data portal proxy to relay data portal
  ''' calls to an application server hosted in COM+.
  ''' </summary>
  Public MustInherit Class EnterpriseServicesProxy

    Implements DataPortalClient.IDataPortalProxy

    ''' <summary>
    ''' Override this method to return a reference to
    ''' the server-side COM+ (ServicedComponent) object
    ''' implementing the data portal server functionality.
    ''' </summary>
    Protected MustOverride Function GetServerObject() As _
      Server.Hosts.EnterpriseServicesPortal

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to create a
    ''' new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to load an
    ''' existing business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to update a
    ''' business object.
    ''' </summary>
    ''' <param name="obj">The business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to delete a
    ''' business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
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

    ''' <summary>
    ''' Get a value indicating whether this proxy will invoke
    ''' a remote data portal server, or run the "server-side"
    ''' data portal in the caller's process and AppDomain.
    ''' </summary>
    Public ReadOnly Property IsServerRemote() As Boolean _
      Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

  End Class

End Namespace
