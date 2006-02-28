<Serializable()> _
Public Class ResourceAssignments
  Inherits BusinessListBase(Of ResourceAssignments, ResourceAssignment)

#Region " Business Methods "

  Default Public Overloads ReadOnly Property Item(ByVal projectId As Guid) As ResourceAssignment
    Get
      For Each res As ResourceAssignment In Me
        If res.ProjectId.Equals(projectId) Then
          Return res
        End If
      Next
      Return Nothing
    End Get
  End Property

  Public Sub AssignTo(ByVal projectId As Guid)

    If Not Contains(projectId) Then
      Dim project As ResourceAssignment = ResourceAssignment.NewResourceAssignment(projectId)
      Me.Add(project)

    Else
      Throw New InvalidOperationException("Resource already assigned to project")
    End If

  End Sub

  Public Overloads Sub Remove(ByVal projectId As Guid)

    For Each res As ResourceAssignment In Me
      If res.ProjectId.Equals(projectId) Then
        Remove(res)
        Exit For
      End If
    Next

  End Sub

  Public Overloads Function Contains(ByVal projectId As Guid) As Boolean

    For Each project As ResourceAssignment In Me
      If project.ProjectId = projectId Then
        Return True
      End If
    Next
    Return False

  End Function

  Public Overloads Function ContainsDeleted(ByVal projectId As Guid) As Boolean

    For Each project As ResourceAssignment In DeletedList
      If project.ProjectId = projectId Then
        Return True
      End If
    Next
    Return False

  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function NewResourceAssignments() As ResourceAssignments

    Return New ResourceAssignments

  End Function

  Friend Shared Function GetResourceAssignments( _
          ByVal dr As SafeDataReader) As ResourceAssignments

    Return New ResourceAssignments(dr)

  End Function

  Private Sub New()

    MarkAsChild()

  End Sub

  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    Fetch(dr)

  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SafeDataReader)

    Me.RaiseListChangedEvents = False
    While dr.Read()
      Me.Add(ResourceAssignment.GetResourceAssignment(dr))
    End While
    Me.RaiseListChangedEvents = True

  End Sub

  Friend Sub Update(ByVal cn As SqlConnection, ByVal resource As Resource)

    Me.RaiseListChangedEvents = False
    ' update (thus deleting) any deleted child objects
    For Each item As ResourceAssignment In DeletedList
      item.DeleteSelf(cn, resource)
    Next
    ' now that they are deleted, remove them from memory too
    DeletedList.Clear()

    ' add/update any current child objects
    For Each item As ResourceAssignment In Me
      If item.IsNew Then
        item.Insert(cn, resource)

      Else
        item.Update(cn, resource)
      End If
    Next
    Me.RaiseListChangedEvents = True

  End Sub

#End Region

End Class
