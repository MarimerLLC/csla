Imports System.Configuration.ConfigurationManager
Imports System.ComponentModel
Imports System.Data.SqlClient

<Serializable()> _
Public Class OrderList
  Inherits ReadOnlyListBase(Of OrderList, OrderInfo)

#Region " Business Methods "

  Public Function FindById(ByVal id As Integer) As OrderInfo

    For Each item As OrderInfo In Me
      If item.Id = id Then
        Return item
      End If
    Next
    Return Nothing

  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function GetList() As OrderList

    Return DataPortal.Fetch(Of OrderList)()

  End Function

  Private Sub New()

  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    Dim dalTypeName As String = AppSettings("OrderData")
    If Len(dalTypeName) = 0 Then
      Fetch()

    ElseIf dalTypeName.IndexOf("DAL.Direct") > 0 Then
      FetchDal()

    Else
      FetchDto()
    End If

  End Sub

  Private Sub Fetch()

    Try
      Me.RaiseListChangedEvents = False
      IsReadOnly = False
      Using cn As New SqlConnection(ConnectionStrings("DeepData.My.MySettings.DeepDataStoreConnectionString").ConnectionString)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandText = "SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail"
          cm.CommandType = CommandType.Text
          Using data As New SafeDataReader(cm.ExecuteReader)
            LoadOrders(data)
          End Using
        End Using
      End Using

    Finally
      IsReadOnly = True
      Me.RaiseListChangedEvents = True
    End Try

  End Sub

  Private Sub FetchDal()

    Try
      Me.RaiseListChangedEvents = False
      IsReadOnly = False
      Dim df As New DeepData.DAL.DataFactory
      Using dal As DeepData.DAL.OrderData = df.GetOrderDataObject
        Dim data As SafeDataReader = CType(dal.GetOrders, SafeDataReader)
        LoadOrders(data)
      End Using

    Finally
      IsReadOnly = True
      Me.RaiseListChangedEvents = True
    End Try

  End Sub

  Private Sub LoadOrders(ByVal data As SafeDataReader)

    While data.Read
      Add(OrderInfo.GetOrderInfo(data))
    End While

    data.NextResult()
    While data.Read
      FindById(data.GetInt32("OrderId")).LoadItem(data)
    End While

    data.NextResult()
    While data.Read
      FindById(data.GetInt32("OrderId")).LoadDetail(data)
    End While

  End Sub

#End Region

End Class

