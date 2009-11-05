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

    Dim svc As New SalesService.SalesServiceClient
    Dim projectList() As SalesService.ProjectData = svc.GetProjectList(_clientId)

    ' load business objects from DTOs
    For Each item As SalesService.ProjectData In projectList
      Add(ClientProjectEdit.GetProject(_clientId, item))
    Next

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Update()

    ' copy changed objects to DTOs
    Dim projectList As New List(Of SalesService.ProjectDataUpdate)
    For Each child As ClientProjectEdit In DeletedList
      Dim data As New SalesService.ProjectDataUpdate
      data.Id = child.Id
      data.Delete = True
      projectList.Add(data)
    Next
    For Each child As ClientProjectEdit In Me
      If child.IsDirty Then
        Dim data As New SalesService.ProjectDataUpdate
        Csla.Data.DataMapper.Map(child, data)
        data.IsNew = child.IsNew
        projectList.Add(data)
      End If
    Next

    ' update data on server
    Dim svc As New SalesService.SalesServiceClient
    svc.UpdateProjects(projectList.ToArray)

    ' clear all data
    Me.Clear()
    DeletedList.Clear()

    ' reload list
    DataPortal_Fetch(New Criteria(_clientId))

    ' raise reset event
    OnListChanged(New System.ComponentModel.ListChangedEventArgs(ComponentModel.ListChangedType.Reset, -1))

  End Sub

  Public Overrides Function Save() As ClientProjectList

    Dim result As ClientProjectList = MyBase.Save()
    ClientProjectNVList.FlushCache()
    Return result

  End Function

#End Region

End Class
