<Serializable()> _
Public Class Grandchildren
  Inherits BusinessCollectionBase

  Default Public ReadOnly Property Item(ByVal index As Integer) As Grandchild
    Get
      Return CType(list(index), Grandchild)
    End Get
  End Property

  Public Sub Add(ByVal Data As String)
    list.Add(Grandchild.NewGrandChild(Data))
  End Sub

  Public Sub Remove(ByVal child As Grandchild)
    list.Remove(child)
  End Sub

  Friend Shared Function NewGrandChildren() As Grandchildren
    Return New Grandchildren
  End Function

  Friend Shared Function GetGrandChildren(ByVal dr As IDataReader) As Grandchildren
    ' TODO: load child data
  End Function

  Friend Sub Update(ByVal tr As IDbTransaction)
    Dim child As Grandchild

    For Each child In list
      child.Update(tr)
    Next
  End Sub

  Private Sub New()
    ' prevent direct creation
    MarkAsChild()
  End Sub

End Class
