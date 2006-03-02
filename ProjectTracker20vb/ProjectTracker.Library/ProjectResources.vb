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

    Return New ProjectResources

  End Function

  Friend Shared Function GetProjectResources( _
    ByVal dr As SafeDataReader) As ProjectResources

    Return New ProjectResources(dr)

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
      Me.Add(ProjectResource.GetResource(dr))
    End While
    Me.RaiseListChangedEvents = True

  End Sub

  Friend Sub Update(ByVal project As Project)

    Me.RaiseListChangedEvents = False
    ' update (thus deleting) any deleted child objects
    For Each obj As ProjectResource In DeletedList
      obj.DeleteSelf(project)
    Next
    ' now that they are deleted, remove them from memory too
    DeletedList.Clear()

    ' add/update any current child objects
    For Each obj As ProjectResource In Me
      If obj.IsNew Then
        obj.Insert(project)

      Else
        obj.Update(project)
      End If
    Next
    Me.RaiseListChangedEvents = True

  End Sub

#End Region

End Class
