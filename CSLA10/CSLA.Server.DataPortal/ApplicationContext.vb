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
  ''' <para>
  ''' The return value is a NameValueCollection. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </para><para>
  ''' Note that data in this context is transferred from
  ''' the client to the server. No data is transferred from
  ''' the server to the client.
  ''' <para>
  ''' </remarks>
  Public Shared ReadOnly Property ClientContext() As NameValueCollection
    Get
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("CSLA.ClientContext")
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
  ''' Returns the application-specific context data shared
  ''' on both client and server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The return value is a NameValueCollection. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </para><para>
  ''' Note that data in this context is transferred to and from
  ''' the client and server. Any objects or data in this context
  ''' will be transferred bi-directionally across the network.
  ''' <para>
  ''' </remarks>
  Public Shared ReadOnly Property GlobalContext() As NameValueCollection
    Get
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("CSLA.GlobalContext")
      Dim ctx As NameValueCollection = _
        CType(Thread.GetData(slot), NameValueCollection)
      If ctx Is Nothing Then
        ctx = New NameValueCollection
        Threading.Thread.SetData(slot, ctx)
      End If
      Return ctx
    End Get
  End Property

  Friend Shared Function GetClientContext() As Object

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.ClientContext")
    Return Thread.GetData(slot)

  End Function

  Friend Shared Function GetGlobalContext() As Object

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.GlobalContext")
    Return Thread.GetData(slot)

  End Function

  Friend Shared Sub SetContext(ByVal clientContext As NameValueCollection, ByVal globalContext As NameValueCollection)

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.ClientContext")
    Threading.Thread.SetData(slot, clientContext)

    slot = Thread.GetNamedDataSlot("CSLA.GlobalContext")
    Threading.Thread.SetData(slot, globalContext)

  End Sub

  Friend Shared Sub ClearContext()

    Thread.FreeNamedDataSlot("CSLA.ClientContext")
    Thread.FreeNamedDataSlot("CSLA.GlobalContext")

  End Sub

  Private Sub New()
    ' prevent instantiation
  End Sub

End Class
