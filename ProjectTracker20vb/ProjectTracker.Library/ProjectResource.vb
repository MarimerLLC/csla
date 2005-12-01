Imports System.Data.SqlClient

<Serializable()> _
Public Class ProjectResource
  Inherits BusinessBase(Of ProjectResource)

#Region " Business Methods "

  Private mResourceId As String
  Private mFirstName As String = ""
  Private mLastName As String = ""
  Private mAssigned As New SmartDate(Today)
  Private mRole As Integer

  Public ReadOnly Property ResourceId() As String
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
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
  End Property

  Public ReadOnly Property Assigned() As String
    Get
      CanReadProperty(True)
      Return mAssigned.Text
    End Get
  End Property

  Public Property Role() As Integer
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

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf ValidRole, "Role")

  End Sub

  Private Function ValidRole(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean

    If RoleList.GetList().ContainsKey(mRole) Then
      Return True

    Else
      e.Description = "Role must be in RoleList"
      Return False
    End If

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function NewProjectResource(ByVal resource As Resource, ByVal role As Integer) As ProjectResource

    Return New ProjectResource(resource, role)

  End Function

  Friend Shared Function NewProjectResource(ByVal resourceID As String, ByVal role As Integer) As ProjectResource

    Return New ProjectResource(Resource.GetResource(resourceID), role)

  End Function

  Friend Shared Function NewProjectResource(ByVal resourceID As String) As ProjectResource

    Return New ProjectResource(Resource.GetResource(resourceID), RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResource(ByVal dr As SafeDataReader) As ProjectResource

    Return New ProjectResource(dr)

  End Function

#End Region

#Region " Data Access "

  Private Sub New(ByVal resource As Resource, ByVal role As Integer)

    MarkAsChild()
    With resource
      mResourceId = .Id
      mLastName = .LastName
      mFirstName = .FirstName
      mAssigned.Date = Now
      mRole = role
    End With

  End Sub

  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    With dr
      mResourceId = .GetString(0)
      mLastName = .GetString(1)
      mFirstName = .GetString(2)
      mAssigned = .GetSmartDate(3)
      mRole = .GetInt32(4)
    End With
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "addAssignment"
          LoadParameters(cm, project)

          .ExecuteNonQuery()

          MarkOld()
        End With
      End Using
      cn.Close()
    End Using

  End Sub

  Friend Sub Update(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "updateAssignment"
          LoadParameters(cm, project)

          .ExecuteNonQuery()

          MarkOld()
        End With
      End Using
      cn.Close()
    End Using

  End Sub

  Private Sub LoadParameters(ByVal cm As SqlCommand, ByVal project As Project)

    With cm
      .Parameters.AddWithValue("@ProjectID", project.Id)
      .Parameters.AddWithValue("@ResourceID", mResourceId)
      .Parameters.AddWithValue("@Assigned", mAssigned.DBValue)
      .Parameters.AddWithValue("@Role", mRole)
    End With

  End Sub

  Friend Sub DeleteSelf(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' if we're new then don't update the database
    If Me.IsNew Then Exit Sub

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "deleteAssignment"
          .Parameters.AddWithValue("@ProjectID", project.Id)
          .Parameters.AddWithValue("@ResourceID", mResourceId)

          .ExecuteNonQuery()

          MarkNew()
        End With
      End Using
      cn.Close()
    End Using

  End Sub

#End Region

End Class
