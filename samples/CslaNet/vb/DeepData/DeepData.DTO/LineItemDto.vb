Public Class LineItemDto

  Private _orderId As Integer
  Public Property OrderId() As Integer
    Get
      Return _orderId
    End Get
    Set(ByVal value As Integer)
      _orderId = value
    End Set
  End Property

  Private _id As Integer
  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Private _product As String
  Public Property Product() As String
    Get
      Return _product
    End Get
    Set(ByVal value As String)
      _product = value
    End Set
  End Property

  Private _detailItems As List(Of DetailItemDto)
  <Xml.Serialization.XmlIgnore()> _
  Public ReadOnly Property OrderLineDetailsList() As List(Of DetailItemDto)
    Get
      If _detailItems Is Nothing Then
        _detailItems = New List(Of DetailItemDto)
      End If
      Return _detailItems
    End Get
  End Property

  Public Property OrderLineDetails() As DetailItemDto()
    Get
      Return OrderLineDetailsList.ToArray
    End Get
    Set(ByVal value As DetailItemDto())
      _detailItems = New List(Of DetailItemDto)(value)
    End Set
  End Property

End Class
