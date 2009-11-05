Option Strict Off

Partial Public Class LineItemInfo

  Friend Shared Function GetItem(ByVal data)

    Return New LineItemInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub

  Private Sub Fetch(ByVal data)

    _orderId = data.OrderId
    _id = data.Id
    _product = data.Product
    _detailList.LoadDetail(data.OrderLineDetails)

  End Sub

End Class
