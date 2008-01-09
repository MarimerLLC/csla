<Serializable()> _
Public Class Resource
  Inherits BusinessBase(Of Resource)

#Region " Business Methods "

  Private mTimestamp(7) As Byte

  Private Shared IdProperty As New PropertyInfo(Of Integer)("Id")
  Private mId As Integer = IdProperty.DefaultValue
  Public ReadOnly Property Id() As Integer
    Get
      Return GetProperty(Of Integer)(IdProperty, mId)
    End Get
  End Property

  Private Shared LastNameProperty As New PropertyInfo(Of String)("LastName", "Last name")
  Private mLastName As String = LastNameProperty.DefaultValue
  Public Property LastName() As String
    Get
      Return GetProperty(Of String)(LastNameProperty, mLastName)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(LastNameProperty, mLastName, value)
    End Set
  End Property

  Private Shared FirstNameProperty As New PropertyInfo(Of String)("FirstName", "First name")
  Private mFirstName As String = FirstNameProperty.DefaultValue
  Public Property FirstName() As String
    Get
      Return GetProperty(Of String)(FirstNameProperty, mFirstName)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(FirstNameProperty, mFirstName, value)
    End Set
  End Property

  Private Shared FullNameProperty As New PropertyInfo(Of String)("FullName", "Full name")
  Public ReadOnly Property FullName() As String
    Get
      Return LastName & ", " & FirstName
    End Get
  End Property

  Private Shared AssignmentsProperty As New PropertyInfo(Of ResourceAssignments)("Assignments")
  Public ReadOnly Property Assignments() As ResourceAssignments
    Get
      If Not FieldManager.FieldExists(AssignmentsProperty) Then
        SetProperty(Of ResourceAssignments)(AssignmentsProperty, ResourceAssignments.NewResourceAssignments())
      End If
      Return GetProperty(Of ResourceAssignments)(AssignmentsProperty)
    End Get
  End Property

  Public Overrides Function ToString() As String

    Return mId.ToString

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
    DataPortal.Delete(New Criteria(id))

  End Sub

  Public Shared Function GetResource(ByVal id As Integer) As Resource

    If Not CanGetObject() Then
      Throw New System.Security.SecurityException("User not authorized to view a resource")
    End If
    Return CType(DataPortal.Fetch(New Criteria(id)), Resource)

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

  <Serializable()> _
Private Class Criteria
    Private mId As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return mId
      End Get
    End Property

    Public Sub New(ByVal id As Integer)
      mId = id
    End Sub
  End Class

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Create()
    ' nothing to initialize
    ValidationRules.CheckRules()
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getResource"
          .Parameters.AddWithValue("@id", criteria.Id)

          Using dr As New SafeDataReader(.ExecuteReader)
            dr.Read()
            With dr
              mId = .GetInt32("Id")
              mLastName = .GetString("LastName")
              mFirstName = .GetString("FirstName")
              .GetBytes("LastChanged", 0, mTimestamp, 0, 8)
            End With

            ' load child objects
            dr.NextResult()
            SetProperty(Of ResourceAssignments)(AssignmentsProperty, ResourceAssignments.GetResourceAssignments(dr))
          End Using
        End With
      End Using
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      ApplicationContext.LocalContext("cn") = cn
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "addResource"
          .Parameters.AddWithValue("@lastName", mLastName)
          .Parameters.AddWithValue("@firstName", mFirstName)
          Dim param As New SqlParameter("@newId", SqlDbType.Int)
          param.Direction = ParameterDirection.Output
          .Parameters.Add(param)
          param = New SqlParameter("@newLastChanged", SqlDbType.Timestamp)
          param.Direction = ParameterDirection.Output
          .Parameters.Add(param)

          .ExecuteNonQuery()

          mId = CInt(.Parameters("@newId").Value)
          mTimestamp = CType(.Parameters("@newLastChanged").Value, Byte())
        End With
      End Using
      ' update child objects
      GetProperty(Of ResourceAssignments)(AssignmentsProperty).Update(Me)
      ' removing of item only needed for local data portal
      If ApplicationContext.ExecutionLocation = ExecutionLocations.Client Then
        ApplicationContext.LocalContext.Remove("cn")
      End If
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      ApplicationContext.LocalContext("cn") = cn
      If MyBase.IsDirty Then
        Using cm As SqlCommand = cn.CreateCommand
          With cm
            .CommandType = CommandType.StoredProcedure
            .CommandText = "updateResource"
            With cm
              .Parameters.AddWithValue("@id", mId)
              .Parameters.AddWithValue("@lastName", mLastName)
              .Parameters.AddWithValue("@firstName", mFirstName)
              .Parameters.AddWithValue("@lastChanged", mTimestamp)
              Dim param As New SqlParameter("@newLastChanged", SqlDbType.Timestamp)
              param.Direction = ParameterDirection.Output
              .Parameters.Add(param)

              .ExecuteNonQuery()

              mTimestamp = CType(.Parameters("@newLastChanged").Value, Byte())
            End With
          End With
        End Using
      End If
      ' update child objects
      GetProperty(Of ResourceAssignments)(AssignmentsProperty).Update(Me)
      ' removing of item only needed for local data portal
      If ApplicationContext.ExecutionLocation = ExecutionLocations.Client Then
        ApplicationContext.LocalContext.Remove("cn")
      End If
    End Using

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
          .CommandType = CommandType.StoredProcedure
          .CommandText = "deleteResource"
          .Parameters.AddWithValue("@id", criteria.Id)
          .ExecuteNonQuery()
        End With
      End Using
    End Using

  End Sub

#End Region

#Region " Exists "

  Public Shared Function Exists(ByVal id As String) As Boolean

    Return ExistsCommand.Exists(id)

  End Function

  <Serializable()> _
  Private Class ExistsCommand
    Inherits CommandBase

    Private mId As String
    Private mExists As Boolean

    Public ReadOnly Property ResourceExists() As Boolean
      Get
        Return mExists
      End Get
    End Property

    Public Shared Function Exists(ByVal id As String) As Boolean

      Dim result As ExistsCommand
      result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(id))
      Return result.ResourceExists

    End Function

    Private Sub New(ByVal id As String)
      mId = id
    End Sub

    Protected Overrides Sub DataPortal_Execute()

      Using cn As New SqlConnection(Database.PTrackerConnection)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.Text
          cm.CommandText = "SELECT id FROM Resources WHERE id=@id"
          cm.Parameters.AddWithValue("@id", mId)

          Dim count As Integer = CInt(cm.ExecuteScalar)
          mExists = (count > 0)
        End Using
      End Using

    End Sub

  End Class

#End Region

End Class
