<Serializable()> _
Public Class ResourceAssignment
  Inherits BusinessBase(Of ResourceAssignment)

  Implements IHoldRoles

#Region " Business Methods "

  Private mProjectId As Guid = Guid.Empty
  Private mProjectName As String = ""
  Private mAssigned As New SmartDate(Today)
  Private mRole As Integer
  Private mTimestamp(7) As Byte

  Public ReadOnly Property ProjectId() As Guid
    Get
      CanReadProperty(True)
      Return mProjectId
    End Get
  End Property

  Public ReadOnly Property ProjectName() As String
    Get
      CanReadProperty(True)
      Return mProjectName
    End Get
  End Property

  Public ReadOnly Property Assigned() As String
    Get
      CanReadProperty(True)
      Return mAssigned.Text
    End Get
  End Property

  Public Property Role() As Integer Implements IHoldRoles.Role
    Get
      CanReadProperty(True)
      Return mRole
    End Get
    Set(ByVal value As Integer)
      CanWriteProperty(True)
      If Not mRole.Equals(value) Then
        mRole = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public Function GetProject() As Project

    Return Project.GetProject(mProjectId)

  End Function

  Protected Overrides Function GetIdValue() As Object

    Return mProjectId

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

  Friend Sub Insert(ByVal cn As SqlConnection, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    mTimestamp = Assignment.AddAssignment( _
      cn, mProjectId, resource.Id, mAssigned, mRole)
    MarkOld()

  End Sub

  Friend Sub Update( _
    ByVal cn As SqlClient.SqlConnection, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    mTimestamp = Assignment.UpdateAssignment( _
      cn, mProjectId, resource.Id, mAssigned, mRole, mTimestamp)
    MarkOld()

  End Sub

  Friend Sub DeleteSelf(ByVal cn As SqlClient.SqlConnection, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' if we're new then don't update the database
    If Me.IsNew Then Exit Sub

    Assignment.RemoveAssignment(cn, mProjectId, resource.Id)
    MarkNew()

  End Sub

#End Region

End Class
