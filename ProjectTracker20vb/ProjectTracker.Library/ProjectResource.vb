<Serializable()> _
Public Class ProjectResource
  Inherits BusinessBase(Of ProjectResource)

  Implements IHoldRoles

#Region " Business Methods "

  Private mResourceId As Integer
  Private mFirstName As String = ""
  Private mLastName As String = ""
  Private mAssigned As New SmartDate(Today)
  Private mRole As Integer
  Private mTimestamp(7) As Byte

  Public ReadOnly Property ResourceId() As Integer
    Get
      CanReadProperty(True)
      Return mResourceId
    End Get
  End Property

  Public ReadOnly Property FirstName() As String
    Get
      CanReadProperty(True)
      Return mFirstName
    End Get
  End Property

  Public ReadOnly Property LastName() As String
    Get
      CanReadProperty(True)
      Return mLastName
    End Get
  End Property

  Public ReadOnly Property FullName() As String
    Get
      If CanReadProperty("FirstName") AndAlso CanReadProperty("LastName") Then
        Return LastName & ", " & FirstName
      Else
        Throw _
          New System.Security.SecurityException("Property read not allowed")
      End If
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

  Public Function GetResource() As Resource

    Return Resource.GetResource(mResourceId)

  End Function

  Protected Overrides Function GetIdValue() As Object

    Return mResourceId

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
