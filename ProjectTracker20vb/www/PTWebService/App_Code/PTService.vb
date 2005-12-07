Option Strict On

Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Collections.Generic
Imports ProjectTracker.Library

<WebService(Namespace:="http://ws.lhotka.net/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class PTService
  Inherits System.Web.Services.WebService

#Region " Projects "

  <WebMethod(Description:="Get a list of projects")> _
  Public Function GetProjectList() As ProjectInfo()

    ProjectTracker.Library.Security.PTPrincipal.Logout()

    Dim list As ProjectList = ProjectList.GetProjectList
    Dim result As New List(Of ProjectInfo)
    For Each item As ProjectList.ProjectInfo In list
      Dim info As New ProjectInfo
      Csla.Data.DataMapper.Map(item, info)
      result.Add(info)
    Next
    Return result.ToArray

  End Function

  <WebMethod(Description:="Get a project")> _
  Public Function GetProject(ByVal id As String) As ProjectInfo

    ProjectTracker.Library.Security.PTPrincipal.Logout()

    Dim proj As Project = Project.GetProject(New Guid(id))
    Dim result As New ProjectInfo
    Csla.Data.DataMapper.Map(proj, result, "Resources", _
      "IsDirty", "IsNew", "IsDeleted", "IsValid", "IsSavable", "BrokenRulesCollection")
    For Each resource As ProjectResource In proj.Resources
      Dim info As New ProjectResourceInfo
      Csla.Data.DataMapper.Map(resource, info, "FullName", _
        "IsDirty", "IsNew", "IsDeleted", "IsValid", "IsSavable", "BrokenRulesCollection")
      result.AddResource(info)
    Next
    Return result

  End Function

#End Region

End Class
