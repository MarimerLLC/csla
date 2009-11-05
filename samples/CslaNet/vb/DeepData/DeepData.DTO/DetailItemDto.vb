Public Class DetailItemDto

  Private _orderId As Integer
  Public Property OrderId() As Integer
    Get
      Return _orderId
    End Get
    Set(ByVal value As Integer)
      _orderId = value
    End Set
  End Property

  Private _lineId As Integer
  Public Property LineId() As Integer
    Get
      Return _lineId
    End Get
    Set(ByVal value As Integer)
      _lineId = value
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

  Private _detail As String
  Public Property Detail() As String
    Get
      Return _detail
    End Get
    Set(ByVal value As String)
      _detail = value
    End Set
  End Property

End Class
