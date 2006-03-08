Namespace Server

  ''' <summary>
  ''' Interface implemented by server-side data portal
  ''' components.
  ''' </summary>
  Public Interface IDataPortalServer
    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Function Create( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Function Update( _
      ByVal obj As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Function Delete( _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
  End Interface

End Namespace
