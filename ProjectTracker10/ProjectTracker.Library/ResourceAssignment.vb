Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ResourceAssignment
  Inherits Assignment

  Private mProjectID As Guid = Guid.Empty
  Private mProjectName As String = ""

#Region " Business Properties and Methods "

  Public ReadOnly Property ProjectID() As Guid
    Get
      Return mProjectID
    End Get
  End Property

  Public ReadOnly Property ProjectName() As String
    Get
      Return mProjectName
    End Get
  End Property

  Public Function GetProject() As Project
    Return Project.GetProject(mProjectID)
  End Function

#End Region

#Region " System.Object Overrides "

  Public Overrides Function ToString() As String
    Return mProjectName
  End Function

  Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
    If TypeOf objA Is ResourceAssignment AndAlso TypeOf objB Is ResourceAssignment Then
      Return DirectCast(objA, ResourceAssignment).Equals(DirectCast(objB, ResourceAssignment))

    Else
      Return False
    End If
  End Function

  Public Overloads Overrides Function Equals(ByVal ResourceAssignment As Object) As Boolean
    If TypeOf ResourceAssignment Is ResourceAssignment Then
      Return Equals(DirectCast(ResourceAssignment, ResourceAssignment))

    Else
      Return False
    End If
  End Function

  Public Overloads Function Equals(ByVal Assignment As ResourceAssignment) As Boolean
    Return mProjectID.Equals(Assignment.ProjectID)
  End Function

  Public Overrides Function GetHashCode() As Integer
    Return mProjectID.GetHashCode
  End Function

#End Region

#Region " Shared Methods "

  Friend Shared Function NewResourceAssignment(ByVal Project As Project, ByVal Role As String) As ResourceAssignment
    Return New ResourceAssignment(Project, Role)
  End Function

  Friend Shared Function NewResourceAssignment(ByVal ProjectID As Guid, ByVal Role As String) As ResourceAssignment
    Return New ResourceAssignment(Project.GetProject(ProjectID), Role)
  End Function

  Friend Shared Function NewResourceAssignment(ByVal ProjectID As Guid) As ResourceAssignment
    Return New ResourceAssignment(Project.GetProject(ProjectID), DefaultRole)
  End Function

  Friend Shared Function GetResourceAssignment(ByVal dr As SafeDataReader) As ResourceAssignment
    Dim child As New ResourceAssignment
    child.Fetch(dr)
    Return child
  End Function

  Private Sub New(ByVal Project As Project, ByVal Role As String)
    mProjectID = Project.ID
    mProjectName = Project.Name
    mAssigned.Date = Now
    mRole = CInt(Roles.Key(Role))
    MarkAsChild()
  End Sub

  Private Sub New()
    ' prevent direct creation of this object
    MarkAsChild()
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SafeDataReader)
    With dr
      mProjectID = .GetGuid(0)
      mProjectName = .GetString(1)
      mAssigned.Date = .GetDateTime(2)
      mRole = .GetInt32(3)
      MarkOld()
    End With
  End Sub

  Friend Sub Update(ByVal tr As SqlClient.SqlTransaction, ByVal Resource As Resource)
    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' do the update 
    Dim cm As New SqlClient.SqlCommand
    With cm
      .Connection = tr.Connection
      .Transaction = tr
      .CommandType = CommandType.StoredProcedure

      If Me.IsDeleted Then
        If Not Me.IsNew Then
          ' we're not new, so delete
          .CommandText = "deleteAssignment"
          .Parameters.Add("@ProjectID", mProjectID)
          .Parameters.Add("@ResourceID", Resource.ID)

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
        .Parameters.Add("@ProjectID", mProjectID)
        .Parameters.Add("@ResourceID", Resource.ID)
        .Parameters.Add("@Assigned", mAssigned.DBValue)
        .Parameters.Add("@Role", mRole)

        .ExecuteNonQuery()

        MarkOld()

      End If
    End With

  End Sub

#End Region

End Class
