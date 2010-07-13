Option Strict Off

Partial Public Class LineItemInfo

  Friend Shared Function GetItem(ByVal data)

    Return New LineItemInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub

  Private Sub Fetch(ByVal data)

    LoadProperty(OrderIdProperty, data.OrderId)
    LoadProperty(IdProperty, data.Id)
    LoadProperty(ProductProperty, data.Product)
    Details.LoadDetail(data.OrderLineDetails)
  End Sub

End Class
