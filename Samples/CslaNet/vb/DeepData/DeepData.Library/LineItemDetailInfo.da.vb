Option Strict Off

Partial Public Class LineItemDetailInfo

  Friend Shared Function GetItem(ByVal data)

    Return New LineItemDetailInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub

  Private Sub Fetch(ByVal data)

    _orderId = data.OrderId
    _lineId = data.LineId
    _id = data.Id
    _detail = data.Detail

  End Sub

End Class
