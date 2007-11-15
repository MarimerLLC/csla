<Serializable()> _
Public Class ProjectResource
  Inherits BusinessBase(Of ProjectResource)

  Implements IHoldRoles

#Region " Business Methods "

  Private mTimestamp(7) As Byte

  Private Shared ResourceIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer, ProjectResource)("ResourceId", "Resource id")
  Private mResourceId As Integer = ResourceIdProperty.DefaultValue
  Public ReadOnly Property ResourceId() As Integer
    Get
      Return GetProperty(Of Integer)(ResourceIdProperty, mResourceId)
    End Get
  End Property

  Private Shared FirstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String, ProjectResource)("FirstName", "First name")
  Private mFirstName As String = FirstNameProperty.DefaultValue
  Public ReadOnly Property FirstName() As String
    Get
      Return GetProperty(Of String)(FirstNameProperty, mFirstName)
    End Get
  End Property

  Private Shared LastNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String, ProjectResource)("LastName", "Last name")
  Private mLastName As String = ""
  Public ReadOnly Property LastName() As String
    Get
      Return GetProperty(Of String)(LastNameProperty, mLastName)
    End Get
  End Property

  Public ReadOnly Property FullName() As String
    Get
      Return LastName & ", " & FirstName
    End Get
  End Property

  Private Shared AssignedProperty As PropertyInfo(Of String) = RegisterProperty(Of String, ProjectResource)("Assigned", "Date assigned")
  Private mAssigned As New SmartDate(Today)
  Public ReadOnly Property Assigned() As String
    Get
      Return GetProperty(Of String)(AssignedProperty, mAssigned)
    End Get
  End Property

  Private Shared RoleProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer, ProjectResource)("Role", "Role assigned")
  Private mRole As Integer
  Public Property Role() As Integer Implements IHoldRoles.Role
    Get
      Return GetProperty(Of Integer)(RoleProperty, mRole)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Of Integer)(RoleProperty, mRole, value)
    End Set
  End Property

  Public Function GetResource() As Resource

    CanExecuteMethod("GetResource", True)
    Return Resource.GetResource(mResourceId)

  End Function

  Public Overrides Function ToString() As String

    Return mResourceId.ToString

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

  Friend Shared Function NewProjectResource( _
    ByVal resourceId As Integer) As ProjectResource

    Return New ProjectResource( _
      Resource.GetResource(resourceId), RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResource( _
    ByVal dr As SafeDataReader) As ProjectResource

    Return New ProjectResource(dr)

  End Function

  Private Sub New()

    MarkAsChild()

  End Sub

  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    Fetch(dr)

  End Sub

  Private Sub New(ByVal resource As Resource, ByVal role As Integer)

    MarkAsChild()
    With resource
      mResourceId = .Id
      mLastName = .LastName
      mFirstName = .FirstName
      mAssigned.Date = Assignment.GetDefaultAssignedDate
      mRole = role
    End With

  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SafeDataReader)

    With dr
      mResourceId = .GetInt32("ResourceId")
      mLastName = .GetString("LastName")
      mFirstName = .GetString("FirstName")
      mAssigned = .GetSmartDate("Assigned")
      mRole = .GetInt32("Role")
      .GetBytes("LastChanged", 0, mTimestamp, 0, 8)
    End With
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      mTimestamp = Assignment.AddAssignment( _
        cn, project.Id, mResourceId, mAssigned, mRole)
      MarkOld()
    End Using

  End Sub

  Friend Sub Update(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      mTimestamp = Assignment.UpdateAssignment( _
        cn, project.Id, mResourceId, mAssigned, mRole, mTimestamp)
      MarkOld()
    End Using

  End Sub

  Friend Sub DeleteSelf(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' if we're new then don't update the database
    If Me.IsNew Then Exit Sub

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Assignment.RemoveAssignment(cn, project.Id, mResourceId)
      MarkNew()
    End Using

  End Sub

#End Region

End Class
