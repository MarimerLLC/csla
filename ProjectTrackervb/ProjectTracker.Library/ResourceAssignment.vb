<Serializable()> _
Public Class ResourceAssignment
  Inherits BusinessBase(Of ResourceAssignment)

  Implements IHoldRoles

#Region " Business Methods "

  Private mTimestamp(7) As Byte

  Private Shared ProjectIdProperty As PropertyInfo(Of Guid) = RegisterProperty(Of Guid, ResourceAssignment)("ProjectId", Guid.Empty)
  Private mProjectId As Guid = ProjectIdProperty.DefaultValue
  Public ReadOnly Property ProjectId() As Guid
    Get
      Return GetProperty(Of Guid)(ProjectIdProperty, mProjectId)
    End Get
  End Property

  Private Shared ProjectNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String, ResourceAssignment)("ProjectName")
  Private mProjectName As String = ProjectNameProperty.DefaultValue
  Public ReadOnly Property ProjectName() As String
    Get
      Return GetProperty(Of String)(ProjectNameProperty, mProjectName)
    End Get
  End Property

  Private Shared AssignedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate, ResourceAssignment)("Assigned")
  Private mAssigned As New SmartDate(Today)
  Public ReadOnly Property Assigned() As String
    Get
      Return GetProperty(Of SmartDate)(AssignedProperty, mAssigned)
    End Get
  End Property

  Private Shared RoleProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer, ResourceAssignment)("Role")
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

    ValidationRules.AddRule(AddressOf Assignment.ValidRole, "Role")

  End Sub

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    AuthorizationRules.AllowWrite("Role", "ProjectManager")

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function NewResourceAssignment( _
    ByVal projectId As Guid) As ResourceAssignment

    Return New ResourceAssignment(Project.GetProject(projectId), RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResourceAssignment( _
    ByVal dr As SafeDataReader) As ResourceAssignment

    Return New ResourceAssignment(dr)

  End Function

  Private Sub New()

    MarkAsChild()

  End Sub

  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    Fetch(dr)

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

  Private Sub Fetch(ByVal dr As SafeDataReader)

    With dr
      mProjectId = .GetGuid("ProjectId")
      mProjectName = .GetString("Name")
      mAssigned = .GetSmartDate("Assigned")
      mRole = .GetInt32("Role")
      .GetBytes("LastChanged", 0, mTimestamp, 0, 8)
    End With
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal resource As Resource)

    Dim cn As SqlClient.SqlConnection = _
      CType(ApplicationContext.LocalContext("cn"), SqlClient.SqlConnection)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    mTimestamp = Assignment.AddAssignment( _
      cn, mProjectId, resource.Id, mAssigned, mRole)
    MarkOld()

  End Sub

  Friend Sub Update(ByVal resource As Resource)

    Dim cn As SqlClient.SqlConnection = _
      CType(ApplicationContext.LocalContext("cn"), SqlClient.SqlConnection)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    mTimestamp = Assignment.UpdateAssignment( _
      cn, mProjectId, resource.Id, mAssigned, mRole, mTimestamp)
    MarkOld()

  End Sub

  Friend Sub DeleteSelf(ByVal resource As Resource)

    Dim cn As SqlClient.SqlConnection = _
      CType(ApplicationContext.LocalContext("cn"), SqlClient.SqlConnection)
    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' if we're new then don't update the database
    If Me.IsNew Then Exit Sub

    Assignment.RemoveAssignment(cn, mProjectId, resource.Id)
    MarkNew()

  End Sub

#End Region

End Class
