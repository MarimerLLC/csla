Imports System.Data.SqlClient

<Serializable()> _
Public Class ClientNVList
  Inherits NameValueListBase(Of Integer, String)

  Public ReadOnly Property DefaultValue() As Integer
    Get
      If Me.Count > 0 Then
        Return Me(0).Key

      Else
        Return -1
      End If
    End Get
  End Property

  Private Shared _list As ClientNVList

  Public Shared Function GetList() As ClientNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ClientNVList)()
    End If
    Return _list

  End Function

  Public Shared Sub FlushCache()

    _list = Nothing

  End Sub

  Private Sub New()

    ' require use of factory methods

  End Sub

  Private Overloads Sub DataPortal_Fetch()

    Dim svc As New SalesService.SalesServiceClient
    Dim clientList() As SalesService.ClientData = svc.GetClientList
    ' load business objects from DTOs
    IsReadOnly = False
    For Each item As SalesService.ClientData In clientList
      Add(New NameValuePair(item.Id, item.Name))
    Next
    IsReadOnly = True

  End Sub

End Class
