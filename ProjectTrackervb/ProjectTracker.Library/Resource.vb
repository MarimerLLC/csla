<Serializable()> _
Public Class Resource
  Inherits BusinessBase(Of Resource)

#Region " Business Methods "

  Private _timestamp(7) As Byte

  Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(GetType(Resource), New PropertyInfo(Of Integer)("Id"))
  Private _id As Integer = IdProperty.DefaultValue
  Public ReadOnly Property Id() As Integer
    Get
      Return GetProperty(Of Integer)(IdProperty, _id)
    End Get
  End Property

  Private Shared LastNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Resource), New PropertyInfo(Of String)("LastName", "Last name"))
  Private _lastName As String = LastNameProperty.DefaultValue
  Public Property LastName() As String
    Get
      Return GetProperty(Of String)(LastNameProperty, _lastName)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(LastNameProperty, _lastName, value)
    End Set
  End Property

  Private Shared FirstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Resource), New PropertyInfo(Of String)("FirstName", "First name"))
  Private _firstName As String = FirstNameProperty.DefaultValue
  Public Property FirstName() As String
    Get
      Return GetProperty(Of String)(FirstNameProperty, _firstName)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(FirstNameProperty, _firstName, value)
    End Set
  End Property

  Private Shared FullNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Resource), New PropertyInfo(Of String)("FullName", "Full name"))
  Public ReadOnly Property FullName() As String
    Get
      Return LastName & ", " & FirstName
    End Get
  End Property

  Private Shared AssignmentsProperty As PropertyInfo(Of ResourceAssignments) = RegisterProperty(Of ResourceAssignments)(GetType(Resource), New PropertyInfo(Of ResourceAssignments)("Assignments"))
  Public ReadOnly Property Assignments() As ResourceAssignments
    Get
      If Not FieldManager.FieldExists(AssignmentsProperty) Then
        SetProperty(Of ResourceAssignments)(AssignmentsProperty, ResourceAssignments.NewResourceAssignments())
      End If
      Return GetProperty(Of ResourceAssignments)(AssignmentsProperty)
    End Get
  End Property

  Public Overrides Function ToString() As String

    Return _id.ToString

  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule( _
      AddressOf Validation.CommonRules.StringRequired, FirstNameProperty)
    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs(FirstNameProperty, 50))

    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringRequired, LastNameProperty)
    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs(LastNameProperty, 50))

  End Sub

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    ' add AuthorizationRules here
    AuthorizationRules.AllowWrite(LastNameProperty, "ProjectManager")
    AuthorizationRules.AllowWrite(FirstNameProperty, "ProjectManager")

  End Sub

  Public Shared Function CanAddObject() As Boolean

    Return Csla.ApplicationContext.User.IsInRole("ProjectManager")

  End Function

  Public Shared Function CanGetObject() As Boolean

    Return True

  End Function

  Public Shared Function CanDeleteObject() As Boolean

    Dim result As Boolean
    If Csla.ApplicationContext.User.IsInRole("ProjectManager") Then
      result = True
    End If
    If Csla.ApplicationContext.User.IsInRole("Administrator") Then
      result = True
    End If
    Return result

  End Function

  Public Shared Function CanEditObject() As Boolean

    Return Csla.ApplicationContext.User.IsInRole("ProjectManager")

  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewResource() As Resource

    If Not CanAddObject() Then
      Throw New System.Security.SecurityException("User not authorized to add a resource")
    End If
    Return DataPortal.Create(Of Resource)()

  End Function

  Public Shared Sub DeleteResource(ByVal id As Integer)

    If Not CanDeleteObject() Then
      Throw New System.Security.SecurityException("User not authorized to remove a resource")
    End If
    DataPortal.Delete(New SingleCriteria(Of Resource, Integer)(id))

  End Sub

  Public Shared Function GetResource(ByVal id As Integer) As Resource

    If Not CanGetObject() Then
      Throw New System.Security.SecurityException("User not authorized to view a resource")
    End If
    Return DataPortal.Fetch(Of Resource)(New SingleCriteria(Of Resource, Integer)(id))

  End Function

  Public Overrides Function Save() As Resource

    If IsDeleted AndAlso Not CanDeleteObject() Then
      Throw New System.Security.SecurityException("User not authorized to remove a resource")

    ElseIf IsNew AndAlso Not CanAddObject() Then
      Throw New System.Security.SecurityException("User not authorized to add a resource")

    ElseIf Not CanEditObject() Then
      Throw New System.Security.SecurityException("User not authorized to update a resource")
    End If
    Return MyBase.Save

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Resource, Integer))

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim data = (From r In ctx.DataContext.Resources Where r.Id = criteria.Value Select r).Single
      _id = data.Id
      _lastName = data.LastName
      _firstName = data.FirstName
      _timestamp = data.LastChanged.ToArray

      SetProperty(Of ResourceAssignments)(AssignmentsProperty, ResourceAssignments.GetResourceAssignments(data.Assignments.ToArray))
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim newId As Integer?
      Dim newLastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.addResource(_lastName, _firstName, newId, newLastChanged)
      _id = CInt(newId)
      _timestamp = newLastChanged.ToArray
      FieldManager.UpdateChildren(Me)
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim newLastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.UpdateResource(_id, _lastName, _firstName, _timestamp, newLastChanged)
      _timestamp = newLastChanged.ToArray
      FieldManager.UpdateChildren(Me)
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New SingleCriteria(Of Resource, Integer)(_id))

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As SingleCriteria(Of Resource, Integer))

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      ctx.DataContext.DeleteResource(_id)
    End Using

  End Sub

#End Region

#Region " Exists "

  Public Shared Function Exists(ByVal id As Integer) As Boolean

    Return ExistsCommand.Exists(id)

  End Function

  <Serializable()> _
  Private Class ExistsCommand
    Inherits CommandBase

    Private _id As Integer
    Private _exists As Boolean

    Public ReadOnly Property ResourceExists() As Boolean
      Get
        Return _exists
      End Get
    End Property

    Public Shared Function Exists(ByVal id As Integer) As Boolean

      Dim result As ExistsCommand
      result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(id))
      Return result.ResourceExists

    End Function

    Private Sub New(ByVal id As Integer)
      _id = id
    End Sub

    Protected Overrides Sub DataPortal_Execute()

      Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
        _exists = (From p In ctx.DataContext.Resources Where p.Id = _id).Count > 0
      End Using

    End Sub

  End Class

#End Region

End Class
