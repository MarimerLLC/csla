Imports System.EnterpriseServices
Imports System.Runtime.InteropServices

Namespace Server

  ''' <summary>
  ''' Implements the server-side Serviced 
  ''' DataPortal described in Chapter 4.
  ''' </summary>
  <Transaction(TransactionOption.Required)> _
  <EventTrackingEnabled(True)> _
  <ComVisible(True)> _
  Public Class ServicedDataPortal
    Inherits ServicedComponent

    Implements IDataPortalServer

    ''' <summary>
    ''' Wraps a Create call in a ServicedComponent.
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a COM+ transaction
    ''' to provide transactional support.
    ''' </remarks>
    ''' <param name="objectType">A <see cref="Type">Type</see> object
    ''' indicating the type of business object to be created.</param>
    ''' <param name="criteria">A custom criteria object providing any
    ''' extra information that may be required to properly create
    ''' the object.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A populated business object.</returns>
    <AutoComplete(True)> _
    Public Function Create( _
      ByVal objectType As System.Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Dim portal As New SimpleDataPortal
      Return portal.Create(objectType, criteria, context)

    End Function

    ''' <summary>
    ''' Wraps a Fetch call in a ServicedComponent.
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a COM+ transaction
    ''' to provide transactional support.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Object containing context data from client.</param>
    ''' <returns>A populated business object.</returns>
    <AutoComplete(True)> _
    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Dim portal As New SimpleDataPortal
      Return portal.Fetch(criteria, context)

    End Function

    ''' <summary>
    ''' Wraps an Update call in a ServicedComponent.
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a COM+ transaction
    ''' to provide transactional support.
    ''' </remarks>
    ''' <param name="obj">A reference to the object being updated.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A reference to the newly updated object.</returns>
    <AutoComplete(True)> _
    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Dim portal As New SimpleDataPortal
      Return portal.Update(obj, context)

    End Function

    ''' <summary>
    ''' Wraps a Delete call in a ServicedComponent.
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a COM+ transaction
    ''' to provide transactional support.
    ''' </remarks>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    <AutoComplete(True)> _
    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Dim portal As New SimpleDataPortal
      Return portal.Delete(criteria, context)

    End Function

  End Class

End Namespace
