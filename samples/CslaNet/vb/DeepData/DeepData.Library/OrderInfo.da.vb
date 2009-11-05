Option Strict Off

Partial Public Class OrderInfo

  Friend Shared Function GetOrderInfo(ByVal data)

    Return New OrderInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub


  Private Sub Fetch(ByVal data)

    _id = data.Id
    _customer = data.Customer
    _lineItems.LoadItems(data.OrderLines)

  End Sub

End Class
