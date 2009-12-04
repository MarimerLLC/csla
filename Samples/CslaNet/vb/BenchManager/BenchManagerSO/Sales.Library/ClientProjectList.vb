Imports System.Data.SqlClient

<Serializable()> _
  Public Class ClientProjectList
  Inherits ReadOnlyListBase(Of ClientProjectList, ClientProjectInfo)

#Region " Business Methods "

  Private _clientId As Integer
  Public ReadOnly Property ClientId() As Integer
    Get
      Return _clientId
    End Get
  End Property

#End Region

#Region " Factory Methods "

  Public Shared Function GetList(ByVal clientId As Integer) As ClientProjectList

    Return DataPortal.Fetch(Of ClientProjectList)(New Criteria(clientId))

  End Function

  Public Shared Function GetProject(ByVal projectId As Integer) As ClientProjectList

    Return DataPortal.Fetch(Of ClientProjectList)(New ProjectCriteria(projectId))

  End Function

  Public Shared Function GetList() As ClientProjectList

    Return DataPortal.Fetch(Of ClientProjectList)()

  End Function

  Private Sub New()

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

  <Serializable()> _
  Private Class ProjectCriteria

    Private _projectId As Integer
    Public ReadOnly Property ProjectId() As Integer
      Get
        Return _projectId
      End Get
    End Property

    Public Sub New(ByVal projectId As Integer)
      _projectId = ProjectId
    End Sub

  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    _clientId = criteria.ClientId

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,clientId,name,description FROM Project WHERE clientId=@clientId ORDER BY clientId"
        cm.Parameters.AddWithValue("@clientId", criteria.ClientId)
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            Add(ClientProjectInfo.GetProject(dr))
          End While
          IsReadOnly = True
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As ProjectCriteria)

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,clientId,name,description FROM Project WHERE id=@projectId"
        cm.Parameters.AddWithValue("@projectId", criteria.ProjectId)
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            _clientId = dr.GetInt32("clientId")
            Add(ClientProjectInfo.GetProject(dr))
          End While
          IsReadOnly = True
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Private Overloads Sub DataPortal_Fetch()

    _clientId = -1

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,clientId,name,description FROM Project ORDER BY clientId"
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            Add(ClientProjectInfo.GetProject(dr))
          End While
          IsReadOnly = True
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

#End Region

End Class
