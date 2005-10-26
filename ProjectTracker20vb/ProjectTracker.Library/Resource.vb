Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class Resource
  Inherits BusinessBase(Of Resource)

#Region " Business Properties and Methods "

  Private mId As String = ""
  Private mLastName As String = ""
  Private mFirstName As String = ""

  Private mAssignments As ResourceAssignments = _
    ResourceAssignments.NewResourceAssignments()

  Public ReadOnly Property Id() As String
    Get
      If CanReadProperty() Then
        Return mId
      Else
        Throw New System.Security.SecurityException("Property get not allowed")
      End If
    End Get
  End Property

  Public Property LastName() As String
    Get
      If CanReadProperty() Then
        Return mLastName
      Else
        Throw New System.Security.SecurityException("Property get not allowed")
      End If
    End Get
    Set(ByVal value As String)
      If CanWriteProperty() Then
        If mLastName <> value Then
          mLastName = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property set not allowed")
      End If
    End Set
  End Property

  Public Property FirstName() As String
    Get
      If CanReadProperty() Then
        Return mFirstName
      Else
        Throw New System.Security.SecurityException("Property get not allowed")
      End If
    End Get
    Set(ByVal value As String)
      If CanWriteProperty() Then
        If mFirstName <> value Then
          mFirstName = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property set not allowed")
      End If
    End Set
  End Property

  Public ReadOnly Property FullName() As String
    Get
      Return mLastName & ", " & mFirstName
    End Get
  End Property

  Public ReadOnly Property Assignments() As ResourceAssignments
    Get
      Return mAssignments
    End Get
  End Property

  Public Overrides ReadOnly Property IsValid() As Boolean
    Get
      Return MyBase.IsValid AndAlso mAssignments.IsValid
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mAssignments.IsDirty
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return mId

  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringRequired, "FirstName")
    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs("FirstName", 50))

    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _
      New Validation.CommonRules.MaxLengthRuleArgs("LastName", 50))

  End Sub

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    ' add AuthorizationRules here
    AuthorizationRules.AllowWrite("LastName", "ProjectManager")
    AuthorizationRules.AllowWrite("FirstName", "ProjectManager")

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

#Region " Shared Methods "

  Public Shared Function NewResource(ByVal id As String) As Resource

    If Not CanAddObject() Then
      Throw New System.Security.SecurityException("User not authorized to add a resource")
    End If
    Return DataPortal.Create(Of Resource)(New Criteria(id))

  End Function

  Public Shared Sub DeleteResource(ByVal id As String)

    If Not CanDeleteObject() Then
      Throw New System.Security.SecurityException("User not authorized to remove a resource")
    End If
    DataPortal.Delete(New Criteria(id))

  End Sub

  Public Shared Function GetResource(ByVal id As String) As Resource

    If Not CanGetObject() Then
      Throw New System.Security.SecurityException("User not authorized to view a resource")
    End If
    Return CType(DataPortal.Fetch(New Criteria(id)), Resource)

  End Function

  Public Overrides Function Save() As Resource

    If IsDeleted Then
      If Not CanDeleteObject() Then
        Throw New System.Security.SecurityException("User not authorized to remove a resource")
      End If

    Else
      If Not CanSaveObject() Then
        Throw New System.Security.SecurityException( _
          "User not authorized to update a resource")
      End If
    End If

    Return MyBase.Save

  End Function

#End Region

#Region " Constructors "

  Private Sub New()
    ' prevent direct instantiation
  End Sub

#End Region

#Region " Criteria "

  <Serializable()> _
  Private Class Criteria
    Private mId As String
    Public ReadOnly Property Id() As String
      Get
        Return mId
      End Get
    End Property

    Public Sub New(ByVal id As String)
      mId = id
    End Sub
  End Class

#End Region

#Region " Data Access "

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Create(ByVal criteria As Object)

    mId = CType(criteria, Criteria).Id

  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)

    Dim crit As Criteria = CType(Criteria, Criteria)

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getResource"
          .Parameters.AddWithValue("@ID", crit.Id)

          Using dr As New SafeDataReader(.ExecuteReader)
            dr.Read()
            With dr
              mId = .GetString(0)
              mLastName = .GetString(1)
              mFirstName = .GetString(2)
            End With

            ' load child objects
            dr.NextResult()
            mAssignments = ResourceAssignments.GetResourceAssignments(dr)
            dr.Close()
          End Using
        End With
      End Using
      cn.Close()
    End Using

  End Sub

  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using tr As SqlTransaction = cn.BeginTransaction
        Try
          Using cm As SqlCommand = cn.CreateCommand
            With cm
              .Transaction = tr
              .CommandType = CommandType.StoredProcedure
              .CommandText = "addResource"
              LoadParameters(cm)

              .ExecuteNonQuery()

              ' update child objects
              mAssignments.Update(tr, Me)
            End With
          End Using
          tr.Commit()

        Catch ex As Exception
          tr.Rollback()
          Throw ex
        End Try
      End Using
      cn.Close()
    End Using

  End Sub

  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using tr As SqlTransaction = cn.BeginTransaction
        Try
          Using cm As SqlCommand = cn.CreateCommand
            With cm
              .Transaction = tr
              .CommandType = CommandType.StoredProcedure
              .CommandText = "updateResource"
              LoadParameters(cm)

              .ExecuteNonQuery()

              ' update child objects
              mAssignments.Update(tr, Me)
            End With
          End Using
          tr.Commit()

        Catch ex As Exception
          tr.Rollback()
          Throw ex
        End Try
      End Using
      cn.Close()
    End Using

  End Sub

  Private Sub LoadParameters(ByVal cm As SqlCommand)

    With cm
      .Parameters.AddWithValue("@ID", mId)
      .Parameters.AddWithValue("@LastName", mLastName)
      .Parameters.AddWithValue("@FirstName", mFirstName)
    End With

  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()

    If Not Me.IsNew Then
      ' we're not new, so get rid of our data
      DataPortal_Delete(New Criteria(mId))
    End If

  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

    Dim crit As Criteria = CType(criteria, Criteria)

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using tr As SqlTransaction = cn.BeginTransaction
        Try
          Using cm As SqlCommand = cn.CreateCommand
            With cm
              .Transaction = tr
              .CommandType = CommandType.StoredProcedure
              .CommandText = "deleteResource"
              .Parameters.AddWithValue("@ID", crit.Id)
              .ExecuteNonQuery()
            End With
          End Using
          tr.Commit()

        Catch ex As Exception
          tr.Rollback()
          Throw ex
        End Try
      End Using
      cn.Close()
    End Using

  End Sub

#End Region

#Region " Exists "

  Public Shared Function Exists(ByVal id As String) As Boolean

    Dim result As ExistsCommand
    result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(id))
    Return result.Exists

  End Function

  <Serializable()> _
  Private Class ExistsCommand
    Inherits CommandBase

    Private mId As String
    Private mExists As Boolean

    Public ReadOnly Property Exists() As Boolean
      Get
        Return mExists
      End Get
    End Property

    Public Sub New(ByVal id As String)
      mId = id
    End Sub

    Protected Overrides Sub DataPortal_Execute()

      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.Text
          cm.CommandText = "SELECT id FROM Resources WHERE id=@id"
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
