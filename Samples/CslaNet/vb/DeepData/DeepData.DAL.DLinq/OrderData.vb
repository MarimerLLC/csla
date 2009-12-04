Public Class OrderData
  Inherits DeepData.DAL.OrderData

  Private db As New DataClasses1DataContext

  Public Overrides Function GetOrders() As Object

    Dim q = From o In db.Orders
    Dim r() = q.ToArray
    Return r

  End Function

#Region " IDisposable "

  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      db.Dispose()
    End If
  End Sub

#End Region

End Class
