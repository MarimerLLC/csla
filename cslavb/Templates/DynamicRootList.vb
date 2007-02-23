Imports System.Data.SqlClient

<Serializable()> _
Public Class DynamicRootList
  Inherits EditableRootListBase(Of EditableRoot)

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
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    ' TODO: load values
    RaiseListChangedEvents = False
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(EditableRoot.GetEditableRoot(dr))
      End While
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
