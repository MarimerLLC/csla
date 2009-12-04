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

  Friend Shared Function GetConsultantProject(ByVal item As BenchService.ProjectData)

    Return New ConsultantProject(item)

  End Function

  Private Sub New()
    MarkAsChild()
  End Sub

  Private Sub New(ByVal item As BenchService.ProjectData)
    Me.New()
    Fetch(item)
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

  Private Sub Fetch(ByVal item As BenchService.ProjectData)

    _projectId = item.ProjectId
    MarkOld()

  End Sub

#End Region

End Class
