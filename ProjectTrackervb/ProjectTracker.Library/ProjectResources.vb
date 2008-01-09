<Serializable()> _
Public Class ProjectResources
  Inherits BusinessListBase(Of ProjectResources, ProjectResource)

#Region " Business Methods "

  Public Function GetItem(ByVal resourceId As Integer) As ProjectResource

    For Each res As ProjectResource In Me
      If res.ResourceId = resourceId Then
        Return res
      End If
    Next
    Return Nothing

  End Function

  Public Sub Assign(ByVal resourceId As Integer)

    If Not Contains(resourceId) Then
      Dim resource As ProjectResource = _
        ProjectResource.NewProjectResource(resourceId)
      Me.Add(resource)

    Else
      Throw _
        New InvalidOperationException("Resource already assigned to project")
    End If

  End Sub

  Public Overloads Sub Remove(ByVal resourceId As Integer)

    For Each res As ProjectResource In Me
      If res.ResourceId = resourceId Then
        Remove(res)
        Exit For
      End If
    Next

  End Sub

  Public Overloads Function Contains( _
      ByVal resourceId As Integer) As Boolean

    For Each res As ProjectResource In Me
      If res.ResourceId = resourceId Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function ContainsDeleted( _
    ByVal resourceId As Integer) As Boolean

    For Each res As ProjectResource In DeletedList
      If res.ResourceId = resourceId Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function NewProjectResources() As ProjectResources

    Return DataPortal.CreateChild(Of ProjectResources)()

  End Function

  Friend Shared Function GetProjectResources( _
    ByVal data As ProjectTracker.DalLinq.Assignment()) As ProjectResources

    Return DataPortal.FetchChild(Of ProjectResources)( _
      New SingleCriteria(Of ProjectResources, ProjectTracker.DalLinq.Assignment())(data))

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Sub Child_Fetch(ByVal criteria As SingleCriteria(Of ProjectResources, ProjectTracker.DalLinq.Assignment()))

    Me.RaiseListChangedEvents = False
    For Each value In criteria.Value
      Me.Add(ProjectResource.GetResource(value))
    Next
    Me.RaiseListChangedEvents = True

  End Sub

#End Region

End Class
