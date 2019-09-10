Imports Csla
Imports Csla.Security

<Serializable()>
Public Class DynamicRootBindingList
  Inherits DynamicBindingListBase(Of DynamicRoot)

  Protected Overrides Function AddNewCore() As Object

    Dim item As DynamicRoot = DataPortal.Create(Of DynamicRoot)()
    Add(item)
    Return item

  End Function

  Private Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'AuthorizationRules.AllowGet(GetType(DynamicRootBindingList), "Role")
    'AuthorizationRules.AllowEdit(GetType(DynamicRootBindingList), "Role")
  End Sub

  <Fetch>
  Private Sub Fetch()

    ' TODO: load values
    RaiseListChangedEvents = False
    Dim listData As Object = Nothing
    For Each oneItemData As Object In CType(listData, List(Of Object))
      Add(DataPortal.Fetch(Of DynamicRoot)(oneItemData))
    Next
    RaiseListChangedEvents = True

  End Sub

End Class
