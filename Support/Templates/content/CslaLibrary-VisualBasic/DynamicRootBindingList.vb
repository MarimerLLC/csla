Imports Csla
Imports Csla.Security

<Serializable()>
Public Class DynamicRootBindingList
  Inherits DynamicBindingListBase(Of DynamicRoot)

  Protected Overrides Function AddNewCore() As Object

    Dim dataPortal As IDataPortal(Of DynamicRoot) = ApplicationContext.GetRequiredService(Of IDataPortal(Of DynamicRoot))()
    Dim item As DynamicRoot = dataPortal.Create()
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
    Dim dataPortal As IDataPortal(Of DynamicRoot) = ApplicationContext.GetRequiredService(Of IDataPortal(Of DynamicRoot))()
    For Each oneItemData As Object In CType(listData, List(Of Object))
      Add(dataPortal.Fetch(oneItemData))
    Next
    RaiseListChangedEvents = True

  End Sub

End Class
