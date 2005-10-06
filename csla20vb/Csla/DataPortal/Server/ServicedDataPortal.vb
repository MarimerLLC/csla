Imports System.EnterpriseServices
Imports System.Runtime.InteropServices

Namespace Server

  ''' <summary>
  ''' Implements the server-side Serviced 
  ''' DataPortal described in Chapter 5.
  ''' </summary>
  <Transaction(TransactionOption.Required)> _
  <EventTrackingEnabled(True)> _
  <ComVisible(True)> _
  Public Class ServicedDataPortal
    Inherits ServicedComponent

    Implements IDataPortalServer

    ''' <summary>
    ''' Called by the client-side DataPortal to create a new object.
    ''' </summary>
    ''' <remarks>
    ''' This method runs in a distributed transactional context
    ''' within Enterprise Services. To indicate failure and trigger
    ''' a rollback your code must throw an exception.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A populated business object.</returns>
    <AutoComplete(True)> _
    Public Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create

      Dim portal As New SimpleDataPortal
      Return portal.Create(objectType, criteria, context)

    End Function

    ''' <summary>
    ''' Called by the client-side DataProtal to retrieve an object.
    ''' </summary>
    ''' <remarks>
    ''' This method runs in a distributed transactional context
    ''' within Enterprise Services. To indicate failure and trigger
    ''' a rollback your code must throw an exception.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Object containing context data from client.</param>
    ''' <returns>A populated business object.</returns>
    <AutoComplete(True)> _
    Public Function Fetch(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch

      Dim portal As New SimpleDataPortal
      Return portal.Fetch(criteria, context)

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to update an object.
    ''' </summary>
    ''' <remarks>
    ''' This method runs in a distributed transactional context
    ''' within Enterprise Services. To indicate failure and trigger
    ''' a rollback your code must throw an exception.
    ''' </remarks>
    ''' <param name="obj">A reference to the object being updated.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A reference to the newly updated object.</returns>
    <AutoComplete(True)> _
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update

      Dim portal As New SimpleDataPortal
      Return portal.Update(obj, context)

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to delete an object.
    ''' </summary>
    ''' <remarks>
    ''' This method runs in a distributed transactional context
    ''' within Enterprise Services. To indicate failure and trigger
    ''' a rollback your code must throw an exception.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    <AutoComplete(True)> _
    Public Function Delete(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete

      Dim portal As New SimpleDataPortal
      Return portal.Delete(criteria, context)

    End Function

  End Class

End Namespace
