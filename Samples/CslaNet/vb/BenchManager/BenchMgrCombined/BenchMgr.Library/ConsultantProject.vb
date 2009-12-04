Imports System.Data.SqlClient

<Serializable()> _
Public Class ConsultantProject
  Inherits BusinessBase(Of ConsultantProject)

#Region " Business Methods "

  Private _projectId As Integer
  Public ReadOnly Property ProjectId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _projectId
    End Get
  End Property

  Public ReadOnly Property ProjectName() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Dim result As String = ClientProjectNVList.GetList.GetValue(_projectId)
      If result Is Nothing Then
        result = "<not current>"
      End If
      Return result
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return _projectId
  End Function

  Public Overrides Function ToString() As String
    Return ProjectName
  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewConsultantProject(ByVal projectId As Integer)

    Return DataPortal.Create(Of ConsultantProject)(New Criteria(projectId))

  End Function

  Friend Shared Function GetConsultantProject(ByVal dr As Csla.Data.SafeDataReader)

    Return New ConsultantProject(dr)

  End Function

  Private Sub New()
    MarkAsChild()
  End Sub

  Private Sub New(ByVal dr As Csla.Data.SafeDataReader)
    Me.New()
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria

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

  <RunLocal()> _
  Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

    _projectId = criteria.ProjectId

  End Sub

  Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

    _projectId = dr.GetInt32("projectid")
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal parent As ConsultantProjectList)

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "INSERT INTO ConsultantProject (consultantid,projectid) VALUES (@consultantid,@projectid)"
        cm.Parameters.AddWithValue("@consultantid", parent.ConsultantId)
        cm.Parameters.AddWithValue("@projectid", _projectId)
        cm.ExecuteNonQuery()
      End Using
      MarkOld()
    End Using

  End Sub

  Friend Sub DeleteSelf(ByVal parent As ConsultantProjectList)

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "DELETE ConsultantProject WHERE consultantid=@consultantid AND projectid=@projectid"
        cm.Parameters.AddWithValue("@consultantid", parent.ConsultantId)
        cm.Parameters.AddWithValue("@projectid", _projectId)
        cm.ExecuteNonQuery()
      End Using
      MarkNew()
    End Using

  End Sub

#End Region

End Class
