Imports System.Data.SqlClient

<Serializable()> _
Public Class ResourceAssignments
  Inherits BusinessListBase(Of ResourceAssignments, ResourceAssignment)

#Region " Business Methods "

  Default Public Overloads ReadOnly Property Item(ByVal projectID As Guid) As ResourceAssignment
    Get
      For Each res As ResourceAssignment In Me
        If res.ProjectID.Equals(projectID) Then
          Return res
        End If
      Next
      Return Nothing
    End Get
  End Property

  Public Sub AssignTo(ByVal projectId As Guid)

    DoAssignment(ResourceAssignment.NewResourceAssignment(projectID))

  End Sub

  Private Sub DoAssignment(ByVal project As ResourceAssignment)

    If Not Contains(project) Then
      Me.Add(project)

    Else
      Throw New InvalidOperationException("Resource already assigned to project")
    End If

  End Sub

  Public Overloads Sub Remove(ByVal projectId As Guid)

    For Each res As ResourceAssignment In Me
      If res.ProjectID.Equals(projectId) Then
        Remove(res)
        Exit For
      End If
    Next

  End Sub

#End Region

#Region " Factory Methods"

  Friend Shared Function NewResourceAssignments() As ResourceAssignments

    Return New ResourceAssignments

  End Function

  Friend Shared Function GetResourceAssignments( _
          ByVal dr As SafeDataReader) As ResourceAssignments

    Return New ResourceAssignments(dr)

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Data Access "

  Private Sub New(ByVal dr As SafeDataReader)

    While dr.Read()
      Me.Add(ResourceAssignment.GetResourceAssignment(dr))
    End While

  End Sub

  Friend Sub Update(ByVal tr As SqlTransaction, ByVal Resource As Resource)

    ' update (thus deleting) any deleted child objects
    For Each item As ResourceAssignment In DeletedList
      item.DeleteSelf(tr, Resource)
    Next
    ' now that they are deleted, remove them from memory too
    DeletedList.Clear()

    ' add/update any current child objects
    For Each item As ResourceAssignment In Me
      If item.IsNew Then
        item.Insert(tr, Resource)

      Else
        item.Update(tr, Resource)
      End If
    Next

  End Sub

#End Region

End Class
