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
          ByVal data() As ProjectTracker.DalLinq.Assignment) As ResourceAssignments

    Return DataPortal.FetchChild(Of ResourceAssignments)(data)

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Sub Child_Fetch(ByVal data() As ProjectTracker.DalLinq.Assignment)

    Me.RaiseListChangedEvents = False
    For Each child In data
      Add(ResourceAssignment.GetResourceAssignment(child))
    Next
    Me.RaiseListChangedEvents = True

  End Sub

#End Region

End Class
