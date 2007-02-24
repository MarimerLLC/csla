Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.Serialization

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Request message for updating
  ''' a business object.
  ''' </summary>
  <DataContract()> _
  Public Class UpdateRequest
    <DataMember()> _
    Private _object As Object
    <DataMember()> _
    Private _context As Server.DataPortalContext

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">Data portal context from client.</param>
    Public Sub New(ByVal obj As Object, ByVal context As Server.DataPortalContext)
      _object = obj
      _context = context
    End Sub

    ''' <summary>
    ''' Business object to be updated.
    ''' </summary>
    Public Property [Object]() As Object
      Get
        Return _object
      End Get
      Set(ByVal value As Object)
        _object = value
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
