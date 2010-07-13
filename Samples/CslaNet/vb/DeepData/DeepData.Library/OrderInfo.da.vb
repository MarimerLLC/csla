Option Strict Off

Partial Public Class OrderInfo

  Friend Shared Function GetOrderInfo(ByVal data)

    Return New OrderInfo(data)

  End Function

  Private Sub New(ByVal data)

    Fetch(data)

  End Sub


  Private Sub Fetch(ByVal data)

    LoadProperty(IdProperty, data.Id)
    LoadProperty(CustomerProperty, data.Customer)
    LineItems.LoadItems(data.OrderLines)

  End Sub

End Class
