Imports Csla.Security

<Serializable()> _
Public Class EditableChildList
  Inherits BusinessListBase(Of EditableChildList, EditableChild)

  <FetchChild>
  Private Sub Fetch(ByVal childData As Object)
    RaiseListChangedEvents = False
    For Each child As Object In CType(childData, IList(Of Object))
      Me.Add(DataPortal.FetchChild(Of EditableChild)(child))
    Next
    RaiseListChangedEvents = True
  End Sub

End Class
