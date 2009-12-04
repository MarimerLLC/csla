Option Strict Off

Partial Public Class OrderList

  Private Sub FetchDto()

    Try
      Me.RaiseListChangedEvents = False
      IsReadOnly = False
      Dim df As New DeepData.DAL.DataFactory
      'Dim data() As DeepData.DTO.OrderDto
      Dim data '() As Object
      Using dal As DeepData.DAL.OrderData = df.GetOrderDataObject
        'data = CType(dal.GetOrders, DeepData.DTO.OrderDto())
        data = dal.GetOrders
        If data IsNot Nothing Then
          Dim order
          For Each order In data 'As DeepData.DTO.OrderDto In data
            Add(OrderInfo.GetOrderInfo(order))
          Next
        End If
      End Using

    Finally
      IsReadOnly = True
      Me.RaiseListChangedEvents = True
    End Try

  End Sub

End Class
