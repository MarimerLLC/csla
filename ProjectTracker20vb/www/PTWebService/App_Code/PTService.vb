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

  ''' <summary>
  ''' Credentials for custom authentication.
  ''' </summary>
  Public Credentials As New CslaCredentials

#Region " Projects "

  <WebMethod(Description:="Get a list of projects")> _
  Public Function GetProjectList() As ProjectInfo()

    ' anonymous access allowed
    Security.UseAnonymous()

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

    ' anonymous access allowed
    Security.UseAnonymous()

    Dim proj As Project = Project.GetProject(New Guid(id))
    Dim result As New ProjectInfo
    Csla.Data.DataMapper.Map(proj, result, "Resources")
    For Each resource As ProjectResource In proj.Resources
      Dim info As New ProjectResourceInfo
      Csla.Data.DataMapper.Map(resource, info, "FullName")
      result.AddResource(info)
    Next
    Return result

  End Function

  <WebMethod(Description:="Add a project")> _
  <SoapHeader("Credentials")> _
  Public Function AddProject(ByVal name As String, ByVal started As String, ByVal ended As String, ByVal description As String) As ProjectInfo

    ' user credentials required
    Security.Login(Credentials)

    Dim proj As Project = Project.NewProject
    With proj
      .Name = name
      .Started = started
      .Ended = ended
      .Description = description
    End With
    proj = proj.Save

    Dim result As New ProjectInfo
    Csla.Data.DataMapper.Map(proj, result, "Resources")
    Return result

  End Function

  <WebMethod(Description:="Add a project")> _
  <SoapHeader("Credentials")> _
  Public Function EditProject(ByVal id As Guid, ByVal name As String, _
    ByVal started As String, ByVal ended As String, _
    ByVal description As String) As ProjectInfo

    ' user credentials required
    Security.Login(Credentials)

    Dim proj As Project = Project.GetProject(id)
    With proj
      .Name = name
      .Started = started
      .Ended = ended
      .Description = description
    End With
    proj = proj.Save

    Dim result As New ProjectInfo
    Csla.Data.DataMapper.Map(proj, result, "Resources")
    Return result

  End Function

#End Region

#Region " Resources "

  <WebMethod(Description:="Get a list of resources")> _
  Public Function GetResourceList() As ResourceInfo()

    ' anonymous access allowed
    Security.UseAnonymous()

    Dim list As ResourceList = ResourceList.GetResourceList
    Dim result As New List(Of ResourceInfo)
    For Each item As ResourceList.ResourceInfo In list
      Dim info As New ResourceInfo
      Csla.Data.DataMapper.Map(item, info)
      result.Add(info)
    Next
    Return result.ToArray

  End Function

  <WebMethod(Description:="Get a resource")> _
  Public Function GetResource(ByVal id As Integer) As ResourceInfo

    ' anonymous access allowed
    Security.UseAnonymous()

    Dim res As Resource = Resource.GetResource(id)
    Dim result As New ResourceInfo
    result.Id = res.Id
    result.Name = String.Format("{1}, {0}", res.FirstName, res.LastName)
    For Each resource As ResourceAssignment In res.Assignments
      Dim info As New ResourceAssignmentInfo
      Csla.Data.DataMapper.Map(resource, info)
      result.AddProject(info)
    Next
    Return result

  End Function

  <WebMethod(Description:="Change a resource's name")> _
  <SoapHeader("Credentials")> _
  Public Function ChangeResourceName(ByVal id As Integer, _
    ByVal firstName As String, ByVal lastName As String) As ResourceInfo

    ' user credentials required
    Security.Login(Credentials)

    Try
      Dim res As Resource = Resource.GetResource(id)
      With res
        .FirstName = firstName
        .LastName = lastName
      End With
      res = res.Save

      Dim result As New ResourceInfo
      result.Id = res.Id
      result.Name = String.Format("{1}, {0}", res.FirstName, res.LastName)
      Return result

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

#End Region

#Region " Assign Resource to Project "

  <WebMethod(Description:="Assign resource to a project")> _
  <SoapHeader("Credentials")> _
  Public Sub AssignResource(ByVal resourceId As Integer, ByVal projectId As Guid)

    ' user credentials required
    Security.Login(Credentials)

    Dim res As Resource = Resource.GetResource(resourceId)
    res.Assignments.AssignTo(projectId)
    res.Save()

  End Sub

#End Region

#Region " Roles "

  <WebMethod(Description:="Get a list of roles")> _
  Public Function GetRoles() As RoleInfo()

    ' anonymous access allowed
    Security.UseAnonymous()

    Dim list As RoleList = RoleList.GetList
    Dim result As New List(Of RoleInfo)
    For Each role As RoleList.NameValuePair In list
      Dim info As New RoleInfo
      info.Id = role.Key
      info.Name = role.Value
      result.Add(info)
    Next
    Return result.ToArray

  End Function

#End Region

End Class
