Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ProjectResource
  Inherits Assignment

  Private mResourceID As String = ""
  Private mLastName As String = ""
  Private mFirstName As String = ""

#Region " Business Properties and Methods "

  Public ReadOnly Property ResourceID() As String
    Get
      Return mResourceID
    End Get
  End Property

  Public ReadOnly Property LastName() As String
    Get
      Return mLastName
    End Get
  End Property

  Public ReadOnly Property FirstName() As String
    Get
      Return mFirstName
    End Get
  End Property

  Public Function GetResource() As Resource
    Return Resource.GetResource(mResourceID)
  End Function

#End Region

#Region " System.Object Overrides "

  Public Overrides Function ToString() As String
    Return mLastName & ", " & mFirstName
  End Function

  Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
    If TypeOf objA Is ProjectResource AndAlso TypeOf objB Is ProjectResource Then
      Return DirectCast(objA, ProjectResource).Equals(DirectCast(objB, ProjectResource))

    Else
      Return False
    End If
  End Function

  Public Overloads Overrides Function Equals(ByVal ProjectResource As Object) As Boolean
    If TypeOf ProjectResource Is ProjectResource Then
      Return Equals(DirectCast(ProjectResource, ProjectResource))

    Else
      Return False
    End If
  End Function

  Public Overloads Function Equals(ByVal Assignment As ProjectResource) _
      As Boolean

    Return mResourceID = Assignment.ResourceID
  End Function

  Public Overrides Function GetHashCode() As Integer
    Return mResourceID.GetHashCode
  End Function

#End Region

#Region " Shared Methods "

  Friend Shared Function NewProjectResource(ByVal Resource As Resource, ByVal Role As String) As ProjectResource
    Return New ProjectResource(Resource, Role)
  End Function

  Friend Shared Function NewProjectResource(ByVal ResourceID As String, ByVal Role As String) As ProjectResource
    Return New ProjectResource(Resource.GetResource(ResourceID), Role)
  End Function

  Friend Shared Function NewProjectResource(ByVal ResourceID As String) As ProjectResource
    Return New ProjectResource(Resource.GetResource(ResourceID), DefaultRole)
  End Function

  Friend Shared Function GetProjectResource(ByVal dr As SafeDataReader) As ProjectResource
    Dim child As New ProjectResource
    child.Fetch(dr)
    Return child
  End Function

#End Region

#Region " Constructors "

  Private Sub New(ByVal Resource As Resource, ByVal Role As String)
    With Resource
      mResourceID = .ID
      mLastName = .LastName
      mFirstName = .FirstName
      mAssigned.Date = Now
      mRole = CInt(Roles.Key(Role))
    End With
  End Sub

  Private Sub New()
    ' prevent direct creation of this object
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SafeDataReader)
    With dr
      mResourceID = .GetString(0)
      mLastName = .GetString(1)
      mFirstName = .GetString(2)
      mAssigned = .GetSmartDate(3)
      mRole = .GetInt32(4)
      MarkOld()
    End With
  End Sub

  Friend Sub Update(ByVal Project As Project)
    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' do the update 
    Dim cn As New SqlConnection(db("PTracker"))
    cn.Open()

    Try
      Dim cm As New SqlCommand
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        If Me.IsDeleted Then
          If Not Me.IsNew Then
            ' we're not new, so delete
            .CommandText = "deleteAssignment"
            .Parameters.Add("@ProjectID", Project.ID)
            .Parameters.Add("@ResourceID", mResourceID)

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
          .Parameters.Add("@ProjectID", Project.ID)
          .Parameters.Add("@ResourceID", mResourceID)
          .Parameters.Add("@Assigned", mAssigned.DBValue)
          .Parameters.Add("@Role", mRole)

          .ExecuteNonQuery()

          MarkOld()
        End If

      End With

    Finally
      cn.Close()
    End Try
  End Sub

#End Region

End Class
