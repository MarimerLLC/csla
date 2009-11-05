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

  <RunLocal()> _
  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    _consultantId = criteria.ConsultantId

    Dim svc As New BenchService.BenchServiceClient
    Dim list As New List(Of BenchService.ProjectData)(svc.GetProjectList(criteria.ConsultantId))
    For Each item As BenchService.ProjectData In list
      Add(ConsultantProject.GetConsultantProject(item))
    Next

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Update()

    Dim request As New List(Of BenchService.ProjectUpdateData)
    For Each child As ConsultantProject In DeletedList
      Dim data As New BenchService.ProjectUpdateData
      data.ProjectId = child.ProjectId
      data.Delete = True
      request.Add(data)
    Next

    For Each child As ConsultantProject In Me
      If child.IsDirty Then
        If child.IsNew Then
          If Not child.IsDeleted Then
            Dim data As New BenchService.ProjectUpdateData
            data.ProjectId = child.ProjectId
            data.IsNew = True
            request.Add(data)
          End If

        Else
          Throw New NotSupportedException("Updating a row is not supported")
        End If
      End If
    Next

    Dim svc As New BenchService.BenchServiceClient
    svc.UpdateProjects(_consultantId, request.ToArray)

    Me.Clear()
    DeletedList.Clear()

    DataPortal_Fetch(New Criteria(_consultantId))

  End Sub

#End Region

End Class
