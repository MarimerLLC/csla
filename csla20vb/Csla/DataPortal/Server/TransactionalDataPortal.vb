Imports System.Transactions

Namespace Server

  ''' <summary>
  ''' Implements the server-side Serviced 
  ''' DataPortal described in Chapter 4.
  ''' </summary>
  Public Class TransactionalDataPortal

    Implements IDataPortalServer

    ''' <summary>
    ''' Wraps a Create call in a TransactionScope
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a
    ''' <see cref="TransactionScope">TransactionScope</see>
    ''' to provide transactional support via
    ''' System.Transactions.
    ''' </remarks>
    ''' <param name="objectType">A <see cref="Type">Type</see> object
    ''' indicating the type of business object to be created.</param>
    ''' <param name="criteria">A custom criteria object providing any
    ''' extra information that may be required to properly create
    ''' the object.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A populated business object.</returns>
    Public Function Create( _
      ByVal objectType As System.Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Dim result As DataPortalResult
      Using tr As New TransactionScope
        Dim portal As New SimpleDataPortal
        result = portal.Create(objectType, criteria, context)
        tr.Complete()
      End Using
      Return result

    End Function


    ''' <summary>
    ''' Wraps a Fetch call in a TransactionScope
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a
    ''' <see cref="TransactionScope">TransactionScope</see>
    ''' to provide transactional support via
    ''' System.Transactions.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Object containing context data from client.</param>
    ''' <returns>A populated business object.</returns>
    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Dim result As DataPortalResult
      Using tr As New TransactionScope
        Dim portal As New SimpleDataPortal
        result = portal.Fetch(criteria, context)
        tr.Complete()
      End Using
      Return result

    End Function

    ''' <summary>
    ''' Wraps an Update call in a TransactionScope
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a
    ''' <see cref="TransactionScope">TransactionScope</see>
    ''' to provide transactional support via
    ''' System.Transactions.
    ''' </remarks>
    ''' <param name="obj">A reference to the object being updated.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A reference to the newly updated object.</returns>
    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Dim result As DataPortalResult
      Using tr As New TransactionScope
        Dim portal As New SimpleDataPortal
        result = portal.Update(obj, context)
        tr.Complete()
      End Using
      Return result

    End Function

    ''' <summary>
    ''' Wraps a Delete call in a TransactionScope
    ''' </summary>
    ''' <remarks>
    ''' This method delegates to 
    ''' <see cref="SimpleDataPortal">SimpleDataPortal</see>
    ''' but wraps that call within a
    ''' <see cref="TransactionScope">TransactionScope</see>
    ''' to provide transactional support via
    ''' System.Transactions.
    ''' </remarks>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Dim result As DataPortalResult
      Using tr As New TransactionScope
        Dim portal As New SimpleDataPortal
        result = portal.Delete(criteria, context)
        tr.Complete()
      End Using
      Return result

    End Function

  End Class

End Namespace