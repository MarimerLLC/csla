Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class DynamicRootList
  Inherits DynamicListBase(Of DynamicRoot)

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
    'AuthorizationRules.AllowGet(GetType(DynamicRootList), "Role")
    'AuthorizationRules.AllowEdit(GetType(DynamicRootList), "Role")
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewDynamicRootList() As DynamicRootList
    Return DataPortal.Create(Of DynamicRootList)()
  End Function

  Public Shared Function GetDynamicRootList() As DynamicRootList
    Return DataPortal.Fetch(Of DynamicRootList)()
  End Function

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
