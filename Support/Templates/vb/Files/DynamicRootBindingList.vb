Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class DynamicRootBindingList
  Inherits DynamicBindingListBase(Of DynamicRoot)

#Region " Business Methods "

  Protected Overrides Function AddNewCore() As DynamicRoot

    Dim item As DynamicRoot = DynamicRoot.NewDynamicRoot
    Add(item)
    Return item

  End Function
#End Region

#Region " Authorization Rules "

  Private Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'AuthorizationRules.AllowGet(GetType(DynamicRootBindingList), "Role")
    'AuthorizationRules.AllowEdit(GetType(DynamicRootBindingList), "Role")
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewDynamicRootBindingList() As DynamicRootBindingList
    Return DataPortal.Create(Of DynamicRootBindingList)()
  End Function

  Public Shared Function GetDynamicRootBindingList() As DynamicRootBindingList
    Return DataPortal.Fetch(Of DynamicRootBindingList)()
  End Function

  Private Sub New()
    ' require use of factory methods
    AllowNew = True
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    ' TODO: load values
    RaiseListChangedEvents = False
    Dim listData As Object = Nothing
    For Each oneItemData As Object In CType(listData, List(Of Object))
      Add(DynamicRoot.GetDynamicRoot(oneItemData))
    Next
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
