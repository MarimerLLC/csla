<Serializable()> _
Public Class ResourceAssignment
  Inherits BusinessBase(Of ResourceAssignment)

  Implements IHoldRoles

#Region " Business Methods "

  Private _timestamp(7) As Byte

  Private Shared ProjectIdProperty As PropertyInfo(Of Guid) = RegisterProperty(New PropertyInfo(Of Guid)("ProjectId", "Project id", Guid.Empty))
  Private _projectId As Guid = ProjectIdProperty.DefaultValue
  Public ReadOnly Property ProjectId() As Guid
    Get
      Return GetProperty(Of Guid)(ProjectIdProperty, _projectId)
    End Get
  End Property

  Private Shared ProjectNameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("ProjectName"))
  Private _projectName As String = ProjectNameProperty.DefaultValue
  Public ReadOnly Property ProjectName() As String
    Get
      Return GetProperty(Of String)(ProjectNameProperty, _projectName)
    End Get
  End Property

  Private Shared AssignedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(New PropertyInfo(Of SmartDate)("Assigned"))
  Private _assigned As New SmartDate(Today)
  Public ReadOnly Property Assigned() As String
    Get
      Return GetProperty(Of SmartDate, String)(AssignedProperty, _assigned)
    End Get
  End Property

  Private Shared RoleProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Role"))
  Private _role As Integer = RoleProperty.DefaultValue
  Public Property Role() As Integer Implements IHoldRoles.Role
    Get
      Return GetProperty(Of Integer)(RoleProperty, _role)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Of Integer)(RoleProperty, _role, value)
    End Set
  End Property

  Public Function GetProject() As Project

    CanExecuteMethod("GetProject", True)
    Return Project.GetProject(_projectId)

  End Function

  Public Overrides Function ToString() As String

    Return _projectId.ToString

  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf Assignment.ValidRole, RoleProperty)

  End Sub

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    AuthorizationRules.AllowWrite(RoleProperty, "ProjectManager")

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function NewResourceAssignment( _
    ByVal projectId As Guid) As ResourceAssignment

    Return DataPortal.CreateChild(Of ResourceAssignment)(projectId, RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResourceAssignment( _
    ByVal data As ProjectTracker.DalLinq.Assignment) As ResourceAssignment

    Return DataPortal.FetchChild(Of ResourceAssignment)(data)

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Private Sub New(ByVal project As Project, ByVal role As Integer)

    MarkAsChild()
    _projectId = project.Id
    _projectName = project.Name
    _assigned.Date = Assignment.GetDefaultAssignedDate
    _role = role

  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub Child_Create(ByVal projectId As Guid, ByVal role As Integer)

    Dim proj = Project.GetProject(projectId)
    _projectId = proj.Id
    _projectName = proj.Name
    _assigned.Date = Assignment.GetDefaultAssignedDate
    _role = role

  End Sub

  Private Sub Child_Fetch(ByVal data As ProjectTracker.DalLinq.Assignment)

    _projectId = data.ProjectId
    _projectName = data.Project.Name
    _assigned = data.Assigned
    _role = data.Role
    _timestamp = data.LastChanged.ToArray

  End Sub

  Private Sub Child_Insert(ByVal resource As Resource)

    _timestamp = Assignment.AddAssignment( _
      _projectId, resource.Id, _assigned, _role)

  End Sub

  Private Sub Child_Update(ByVal resource As Resource)

    _timestamp = Assignment.UpdateAssignment( _
      _projectId, resource.Id, _assigned, _role, _timestamp)

  End Sub

  Private Sub Child_DeleteSelf(ByVal resource As Resource)

    Assignment.RemoveAssignment(_projectId, resource.Id)

  End Sub

#End Region

End Class
