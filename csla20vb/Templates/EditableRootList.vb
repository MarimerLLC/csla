Imports System.Data.SqlClient

<Serializable()> _
Public Class EditableRootList
  Inherits BusinessListBase(Of EditableRootList, EditableChild)

#Region " Authorization Rules "

  Public Shared Function CanAddObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanGetObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanEditObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanDeleteObject() As Boolean
    ' TODO: customize to check user role
    Return ApplicationContext.User.IsInRole("")
  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewEditableRootList() As EditableRootList
    Return New EditableRootList()
  End Function

  Public Shared Function GetEditableRootList(ByVal id As Integer) As EditableRootList
    Return DataPortal.Fetch(Of EditableRootList)(New Criteria(id))
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    Private mId As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return mId
      End Get
    End Property
    Public Sub New(ByVal id As Integer)
      mId = id
    End Sub
  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    ' TODO: load values
    RaiseListChangedEvents = False
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(EditableChild.GetEditableChild(dr))
      End While
    End Using
    RaiseListChangedEvents = True

  End Sub

  Protected Overrides Sub DataPortal_Update()

    RaiseListChangedEvents = False
    For Each item As EditableChild In DeletedList
      item.DeleteSelf()
    Next
    DeletedList.Clear()

    For Each item As EditableChild In Me
      If item.IsNew Then
        item.Insert(Me)

      Else
        item.Update(Me)
      End If
    Next
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
