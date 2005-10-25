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

  Public Property ResourceId() As String
    Get
      If CanReadProperty() Then
        Return mResourceId
      Else
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
    Set(ByVal value As String)
      If CanWriteProperty() Then
        If Not mResourceId.Equals(value) Then
          mResourceId = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property write not allowed")
      End If
    End Set
  End Property

  Public Property FirstName() As String
    Get
      If CanReadProperty() Then
        Return mFirstName
      Else
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
    Set(ByVal value As String)
      If CanWriteProperty() Then
        If Not mFirstName.Equals(value) Then
          mFirstName = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property write not allowed")
      End If
    End Set
  End Property

  Public Property LastName() As String
    Get
      If CanReadProperty() Then
        Return mLastName
      Else
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
    Set(ByVal value As String)
      If CanWriteProperty() Then
        If Not mLastName.Equals(value) Then
          mLastName = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property write not allowed")
      End If
    End Set
  End Property

  Public ReadOnly Property Assigned() As String
    Get
      If CanReadProperty() Then
        Return mAssigned.Text
      Else
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
  End Property

  Public Property Role() As Integer
    Get
      If CanReadProperty() Then
        Return mRole
      Else
        Throw New System.Security.SecurityException("Property read not allowed")
      End If
    End Get
    Set(ByVal value As Integer)
      If CanWriteProperty() Then
        If Not mRole.Equals(value) Then
          mRole = value
          PropertyHasChanged()
        End If
      Else
        Throw New System.Security.SecurityException("Property write not allowed")
      End If
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return mResourceId

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function GetResource(ByVal dr As SafeDataReader) As ProjectResource

    Return New ProjectResource(dr)

  End Function

#End Region

#Region " Data Access "

  Private ReadOnly Property DbConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

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

  Friend Sub Update(ByVal project As Project)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cn As New SqlConnection(DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .Connection = cn
          .CommandType = CommandType.StoredProcedure
          If Me.IsDeleted Then
            If Not Me.IsNew Then
              ' we're not new, so delete
              .CommandText = "deleteAssignment"
              .Parameters.AddWithValue("@ProjectID", project.Id)
              .Parameters.AddWithValue("@ResourceID", mResourceId)

              .ExecuteNonQuery()

              MarkNew()
            End If

          Else
            ' we are either adding or updating
            If Me.IsNew Then
              ' we're new, so insert
              .CommandText = "addAssignment"

            Else
              ' we're not new, so update
              .CommandText = "updateAssignment"

            End If
            .Parameters.AddWithValue("@ProjectID", project.Id)
            .Parameters.AddWithValue("@ResourceID", mResourceId)
            .Parameters.AddWithValue("@Assigned", mAssigned.DBValue)
            .Parameters.AddWithValue("@Role", mRole)

            .ExecuteNonQuery()

            MarkOld()
          End If

        End With
      End Using
      cn.Close()
    End Using

  End Sub

#End Region

End Class
