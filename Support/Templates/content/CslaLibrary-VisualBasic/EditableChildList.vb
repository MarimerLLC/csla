Imports Csla.Security

<Serializable()> _
Public Class EditableChildList
  Inherits BusinessListBase(Of EditableChildList, EditableChild)

  <FetchChild>
  Private Sub Fetch(ByVal childData As Object)
    RaiseListChangedEvents = False
    Dim dataPortal As IChildDataPortal(Of EditableChild) = ApplicationContext.GetRequiredService(Of IChildDataPortal(Of EditableChild))()
    For Each child As Object In CType(childData, IList(Of Object))
      Me.Add(dataPortal.FetchChild(child))
    Next
    RaiseListChangedEvents = True
  End Sub

End Class
