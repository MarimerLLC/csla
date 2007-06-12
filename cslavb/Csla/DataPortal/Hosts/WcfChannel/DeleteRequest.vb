#If Not NET20 Then
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.Serialization

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Request message for deleting
  ''' a business object.
  ''' </summary>
  <DataContract()> _
  Public Class DeleteRequest
    <DataMember()> _
    Private _criteria As Object
    <DataMember()> _
    Private _context As Server.DataPortalContext

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">Data portal context from client.</param>
    Public Sub New(ByVal criteria As Object, ByVal context As Server.DataPortalContext)
      _criteria = criteria
      _context = context
    End Sub

    ''' <summary>
    ''' Criteria object describing business object.
    ''' </summary>
    Public Property Criteria() As Object
      Get
        Return _criteria
      End Get
      Set(ByVal value As Object)
        _criteria = value
      End Set
    End Property

    ''' <summary>
    ''' Data portal context from client.
    ''' </summary>
    Public Property Context() As Server.DataPortalContext
      Get
        Return _context
      End Get
      Set(ByVal value As Server.DataPortalContext)
        _context = value
      End Set
    End Property
  End Class
End Namespace
#End If