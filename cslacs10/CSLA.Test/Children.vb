<Serializable()> _
Public Class Children
  Inherits BusinessCollectionBase

  Default Public ReadOnly Property Item(ByVal index As Integer) As Child
    Get
      Return CType(list(index), Child)
    End Get
  End Property

  Public Sub Add(ByVal Data As String)
    list.Add(Child.NewChild(Data))
  End Sub

  Public Sub Remove(ByVal child As Child)
    list.Remove(child)
  End Sub

  Friend Shared Function NewChildren() As Children
    Return New Children()
  End Function

  Friend Shared Function GetChildren(ByVal dr As IDataReader) As Children
    ' TODO: load child data
  End Function

  Friend Sub Update(ByVal tr As IDbTransaction)
    Dim child As child

    For Each child In list
      child.Update(tr)
    Next
  End Sub

  Private Sub New()
    ' prevent direct creation
    MarkAsChild()
  End Sub

End Class
