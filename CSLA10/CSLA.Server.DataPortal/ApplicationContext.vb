Imports System.Threading
Imports System.Collections.Specialized

''' <summary>
''' Provides consistent context information between the client
''' and server DataPortal objects. 
''' </summary>
Public Class ApplicationContext

  ''' <summary>
  ''' Returns the application-specific context data provided
  ''' by the client.
  ''' </summary>
  ''' <remarks>
  ''' The return value is a NameValueCollection. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </remarks>
  Public Shared ReadOnly Property Current() As NameValueCollection
    Get
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("CSLA.DataPortalContext")
      Dim ctx As NameValueCollection = _
        CType(Thread.GetData(slot), NameValueCollection)
      If ctx Is Nothing Then
        ctx = New NameValueCollection
        Threading.Thread.SetData(slot, ctx)
      End If
      Return ctx
    End Get
  End Property

  ''' <summary>
  ''' Returns the application-specific context data provided
  ''' by the client. Nothing is a valid return.
  ''' </summary>
  Friend Shared Function GetContext() As Object

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.DataPortalContext")
    Return Thread.GetData(slot)

  End Function

  Friend Shared Sub SetContext(ByVal context As NameValueCollection)

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.DataPortalContext")
    Threading.Thread.SetData(slot, context)

  End Sub

  Friend Shared Sub ClearContext()

    Thread.FreeNamedDataSlot("CSLA.DataPortalContext")

  End Sub

  Private Sub New()
    ' prevent instantiation
  End Sub

End Class
