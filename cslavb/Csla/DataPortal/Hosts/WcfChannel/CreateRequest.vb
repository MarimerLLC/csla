Imports System.Runtime.Serialization

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Request message for creating
  ''' a new business object.
  ''' </summary>
  <DataContract()> _
  Public Class CreateRequest

    <DataMember()> _
    Private _objectType As Type
    <DataMember()> _
    Private _criteria As Object
    <DataMember()> _
    Private _context As Server.DataPortalContext

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">Data portal context from client.</param>
    Public Sub New(ByVal objectType As Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext)
      _objectType = objectType
      _criteria = criteria
      _context = context
    End Sub

    ''' <summary>
    ''' Type of business object to create.
    ''' </summary>
    Public Property ObjectType() As Type
      Get
        Return _objectType
      End Get
      Set(ByVal value As Type)
        _objectType = value
      End Set
    End Property

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
