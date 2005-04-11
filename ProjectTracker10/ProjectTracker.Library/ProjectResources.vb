Imports System.Data.SqlClient

<Serializable()> _
Public Class ProjectResources
  Inherits BusinessCollectionBase

#Region " Business Properties and Methods "

  Default Public ReadOnly Property Item(ByVal Index As Integer) As ProjectResource
    Get
      Return CType(list.Item(Index), ProjectResource)
    End Get
  End Property

  Default Public ReadOnly Property Item(ByVal ResourceID As String) As ProjectResource
    Get
      Dim r As ProjectResource

      For Each r In list
        If r.ResourceID = ResourceID Then
          Return r
        End If
      Next
      Return Nothing
    End Get
  End Property

  Public Sub Assign(ByVal Resource As Resource, ByVal Role As String)
    DoAssignment(ProjectResource.NewProjectResource(Resource, Role))
  End Sub

  Public Sub Assign(ByVal ResourceID As String, ByVal Role As String)
    DoAssignment(ProjectResource.NewProjectResource(ResourceID, Role))
  End Sub

  Public Sub Assign(ByVal ResourceID As String)
    DoAssignment(ProjectResource.NewProjectResource(ResourceID))
  End Sub

  Private Sub DoAssignment(ByVal Resource As ProjectResource)
    If Not Contains(Resource) Then

      list.Add(Resource)

    Else
      Throw New Exception("Resource already assigned")
    End If
  End Sub

  Public Sub Remove(ByVal Resource As ProjectResource)
    list.Remove(Resource)
  End Sub

  Public Sub Remove(ByVal ResourceID As String)
    Dim r As ProjectResource

    For Each r In list
      If r.ResourceID = ResourceID Then
        Remove(r)
        Exit For
      End If
    Next
  End Sub

#End Region

#Region " Contains "

  'Public Overloads Function Contains( _
  '    ByVal Assignment As ProjectResource) As Boolean

  '  Dim child As ProjectResource

  '  For Each child In list
  '    If child.Equals(Assignment) Then
  '      Return True
  '    End If
  '  Next

  '  Return False

  'End Function

  Public Overloads Function ContainsDeleted( _
      ByVal Assignment As ProjectResource) As Boolean

    Dim child As ProjectResource

    For Each child In deletedList
      If child.Equals(Assignment) Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function Contains( _
      ByVal ResourceID As String) As Boolean

    Dim r As ProjectResource

    For Each r In list
      If r.ResourceID = ResourceID Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function ContainsDeleted( _
    ByVal ResourceID As String) As Boolean

    Dim r As ProjectResource

    For Each r In deletedList
      If r.ResourceID = ResourceID Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Shared Methods "

  Friend Shared Function NewProjectResources() As ProjectResources
    Return New ProjectResources
  End Function

  Friend Shared Function GetProjectResources( _
    ByVal dr As SafeDataReader) As ProjectResources
    Dim col As New ProjectResources
    col.Fetch(dr)
    Return col
  End Function

#End Region

#Region " Constructors "

  Private Sub New()
    ' disallow direct creation

    ' mark us as a child collection
    MarkAsChild()
  End Sub

#End Region

#Region " Data Access "

  ' called to load data from the database
  Private Sub Fetch(ByVal dr As SafeDataReader)
    While dr.Read()
      List.Add(ProjectResource.GetProjectResource(dr))
    End While
  End Sub

  ' called by Project to delete/add/update data into the database
  Friend Sub Update(ByVal Project As Project)
    Dim obj As ProjectResource

    ' update (thus deleting) any deleted child objects
    For Each obj In deletedList
      obj.Update(Project)
    Next
    ' now that they are deleted, remove them from memory too
    deletedList.Clear()

    ' add/update any current child objects
    For Each obj In List()
      obj.Update(Project)
    Next

  End Sub

#End Region

End Class
