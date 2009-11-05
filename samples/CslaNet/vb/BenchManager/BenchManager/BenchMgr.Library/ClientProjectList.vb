Imports System.Data.SqlClient

<Serializable()> _
  Public Class ClientProjectList
  Inherits BusinessListBase(Of ClientProjectList, ClientProjectEdit)

#Region " Business Methods "

  Private _clientId As Integer
  Public ReadOnly Property ClientId() As Integer
    Get
      Return _clientId
    End Get
  End Property

  Protected Overrides Function AddNewCore() As Object

    Dim item As ClientProjectEdit = ClientProjectEdit.NewProject(ClientId)
    Add(item)
    Return item

  End Function

#End Region

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

    _clientId = criteria.ClientId

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

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      For Each child As ClientProjectEdit In DeletedList
        If Not child.IsNew Then
          child.DeleteSelf(Me)
        End If
      Next
      DeletedList.Clear()

      For Each child As ClientProjectEdit In Me
        If child.IsDirty Then
          If child.IsNew Then
            If Not child.IsDeleted Then
              child.Insert(Me)
            End If
          Else
            child.Update(Me)
          End If
        End If
      Next
      ApplicationContext.LocalContext.Remove("cn")
    End Using
    If ApplicationContext.ExecutionLocation = ExecutionLocations.Server Then
      ClientProjectNVList.FlushCache()
    End If

  End Sub

  Public Overrides Function Save() As ClientProjectList

    Dim result As ClientProjectList = MyBase.Save()
    ClientProjectNVList.FlushCache()
    Return result

  End Function

#End Region

End Class
