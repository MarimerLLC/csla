Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports BenchMgr.Library

Public Class BenchSvc
  Implements IBenchService

  Public Function GetConsultantList(ByVal benchOnly As Boolean) As ConsultantData() Implements IBenchService.GetConsultantList

    Dim list As ConsultantList = ConsultantList.GetList
    Dim result As New List(Of ConsultantData)
    For Each item As ConsultantEdit In list
      If Not benchOnly OrElse item.OnBench Then
        Dim data As New ConsultantData
        Csla.Data.DataMapper.Map(item, data)
        result.Add(data)
      End If
    Next
    Return result.ToArray

  End Function

  Public Function GetConsultant(ByVal id As Integer) As ConsultantData Implements IBenchService.GetConsultant

    Dim list As ConsultantList = ConsultantList.GetConsultant(id)
    Dim result As New ConsultantData
    Csla.Data.DataMapper.Map(list(0), result)
    Return result

  End Function

  Public Function UpdateConsultant(ByVal consultant As ConsultantUpdateData) As ConsultantData Implements IBenchService.UpdateConsultant

    Dim list As ConsultantList
    Dim item As ConsultantEdit = Nothing
    If consultant.IsNew Then
      list = ConsultantList.NewList
      item = list.AddNew

    Else
      list = ConsultantList.GetConsultant(consultant.Id)
      item = list(0)
      If consultant.Delete Then
        item.Delete()
      End If
    End If
    item.Name = consultant.Name
    item.OnBench = consultant.OnBench

    item = item.Save

    Dim result As New ConsultantData
    Csla.Data.DataMapper.Map(item, result)
    Return result

  End Function

  Public Function GetProjectList(ByVal consultantId As Integer) As ProjectData() Implements IBenchService.GetProjectList

    Dim list As ConsultantProjectList = ConsultantProjectList.GetList(consultantId)
    Dim result As New List(Of ProjectData)
    For Each item As ConsultantProject In list
      Dim data As New ProjectData
      data.ProjectId = item.ProjectId
      result.Add(data)
    Next
    Return result.ToArray

  End Function

  Public Sub UpdateProjects(ByVal consultantId As Integer, ByVal projects() As ProjectUpdateData) Implements IBenchService.UpdateProjects

    Dim list As ConsultantProjectList = ConsultantProjectList.GetList(consultantId)
    For Each data As ProjectUpdateData In projects
      If data.Delete Then
        Dim item As ConsultantProject = list.GetProject(data.ProjectId)
        If item IsNot Nothing Then
          list.Remove(item)
        End If

      Else
        ' new entry
        list.Add(ConsultantProject.NewConsultantProject(data.ProjectId))
      End If
    Next
    list.Save()

  End Sub

End Class