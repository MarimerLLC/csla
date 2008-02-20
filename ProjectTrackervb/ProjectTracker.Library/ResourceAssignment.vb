<Serializable()> _
Public Class ResourceAssignment
  Inherits BusinessBase(Of ResourceAssignment)

  Implements IHoldRoles

#Region " Business Methods "

  Private mTimestamp(7) As Byte

  Private Shared ProjectIdProperty As PropertyInfo(Of Guid) = RegisterProperty(Of Guid)(GetType(ResourceAssignment), New PropertyInfo(Of Guid)("ProjectId", Guid.Empty))
  Private mProjectId As Guid = ProjectIdProperty.DefaultValue
  Public ReadOnly Property ProjectId() As Guid
    Get
      Return GetProperty(Of Guid)(ProjectIdProperty, mProjectId)
    End Get
  End Property

  Private Shared ProjectNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(ResourceAssignment), New PropertyInfo(Of String)("ProjectName"))
  Private mProjectName As String = ProjectNameProperty.DefaultValue
  Public ReadOnly Property ProjectName() As String
    Get
      Return GetProperty(Of String)(ProjectNameProperty, mProjectName)
    End Get
  End Property

  Private Shared AssignedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(GetType(ResourceAssignment), New PropertyInfo(Of SmartDate)("Assigned"))
  Private mAssigned As New SmartDate(Today)
  Public ReadOnly Property Assigned() As String
    Get
      Return GetProperty(Of SmartDate, String)(AssignedProperty, mAssigned)
    End Get
  End Property

  Private Shared RoleProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(GetType(ResourceAssignment), New PropertyInfo(Of Integer)("Role"))
  Private mRole As Integer = RoleProperty.DefaultValue
  Public Property Role() As Integer Implements IHoldRoles.Role
    Get
      Return GetProperty(Of Integer)(RoleProperty, mRole)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Of Integer)(RoleProperty, mRole, value)
    End Set
  End Property

  Public Function GetProject() As Project

    CanExecuteMethod("GetProject", True)
    Return Project.GetProject(mProjectId)

  End Function

  Public Overrides Function ToString() As String

    Return mProjectId.ToString

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
    mProjectId = project.Id
    mProjectName = project.Name
    mAssigned.Date = Assignment.GetDefaultAssignedDate
    mRole = role

  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub Child_Create(ByVal projectId As Guid, ByVal role As Integer)

    Dim proj = Project.GetProject(projectId)
    mProjectId = proj.Id
    mProjectName = proj.Name
    mAssigned.Date = Assignment.GetDefaultAssignedDate
    mRole = role

  End Sub

  Private Sub Child_Fetch(ByVal data As ProjectTracker.DalLinq.Assignment)

    mProjectId = data.ProjectId
    mProjectName = data.Project.Name
    mAssigned = data.Assigned
    mRole = data.Role
    mTimestamp = data.LastChanged.ToArray

  End Sub

  Private Sub Child_Insert(ByVal resource As Resource)

    mTimestamp = Assignment.AddAssignment( _
      mProjectId, resource.Id, mAssigned, mRole)

  End Sub

  Private Sub Child_Update(ByVal resource As Resource)

    mTimestamp = Assignment.UpdateAssignment( _
      mProjectId, resource.Id, mAssigned, mRole, mTimestamp)

  End Sub

  Private Sub Child_DeleteSelf(ByVal resource As Resource)

    Assignment.RemoveAssignment(mProjectId, resource.Id)

  End Sub

#End Region

End Class
