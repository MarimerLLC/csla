<Serializable()> _
Public Class Project
  Inherits BusinessBase(Of Project)

#Region " Business Methods "

  Private mId As Guid = Guid.NewGuid
  Private mName As String = ""
  Private mStarted As New SmartDate
  Private mEnded As New SmartDate(False)
  Private mDescription As String = ""
  Private mTimestamp(7) As Byte

  Private mResources As ProjectResources = _
    ProjectResources.NewProjectResources()

  <System.ComponentModel.DataObjectField(True, True)> _
  Public ReadOnly Property Id() As Guid
    Get
      CanReadProperty(True)
      Return mId
    End Get
  End Property

  Public Property Name() As String
    Get
      CanReadProperty(True)
      Return mName
    End Get
    Set(ByVal Value As String)
      CanWriteProperty(True)
      If mName <> Value Then
        mName = Value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public Property Started() As String
    Get
      CanReadProperty(True)
      Return mStarted.Text
    End Get
    Set(ByVal Value As String)
      CanWriteProperty(True)
      If mStarted <> Value Then
        mStarted.Text = Value
        ValidationRules.CheckRules("Ended")
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public Property Ended() As String
    Get
      CanReadProperty(True)
      Return mEnded.Text
    End Get
    Set(ByVal Value As String)
      CanWriteProperty(True)
      If mEnded <> Value Then
        mEnded.Text = Value
        ValidationRules.CheckRules("Started")
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public Property Description() As String
    Get
      CanReadProperty(True)
      Return mDescription
    End Get
    Set(ByVal Value As String)
      CanWriteProperty(True)
      If mDescription <> Value Then
        mDescription = Value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public ReadOnly Property Resources() As ProjectResources
    Get
      Return mResources
    End Get
  End Property

  Public Overrides ReadOnly Property IsValid() As Boolean
    Get
      Return MyBase.IsValid AndAlso mResources.IsValid
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mResources.IsDirty
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule( _
      AddressOf Validation.CommonRules.StringRequired, "Name")
    ValidationRules.AddRule( _
      AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs("Name", 50))

    ValidationRules.AddRule(AddressOf StartDateGTEndDate, "Started")
    ValidationRules.AddRule(AddressOf StartDateGTEndDate, "Ended")

  End Sub

  Private Function StartDateGTEndDate( _
    ByVal target As Object, _
    ByVal e As Validation.RuleArgs) As Boolean

    If mStarted > mEnded Then
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
    AuthorizationRules.AllowWrite("Name", "ProjectManager")
    AuthorizationRules.AllowWrite("Started", "ProjectManager")
    AuthorizationRules.AllowWrite("Ended", "ProjectManager")
    AuthorizationRules.AllowWrite("Description", "ProjectManager")

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
    Return DataPortal.Fetch(Of Project)(New Criteria(id))

  End Function

  Public Shared Sub DeleteProject(ByVal id As Guid)

    If Not CanDeleteObject() Then
      Throw New System.Security.SecurityException( _
        "User not authorized to remove a project")
    End If
    DataPortal.Delete(New Criteria(id))

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

  <Serializable()> _
  Private Class Criteria

    Private mId As Guid
    Public ReadOnly Property Id() As Guid
      Get
        Return mId
      End Get
    End Property

    Public Sub New(ByVal id As Guid)
      mId = id
    End Sub
  End Class

  <RunLocal()> _
  Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

    mId = Guid.NewGuid
    Started = CStr(Today)
    ValidationRules.CheckRules()

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "getProject"
        cm.Parameters.AddWithValue("@id", criteria.Id)

        Using dr As New SafeDataReader(cm.ExecuteReader)
          dr.Read()
          With dr
            mId = .GetGuid("Id")
            mName = .GetString("Name")
            mStarted = .GetSmartDate("Started", mStarted.EmptyIsMin)
            mEnded = .GetSmartDate("Ended", mEnded.EmptyIsMin)
            mDescription = .GetString("Description")
            .GetBytes("LastChanged", 0, mTimestamp, 0, 8)

            ' load child objects
            .NextResult()
            mResources = ProjectResources.GetProjectResources(dr)
          End With
        End Using
      End Using
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandText = "addProject"
        DoInsertUpdate(cm)
      End Using
    End Using
    ' update child objects
    mResources.Update(Me)

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    If MyBase.IsDirty Then
      Using cn As New SqlConnection(Database.PTrackerConnection)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandText = "updateProject"
          cm.Parameters.AddWithValue("@lastChanged", mTimestamp)
          DoInsertUpdate(cm)
        End Using
      End Using
    End If
    ' update child objects
    mResources.Update(Me)

  End Sub

  Private Sub DoInsertUpdate(ByVal cm As SqlCommand)

    With cm
      .CommandType = CommandType.StoredProcedure
      .Parameters.AddWithValue("@id", mId)
      .Parameters.AddWithValue("@name", mName)
      .Parameters.AddWithValue("@started", mStarted.DBValue)
      .Parameters.AddWithValue("@ended", mEnded.DBValue)
      .Parameters.AddWithValue("@description", mDescription)
      Dim param As New SqlParameter("@newLastChanged", SqlDbType.Timestamp)
      param.Direction = ParameterDirection.Output
      .Parameters.Add(param)

      .ExecuteNonQuery()

      mTimestamp = CType(.Parameters("@newLastChanged").Value, Byte())
    End With

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New Criteria(mId))

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .Connection = cn
          .CommandType = CommandType.StoredProcedure
          .CommandText = "deleteProject"
          .Parameters.AddWithValue("@id", criteria.Id)
          .ExecuteNonQuery()
        End With
      End Using
    End Using

  End Sub

#End Region

#Region " Exists "

  Public Shared Function Exists(ByVal id As Guid) As Boolean

    Dim result As ExistsCommand
    result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(id))
    Return result.Exists

  End Function

  <Serializable()> _
  Private Class ExistsCommand
    Inherits CommandBase

    Private mId As Guid
    Private mExists As Boolean

    Public ReadOnly Property Exists() As Boolean
      Get
        Return mExists
      End Get
    End Property

    Public Sub New(ByVal id As Guid)
      mId = id
    End Sub

    Protected Overrides Sub DataPortal_Execute()

      Using cn As New SqlConnection(Database.PTrackerConnection)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.Text
          cm.CommandText = "SELECT Id FROM Projects WHERE Id=@id"
          cm.Parameters.AddWithValue("@id", mId)

          Dim count As Integer = CInt(cm.ExecuteScalar)
          mExists = (count > 0)
        End Using
      End Using

    End Sub

  End Class

#End Region

End Class
