Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports Sales.Library

' Use a data contract as illustrated in the sample below to add composite 
' types to service operations
Public Class SalesSvc
  Implements ISalesService

  Public Function GetClientList() As ClientData() Implements ISalesService.GetClientList

    Dim list As ClientList = ClientList.GetList
    Dim result As New List(Of ClientData)
    For Each item As ClientInfo In list
      Dim data As New ClientData
      Csla.Data.DataMapper.Map(item, data)
      result.Add(data)
    Next
    Return result.ToArray

  End Function

  Public Sub DeleteClient(ByVal clientId As Integer) Implements ISalesService.DeleteClient

    ClientEdit.DeleteClient(clientId)

  End Sub

  Public Sub DeleteProject(ByVal projectId As Integer) Implements ISalesService.DeleteProject

    ClientProjectEdit.DeleteProject(projectId)

  End Sub

  Public Function GetProjectList(ByVal clientId As Integer) As ProjectData() Implements ISalesService.GetProjectList

    Dim list As ClientProjectList = ClientProjectList.GetList(clientId)
    Dim result As New List(Of ProjectData)
    For Each item As ClientProjectInfo In list
      Dim data As New ProjectData
      Csla.Data.DataMapper.Map(item, data, "ClientId")
      result.Add(data)
    Next
    Return result.ToArray

  End Function

  Public Function GetProject(ByVal projectId As Integer) As ProjectData Implements ISalesService.GetProject

    Dim list As ClientProjectList = ClientProjectList.GetProject(projectId)
    If list.Count > 0 Then
      Dim result As New ProjectData
      Csla.Data.DataMapper.Map(list(0), result, "ClientId")
      Return result

    Else
      Return Nothing
    End If

  End Function

  Public Function GetFullProjectList() As ProjectSummaryData() Implements ISalesService.GetFullProjectList

    Dim list As ClientProjectNVList = ClientProjectNVList.GetList
    Dim result As New List(Of ProjectSummaryData)
    For Each item As ClientProjectNVList.NameValuePair In list
      Dim data As New ProjectSummaryData
      data.Id = item.Key
      data.Name = item.Value
      result.Add(data)
    Next
    Return result.ToArray

  End Function

  Public Sub UpdateClients(ByVal clientList As ClientDataUpdate()) Implements ISalesService.UpdateClients

    Dim list As New List(Of ClientEdit)
    For Each client As ClientDataUpdate In clientList
      ' create new business object
      Dim item As ClientEdit = ClientEdit.NewClient(client.Id)
      ' copy values from DTO into business object
      Csla.Data.DataMapper.Map(client, item, "Id", "Delete", "IsNew")
      If client.Delete Then
        ' mark item for deletion
        item.MarkForUpdate()
        item.Delete()

      ElseIf Not client.IsNew Then
        ' mark item for update (default is insert)
        item.MarkForUpdate()
      End If
      list.Add(item)
    Next

    ' update the list
    ClientUpdater.UpdateClients(list.ToArray)

  End Sub

  Public Sub UpdateProjects(ByVal projectList As ProjectDataUpdate()) Implements ISalesService.UpdateProjects

    Dim list As New List(Of ClientProjectEdit)
    For Each project As ProjectDataUpdate In projectList
      ' create new business object
      Dim item As ClientProjectEdit = ClientProjectEdit.NewProject(project.Id)
      ' copy values from DTO into business object
      Csla.Data.DataMapper.Map(project, item, "Id", "Delete", "IsNew")
      If project.Delete Then
        ' mark item for deletion
        item.MarkForUpdate()
        item.Delete()

      ElseIf Not project.IsNew Then
        ' mark item for update (default is insert)
        item.MarkForUpdate()
      End If
      list.Add(item)
    Next

    ' update the list
    ClientProjectUpdater.UpdateProjects(list.ToArray)

  End Sub

  Public Function Test() As String Implements ISalesService.Test
    Return "Hello world"
  End Function

End Class
