Imports System.Data.SqlClient

Namespace Library

  <Serializable()> _
  Public Class ClientProjectList
    Inherits BusinessListBase(Of ClientProjectList, ClientProjectEdit)

#Region " Factory Methods "

    Public Shared Function GetList(ByVal clientId As Integer) As ClientProjectList

      Return DataPortal.Fetch(Of ClientProjectList)(New Criteria(clientId))

    End Function

    Private Sub New()

      AllowEdit = True
      AllowNew = True
      AllowRemove = True

    End Sub

#End Region

#Region " Data Access "

    <Serializable()> _
    Private Class Criteria

      Private _clientId As Integer
      Public ReadOnly Property ClientId() As Integer
        Get
          Return _clientId
        End Get
      End Property

      Public Sub New(ByVal clientId As Integer)
        _clientId = clientId
      End Sub
    End Class

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

      Using cn As New SqlConnection(Database.SalesConnectionString)
        cn.Open()
        ApplicationContext.LocalContext.Add("cn", cn)
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = System.Data.CommandType.Text
          cm.CommandText = "SELECT id,clientId,name,description FROM Project WHERE clientId=@clientId ORDER BY clientId"
          cm.Parameters.AddWithValue("@clientId", criteria.ClientId)
          Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
            While dr.Read
              Add(ClientProjectEdit.GetProject(dr))
            End While
          End Using
        End Using
        ApplicationContext.LocalContext.Remove("cn")
      End Using

    End Sub

#End Region

  End Class

End Namespace