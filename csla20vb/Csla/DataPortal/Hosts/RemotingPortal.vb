Namespace Server.Hosts

  ''' <summary>
  ''' Exposes server-side DataPortal functionality
  ''' through .NET Remoting.
  ''' </summary>
  Public Class RemotingPortal
    Inherits MarshalByRefObject

    Implements Server.IDataPortalServer

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create( _
      ByVal objectType As System.Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Dim portal As New Server.DataPortal
      Return portal.Create(objectType, criteria, context)

    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Dim portal As New Server.DataPortal
      Return portal.Fetch(criteria, context)

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Dim portal As New Server.DataPortal
      Return portal.Update(obj, context)

    End Function

    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Dim portal As New Server.DataPortal
      Return portal.Delete(criteria, context)

    End Function

  End Class

End Namespace
