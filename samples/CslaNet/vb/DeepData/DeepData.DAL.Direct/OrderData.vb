Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Csla.Data

Public Class OrderData
  Inherits DeepData.DAL.OrderData

  Private _cn As SqlConnection
  Private _cm As SqlCommand
  Private _data As SafeDataReader

  Public Sub New()

    _cn = New SqlConnection(ConnectionStrings("DeepData.My.MySettings.DeepDataStoreConnectionString").ConnectionString)
    _cn.Open()
    _cm = _cn.CreateCommand
    _cm.CommandText = "SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail"
    _cm.CommandType = CommandType.Text
    _data = New SafeDataReader(_cm.ExecuteReader)

  End Sub

  Public Overrides Function GetOrders() As Object

    Return _data

  End Function

#Region " IDisposable Support "

  ' IDisposable
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      _data.Dispose()
      _cm.Dispose()
      _cn.Dispose()
    End If
  End Sub

#End Region

End Class
