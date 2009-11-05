Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Csla.Data
Imports DeepData.DTO

Public Class OrderData
  Inherits DeepData.DAL.OrderData

  Private _orders As List(Of OrderDto)

  Public Sub New()

    Using cn As New SqlConnection(ConnectionStrings("DeepData.My.MySettings.DeepDataStoreConnectionString").ConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandText = "SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail"
        cm.CommandType = CommandType.Text
        Using data As New SafeDataReader(cm.ExecuteReader)
          _orders = New List(Of OrderDto)
          While data.Read
            Dim item As New OrderDto
            item.Id = data.GetInt32("Id")
            item.Customer = data.GetString("Customer")
            _orders.Add(item)
          End While

          data.NextResult()
          While data.Read
            LoadItem(data)
          End While

          data.NextResult()
          While data.Read
            LoadDetail(data)
          End While

        End Using
      End Using
    End Using

  End Sub

  Private Sub LoadItem(ByVal data As SafeDataReader)

    Dim orderId As Integer = data.GetInt32("OrderId")
    For Each order As OrderDto In _orders
      If order.Id = orderId Then
        Dim item As New LineItemDto
        item.OrderId = orderId
        item.Id = data.GetInt32("Id")
        item.Product = data.GetString("Product")
        order.OrderLinesList.Add(item)
      End If
    Next

  End Sub

  Private Sub LoadDetail(ByVal data As SafeDataReader)

    Dim orderId As Integer = data.GetInt32("OrderId")
    For Each order As OrderDto In _orders
      If order.Id = orderId Then
        Dim lineId As Integer = data.GetInt32("LineId")
        For Each line As LineItemDto In order.OrderLinesList
          If line.Id = lineId Then
            Dim item As New DetailItemDto
            item.OrderId = orderId
            item.LineId = lineId
            item.Id = data.GetInt32("Id")
            item.Detail = data.GetString("Detail")
            line.OrderLineDetailsList.Add(item)
          End If
        Next
      End If
    Next

  End Sub

  Public Overrides Function GetOrders() As Object

    Return _orders.ToArray

  End Function

End Class
