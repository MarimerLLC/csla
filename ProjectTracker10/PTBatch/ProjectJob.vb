Imports CSLA.BatchQueue
Imports ProjectTracker.Library

Public Class ProjectJob

  Implements IBatchEntry

  Public Sub Execute(ByVal State As Object) _
      Implements CSLA.BatchQueue.IBatchEntry.Execute

    Dim projects As ProjectList = ProjectList.GetProjectList
    Dim info As ProjectList.ProjectInfo
    Dim project As Project

    For Each info In projects
      project = project.GetProject(info.ID)
      project.Name = project.Name & " (batch)"
      project.Save()
    Next

  End Sub

End Class
