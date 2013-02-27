Imports Csla

<Serializable()> _
Public Class RootList
  Inherits DynamicBindingListBase(Of Root)

  Public Shared Function GetList() As RootList
    Return New RootList
  End Function

  Protected Overrides Function AddNewCore() As Object

    Dim item As Root = Root.NewRoot
    Add(item)
    Debug.WriteLine("Added " & Me.Count - 1)
    Return item

  End Function

  Private Sub New()

    Me.AllowEdit = True
    Me.AllowNew = True
    Me.AllowRemove = True

  End Sub

  Protected Overrides Sub RemoveItem(ByVal index As Integer)
    Debug.WriteLine("RemoveItem " & index.ToString)
    MyBase.RemoveItem(index)
  End Sub

  Public Overrides Function SaveItem(ByVal index As Integer) As Root

    Dim result As Root = Nothing
    Try
      result = MyBase.SaveItem(index)

    Catch ex As Csla.Rules.ValidationException
      Debug.WriteLine("Item not saved " & index.ToString)
    End Try
    Return result
  End Function
End Class
