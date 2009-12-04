Public Class OrderDto

  Private _id As Integer
  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Private _customer As String
  Public Property Customer() As String
    Get
      Return _customer
    End Get
    Set(ByVal value As String)
      _customer = value
    End Set
  End Property

  Private _lineItems As List(Of LineItemDto)
  <Xml.Serialization.XmlIgnore()> _
  Public ReadOnly Property OrderLinesList() As List(Of LineItemDto)
    Get
      If _lineItems Is Nothing Then
        _lineItems = New List(Of LineItemDto)
      End If
      Return _lineItems
    End Get
  End Property

  Public Property OrderLines() As LineItemDto()
    Get
      Return OrderLinesList.ToArray
    End Get
    Set(ByVal value As LineItemDto())
      _lineItems = New List(Of LineItemDto)(value)
    End Set
  End Property

End Class
