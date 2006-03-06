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
  Public Function GetProjectList() As ProjectData()

    ' anonymous access allowed
    Security.UseAnonymous()

    Try
      Dim list As ProjectList = ProjectList.GetProjectList
      Dim result As New List(Of ProjectData)
      For Each item As ProjectInfo In list
        Dim info As New ProjectData
        Csla.Data.DataMapper.Map(item, info)
        result.Add(info)
      Next
      Return result.ToArray

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

  <WebMethod(Description:="Get a project")> _
  Public Function GetProject(ByVal request As ProjectRequest) As ProjectData

    ' anonymous access allowed
    Security.UseAnonymous()

    Try
      Dim proj As Project = Project.GetProject(request.Id)
      Dim result As New ProjectData
      Csla.Data.DataMapper.Map(proj, result, "Resources")
      For Each resource As ProjectResource In proj.Resources
        Dim info As New ProjectResourceData
        Csla.Data.DataMapper.Map(resource, info, "FullName")
        result.AddResource(info)
      Next
      Return result

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

  <WebMethod(Description:="Add a project")> _
  <SoapHeader("Credentials")> _
  Public Function AddProject( _
    ByVal name As String, ByVal started As String, ByVal ended As String, _
    ByVal description As String) As ProjectData

    ' user credentials required
    Security.Login(Credentials)

    Try
      Dim proj As Project = Project.NewProject
      With proj
        .Name = name
        .Started = started
        .Ended = ended
        .Description = description
      End With
      proj = proj.Save

      Dim result As New ProjectData
      Csla.Data.DataMapper.Map(proj, result, "Resources")
      Return result

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

  <WebMethod(Description:="Edit a project")> _
  <SoapHeader("Credentials")> _
  Public Function EditProject(ByVal id As Guid, ByVal name As String, _
    ByVal started As String, ByVal ended As String, _
    ByVal description As String) As ProjectData

    ' user credentials required
    Security.Login(Credentials)

    Try
      Dim proj As Project = Project.GetProject(id)
      With proj
        .Name = name
        .Started = started
        .Ended = ended
        .Description = description
      End With
      proj = proj.Save

      Dim result As New ProjectData
      Csla.Data.DataMapper.Map(proj, result, "Resources")
      Return result

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

#End Region

#Region " Resources "

  <WebMethod(Description:="Get a list of resources")> _
  Public Function GetResourceList() As ResourceData()

    ' anonymous access allowed
    Security.UseAnonymous()

    Try
      Dim list As ResourceList = ResourceList.GetResourceList
      Dim result As New List(Of ResourceData)
      For Each item As ResourceInfo In list
        Dim info As New ResourceData
        Csla.Data.DataMapper.Map(item, info)
        result.Add(info)
      Next
      Return result.ToArray

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

  <WebMethod(Description:="Get a resource")> _
  Public Function GetResource(ByVal request As ResourceRequest) As ResourceData

    ' anonymous access allowed
    Security.UseAnonymous()

    Try
      Dim res As Resource = Resource.GetResource(request.Id)
      Dim result As New ResourceData
      result.Id = res.Id
      result.Name = res.FullName
      For Each resource As ResourceAssignment In res.Assignments
        Dim info As New ResourceAssignmentData
        Csla.Data.DataMapper.Map(resource, info)
        result.AddProject(info)
      Next
      Return result

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

  <WebMethod(Description:="Change a resource's name")> _
  <SoapHeader("Credentials")> _
  Public Function ChangeResourceName(ByVal id As Integer, _
    ByVal firstName As String, ByVal lastName As String) As ResourceData

    ' user credentials required
    Security.Login(Credentials)

    Try
      Dim res As Resource = Resource.GetResource(id)
      With res
        .FirstName = firstName
        .LastName = lastName
      End With
      res = res.Save

      Dim result As New ResourceData
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

    Try
      Dim res As Resource = Resource.GetResource(resourceId)
      res.Assignments.AssignTo(projectId)
      res.Save()

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Sub

#End Region

#Region " Roles "

  <WebMethod(Description:="Get a list of roles")> _
  Public Function GetRoles() As RoleData()

    ' anonymous access allowed
    Security.UseAnonymous()

    Try
      Dim list As RoleList = RoleList.GetList
      Dim result As New List(Of RoleData)
      For Each role As RoleList.NameValuePair In list
        Dim info As New RoleData
        info.Id = role.Key
        info.Name = role.Value
        result.Add(info)
      Next
      Return result.ToArray

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

#End Region

End Class
