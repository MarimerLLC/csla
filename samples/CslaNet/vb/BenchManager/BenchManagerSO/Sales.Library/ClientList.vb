Imports System.Data.SqlClient

<Serializable()> _
  Public Class ClientList
  Inherits ReadOnlyListBase(Of ClientList, ClientInfo)

#Region " Factory Methods "

  Public Shared Function GetList() As ClientList

    Return DataPortal.Fetch(Of ClientList)()

  End Function

  Private Sub New()

  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,name FROM Client ORDER BY name"
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            Add(ClientInfo.GetClient(dr))
          End While
          IsReadOnly = True
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

#End Region

End Class
