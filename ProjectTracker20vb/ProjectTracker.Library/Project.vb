Imports System.Data.SqlClient

<Serializable()> _
Public Class Project
  Inherits BusinessBase(Of Project)

#Region " Business Methods "

  Private mId As Guid = Guid.NewGuid
  Private mName As String = ""
  Private mStarted As New SmartDate(False)
  Private mEnded As New SmartDate
  Private mDescription As String = ""

  Private mResources As ProjectResources = _
    ProjectResources.NewProjectResources()

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

    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringRequired, "Name")
    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs("Name", 50))

    ValidationRules.AddRule(AddressOf StartDateGTEndDate, "Started")
    ValidationRules.AddRule(AddressOf StartDateGTEndDate, "Ended")

  End Sub

  Private Function StartDateGTEndDate(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean

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

    Return My.User.IsInRole("ProjectManager")

  End Function

  Public Shared Function CanGetObject() As Boolean

    Return True

  End Function

  Public Shared Function CanDeleteObject() As Boolean

    Dim result As Boolean
    If My.User.IsInRole("ProjectManager") Then
      result = True
    End If
    If My.User.IsInRole("Administrator") Then
      result = True
    End If
    Return result

  End Function

  Public Shared Function CanSaveObject() As Boolean

    Return Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager")

  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewProject() As Project

    If Not CanAddObject() Then
      Throw New System.Security.SecurityException("User not authorized to add a project")
    End If
    Return DataPortal.Create(Of Project)(Nothing)

  End Function

  Public Shared Function GetProject(ByVal id As Guid) As Project

    If Not CanGetObject() Then
      Throw New System.Security.SecurityException("User not authorized to view a project")
    End If
    Return DataPortal.Fetch(Of Project)(New Criteria(id))

  End Function

  Public Shared Sub DeleteProject(ByVal id As Guid)

    If Not CanDeleteObject() Then
      Throw New System.Security.SecurityException("User not authorized to remove a project")
    End If
    DataPortal.Delete(New Criteria(id))

  End Sub

  Public Overrides Function Save() As Project
    If IsDeleted Then
      If Not CanDeleteObject() Then
        Throw New System.Security.SecurityException("User not authorized to remove a project")
      End If

    Else
      ' no deletion - we're adding or updating
      If Not CanSaveObject() Then
        Throw New System.Security.SecurityException("User not authorized to update a project")
      End If
    End If

    Return MyBase.Save
  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    AddAuthorizationRules()

  End Sub

#End Region

#Region " Criteria "

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

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

    mId = Guid.NewGuid
    Started = CStr(Today)
    Name = ""
    ValidationRules.CheckRules()

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "getProject"
        cm.Parameters.AddWithValue("@ID", criteria.Id)

        Using dr As New SafeDataReader(cm.ExecuteReader)
          dr.Read()
          With dr
            mId = .GetGuid(0)
            mName = .GetString(1)
            mStarted = .GetSmartDate(2, mStarted.EmptyIsMin)
            mEnded = .GetSmartDate(3, mEnded.EmptyIsMin)
            mDescription = .GetString(4)

            ' load child objects
            .NextResult()
            mResources = ProjectResources.GetProjectResources(dr)
          End With
          dr.Close()
        End Using
      End Using
      cn.Close()
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "addProject"
        LoadParameters(cm)
        cm.ExecuteNonQuery()
      End Using
      cn.Close()
    End Using

    ' update child objects
    mResources.Update(Me)

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "updateProject"
        LoadParameters(cm)
        cm.ExecuteNonQuery()
      End Using
      cn.Close()
    End Using

    ' update child objects
    mResources.Update(Me)

  End Sub

  Private Sub LoadParameters(ByVal cm As SqlCommand)

    With cm
      .Parameters.AddWithValue("@ID", mId.ToString)
      .Parameters.AddWithValue("@Name", mName)
      .Parameters.AddWithValue("@Started", mStarted.DBValue)
      .Parameters.AddWithValue("@Ended", mEnded.DBValue)
      .Parameters.AddWithValue("@Description", mDescription)
    End With

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New Criteria(mId))

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)

    Dim cn As New SqlConnection(DataBase.DbConn)
    Dim cm As New SqlCommand()

    cn.Open()

    Try
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        .CommandText = "deleteProject"
        .Parameters.AddWithValue("@ID", criteria.Id.ToString)
        .ExecuteNonQuery()
      End With

    Finally
      cn.Close()
    End Try

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

      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.Text
          cm.CommandText = "SELECT id FROM Projects WHERE id=@id"
          cm.Parameters.AddWithValue("@ID", mId)

          Using dr As SqlDataReader = cm.ExecuteReader
            If dr.Read() Then
              mExists = True

            Else
              mExists = False
            End If
            dr.Close()
          End Using
        End Using
        cn.Close()
      End Using

    End Sub

  End Class

#End Region

End Class
