Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ResourceAssignments
  Inherits BusinessCollectionBase

#Region " Business Properties and Methods "

  Default Public ReadOnly Property Item(ByVal Index As Integer) As ResourceAssignment
    Get
      Return CType(list.Item(Index), ResourceAssignment)
    End Get
  End Property

  Default Public ReadOnly Property Item(ByVal ProjectID As Guid) As ResourceAssignment
    Get
      Dim r As ResourceAssignment

      For Each r In list
        If r.ProjectID.Equals(ProjectID) Then
          Return r
        End If
      Next
    End Get
  End Property

  Public Sub AssignTo(ByVal Project As Project, ByVal Role As String)
    DoAssignment(ResourceAssignment.NewResourceAssignment(Project, Role))
  End Sub

  Public Sub AssignTo(ByVal ProjectID As Guid, ByVal Role As String)
    DoAssignment(ResourceAssignment.NewResourceAssignment(ProjectID, Role))
  End Sub

  Public Sub AssignTo(ByVal ProjectID As Guid)
    DoAssignment(ResourceAssignment.NewResourceAssignment(ProjectID))
  End Sub

  Private Sub DoAssignment(ByVal Project As ResourceAssignment)
    If Not Contains(Project) Then

      list.Add(Project)

    Else
      Throw New Exception("Resource already assigned to project")
    End If
  End Sub

  Public Sub Remove(ByVal Project As ResourceAssignment)
    list.Remove(Project)
  End Sub

  Public Sub Remove(ByVal ProjectID As Guid)
    Dim obj As ResourceAssignment

    For Each obj In list
      If obj.ProjectID.Equals(ProjectID) Then
        Remove(obj)
        Exit For
      End If
    Next
  End Sub

#End Region

#Region " Contains "

  Public Overloads Function Contains( _
      ByVal Assignment As ResourceAssignment) As Boolean

    Dim child As ResourceAssignment

    For Each child In list
      If child.Equals(Assignment) Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function ContainsDeleted( _
      ByVal Assignment As ResourceAssignment) As Boolean

    Dim child As ResourceAssignment

    For Each child In deletedList
      If child.Equals(Assignment) Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function Contains( _
      ByVal ProjectID As Guid) As Boolean

    Dim r As ResourceAssignment

    For Each r In list
      If r.ProjectID.Equals(ProjectID) Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function ContainsDeleted( _
    ByVal ProjectID As Guid) As Boolean

    Dim r As ResourceAssignment

    For Each r In deletedList
      If r.ProjectID.Equals(ProjectID) Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Shared Methods"

  Friend Shared Function NewResourceAssignments() As ResourceAssignments
    Return New ResourceAssignments
  End Function

  Friend Shared Function GetResourceAssignments( _
          ByVal dr As SafeDataReader) As ResourceAssignments
    Dim col As New ResourceAssignments
    col.Fetch(dr)
    Return col
  End Function

#End Region

#Region " Constructors "

  Private Sub New()
    ' allow creation only from parent
    MarkAsChild()
  End Sub

#End Region

#Region " Data Access "

  ' called by Resource to load data from the database
  Private Sub Fetch(ByVal dr As SafeDataReader)
    While dr.Read()
      list.Add(ResourceAssignment.GetResourceAssignment(dr))
    End While
  End Sub

  ' called by Resource to delete/add/update data into the database
  Friend Sub Update(ByVal tr As SqlTransaction, ByVal Resource As Resource)
    Dim obj As ResourceAssignment

    ' update (thus deleting) any deleted child objects
    For Each obj In deletedList
      obj.Update(tr, Resource)
    Next
    ' now that they are deleted, remove them from memory too
    deletedList.Clear()

    ' add/update any current child objects
    For Each obj In list
      obj.Update(tr, Resource)
    Next

  End Sub

#End Region

End Class
