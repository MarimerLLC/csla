Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports CSLA.Security

<System.Web.Services.WebService( _
  Namespace:="http://ws.lhotka.net/PTService/ProjectTracker")> _
Public Class ProjectTracker
  Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

  Public Sub New()
    MyBase.New()

    'This call is required by the Web Services Designer.
    InitializeComponent()

    'Add your own initialization code after the InitializeComponent() call

  End Sub

  'Required by the Web Services Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Web Services Designer
  'It can be modified using the Web Services Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    components = New System.ComponentModel.Container
  End Sub

  Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
    'CODEGEN: This procedure is required by the Web Services Designer
    'Do not modify it using the code editor.
    If disposing Then
      If Not (components Is Nothing) Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub

#End Region

#Region " Security "

  Public Class CSLACredentials
    Inherits SoapHeader

    Public Username As String
    Public Password As String
  End Class

  Public Credentials As New CSLACredentials

  Public Sub Login()

    If Len(Credentials.Username) = 0 Then
      Throw New System.Security.SecurityException( _
        "Valid credentials not provided")
    End If

    With Credentials
      BusinessPrincipal.Login(.Username, .Password)
    End With

    Dim principal As System.Security.Principal.IPrincipal = _
      Threading.Thread.CurrentPrincipal

    If principal.Identity.IsAuthenticated Then
      ' the user is valid - set up the HttpContext
      HttpContext.Current.User = principal

    Else
      ' the user is not valid, raise an error
      Throw New System.Security.SecurityException("Invalid user or password")
    End If

  End Sub

#End Region

#Region " Data Structures "

  Public Structure ProjectInfo
    Public ID As String
    Public Name As String
    Public Started As String
    Public Ended As String
    Public Description As String
    Public Resources() As ProjectResourceInfo
  End Structure

  Public Structure ProjectResourceInfo
    Public ResourceID As String
    Public FirstName As String
    Public LastName As String
    Public Assigned As String
    Public Role As String
  End Structure

  Public Structure ResourceInfo
    Public ID As String
    Public Name As String
    Public Assignments() As ResourceAssignmentInfo
  End Structure

  Public Structure ResourceAssignmentInfo
    Public ProjectID As String
    Public Name As String
    Public Assigned As String
    Public Role As String
  End Structure

#End Region

#Region " Projects "

  <WebMethod(Description:="Get a list of projects"), _
   SoapHeader("Credentials")> _
  Public Function GetProjectList() As ProjectInfo()

    Login()

    Dim list As ProjectList = ProjectList.GetProjectList
    Dim info(list.Count - 1) As ProjectInfo
    Dim project As ProjectList.ProjectInfo
    Dim index As Integer

    For Each project In list
      With info(index)
        ' ProjectList only returns 2 of our data fields
        .ID = project.ID.ToString
        .Name = project.Name
      End With
      index += 1
    Next

    Return info

  End Function

  <WebMethod(Description:="Get detailed data for a specific project"), _
   SoapHeader("Credentials")> _
  Public Function GetProject(ByVal ID As String) As ProjectInfo

    Login()

    Dim info As ProjectInfo
    Dim project As Project = project.GetProject(New Guid(ID))

    With info
      .ID = project.ID.ToString
      .Name = project.Name
      .Started = project.Started
      .Ended = project.Ended
      .Description = project.Description

      ' load child objects
      ReDim .Resources(project.Resources.Count - 1)
      Dim idx As Integer
      For idx = 0 To project.Resources.Count - 1
        .Resources(idx).ResourceID = project.Resources(idx).ResourceID
        .Resources(idx).FirstName = project.Resources(idx).FirstName
        .Resources(idx).LastName = project.Resources(idx).LastName
        .Resources(idx).Assigned = project.Resources(idx).Assigned
        .Resources(idx).Role = project.Resources(idx).Role
      Next
    End With

    Return info

  End Function

  <WebMethod(Description:="Add or update a project (only provide the ID field for an update operation)"), _
   SoapHeader("Credentials")> _
  Public Function UpdateProject(ByVal Data As ProjectInfo) As String

    Login()

    Dim project As Project
    If Len(Data.ID) = 0 Then
      ' no id so this is a new project
      project = project.NewProject()

    Else
      ' they provided an id so we are updating a project
      project = project.GetProject(New Guid(Data.ID))
    End If

    With project
      .Name = Data.Name
      .Started = Data.Started
      .Ended = Data.Ended
      .Description = Data.Description

      ' load child objects
      Dim idx As Integer
      For idx = 0 To UBound(Data.Resources)
        If .Resources.Contains(Data.Resources(idx).ResourceID) Then
          ' update existing resource
          ' of course only the role field is changable...
          .Resources(Data.Resources(idx).ResourceID).Role = _
            Data.Resources(idx).Role

        Else
          ' just add new resource
          .Resources.Assign(Data.Resources(idx).ResourceID, Data.Resources(idx).Role)
        End If
      Next
    End With

    project = project.Save

    Return project.ID.ToString

  End Function

  <WebMethod(Description:="Remove a project from the system"), _
   SoapHeader("Credentials")> _
  Public Sub DeleteProject(ByVal ProjectID As String)

    Login()

    Project.DeleteProject(New Guid(ProjectID))

  End Sub

#End Region

#Region " Resources "

  <WebMethod(Description:="Get a list of resources"), _
   SoapHeader("Credentials")> _
  Public Function GetResourceList() As ResourceInfo()

    Login()

    Dim list As ResourceList = ResourceList.GetResourceList
    Dim info(list.Count - 1) As ResourceInfo
    Dim resource As ResourceList.ResourceInfo
    Dim index As Integer

    For Each resource In list
      With info(index)
        .ID = resource.ID
        .Name = resource.Name
      End With
      index += 1
    Next

    Return info

  End Function

  <WebMethod(Description:="Get details for a specific resource"), _
   SoapHeader("Credentials")> _
  Public Function GetResource(ByVal ID As String) As ResourceInfo

    Login()

    Dim info As ResourceInfo
    Dim resource As Resource = resource.GetResource(ID)

    With info
      .ID = resource.ID
      .Name = resource.LastName & ", " & resource.FirstName

      ' load child objects
      ReDim .Assignments(resource.Assignments.Count - 1)
      Dim idx As Integer
      For idx = 0 To resource.Assignments.Count - 1
        .Assignments(idx).ProjectID = resource.Assignments(idx).ProjectID.ToString
        .Assignments(idx).Name = resource.Assignments(idx).ProjectName
        .Assignments(idx).Assigned = resource.Assignments(idx).Assigned
        .Assignments(idx).Role = resource.Assignments(idx).Role
      Next
    End With

    Return info

  End Function

  <WebMethod(Description:="Add or update a resource"), _
   SoapHeader("Credentials")> _
  Public Sub UpdateResource(ByVal Data As ResourceInfo)

    Login()

    Dim Resource As Resource
    Try
      Resource = Resource.GetResource(Data.ID)

    Catch ex As Exception
      ' failed to retrieve resource, so this must be new
      Resource = Resource.NewResource(Data.ID)

    End Try

    With Resource
      .FirstName = Trim(Mid(Data.Name, InStr(Data.Name, ",") + 1))
      .LastName = Left(Data.Name, InStr(Data.Name, ",") - 1)

      ' load child objects
      Dim idx As Integer
      For idx = 0 To UBound(Data.Assignments)
        If .Assignments.Contains(New Guid(Data.Assignments(idx).ProjectID)) Then
          ' update existing resource
          ' of course only the role field is changable...
          .Assignments(idx).Role = Data.Assignments(idx).Role

        Else
          ' just add new resource
          .Assignments.AssignTo( _
            New Guid(Data.Assignments(idx).ProjectID), _
            Data.Assignments(idx).Role)
        End If
      Next
    End With

    Resource = Resource.Save

  End Sub

  <WebMethod(Description:="Remove a resource from the system"), _
   SoapHeader("Credentials")> _
  Public Sub DeleteResource(ByVal ResourceID As String)

    Login()

    Resource.DeleteResource(ResourceID)

  End Sub

#End Region

End Class
