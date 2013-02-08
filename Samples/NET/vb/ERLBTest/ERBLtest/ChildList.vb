Imports Csla

<Serializable()> _
Public Class ChildList
    Inherits BusinessListBase(Of ChildList, Child)

    Public Sub New()
        Me.AllowEdit = True
        Me.AllowNew = True
        Me.AllowRemove = True
    End Sub

    Protected Overrides Function AddNewCore() As Child
        Dim item As Child = Child.NewChild
        Add(item)
        Debug.WriteLine("Added " & Me.Count - 1)
        Return item
    End Function

End Class
