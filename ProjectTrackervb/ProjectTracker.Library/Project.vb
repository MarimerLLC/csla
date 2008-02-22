<Serializable()> _
Public Class Project
  Inherits BusinessBase(Of Project)

#Region " Business Methods "

  Private _timestamp(7) As Byte

  Private Shared IdProperty As PropertyInfo(Of Guid) = RegisterProperty(Of Guid)(GetType(Project), New PropertyInfo(Of Guid)("Id"))
  <System.ComponentModel.DataObjectField(True, True)> _
  Public ReadOnly Property Id() As Guid
    Get
      Return GetProperty(Of Guid)(IdProperty)
    End Get
  End Property

  Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Project), New PropertyInfo(Of String)("Name"))
  Public Property Name() As String
    Get
      Return GetProperty(Of String)(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(NameProperty, value)
    End Set
  End Property

  Private Shared StartedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(GetType(Project), New PropertyInfo(Of SmartDate)("Started"))
  Public Property Started() As String
    Get
      Return GetProperty(Of SmartDate, String)(StartedProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(Of SmartDate, String)(StartedProperty, value)
    End Set
  End Property

  Private Shared EndedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(GetType(Project), New PropertyInfo(Of SmartDate)("Ended", New SmartDate(SmartDate.EmptyValue.MaxDate)))
  Public Property Ended() As String
    Get
      Return GetProperty(Of SmartDate, String)(EndedProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(Of SmartDate, String)(EndedProperty, value)
    End Set
  End Property

  Private Shared DescriptionProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Project), New PropertyInfo(Of String)("Description"))
  Public Property Description() As String
    Get
      Return GetProperty(Of String)(DescriptionProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(DescriptionProperty, value)
    End Set
  End Property

  Private Shared ResourcesProperty As PropertyInfo(Of ProjectResources) = RegisterProperty(Of ProjectResources)(GetType(Project), New PropertyInfo(Of ProjectResources)("Resources"))
  Public ReadOnly Property Resources() As ProjectResources
    Get
      If Not FieldManager.FieldExists(ResourcesProperty) Then
        SetProperty(Of ProjectResources)(ResourcesProperty, ProjectResources.NewProjectResources())
      End If
      Return GetProperty(Of ProjectResources)(ResourcesProperty)
    End Get
  End Property

  Public Overrides Function ToString() As String
    Return Id.ToString
  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule( _
      AddressOf Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(NameProperty))
    ValidationRules.AddRule( _
      AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs(NameProperty, 50))

    ValidationRules.AddRule(Of Project)(AddressOf StartDateGTEndDate(Of Project), StartedProperty)
    ValidationRules.AddRule(Of Project)(AddressOf StartDateGTEndDate(Of Project), EndedProperty)

    ValidationRules.AddDependentProperty(StartedProperty, EndedProperty, True)

  End Sub

  Private Shared Function StartDateGTEndDate(Of T As Project)( _
    ByVal target As T, _
    ByVal e As Validation.RuleArgs) As Boolean

    If target.GetProperty(Of SmartDate)(StartedProperty) > target.GetProperty(Of SmartDate)(EndedProperty) Then
      e.Description = "Start date can't be after end date"
      Return False

    Else
      Return True
    End If

  End Function

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    ' add AuthorizationRules here
    AuthorizationRules.AllowWrite(NameProperty, "ProjectManager")
    AuthorizationRules.AllowWrite(StartedProperty, "ProjectManager")
    AuthorizationRules.AllowWrite(EndedProperty, "ProjectManager")
    AuthorizationRules.AllowWrite(DescriptionProperty, "ProjectManager")

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

  Public Shared Function NewProject() As Project

    If Not CanAddObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to add a project")
    End If
    Return DataPortal.Create(Of Project)()

  End Function

  Public Shared Function GetProject(ByVal id As Guid) As Project

    If Not CanGetObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to view a project")
    End If
    Return DataPortal.Fetch(Of Project)(New SingleCriteria(Of Project, Guid)(id))

  End Function

  Public Shared Sub DeleteProject(ByVal id As Guid)

    If Not CanDeleteObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to remove a project")
    End If
    DataPortal.Delete(New SingleCriteria(Of Project, Guid)(id))

  End Sub

  Public Overrides Function Save() As Project

    If IsDeleted AndAlso Not CanDeleteObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to remove a project")

    ElseIf IsNew AndAlso Not CanAddObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to add a project")

    ElseIf Not CanEditObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to update a project")
    End If
    Return MyBase.Save

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Create()

    LoadProperty(Of Guid)(IdProperty, Guid.NewGuid)
    Started = CStr(Today)
    ValidationRules.CheckRules()

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Project, Guid))

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      ' get project data
      Dim data = (From p In ctx.DataContext.Projects Where p.Id = criteria.Value Select p).Single
      LoadProperty(Of Guid)(IdProperty, data.Id)
      LoadProperty(Of String)(NameProperty, data.Name)
      LoadProperty(Of SmartDate, Date?)(StartedProperty, data.Started)
      LoadProperty(Of SmartDate, Date?)(EndedProperty, data.Ended)
      LoadProperty(Of String)(DescriptionProperty, data.Description)
      _timestamp = data.LastChanged.ToArray

      ' get child data
      LoadProperty(Of ProjectResources)(ResourcesProperty, _
        ProjectResources.GetProjectResources(data.Assignments.ToArray))
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      ' insert project data
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.addProject(GetProperty(Of Guid)(IdProperty), _
                                 GetProperty(Of String)(NameProperty), _
                                 GetProperty(Of SmartDate)(StartedProperty), _
                                 GetProperty(Of SmartDate)(EndedProperty), _
                                 GetProperty(Of String)(DescriptionProperty), _
                                 lastChanged)
      _timestamp = lastChanged.ToArray
      ' update child objects
      DataPortal.UpdateChild(GetProperty(Of ProjectResources)(ResourcesProperty), Me)
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      ' insert project data
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.UpdateProject(GetProperty(Of Guid)(IdProperty), _
                                    GetProperty(Of String)(NameProperty), _
                                    GetProperty(Of SmartDate)(StartedProperty), _
                                    GetProperty(Of SmartDate)(EndedProperty), _
                                    GetProperty(Of String)(DescriptionProperty), _
                                    _timestamp, _
                                    lastChanged)
      _timestamp = lastChanged.ToArray
      ' update child objects
      DataPortal.UpdateChild(GetProperty(Of ProjectResources)(ResourcesProperty), Me)
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New SingleCriteria(Of Project, Guid)(GetProperty(Of Guid)(IdProperty)))

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As SingleCriteria(Of Project, Guid))

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      ' delete project data
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.DeleteProject(criteria.Value)
      ' reset child list field
      SetProperty(Of ProjectResources)(ResourcesProperty, ProjectResources.NewProjectResources)
    End Using

  End Sub

#End Region

#Region " Exists "

  Public Shared Function Exists(ByVal id As Guid) As Boolean

    Return ExistsCommand.Exists(id)

  End Function

  <Serializable()> _
  Private Class ExistsCommand
    Inherits CommandBase

    Private _id As Guid
    Private _exists As Boolean

    Public ReadOnly Property ProjectExists() As Boolean
      Get
        Return _exists
      End Get
    End Property

    Public Shared Function Exists(ByVal id As Guid) As Boolean

      Dim result As ExistsCommand
      result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(id))
      Return result.ProjectExists

    End Function

    Private Sub New(ByVal id As Guid)
      _id = id
    End Sub

    Protected Overrides Sub DataPortal_Execute()

      Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
        _exists = (From p In ctx.DataContext.Projects Where p.Id = _id).Count > 0
      End Using

    End Sub

  End Class

#End Region

End Class
