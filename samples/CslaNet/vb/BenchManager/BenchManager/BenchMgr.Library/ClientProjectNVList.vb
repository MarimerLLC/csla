Imports System.Data.SqlClient

<Serializable()> _
Public Class ClientProjectNVList
  Inherits NameValueListBase(Of Integer, String)

  Private Shared _list As ClientProjectNVList

  Public Shared Function GetList() As ClientProjectNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ClientProjectNVList)()
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

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT Project.Id, Project.Name AS ProjectName, Client.Name AS ClientName FROM Client INNER JOIN Project ON Project.ClientId = Client.Id ORDER BY Client.Name"
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            Add(New NameValuePair(dr.GetInt32("id"), _
                                  String.Format("{0}: {1}", dr.GetString("ClientName"), dr.GetString("ProjectName"))))
          End While
          IsReadOnly = True
        End Using
      End Using
    End Using

  End Sub

End Class
