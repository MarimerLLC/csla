Imports System.Data.SqlClient

<Serializable()> _
Public Class DynamicRootList
  Inherits EditableRootListBase(Of DynamicRoot)

#Region " Business Methods "

  Protected Overrides Function AddNewCore() As Object

    Dim item As DynamicRoot = DynamicRoot.NewDynamicRoot
    Add(item)
    Return item

  End Function
#End Region

#Region " Authorization Rules "

  Public Shared Function CanGetObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanEditObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewDynamicRootList() As DynamicRootList
    Return New DynamicRootList()
  End Function

  Public Shared Function GetDynamicRootList() As DynamicRootList
    Return DataPortal.Fetch(Of DynamicRootList)()
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
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(DynamicRoot.GetDynamicRoot(dr))
      End While
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
