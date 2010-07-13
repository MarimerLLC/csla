Option Strict Off

Partial Public Class LineItemDetailInfo

  Friend Shared Function GetItem(ByVal data)

    Return New LineItemDetailInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub

  Private Sub Fetch(ByVal data)

    LoadProperty(OrderIdProperty, data.OrderId)
    LoadProperty(LineIdProperty, data.LineId)
    LoadProperty(IdProperty, data.Id)
    LoadProperty(DetailProperty, data.Detail)

  End Sub

End Class
