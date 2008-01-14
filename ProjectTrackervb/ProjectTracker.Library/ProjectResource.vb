<Serializable()> _
Public Class ProjectResource
  Inherits BusinessBase(Of ProjectResource)

  Implements IHoldRoles

#Region " Business Methods "

  Private mTimestamp(7) As Byte

  Private Shared ResourceIdProperty As New PropertyInfo(Of Integer)("ResourceId", "Resource id")
  Public ReadOnly Property ResourceId() As Integer
    Get
      Return GetProperty(Of Integer)(ResourceIdProperty)
    End Get
  End Property

  Private Shared FirstNameProperty As New PropertyInfo(Of String)("FirstName", "First name")
  Public ReadOnly Property FirstName() As String
    Get
      Return GetProperty(Of String)(FirstNameProperty)
    End Get
  End Property

  Private Shared LastNameProperty As New PropertyInfo(Of String)("LastName", "Last name")
  Public ReadOnly Property LastName() As String
    Get
      Return GetProperty(Of String)(LastNameProperty)
    End Get
  End Property

  Public ReadOnly Property FullName() As String
    Get
      Return LastName & ", " & FirstName
    End Get
  End Property

  Private Shared AssignedProperty As New PropertyInfo(Of SmartDate)("Assigned", "Date assigned")
  Public ReadOnly Property Assigned() As String
    Get
      Return GetProperty(Of SmartDate, String)(AssignedProperty)
    End Get
  End Property

  Private Shared RoleProperty As New PropertyInfo(Of Integer)("Role", "Role assigned")
  Public Property Role() As Integer Implements IHoldRoles.Role
    Get
      Return GetProperty(Of Integer)(RoleProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Of Integer)(RoleProperty, value)
    End Set
  End Property

  Public Function GetResource() As Resource

    CanExecuteMethod("GetResource", True)
    Return Resource.GetResource(GetProperty(Of Integer)(ResourceIdProperty))

  End Function

  Public Overrides Function ToString() As String

    Return ResourceId.ToString

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

  Friend Shared Function NewProjectResource( _
    ByVal resourceId As Integer) As ProjectResource

    Return DataPortal.CreateChild(Of ProjectResource)( _
      resourceId, RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResource( _
    ByVal data As ProjectTracker.DalLinq.Assignment) As ProjectResource

    Return DataPortal.FetchChild(Of ProjectResource)(data)

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Sub Child_Create()

    LoadProperty(Of SmartDate)(AssignedProperty, New SmartDate(Today))

  End Sub

  Private Sub Child_Create(ByVal resourceId As Integer, ByVal role As Integer)

    Dim res = Resource.GetResource(resourceId)
    With res
      LoadProperty(Of Integer)(ResourceIdProperty, .Id)
      LoadProperty(Of String)(LastNameProperty, .LastName)
      LoadProperty(Of String)(FirstNameProperty, .FirstName)
      LoadProperty(Of SmartDate)(AssignedProperty, Assignment.GetDefaultAssignedDate)
      LoadProperty(Of Integer)(RoleProperty, role)
    End With

  End Sub

  Private Sub Child_Fetch(ByVal data As ProjectTracker.DalLinq.Assignment)

    With data
      LoadProperty(Of Integer)(ResourceIdProperty, .ResourceId)
      LoadProperty(Of String)(LastNameProperty, .Resource.LastName)
      LoadProperty(Of String)(FirstNameProperty, .Resource.FirstName)
      LoadProperty(Of SmartDate)(AssignedProperty, .Assigned)
      LoadProperty(Of Integer)(RoleProperty, .Role)
      mTimestamp = .LastChanged.ToArray
    End With

  End Sub

  Private Sub Child_Insert(ByVal project As Project)

    mTimestamp = Assignment.AddAssignment(project.Id, _
                                          GetProperty(Of Integer)(ResourceIdProperty), _
                                          GetProperty(Of SmartDate)(AssignedProperty), _
                                          GetProperty(Of Integer)(RoleProperty))

  End Sub

  Private Sub Child_Update(ByVal project As Project)

    mTimestamp = Assignment.UpdateAssignment(project.Id, _
                                             GetProperty(Of Integer)(ResourceIdProperty), _
                                             GetProperty(Of SmartDate)(AssignedProperty), _
                                             GetProperty(Of Integer)(RoleProperty), _
                                             mTimestamp)

  End Sub

  Private Sub Child_DeleteSelf(ByVal project As Project)

    Assignment.RemoveAssignment( _
      project.Id, GetProperty(Of Integer)(ResourceIdProperty))

  End Sub

#End Region

End Class
