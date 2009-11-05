Imports System.Data.SqlClient

<Serializable()> _
Public Class ConsultantProjectList
  Inherits BusinessListBase(Of ConsultantProjectList, ConsultantProject)

#Region " Business Methods "

  Private _consultantId As Integer
  Public ReadOnly Property ConsultantId() As Integer
    Get
      Return _consultantId
    End Get
  End Property

  Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As ConsultantProject)

    Dim id As Integer = item.ProjectId
    Dim duplicate As Boolean

    For Each child As ConsultantProject In Me
      If Not ReferenceEquals(child, item) AndAlso child.ProjectId = id Then
        duplicate = True
        Exit For
      End If
    Next

    If Not duplicate Then
      MyBase.InsertItem(index, item)
    End If

  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function GetList(ByVal consultantId As Integer)

    Return DataPortal.Fetch(Of ConsultantProjectList)(New Criteria(consultantId))

  End Function

  Public Function GetProject(ByVal projectId As Integer)

    For Each item As ConsultantProject In Me
      If item.ProjectId = projectId Then
        Return item
      End If
    Next
    Return Nothing

  End Function

  Private Sub New()

    AllowEdit = True
    AllowNew = False
    AllowRemove = True

  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    Private _id As Integer
    Public ReadOnly Property ConsultantId() As Integer
      Get
        Return _id
      End Get
    End Property

    Public Sub New(ByVal consultantId As Integer)
      _id = consultantId
    End Sub
  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    _consultantId = criteria.ConsultantId

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT projectid FROM ConsultantProject WHERE consultantid=@id"
        cm.Parameters.AddWithValue("@id", criteria.ConsultantId)
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          While dr.Read
            Add(ConsultantProject.GetConsultantProject(dr))
          End While
        End Using
      End Using
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    For Each child As ConsultantProject In DeletedList
      child.DeleteSelf(Me)
    Next
    DeletedList.Clear()

    For Each child As ConsultantProject In Me
      If child.IsDirty Then
        If child.IsNew Then
          If Not child.IsDeleted Then
            child.Insert(Me)
          End If

        Else
          Throw New NotSupportedException("Updating a row is not supported")
        End If
      End If
    Next

  End Sub

#End Region

End Class
