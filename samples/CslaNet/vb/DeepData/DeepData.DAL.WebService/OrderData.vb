Public Class OrderData
  Inherits DeepData.DAL.OrderData

  Public Overrides Function GetOrders() As Object

    Dim svc As New DeepDataDAL.DeepDataDAL
    Return svc.GetOrders

  End Function

End Class
